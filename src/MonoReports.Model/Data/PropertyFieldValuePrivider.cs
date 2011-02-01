// 
// PropertyDataField.cs
//  
// Author:
//       Tomasz Kubacki <tomasz (dot ) kubacki (at) gmail (dot) com>
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
using System.Reflection;
using System.Linq.Expressions;

namespace MonoReports.Model.Data
{
	public class PropertyFieldValuePrivider<T,K> : IFieldValueProvider
	{
 
		public PropertyFieldValuePrivider(){
			 
		}
		
		public PropertyFieldValuePrivider(Field field, ParameterExpression root, Expression parent,string propertyName) {
			Expression<Func<T,K>> lambda = null;		
		    lambda = Expression.Lambda<Func<T,K>>(Expression.Property(parent,propertyName),root);
			compiledMethod = lambda.Compile();
			this.field = field;
		    this.field.expression = lambda;			
		}
		
		protected Field field = null;
 
		public  object GetValue (object current)
		{			
			if (compiledMethod == null) {	
				Compile();				 
			}
			
			object returnVal = null;
			
			try {
				returnVal = compiledMethod((T)current);
			}catch(Exception exp){
				Console.WriteLine(exp);
			}
			
			return returnVal;
		}			

				
		
		public void Compile() {
			compiledMethod = (Func<T,K>) (field.expression as LambdaExpression).Compile ();		 
		}
		
		protected Func<T,K> compiledMethod;
	}

 
}

