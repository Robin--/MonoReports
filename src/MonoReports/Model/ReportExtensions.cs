// 
// ReportExtensions.cs
//  
// Author:
//       Tomasz Kubacki <tomasz (dot) kubacki (at) gmail (dot ) com>
// 
// Copyright (c) 2010 Tomasz Kubacki
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using MonoReports.Model;
using Newtonsoft.Json;
using System.Collections.Generic;
using MonoReports.Extensions.JsonNetExtenssions;
using Cairo;
using MonoReports.Core;
using MonoReports.Renderers;
using MonoReports.Model.Controls;
using MonoReports.Model.Data;
using MonoReports.Extensions.CairoExtensions;
using System.Linq;
 

namespace MonoReports.Model
{
	public static class ReportExtensions
	{		
		
		public static void Load (this Report r,string path) {
			using(System.IO.FileStream file = System.IO.File.OpenRead (path)) {				 
				byte[] bytes = new byte[file.Length];
				file.Read (bytes, 0, (int)file.Length);
				
				var report = JsonConvert.DeserializeObject<Report> (System.Text.Encoding.UTF8.GetString (bytes), 
					new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects,
					Converters = new List<JsonConverter> (
						new JsonConverter[] { 
						new MonoReports.Extensions.PointConverter (), 
						new MonoReports.Extensions.SizeConverter (),
						new MonoReports.Extensions.ColorConverter (),
						new MonoReports.Extensions.ThicknessConverter ()
						
					})  
				});
				report.CopyToReport(r);
				file.Close ();	
				
				foreach(var ef in r.ExpressionFields){
					if(ef is ExpressionField)
						ef.DataProvider = new ExpressionFieldValueProvider(ef as ExpressionField);				
			}	
			
			}
		}
		
		
		public static void Save(this Report report , string path) {		
			
			//dataproviders have to be cleaned because they could be not serializable
		    foreach(var par in report.Parameters){
				par.DataProvider = null;			
			}
			
			foreach(var df in report.DataFields){
				df.DataProvider = null;				
			}	
			
			foreach(var ef in report.ExpressionFields){
				ef.DataProvider = null;				
			}	
			
			report.Pages.Clear();
			report.DataSource = null;
			using (System.IO.FileStream file = System.IO.File.OpenWrite (path)) {
					var serializedProject = JsonConvert.SerializeObject (report,
					Formatting.None, 
					new JsonSerializerSettings { ContractResolver = new MonoReportsContractResolver(), TypeNameHandling = TypeNameHandling.Objects }
				);
				byte[] bytes = System.Text.Encoding.UTF8.GetBytes (serializedProject);
				file.SetLength (bytes.Length);
				file.Write (bytes, 0, bytes.Length);
			
				file.Close ();
			}
		}
		
		public static void ExportToPdf(this Report report ,string path, IDictionary<string,object> parameters) {
			 			
			foreach (KeyValuePair<string, object> kvp in parameters) {
				foreach(var parField in FieldBuilder.CreateFields(kvp.Value, kvp.Key,FieldKind.Parameter))
				{
					var oldField = report.Parameters.FirstOrDefault(par => par.Name == parField.Name);
					if (oldField != null) {						
						oldField.FieldType =  parField.FieldType;
						oldField.DefaultValue = parField.DefaultValue;
					} else {
						report.Parameters.Add(parField);
					}
				}
			}
			
			
			report.ExportToPdf(path);			
		}
		
