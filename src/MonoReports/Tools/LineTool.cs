// 
// LineTool.cs
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
using MonoReports.Core;
using MonoReports.Extensions.CairoExtensions;
using MonoReports.ControlView;
using Cairo;
using MonoReports.Model.Controls;
using MonoReports.Services;
using MonoReports.Gui.Widgets;
using MonoReports.Model;

namespace MonoReports.Tools
{
	public class LineTool : BaseTool
	{

		protected bool startPointHit;
		protected bool endPointHit;
		protected Line line;
		protected SectionView currentSection = null;
		double lineDistance = 2;							

		public LineTool (DesignService designService) : base(designService)
		{		
			designService.OnZoomChanged += HandleDesignServiceOnZoomChanged;
			lineDistance = 2 / designService.Zoom;
		}

		void HandleDesignServiceOnZoomChanged (object sender, EventArgs e)
		{
			 lineDistance = 2 / designService.Zoom;
		}
		
		
		protected virtual Line createLine(double x, double y) {
			var l = new Line (){ 	
				
				Location = new MonoReports.Model.Point(x,y),
				End = new MonoReports.Model.Point(x,y)
				};
			 
			return l;
		}

		public override void CreateNewControl (SectionView sectionView)
		{
			
			var startPoint = sectionView.PointInSectionByAbsolutePoint (designService.StartPressPoint.X, designService.StartPressPoint.Y);
			
			var l = createLine(startPoint.X,startPoint.Y);
			var lineView = sectionView.CreateControlView (l);			
			sectionView.Section.Controls.Add (l);
			lineView.ParentSection = sectionView;
			designService.SelectedControl = lineView;			 
		}

		public override void OnBeforeDraw (Context c)
		{
			
		
		}

		public override void OnMouseMove ()
		{
			if (designService.IsPressed) {
				var control = designService.SelectedControl;
				double realWidth = line.LineWidth * designService.Renderer.UnitMultipilier;
				realWidth = realWidth > 1 ? realWidth : 1;
				realWidth /=designService.Renderer.UnitMultipilier;
				
 				double halfRealLineWidth = (realWidth / 2);
				
				if (designService.IsMoving && control != null) {
					
					double correctionX =  (line.End.Y == line.Location.Y ? 0 : halfRealLineWidth);
					double correctionY =  (line.End.X == line.Location.X ? 0 : halfRealLineWidth);
					
					double x =  Math.Max (correctionX, line.Location.X + designService.DeltaPoint.X);
					double y = Math.Max (correctionY, line.Location.Y + designService.DeltaPoint.Y);
					double x1 = Math.Max (correctionX, line.End.X + designService.DeltaPoint.X);
					double y1 = Math.Max (correctionY, line.End.Y + designService.DeltaPoint.Y);
					
					
					x = Math.Min(x,control.ParentSection.Section.Width - correctionX);				
					y = Math.Min(y,control.ParentSection.Section.Height  - correctionY);
					x1 = Math.Min(x1,control.ParentSection.Section.Width - correctionX);
					y1 = Math.Min(y1,control.ParentSection.Section.Height - correctionY);
					
					
 
					if (startPointHit) {
						
						switch (line.LineMode) {
							
							case LineMode.Vertical:
								line.Location = new MonoReports.Model.Point (line.Location.X,y);	
							break;
							
							case LineMode.Horizontal:
								line.Location = new MonoReports.Model.Point (x,line.Location.Y);		
							break;
							
							default:
								line.Location = new MonoReports.Model.Point (x,y);									
							break;
						}												
						
					} else if (endPointHit) {
						
						switch (line.LineMode) {
							
							case LineMode.Vertical:
								line.End = new MonoReports.Model.Point (line.End.X,y1);	
							break;
							
							case LineMode.Horizontal:
								line.End = new MonoReports.Model.Point (x1,line.End.Y);		
							break;
							
							default:
								line.End = new MonoReports.Model.Point (x1,y1);									
							break;
						}		
												
					} else {						
						line.Location = new MonoReports.Model.Point (x,y);						
						line.End = new MonoReports.Model.Point (x1,y1);
					}

				}
				
			}
		}

		public override string Name {
			get { return "LineTool"; }
		}
		
		public override bool IsToolbarTool {
			get {
				return true;
			}
		}
		
		public override string ToolbarImageName {
			get {
				return "ToolLine.png";
			}
		}

		public override void OnAfterDraw (Context c)
		{
				
			if (designService != null && designService.SelectedControl != null && designService.IsDesign) {
				var p1 = designService
					.SelectedControl
					.ParentSection
					.AbsolutePointByLocalPoint (line.Location.X, line.Location.Y);
				
				var p2 = designService
					.SelectedControl
					.ParentSection
					.AbsolutePointByLocalPoint (line.End.X, line.End.Y);
                c.DrawGripperWithLocationInUnit(p1.X,p1.Y, 1.2 / (designService.Zoom / 2));
                c.DrawGripperWithLocationInUnit(p2.X,p2.Y, 1.2 / (designService.Zoom / 2));
			}
		}

		public override void OnMouseDown ()
		{
            
			currentSection = designService.SelectedControl != null ? designService.SelectedControl.ParentSection : null;
			if (designService.SelectedControl != null) {
                
				line = designService.SelectedControl.ControlModel as Line;
				var location = line.Location;
				var startPoint = currentSection.PointInSectionByAbsolutePoint (designService.StartPressPoint);
			
				Cairo.PointD startDistance = new Cairo.PointD ( location.X - startPoint.X,  location.Y - startPoint.Y);
				Cairo.PointD endDistance = new Cairo.PointD (line.End.X - startPoint.X,line.End.Y - startPoint.Y);
			
				if (startDistance.X < lineDistance && startDistance.X > -lineDistance && startDistance.Y < lineDistance && startDistance.Y > -lineDistance) {
					startPointHit = true;
				} else {
					if (endDistance.X < lineDistance && endDistance.X > -lineDistance && endDistance.Y < lineDistance && endDistance.Y > -lineDistance)
						endPointHit = true;
				}
			}
		}

		public override void OnMouseUp ()
		{
			startPointHit = false;
			endPointHit = false;
			CreateMode = false;
		}
		
		
		
		
		
	}
	
	
	
	
}

