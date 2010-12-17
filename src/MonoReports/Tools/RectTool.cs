// 
// SelectAndResizeTool.cs
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
using MonoReports.ControlView;
using Cairo;
using MonoReports.Model;
using MonoReports.Core;
using MonoReports.Extensions.CairoExtensions;
using MonoReports.Model.Controls;
using MonoReports.Services;

namespace MonoReports.Tools
{
	internal enum GripperType
	{
		NE,
		NW,
		SW,
		SE
	}

	public class RectTool : BaseTool
	{

		double gripSize = 4;
		bool isResizing;
		Border selectBorder;
		GripperType gripperType;

		public RectTool (DesignService designService) : base(designService)
		{
			selectBorder = new Border ();
			selectBorder.Color = new MonoReports.Model.Color(0,0,0);
			selectBorder.WidthAll = 1 ;
		
		}

		public override void OnBeforeDraw (Context c)
		{
 
			
		}
		
		double cx,cy,cw,ch,sw, sh;
		
		public override void OnMouseMove ()
		{
			if (designService.IsPressed && designService.IsMoving && designService.SelectedControl != null) {
				var control = designService.SelectedControl;
				var location = control.ControlModel.Location;
				cx = location.X;
				cy = location.Y;
				cw = control.ControlModel.Size.Width;
				ch = control.ControlModel.Size.Height;
				sw = control.ParentSection.Section.Width;
				sh = control.ParentSection.Section.Height;			
					
				if (designService.IsMoving) {
					double w,h,x,y;
					if (!isResizing) {
						x = Math.Max(0, cx + designService.DeltaPoint.X);
						y = Math.Max(0, cy + designService.DeltaPoint.Y);
						x = Math.Min(sw-cw,x);
						y = Math.Min(sh-ch,y);						
						var point = new MonoReports.Model.Point (x,y);
						control.ControlModel.Location = point;
					} else {
						
						switch (gripperType) {
						case GripperType.NE:
							w = Math.Min( Math.Abs (cw + designService.DeltaPoint.X) , sw);
							h = Math.Min( Math.Abs (ch - designService.DeltaPoint.Y) , sh);
							y = Math.Max( cy + designService.DeltaPoint.Y,0);
							control.ControlModel.Size = new Size (w, h);
							control.ControlModel.Location = new MonoReports.Model.Point (cx, y);
							break;
						case GripperType.SE:
							w = Math.Min( Math.Abs (cw + designService.DeltaPoint.X) , sw);
							h = Math.Min( Math.Abs (ch + designService.DeltaPoint.Y) , sh - cy);
							control.ControlModel.Size = new Size (w,h);
							break;
						case GripperType.SW:
							w = Math.Min( Math.Abs (cw - designService.DeltaPoint.X) , sw);
							h = Math.Min( Math.Abs (ch + designService.DeltaPoint.Y) , sh - cy);
							x = Math.Max( location.X + designService.DeltaPoint.X,0);
							control.ControlModel.Size = new Size (w,h);
							control.ControlModel.Location = new MonoReports.Model.Point (x, location.Y);
							break;
						case GripperType.NW:
							w = Math.Min( Math.Abs (cw - designService.DeltaPoint.X) , sw);
							h = Math.Min( Math.Abs (ch - designService.DeltaPoint.Y) , sh - cy);
							x = Math.Max( location.X + designService.DeltaPoint.X,0);
							y = Math.Max( location.Y + designService.DeltaPoint.Y,0);
							control.ControlModel.Size = new Size (w,h);
							control.ControlModel.Location = new MonoReports.Model.Point (x,y);
							break;
						default:
							break;
						}
						
						
					}
				}
				
			}
		}
		
		public override string Name {get {return "RectTool"; }}
		
		public override bool IsToolbarTool {
			get {
				return false;
			}
		}
		 
		public override void OnAfterDraw (Context c)
		{
			if (designService.SelectedControl != null && designService.IsDesign) {
				c.Save ();
				c.SetDash (new double[] { 1.0 ,1,1}, 2);
				c.DrawInsideBorderInUnit (designService.SelectedControl.AbsoluteBound, selectBorder, true);
				c.DrawSelectBoxInUnits (designService.SelectedControl.AbsoluteBound,gripSize);
				c.Restore ();
			}
		}

		public override void OnMouseDown ()
		{
			if (designService.SelectedControl != null) {
				
				var control = designService.SelectedControl;
				double cw = control.ControlModel.Size.Width;
				double ch = control.ControlModel.Size.Height;
				var pointInSection = control.ParentSection.PointInSectionByAbsolutePoint (designService.MousePoint);
				var location = control.ControlModel.Location;
				isResizing = false;
				
				
				if (pointInSection.Y > location.Y && pointInSection.Y < location.Y + gripSize) {
					if (pointInSection.X > location.X && location.X + gripSize > pointInSection.X) {
						isResizing = true;
						gripperType = GripperType.NW;
					} else if (pointInSection.X > location.X + cw - gripSize && location.X + cw > pointInSection.X) {
						isResizing = true;
						gripperType = GripperType.NE;
					}
					
				} else if (pointInSection.Y > location.Y + ch - gripSize && pointInSection.Y < location.Y + ch) {
					if (pointInSection.X > location.X && location.X + gripSize > pointInSection.X) {
						isResizing = true;
						gripperType = GripperType.SW;
					} else if (pointInSection.X > location.X + cw - gripSize && location.X + cw > pointInSection.X) {
						isResizing = true;
						gripperType = GripperType.SE;
					}
				}
			}
		}

		public override void OnMouseUp ()
		{
			isResizing = false;
		}
		
		
		
	}
}

