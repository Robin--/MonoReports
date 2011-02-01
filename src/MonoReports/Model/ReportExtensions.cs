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
using System.Text;
using System.Reflection;
 

namespace MonoReports.Model
{
	public static class ReportExtensions
	{		
		static bool assemblyResolverHandlerSet;
		
		
		static ReportExtensions(){
			assemblyResolverHandlerSet = false;
		}
		
		
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
		
		public static void ExportToPdf(this Report report, string path, IDictionary<string,object> parameterValues) {
			
			report.ParameterValues = parameterValues;
			updateReportWithParamaters(report);			
			report.ExportToPdf(path);			
		}
		
		public static string ScriptTemplateForDataSourceEvaluation  =@"
using System;
using System.Collections.Generic;
{0}



public sealed class GenerateDataSource {{

IDictionary<string,object> parameters = new Dictionary<string,object>();

    public void Generate(MonoReports.Model.Report r)
    {{ 
 		{1}
		r.DataSource = ds;
		r.ParameterValues = parameters;
    }}
}}";
		
	  
		public static bool EvalDataSourceScript(this Report report) {
 
			MonoReports.Services.CompilerService compiler = new MonoReports.Services.CompilerService(ScriptTemplateForDataSourceEvaluation);			 
			foreach (string r in report.References) {
				compiler.References.Add(r);
			}
			
			return EvalDataSourceScript(report,compiler);
			 
		}
       
		static void updateReportWithParamaters(Report report ){
			
		 
    	  foreach (KeyValuePair<string, object> kvp in report.ParameterValues) {
				  	var f1 = Field.CreateParameterField(kvp.Key,kvp.Value);
 
					var param = report.Parameters.FirstOrDefault(pr => pr.Name == kvp.Key);
					if(param != null){
						param.DataProvider = f1.DataProvider;
						param.FieldType = f1.FieldType;
						
					}else {
						 
						report.Parameters.Add(f1);
					}
				}
			
		}
		
		public static bool EvalDataSourceScript(this Report report, MonoReports.Services.CompilerService compiler) {
			
			if(!assemblyResolverHandlerSet) {
				 
				AppDomain
					.CurrentDomain
						.AssemblyResolve+=  delegate(object sender, ResolveEventArgs args) {	
							Assembly myAssembly = null;
							string asmName = args.Name.Substring(0,	args.Name.IndexOf(',')) +".dll";
						
							string assemblyPath = report.References.FirstOrDefault(asm => asm.EndsWith(asmName));
							if( !string.IsNullOrEmpty(assemblyPath) && System.IO.File.Exists(assemblyPath))
								return  Assembly.LoadFrom(assemblyPath);
							assemblyPath = System.IO.Path.Combine(report.AlternativeReferencedAssembliesPath,asmName);
							if(System.IO.File.Exists(assemblyPath))
								return  Assembly.LoadFrom(assemblyPath);
					
							assemblyPath = System.IO.Path.Combine(Assembly.GetExecutingAssembly().Location,asmName);
							if(System.IO.File.Exists(assemblyPath))
								return  Assembly.LoadFrom(assemblyPath);
 
							return  myAssembly;
						}; 
				assemblyResolverHandlerSet = true;
			}
			
			
			
			string code = report.DataScript;
 			
			object result = null;
			string message = null;			 
			bool res = false;
			StringBuilder stb = new StringBuilder();
			stb.AppendLine("using Newtonsoft.Json.Linq;");
			stb.AppendLine("using System.Linq;");
			stb.AppendLine("using MonoReports.Model.Data;");
			
			foreach (string s in report.Usings) {
				stb.Append("using ");
				stb.Append(s);				
				stb.AppendLine(";");				
			}
			string usings = stb.ToString();
			
			compiler.References.Clear();  
			compiler.References.Add("Newtonsoft.Json.dll");
			compiler.References.Add("MonoReports.Model.dll");
			
			foreach (string r in report.References) {
				string toAdd = r;								
				if (!System.IO.File.Exists (r))
					if(!string.IsNullOrEmpty (report.AlternativeReferencedAssembliesPath))
						toAdd = System.IO.Path.Combine (report.AlternativeReferencedAssembliesPath,System.IO.Path.GetFileName (r));
				
				if (!compiler.References.Contains (toAdd))
					compiler.References.Add (toAdd);
			}

			if( compiler.Evaluate (out result, out message , new string[]{usings,code}, new object[]{ report })  ) {				
 
				report.FillFieldsFromDataSource();
				
				updateReportWithParamaters(report);
				
				
			}else {
				Console.Error.WriteLine(message);
			}
			return res;
		}

		
		public static void ExportToPdf(this Report report ,string path) {
			report.EvalDataSourceScript();						
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

