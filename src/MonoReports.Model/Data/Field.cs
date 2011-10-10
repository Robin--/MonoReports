// 
// Field.cs
//  
// Author:
//       Tomasz Kubacki <Tomasz.Kubacki (at) gmail.com>
// 
// Copyright (c) 2010 Tomasz Kubacki 2010
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
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;
using Mono.Unix;

namespace MonoReports.Model.Data
{
	public class  Field
	{
		public virtual string Name {
			get;
			set;
		}
		
		public object DefaultValue {
			get;	
			set;
		}
		
		public IFieldValueProvider DataProvider {get;set;}
		
		internal Expression expression;
		
		public Type FieldType {get;set;}
		
		
		
		public FieldKind FieldKind {get;set;}
 
		public override string ToString(){
			return Name;
		}
		
		public  string GetStringValue (object current, string format)
		{			
 
			string returnVal = String.Empty;
			if(current != null) {
				try{
					returnVal =  String.Format(format != null ? format : "{0}",DataProvider.GetValue(current) );
				}catch(Exception exp){
					Console.WriteLine( String.Format(Catalog.GetString("Field {0} exp: {1}"),this.Name, exp));
				}
			} else {
				returnVal = String.Format(format != null ? format : "{0}", DefaultValue);
			}
			 
			
			return returnVal;
		}	
		
		public static Field CreateLambdaExpressionField<T,K>(string fieldName, Expression<Func<T,K>> lambda){
			
			Field f = new Field();
			f.Name = fieldName;
			f.FieldType = typeof(K);
		    LambdaFieldValueProvider<T,K> lfvp = new LambdaFieldValueProvider<T,K>(lambda);
			f.DataProvider = lfvp;
			return f; 
		}
		
		public static Field CreateParameterField<K>(string fieldName, K theValue ){
			
			Field f = CreateLambdaExpressionField<IDictionary<string,object>,K>(fieldName, x => (K) x[fieldName]);									
			f.FieldType = theValue.GetType();		    
			f.FieldKind = FieldKind.Parameter;
			f.DefaultValue = theValue;
			return f; 
		}
	}
	
	public interface IFieldValueProvider {
		object GetValue (object current);
	}
	
	public enum FieldKind { Data, Expression, Parameter };

	
}

