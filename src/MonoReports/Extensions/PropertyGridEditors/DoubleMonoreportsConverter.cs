using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Globalization;
using MonoReports.Model;
using Mono.Unix;

namespace MonoReports.Extensions.PropertyGridEditors
{
   	public class DoubleMonoreportsConverter : System.ComponentModel.TypeConverter {

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
						double val = (double) ConvertFromString (text, numberFormatInfo);
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
					throw new Exception (value.ToString() + Catalog.GetString(" is not a valid value for ") + InnerType.Name + ".", e);
				}
			}

			return base.ConvertFrom (context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture,
						 object value, Type destinationType)
		{
			if (value == null)
				throw new ArgumentNullException (Catalog.GetString("value"));

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
}
