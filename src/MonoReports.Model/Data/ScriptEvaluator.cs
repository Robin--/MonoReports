// 
// MonoreportsInteractiveBase.cs
//  
// Author:
//       Tomasz Kubacki <tomasz (dot) kubacki (at) gmail (dot ) com>
// 
// Copyright (c) 2011 Tomasz Kubacki
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
using System.Collections.Generic;
using System.Linq;
using MonoReports.Model.Controls;

namespace MonoReports.Model.Data
{
	
	/// <summary>
	/// Script evaluator it's a little bit overcomplicated since we can't have Mono.CSharp.InteractiveBase
	/// instance (it has to be static) as of Mono 2.10 
	/// </summary>
	public class ScriptEvaluator : Mono.CSharp.InteractiveBase
	{
		static ScriptEvaluator ()
		{
			Mono.CSharp.Evaluator.InitAndGetStartupFiles(new string[]{});				
			Mono.CSharp.Evaluator.LoadAssembly(System.Reflection.Assembly.GetExecutingAssembly().Location);
			Mono.CSharp.Evaluator.SetInteractiveBaseClass(typeof(MonoReports.Model.Data.ScriptEvaluator));
			Mono.CSharp.Evaluator.Run("using System;");
			Mono.CSharp.Evaluator.Run("using MonoReports.Model;");
			scriptingContextDictionary = new Dictionary<string, ScriptingContext>();
		}
								
		static Dictionary<string,ScriptingContext> scriptingContextDictionary;
		
		public static void InitOrUpdateScriptingContextForReport(string reportName, Report report, ReportContext ctx) {
			scriptingContextDictionary[reportName] = new ScriptingContext(){ReportContext = ctx};
		}
		
		public static void ClearContext(){
			scriptingContextDictionary.Clear();
			currentContext = null;
			scriptingContextName = String.Empty;
		}
		
		static string scriptingContextName;
		
		public static string ScriptingContextName {
			get {return scriptingContextName;}
			set {
				scriptingContextName = value;
				currentContext = scriptingContextDictionary[scriptingContextName];
				reportContext = currentContext.ReportContext;
			}
			
		}
		
		static ScriptingContext currentContext;
		
		static ReportContext reportContext;
		
		
		public static void EvalDataControl(IDataControl dataControl) {
			Field field = null;				
 			if (dataControl.FieldKind == FieldKind.Parameter) {
				if (reportContext.ParameterFieldsDict.ContainsKey (dataControl.FieldName)) {
					if(reportContext.Report.ParameterValues.ContainsKey(dataControl.FieldName)) {
						field = reportContext.Report.ParameterValues[dataControl.FieldName] as Field;
						dataControl.Text = field.GetStringValue (reportContext.Report.ParameterValues, dataControl.FieldTextFormat);
					} else {
						dataControl.Text = field.GetStringValue (null, dataControl.FieldTextFormat);
					}
				}
			} else if (dataControl.FieldKind == FieldKind.Data)   {
				//3tk ENGINE2 TODO
			}else {
			  	//2tk ENGIN2 TODO
			}
		}
	 
 
		
 	
		public static int CurrentPageIndex {
			get {return reportContext.CurrentPageIndex;}
		}
		
		public static int RowIndex {
			get {return reportContext.RowIndex;}
		}
		
		public static int NumberOfPages {
			get {return reportContext.NumberOfPages;}
		}
		
		public static object d(string dataFieldName){
			return reportContext.DataFieldsDict [dataFieldName].DataProvider.GetValue(reportContext.DataSource.Current);
		}
		
		public static T d<T>(string dataFieldName){
			return (T) reportContext.DataFieldsDict [dataFieldName].DataProvider.GetValue(reportContext.DataSource.Current);
		}
		
		public static object p(string parameterFieldName){
			object o =  reportContext.ParameterFieldsDict[parameterFieldName].DefaultValue;
			return o;
		}
		
		public static T p<T>(string parameterFieldName){
			return (T) reportContext.ParameterFieldsDict [parameterFieldName].DefaultValue;
		}
		
		public static object e(string expressionFieldName){
			return  reportContext.ExpressionFieldsDict[expressionFieldName].DataProvider.GetValue(expressionFieldName);
		}
		
		public static T e<T>(string expressionFieldName){
			return (T) reportContext.ExpressionFieldsDict [expressionFieldName].DataProvider.GetValue(expressionFieldName);
		}
 
	}
 
	public class ScriptingContext {
 
		ReportContext ctx;
		
		public ReportContext ReportContext {
			get {return ctx;}
			set {
				ctx = value;				
			}
		}
		
		
	}
}

