// 
// ReportRenderer.cs
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
using MonoReports.Model.Controls;
using MonoReports.ControlView;
using System.Collections.Generic;
using MonoReports.Model;
using MonoReports.Services;
using Cairo;
using MonoReports.Extensions.CairoExtensions;

namespace MonoReports.Core
{
	public class ReportRenderer : IReportRenderer
	{
		Dictionary<Type, object> renderersDictionary;

		public Dictionary<Type, object> RenderersDictionary {
			get {
				return this.renderersDictionary;
			}
			set {
				renderersDictionary = value;
			}
		}
		
		
		bool isDesign;
		public bool IsDesign {
			get { return isDesign; }
			set { isDesign = value; 
				foreach (KeyValuePair<Type, object> kvp in renderersDictionary) {
					(kvp.Value as IControlRenderer).DesignMode = isDesign;
				}
			}
		}

		Cairo.Context context;		
		
		public Cairo.Context Context {
			get {
				return this.context;
			}
			set {
				context = value;
			}
		}
		
		double resolution;
		public double Resolution {
			get { return resolution; }
			set { 
				resolution = value; 
			    CairoExtensions.RealFontMultiplier =  Pango.Scale.PangoScale * resolution / 72;
			}
		}
		
		double unitMultipilier;
		public double UnitMultipilier {
			get { return unitMultipilier; }
			set { 
				unitMultipilier = value;
				CairoExtensions.RealFontMultiplier =  Pango.Scale.PangoScale * resolution / 72;
			}
		}
		
		
		UnitType unitType;
		
		public UnitType Unit {
			get { return unitType; }
			set 
			{ 
				unitType = value;
				switch (unitType) {
				case UnitType.mm:
						UnitMultipilier = Resolution / 25.4;
					break;
				case UnitType.cm:
						UnitMultipilier = Resolution / 2.54;
					break;
				case UnitType.inch:
						UnitMultipilier = Resolution / 25.4;
					break;
				default:						
					break;
				}	
				
				CairoExtensions.UnitMultiplier = UnitMultipilier;
			}
			
		}
		
		
		public ReportRenderer ()
		{
			renderersDictionary = new Dictionary<Type, object>();
			unitMultipilier = 1;
		} 

		public void RegisterRenderer(Type t,IControlRenderer renderer) {	
			renderer.UnitMulitipier = unitMultipilier;
			renderersDictionary.Add(t,renderer);
		}

		public void RenderPage (Page p)
		{			 			
			for (int i = 0; i < p.Controls.Count; i++) {
				var control = p.Controls[i];
 					if(control.IsVisible)
						RenderControl (control);				 				
			}
			 
		}
 
		public Size MeasureControl (Control control)
		{
            Type controlType = control.GetType();
			if(renderersDictionary.ContainsKey(controlType)){
				var renderer = renderersDictionary[controlType] as IControlRenderer;                
				return renderer.Measure(context,control);								
			}
			return default(Size);
		}

        public void RenderControl(Control control )
		{
            Type controlType = control.GetType();
			if(renderersDictionary.ContainsKey(controlType)){
				var renderer = renderersDictionary[controlType] as IControlRenderer;							
				renderer.Render(context,control);								
			}
 
		}

		public Control[] BreakOffControlAtMostAtHeight(Control control, double height){
			Control[] controls = new Control[2];
			
			 Type controlType = control.GetType();
			if(renderersDictionary.ContainsKey(controlType)){
				var renderer = renderersDictionary[controlType] as IControlRenderer;							
				 controls = renderer.BreakOffControlAtMostAtHeight(context,control,height);
			}
			
			return controls;
		}
		
	}
}

