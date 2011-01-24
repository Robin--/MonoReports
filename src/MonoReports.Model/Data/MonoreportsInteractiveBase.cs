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

namespace MonoReports.Model.Data
{
	public class MonoreportsInteractiveBase : Mono.CSharp.InteractiveBase
	{
		
		public static Report Report {get;set;}
		
		public MonoreportsInteractiveBase ()
		{
		
		}
		
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
			return ctx.DataFieldsDict [dataFieldName].DataProvider.GetValue(ctx.DataSource.Current);
		}
		
		public static T d<T>(string dataFieldName){
			return (T) ctx.DataFieldsDict [dataFieldName].DataProvider.GetValue(ctx.DataSource.Current);
		}
		
		public static object p(string parameterFieldName){
			object o =  ctx.ParameterFieldsDict[parameterFieldName].DefaultValue;
			return o;
		}
		
		public static T p<T>(string parameterFieldName){
			return (T) ctx.ParameterFieldsDict [parameterFieldName].DefaultValue;
		}
		
		public static object e(string expressionFieldName){
			return  ctx.ExpressionFieldsDict[expressionFieldName].DataProvider.GetValue(expressionFieldName);
		}
		
		public static T e<T>(string expressionFieldName){
			return (T) ctx.ExpressionFieldsDict [expressionFieldName].DataProvider.GetValue(expressionFieldName);
		}
		
		
	}
}

