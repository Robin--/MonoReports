// 
// PointEditor.cs
//  
// Author:
//       tomek <${AuthorEmail}>
// 
// Copyright (c) 2010 tomek
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
using MonoReports.Model.Controls;
using PropertyGrid;
using MonoReports.Model;
using System.ComponentModel;
using PropertyGrid.PropertyEditors;
using System.Globalization;

namespace MonoReports.Extensions.PropertyGridEditors
{
	[PropertyEditorType (typeof (Border))]
	public class BorderEditorCell: PropertyEditorCell
	{
		protected override string GetValueText ()
		{
			return ((Border)Value).ToString ();
		}
		
		protected override IPropertyEditor CreateEditor (Gdk.Rectangle cell_area, Gtk.StateType state)
		{
			return new BorderEditor ();
		}
	}
	
	[PropertyEditorType (typeof (double))]
	public class DoubleEditorCell: TextEditor
	{
		  public DoubleEditorCell(){
			CustomConverter = new MonoreportConverter();
		  }
	}
	
	public class MonoreportConverter : System.ComponentModel.TypeConverter {
		
		string[] units = {"mm","cm","in","pt"};
		
		
		internal virtual string ConvertToString (object value, NumberFormatInfo format){
			return ((double) value).ToString ("R", format);
		}

		internal virtual object ConvertFromString (string value, NumberFormatInfo format){
			return double.Parse (value, NumberStyles.Float, format);
		}

		internal virtual object ConvertFromString (string value, int fromBase)
		{
			if (SupportHex) {
				throw new NotImplementedException ();
			} else {
				throw new InvalidOperationException ();
			}
		}
		
		public override bool IsValid (ITypeDescriptorContext context, object value)
		{
			return base.IsValid (context, value);
		}
		
				internal Type InnerType;

		 
		internal virtual bool SupportHex {
			get;
			set;
			
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof (string) || base.CanConvertFrom (context, sourceType);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type t)
		{
			return t.IsPrimitive || base.CanConvertTo (context, t);
		}
		
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (culture == null)
				culture = CultureInfo.CurrentCulture;

			string text = value as string;
			if (text != null) {
				try {
					if (SupportHex) {
						if (text.Length >= 1 && text[0] == '#') {
							return ConvertFromString (text.Substring (1), 16);
						}

						if (text.StartsWith ("0x") || text.StartsWith ("0X")) {
							return ConvertFromString (text, 16);
						}
					}

 					NumberFormatInfo numberFormatInfo = (NumberFormatInfo) culture.GetFormat(typeof(NumberFormatInfo));
					if(text.EndsWith("mm")) {
						text = text.Replace("mm",string.Empty);
						double val = (double)  ConvertFromString (text, numberFormatInfo);
						return val.mm();
					}else if(text.EndsWith("cm")){
						text = text.Replace("cm",string.Empty);
						double val = (double)  ConvertFromString (text, numberFormatInfo);
						return val.cm();
					}else if(text.EndsWith("in")){
						text = text.Replace("in",string.Empty);
						double val = (double)  ConvertFromString (text, numberFormatInfo);
						return val.inch();
					}else if(text.EndsWith("pt")){
						text = text.Replace("pt",string.Empty);
						double val = (double)  ConvertFromString (text, numberFormatInfo);
						return val.pt();
					}
					
					return ConvertFromString (text, numberFormatInfo);
				} catch (Exception e) {
					// LAMESPEC MS wraps the actual exception in an Exception
					throw new Exception (value.ToString() + " is not a valid "
						+ "value for " + InnerType.Name + ".", e);
				}
			}

			return base.ConvertFrom (context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture,
						 object value, Type destinationType)
		{
			if (value == null)
				throw new ArgumentNullException ("value");

			if (culture == null)
				culture = CultureInfo.CurrentCulture;

#if NET_2_0
			if (destinationType == typeof (string) && value is IConvertible)
				return ((IConvertible) value).ToType (destinationType, culture);
#else
			if (destinationType == typeof (string) && value.GetType () == InnerType) {
				NumberFormatInfo numberFormatInfo = (NumberFormatInfo) culture.GetFormat (typeof (NumberFormatInfo));
				return ConvertToString (value, numberFormatInfo);
			}
#endif

			if (destinationType.IsPrimitive)
				return Convert.ChangeType (value, destinationType, culture);

			return base.ConvertTo (context, culture, value, destinationType);
		}

	}
	
	public class BorderEditor: Gtk.HBox, IPropertyEditor
	{
		Gtk.Entry entry;
		Border border;
		
		public BorderEditor()
		{
			entry = new Gtk.Entry ();
			entry.Changed += OnChanged;
			entry.HasFrame = false;
			PackStart (entry, true, true, 0);
			ShowAll ();
		}
		
		public void Initialize (EditSession session)
		{
		}
		
		public object Value {
			get { return border; }
			set {
				border = (Border) value;
				entry.Changed -= OnChanged;
				entry.Text = String.Format("{0};{1};{2};{3}",border.LeftWidth,border.TopWidth,border.RightWidth,border.BottomWidth);
				entry.Changed += OnChanged;
			}
		}
		
		void OnChanged (object o, EventArgs a)
		{
			string s = entry.Text;
				try {
					if(s != null) {						 
					    var doubles =  s.Split(';');
						if (doubles.Length > 3) {
					    	border = new Border(						
							 double.Parse(doubles[0]),
							double.Parse(doubles[1]),
							 double.Parse(doubles[2]),
							 double.Parse(doubles[3]));
						} else if (doubles.Length == 1) {
							border = new Border( double.Parse(doubles[0]));
						}
					}
					if (ValueChanged != null)
						ValueChanged (this, a);
					
				} catch {
				}
			
		}
		
		public event EventHandler ValueChanged;
		
		 
	}
}