		public static string ScriptTemplateForDataSourceEvaluation  =@"
using System;
using System.Collections.Generic;
{0}

public sealed class GenerateDataSource {{
    public object Generate()
    {{ 
		object datasource = null;
		Dictionary<string,object> parameters = new Dictionary<string,object>();
		 {1}
        return new object[] {{datasource,parameters}};
    }}
}}";
		
	  
		public static bool EvalDataSourceScript(this Report report) {
 
		MonoReports.Services.CompilerService compiler = new MonoReports.Services.CompilerService(ScriptTemplateForDataSourceEvaluation);
		compiler.References.Add("Newtonsoft.Json.dll");
			
			return EvalDataSourceScript(report,compiler);
			 
		}
       
		
		public static bool EvalDataSourceScript(this Report report, MonoReports.Services.CompilerService compiler) {
			
			
			string code = report.DataScript;
 			
			object result = null;
			string message = null;			 
			bool res = false;
			string usings = "using Newtonsoft.Json.Linq;using System.Linq;";						
		
			if( compiler.Evaluate (out result, out message , new object[]{usings,code})  ) {				
				var ds = (result as object[]);
				var datasource = ds[0] ;
 
				if (datasource != null) {
					report.DataSource = datasource;					
					res = true;
				}
				
				Dictionary<string,object> parametersDictionary  = (Dictionary<string, object>) ds[1];
				if(parametersDictionary != null)
					foreach (KeyValuePair<string, object> kvp in parametersDictionary) {
						foreach(var newfield in FieldBuilder.CreateFields(kvp.Value, kvp.Key,FieldKind.Parameter)) {
							var oldField = report.Parameters.FirstOrDefault(par => par.Name == newfield.Name);
							if (oldField != null) {
								oldField.DataProvider = newfield.DataProvider;
								oldField.DefaultValue = newfield.DefaultValue;
								oldField.FieldType = newfield.FieldType;
							} else {
								report.Parameters.Add(newfield);
							}
						}
						
					}
			}else {
				Console.Error.WriteLine(message);
			}
			return res;
		}
		
		
		public static void ExportToPdf(this Report report ,string path) {
			
			double unitMultiplier = CairoExtensions.UnitMultiplier;
			double realFontMultiplier = CairoExtensions.RealFontMultiplier;
			ReportRenderer renderer = new ReportRenderer ();
			renderer.ResolutionX = 72;			
			using (PdfSurface pdfSurface = new PdfSurface (				
				path,report.WidthWithMargins * renderer.UnitMultipilier,
				report.HeightWithMargins * renderer.UnitMultipilier)) {			
				 
				Cairo.Context cr = new Cairo.Context (pdfSurface);
				cr.Antialias = Antialias.None;
				renderer.Context = cr;
				renderer.RegisterRenderer (typeof(TextBlock), new TextBlockRenderer ());
				renderer.RegisterRenderer (typeof(Line), new LineRenderer ());
				PixbufRepository pbr = new PixbufRepository();
				pbr.Report = report;
				
				renderer.RegisterRenderer (typeof(Image), new ImageRenderer (){PixbufRepository = pbr});
				SectionRenderer sr = new SectionRenderer();
				renderer.RegisterRenderer(typeof(ReportHeaderSection), sr);
				renderer.RegisterRenderer(typeof(ReportFooterSection), sr);
				renderer.RegisterRenderer(typeof(DetailSection), sr);
				renderer.RegisterRenderer(typeof(PageHeaderSection), sr);
				renderer.RegisterRenderer(typeof(PageFooterSection), sr);
				
				
				MonoReports.Model.Engine.ReportEngine engine = new MonoReports.Model.Engine.ReportEngine (report,renderer);
				engine.Process ();		
				
				Cairo.Context cr1 = new Cairo.Context (pdfSurface);
				renderer.Context = cr1;
				cr1.Translate(report.Margin.Left * renderer.UnitMultipilier,report.Margin.Top * renderer.UnitMultipilier);
				engine.RenderPages(renderer,report);		
				pdfSurface.Finish ();		
				(cr as IDisposable).Dispose ();
				(cr1 as IDisposable).Dispose ();
			}
			
			CairoExtensions.UnitMultiplier = unitMultiplier;
			CairoExtensions.RealFontMultiplier = realFontMultiplier;
			
		}
		
		
		
	}
}

