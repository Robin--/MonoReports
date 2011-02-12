// 
// JsonDatasource.cs
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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace MonoReports.Model.Data
{
	public class JsonDatasource : IDataSource
	{
		public JsonDatasource (JArray jsonArray)
		{
			this.jsonArray = jsonArray;
			propertiesDictionary = new Dictionary<string, Field> ();
			fields = new List<Field> ();
			CurrentRowIndex = -1;
			
			FillFields(String.Empty, jsonArray[0],fields,FieldKind.Data);
			foreach(var f in fields){
				if(!propertiesDictionary.ContainsKey( f.Name))
					propertiesDictionary.Add(f.Name,f);
			}
			
		}
			
	    public static void FillFields(string prefix, JToken token, List<Field> fieldsListToStore , FieldKind fieldKind){
			if(token.Type ==  JTokenType.Property) {
				JProperty jp = token as JProperty;
				if(jp.Value.Type == JTokenType.Object){
					  FillFields(prefix + jp.Name + "." ,jp.Value,fieldsListToStore,fieldKind);
				}else if(jp.Value.Type == JTokenType.Array) {	
					
				}else {
					string propertyName = prefix + jp.Name;
					Field f = fieldsListToStore.FirstOrDefault(theField => theField.Name == propertyName);
					if(f == null) {
						f = new Field();
						fieldsListToStore.Add(f);
					}
					f.DefaultValue = JsonValueProvider.GetTokenVal(jp.Value, jp.Value.Type);
					f.FieldKind = fieldKind;
					f.FieldType = JsonValueProvider.GetTypeByTokenType(jp.Value.Type);
					f.Name = propertyName;
					f.DataProvider = new JsonValueProvider(jp);
				}
				 
			} else if(token.Type ==  JTokenType.Object) {
				foreach(var c in token.Children())
					  FillFields(prefix,c,fieldsListToStore,fieldKind);
			}
		}			
		

		Dictionary<string, Field> propertiesDictionary;
		List<Field> fields = null;
		JArray jsonArray = null;			
		JToken currentObj = null;

		#region IDataSource implementation
		public string GetValue (string fieldName, string format)
		{
			string realFormat = format != null ? format : "{0}";
 			JToken tk = currentObj.SelectToken(fieldName);
			if(tk != null)
				return string.Format (realFormat, JsonValueProvider.GetTokenVal(tk,tk.Type));
			else
				return String.Empty;
		}

		public void ApplySort (System.Collections.Generic.IEnumerable<string> sortingFields)
		{
			throw new NotImplementedException ();
		}

		public Field[] DiscoverFields ()
		{
			return fields.ToArray ();
		}

		public bool ContainsField (string fieldName)
		{
			return propertiesDictionary.ContainsKey (fieldName);
		}

		public int CurrentRowIndex {
			get;
			set;
		}

		#endregion

		#region IEnumerator implementation
		public bool MoveNext ()
		{
			CurrentRowIndex++;
			if(jsonArray.Count > CurrentRowIndex) {
				currentObj = jsonArray [CurrentRowIndex];
				return true;
			}else {
				return false;
			}			 
		}

		public void Reset ()
		{
			CurrentRowIndex = -1;
		}

		public object Current {
			get {
				return currentObj;
			}
		}
		#endregion
	}
	
	public class JsonValueProvider : IFieldValueProvider
	{
		private JToken token;
		
		public JsonValueProvider(JToken token){
			this.token = token;
		}
 
		#region IFieldValueProvider implementation
		public object GetValue (object current)
		{
			 return GetTokenVal( token, token.Type);
		}
		#endregion
		
		public static object GetTokenVal(JToken token, JTokenType type){
			switch (type) {
				case JTokenType.String:
					return token.Value<string>();
					
				case JTokenType.Integer:
					return token.Value<int>();
					
				case JTokenType.Float:
					return token.Value<double>();
					
				case JTokenType.Boolean:
					return token.Value<bool>();
				
				case JTokenType.Bytes:
					return token.Value<byte[]>();
				
				case JTokenType.Date:
					return token.Value<DateTime>();
				
			default:
				return token.Value<Object>();
			}
		}
		
		
		public static Type GetTypeByTokenType( JTokenType type){
			switch (type) {
				case JTokenType.String:
					return typeof(string);
					
				case JTokenType.Integer:
					return typeof(int);
					
				case JTokenType.Float:
					return typeof(double);
					
				case JTokenType.Boolean:
					return typeof(bool);
				
				case JTokenType.Bytes:
					return typeof(byte[]);
				
				case JTokenType.Date:
					return typeof(DateTime);
				
			default:
				return typeof(object);
			}
		}
		
	}
}

