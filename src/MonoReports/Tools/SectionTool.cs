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
	public class SectionTool : BaseTool
	{
		SectionView currentSection = null;
        public bool IsGripperPressed { get; set; }


		public SectionTool (DesignService designService) : base(designService)
		{
			
		}

		public override void OnBeforeDraw (Context c)
		{
			
			
		}	
				
		
		public override void OnMouseMove(){
			if (designService.IsPressed) {

                if (designService.IsMoving && (designService.SelectedTool as SectionTool) != null)
                {
                    if ((designService.SelectedTool as SectionTool).IsGripperPressed)
                    {
                        var section = designService.SelectedControl as SectionView;
						Size newSize = new Size(section.ControlModel.Width, section.ControlModel.Height + designService.DeltaPoint.Y);
						if(newSize.Height >= section.MinHeight)
                        	section.ControlModel.Size = newSize;
						else{
							section.ControlModel.Size = new Size(newSize.Width,section.MinHeight);
					}
                    }
				} 
				
			}else if (designService.MouseOverSection != null) {
				if(designService.MouseOverSection.GripperAbsoluteBound.ContainsPoint(designService.MousePoint.X,designService.MousePoint.Y)) {
					designService.MouseOverSection.IsSectionGripperHighligted = true;
					designService.WorkspaceService.SetCursor (Gdk.CursorType.BottomSide);	
				}else {
					designService.MouseOverSection.IsSectionGripperHighligted = false;
					designService.WorkspaceService.SetCursor (Gdk.CursorType.LeftPtr);	
				}
			}
		}		
				
		
		public override string Name {get {return "SectionTool"; }}
		
		public override string ToolBarToolTip {
			get { return Mono.Unix.Catalog.GetString ("Section"); }
		}
		
		public override bool IsToolbarTool {
			get {
				return false;
			}
		}
 
		public override void OnAfterDraw (Context c)
		{
			if(currentSection != null)
				c.FillRectangleInUnit (currentSection.GripperAbsoluteBound, currentSection.SectionGripperColor);
		}
		

		public override void OnMouseDown ()
		{
			currentSection = designService.SelectedControl as SectionView;
		}
	 
		public override void OnMouseUp ()
		{			 
			 
			double y = 0;
			SectionView previousSection = null;
			int counter = 0;
			foreach (var sectionView in designService.SectionViews) {
				
				if(counter > 0){
					sectionView.ControlModel.Location = new MonoReports.Model.Point(sectionView.ControlModel.Location.X,y);						
					sectionView.SectionSpan =  new Cairo.PointD (sectionView.ControlModel.Location.X, previousSection.AbsoluteBound.Y + previousSection.AbsoluteBound.Height);
					sectionView.InvalidateBound();
					y+=sectionView.ControlModel.Size.Height;
					
				}else{
					y  = sectionView.ControlModel.Location.Y + sectionView.ControlModel.Size.Height;		
					sectionView.InvalidateBound();
				}
				previousSection = sectionView;
				counter++;
			}
            IsGripperPressed = false;
		 	designService.InvalidateDesignHeight();				
		}
		
	}
}

