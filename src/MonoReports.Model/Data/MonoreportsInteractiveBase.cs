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

namespace MonoReports.Model.Data
{
	public class MonoreportsInteractiveBase : Mono.CSharp.InteractiveBase
	{
		
		static Report  report;
		
		public static Report Report {			
			get {
				return report;
			}
			set {
				report = value;
				if(report != null){
					ParameterFieldsDict = report.Parameters.ToDictionary(p=>p.Name);
					DataFieldsDict = report.DataFields.ToDictionary(df=>df.Name);
					ExpressionFieldsDict = report.ExpressionFields.ToDictionary(ef=>ef.Name);
				}else {
					ParameterFieldsDict = new Dictionary<string, Field>();
					DataFieldsDict = new Dictionary<string, Field>();
					ExpressionFieldsDict = new Dictionary<string, Field>();
				}
			}
		}
		
		public MonoreportsInteractiveBase ():base()
		{
			
		}
		
		public static Dictionary<string,Field> ParameterFieldsDict {get;set;}
		public static Dictionary<string,Field> DataFieldsDict {get;set;}
		public static Dictionary<string,Field> ExpressionFieldsDict {get;set;}
		
		static ReportContext ctx;
		
		public static ReportContext ReportContext {
			get {return ctx;}
			set {
				ctx = value;				
			}
		}
 	
		public static int CurrentPageIndex {
			get {return ctx.CurrentPageIndex;}
		}
		
		public static int RowIndex {
			get {return ctx.RowIndex;}
		}
		
		public static int NumberOfPages {
			get {return ctx.NumberOfPages;}
		}
		
		public static object d(string dataFieldName){
			return DataFieldsDict [dataFieldName].DataProvider.GetValue(ctx.DataSource.Current);
		}
		
		public static T d<T>(string dataFieldName){
			return (T) DataFieldsDict [dataFieldName].DataProvider.GetValue(ctx.DataSource.Current);
		}
		
		public static object p(string parameterFieldName){
			object o =  ParameterFieldsDict[parameterFieldName].DefaultValue;
			return o;
		}
		
		public static T p<T>(string parameterFieldName){
			return (T) ParameterFieldsDict [parameterFieldName].DefaultValue;
		}
		
		public static object e(string expressionFieldName){
			return  ExpressionFieldsDict[expressionFieldName].DataProvider.GetValue(expressionFieldName);
		}
		
		public static T e<T>(string expressionFieldName){
			return (T) ExpressionFieldsDict [expressionFieldName].DataProvider.GetValue(expressionFieldName);
		}
		
		
	}
}

