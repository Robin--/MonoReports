// 
// ControlViewFactory.cs
//  
// Author:
//       Tomasz Kubacki <Tomasz.Kubacki (at) gmail.com>
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
using System.Collections.Generic;
using MonoReports.ControlView;
using MonoReports.Model.Controls;
using MonoReports.Core;
using System.Linq;
using MonoReports.Services;
using MonoReports.Renderers;


namespace MonoReports.Core
{
	public class ControlViewFactory : IControlViewFactory
	{
	
		ReportRenderer reportRenderer = null;
		
		public ReportRenderer ReportRenderer {
			get {
				return this.reportRenderer;
			}
			set {
				reportRenderer = value;
			}
		}

		public ControlViewFactory (ReportRenderer reportRenderer)
		{
			this.reportRenderer = reportRenderer;
			controlViewDictionary = new Dictionary<System.Type, Func<MonoReports.Model.Controls.Control,SectionView,ControlViewBase> >();
			
			controlViewDictionary
				.Add(
				     typeof(TextBlock),
				     	(ctrl, section) => {
							 	var renderer = reportRenderer.RenderersDictionary[typeof(TextBlock)] as TextBlockRenderer;
			                    return new TextBlockView(ctrl as TextBlock,renderer,section);
						}
				);
			
			controlViewDictionary
				.Add(
				     typeof(Line),
				     	(ctrl, section) => {
								var renderer = reportRenderer.RenderersDictionary[typeof(Line)] as LineRenderer;
			                    return new LineView(ctrl as Line,renderer,section);
						}
				);
			
			controlViewDictionary
				.Add(
				     typeof(Image),
				     	(ctrl, section) => {
								var renderer = reportRenderer.RenderersDictionary[typeof(Image)] as ImageRenderer;
								var image = ctrl as Image;																			
			                    return new ImageView(image,renderer,section);
						}
				);
			
			controlViewDictionary
				.Add(
				     typeof(SubReport),
				     	(ctrl, section) => {
								var renderer = reportRenderer.RenderersDictionary[typeof(Image)] as SubreportRenderer;
								var subreport = ctrl as SubReport;					
								return new SubreportView(subreport,renderer,section);
						}
				);
			
		}
		
		private Dictionary<System.Type, Func<MonoReports.Model.Controls.Control,SectionView,ControlViewBase> > controlViewDictionary;

		#region IControlViewFactory implementation
		
		public ControlViewBase CreateControlView (MonoReports.Model.Controls.Control control, SectionView sectionView) 	
		{			
			return controlViewDictionary[control.GetType()](control,sectionView);
		}
		
		#endregion
}
	
	 
}

