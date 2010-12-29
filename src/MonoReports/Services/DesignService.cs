// ReportView.cs
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
using MonoReports.Model;
using System.Collections.Generic;
using Cairo;
using MonoReports.Core;
using MonoReports.Extensions.CairoExtensions;
using MonoReports.Model.Controls;
using System.Collections.ObjectModel;
using MonoReports.Tools;
using MonoReports.ControlView;
using MonoReports.Model.Data;
using MonoReports.Renderers;


namespace MonoReports.Services
{
	public sealed class DesignService
	{
		double zoom;
		public double Zoom { 
			get { return zoom; } 
			set { 
				zoom = value; 
				if (Report != null) {
					width = (int)(Report.Width * Zoom);
					height = (int)(Report.Height * Zoom);
				}
				if(OnZoomChanged != null)
					OnZoomChanged(this,new EventArgs());
			} }
		
		public event ZoomChanged OnZoomChanged;
		double width;
		public double Width { get { return width; } set { width = value; } }

		double height;
		public double Height { get { return height; } set { height = value; } }

		public BaseTool SelectedTool {
			get { return ToolBoxService.SelectedTool; } 
			set {
				ToolBoxService.SelectedTool = value; 
			}
		}

		public Cairo.PointD StartPressPoint {
			get;
			private set;
		}

		public Cairo.PointD EndPressPoint { get; private set; }

		public Cairo.PointD MousePoint { get; private set; }

		public Cairo.PointD PreviousMousePoint { get; private set; }

		public Cairo.PointD DeltaPoint { get; private set; }

		public bool IsPressed { get; private set; }

		public bool IsMoving { get; private set; }

		bool isDesign;
		public bool IsDesign { 
			get { return isDesign; } 
			set { 
				isDesign = value;
				if(renderer != null)
					renderer.IsDesign = isDesign;
			} 
		}

		public bool Render { get; private set; }
		
		private ReportRenderer renderer;

		public ReportRenderer Renderer {
			get {
				return this.renderer;
			}
			set {
				renderer = value;
			}
		}

		public IWorkspaceService WorkspaceService { get; set; }

		public ToolBoxService ToolBoxService{get; set;}
		public event SelectedControlChanged OnSelectedControlChanged;		
		public event ReportDataFieldsRefreshed OnReportDataFieldsRefreshed;				
		public event ReportChanged OnReportChanged;
		
		
		public SectionView MouseOverSection{get;set;}

		ControlViewBase selectedControl;		

		public ControlViewBase SelectedControl { 
			get { return selectedControl; } 
			set { 
				selectedControl = value; 
				if (selectedControl != null) {
					ToolBoxService.SetToolByControlView (selectedControl);
				} else {
					ToolBoxService.UnselectTool ();
				}
				if (OnSelectedControlChanged != null)
					OnSelectedControlChanged (this, new EventArgs ());
			} 
		}

		BaseTool mouseOverTool = null;
		
		ControlViewFactory controlViewFactory;
		Report report;

		public Report Report {
			get { return report;} 
			set {
				report = value;
				initReport ();
				if (OnReportChanged != null)
					OnReportChanged (this, new EventArgs ());
			}
		}

		private List<SectionView> sectionViews;

		public IList<SectionView> SectionViews {
			get { return sectionViews; }
			private set {
				;
			}
		}

		public PixbufRepository PixbufRepository {get; set;}

		public DesignService (IWorkspaceService workspaceService,ReportRenderer renderer,PixbufRepository pixbufRepository, Report report)
		{		
			this.PixbufRepository = pixbufRepository;
			this.WorkspaceService = workspaceService;
			this.renderer = renderer;
			controlViewFactory = new ControlViewFactory (renderer);			
			IsDesign = true;
			Report = report;
			Zoom = 1;
			Render = true;		
			
		}

		void initReport ()
		{
			
			PixbufRepository.Report = report;			
			sectionViews = new List<SectionView> ();
			addSectionView (report.ReportHeaderSection);
			addSectionView (report.PageHeaderSection);
			foreach (var groupHeader in report.GroupHeaderSections) {
				addSectionView (groupHeader);
			}
			addSectionView (report.DetailSection);
			foreach (var groupFooter in report.GroupFooterSections) {
				addSectionView (groupFooter);
			}
			
			addSectionView (report.PageFooterSection);
			addSectionView (report.ReportFooterSection);

		}

		public void RedrawReport (Context c)
		{
			if (Zoom != 1) {
				c.Scale (Zoom, Zoom);				
			}			
			
			if (SelectedTool != null) {
				SelectedTool.OnBeforeDraw (c);
			}
			for (int i = 0; i < SectionViews.Count; i++) {
				var renderedSection = SectionViews [i];
				renderedSection.Render (c);								
			}
			if (SelectedTool != null) {
				SelectedTool.OnAfterDraw (c);
			}
			
		}

		public void CreateTextBlockAtXY (string text, string fieldName, FieldKind fieldKind, double x, double y)
		{			
			var point = new Cairo.PointD (x / (Zoom * Renderer.UnitMultipilier), y / (Zoom * Renderer.UnitMultipilier));
			var sectionView = getSectionViewByXY (x, y);
			
			if (sectionView != null) {
				var localpoint = sectionView.PointInSectionByAbsolutePoint (point);	
				ToolBoxService.SetToolByName ("TextBlockTool");							
				SelectedTool.CreateNewControl (sectionView);
				var textBlock = (SelectedControl.ControlModel as TextBlock);
				textBlock.Text = fieldName;
				textBlock.FieldName = fieldName;
				textBlock.FieldKind = fieldKind;
				textBlock.Location = new MonoReports.Model.Point (localpoint.X,localpoint.Y);
				SelectedTool.CreateMode = false;
			}
		}

		public void CreateImageAtXY (string imageKey,double x, double y)
		{
			var point = new Cairo.PointD (x / (Zoom * Renderer.UnitMultipilier), y / (Zoom * Renderer.UnitMultipilier));
			PixbufRepository.AddOrUpdatePixbufByName(imageKey);
			var sectionView = getSectionViewByXY (x, y);
			var localpoint = sectionView.PointInSectionByAbsolutePoint (point);
			ToolBoxService.SetToolByName ("ImageTool");	
			SelectedTool.CreateNewControl (sectionView);
			var image = (SelectedControl.ControlModel as Image);
			image.ImageKey = imageKey;
			image.Location = new MonoReports.Model.Point (localpoint.X,localpoint.Y);
			SelectedTool.CreateMode = false;
		}

		SectionView getSectionViewByXY (double x, double y)
		{
			var point = new Cairo.PointD (x / (Zoom * Renderer.UnitMultipilier), y / (Zoom * Renderer.UnitMultipilier));
			SectionView sectionView = null;
			
			for (int i = 0; i < SectionViews.Count; i++) {
				var retSectionView = SectionViews [i];
					
				if (retSectionView.AbsoluteBound.ContainsPoint (point.X, point.Y)) {
						
					if (retSectionView.HeaderAbsoluteBound.ContainsPoint (point.X, point.Y)) {
						SelectedControl = retSectionView;
						continue;
					}
					sectionView = retSectionView; 
					break;
				}
			}
			
			
			return sectionView;
		}

		public void RefreshDataFieldsFromDataSource ()
		{
			Report.FillFieldsFromDataSource ();
			if (OnReportDataFieldsRefreshed != null)
				OnReportDataFieldsRefreshed (this, new EventArgs ());
		}

		public void KeyPress (Gdk.Key key)
		{
			if (SelectedTool != null) {
				SelectedTool.KeyPress (key);
			}
		}

		public void DeleteSelectedControl ()
		{
			if (selectedControl != null) {
				SelectedControl.ParentSection.RemoveControlView (selectedControl);				
				SelectedControl.ControlModel = null;
				SelectedControl = null;
				WorkspaceService.InvalidateDesignArea ();			
			}
		}

		public void ButtonPress (double x, double y, int clicks)
		{
			StartPressPoint = new Cairo.PointD (x / (Zoom * Renderer.UnitMultipilier), y / (Zoom * Renderer.UnitMultipilier));
						
			IsPressed = true;
			IsMoving = false;
			
			if (!IsMoving) {
				PreviousMousePoint = StartPressPoint;
				DeltaPoint = new PointD (0, 0);
				for (int i = 0; i < SectionViews.Count; i++) {
					var sectionView = SectionViews [i];
	
					if (sectionView.AbsoluteBound.ContainsPoint (StartPressPoint.X, StartPressPoint.Y)) {
						
						if (sectionView.HeaderAbsoluteBound.ContainsPoint (StartPressPoint.X, StartPressPoint.Y)) {
							SelectedControl = sectionView;
							SelectedTool = null;
							continue;
						} else if (sectionView.GripperAbsoluteBound.ContainsPoint (StartPressPoint.X, StartPressPoint.Y)) {
							SelectedControl = sectionView;
									
						} else {
							
							if (SelectedTool != null && SelectedTool.CreateMode) {
								SelectedTool.CreateNewControl (sectionView);
								SelectedTool.CreateMode = false;
							} else {
							
								SelectedControl = null;
							
								for (int j = 0; j < sectionView.Controls.Count; j++) {
									var controlView = sectionView.Controls [j];
									if (controlView.ContainsPoint (StartPressPoint.X, StartPressPoint.Y)) {
										SelectedControl = controlView;										 
										break;
									}
								}
							}
						}
					
							
					}
				}
					
			}

			
			if (SelectedTool != null) {
				if (clicks == 1) {
					SelectedTool.OnMouseDown ();
				} else {
					SelectedTool.OnDoubleClick ();
				}
			}
			
			WorkspaceService.InvalidateDesignArea ();
			
		}
		
		
		public void Load(string path){
			Report.Load(path);
			initReport ();
				if (OnReportChanged != null)
					OnReportChanged (this, new EventArgs ());
		}
		
		public void Save(string path){
			
			if ( string.IsNullOrEmpty (Report.Title))
			{
				Report.Title = System.IO.Path.GetFileName(path);	
			}
			
			Report.Save(path);
		}
		 
		public void MouseMove (double x, double y)
		{
			
			MousePoint = new Cairo.PointD (x / (Zoom * Renderer.UnitMultipilier), y / (Zoom * Renderer.UnitMultipilier));
			IsMoving = true;
			if (SelectedTool != null)
				SelectedTool.OnMouseMove ();
			DeltaPoint = new PointD (-PreviousMousePoint.X + MousePoint.X, -PreviousMousePoint.Y + MousePoint.Y);	
			
			
			for (int i = 0; i < SectionViews.Count; i++) {					
					var sectionView = SectionViews [i];		
					if(sectionView.ContainsPoint(MousePoint.X,MousePoint.Y)) {						 					
					
						MouseOverSection = sectionView;
						MouseOverSection.IsMouseOver = true;
					
						if(mouseOverTool == null || sectionView.DefaultToolName != mouseOverTool.Name) {						
							mouseOverTool =  ToolBoxService.GetToolByName(sectionView.DefaultToolName);								
						}
							
						if(SelectedTool == null || mouseOverTool != SelectedTool )
							mouseOverTool.OnMouseMove();
					} else {
						if(!IsPressed)
							sectionView.IsMouseOver = false;
					}
			}
			 
			
			WorkspaceService.InvalidateDesignArea (); 
			PreviousMousePoint = MousePoint;
		}
		

		public void ZoomChanged (double zoom)
		{
			Zoom = zoom;
			WorkspaceService.InvalidateDesignArea (); 
		}

		public void ButtonRelease (double x, double y)
		{
			EndPressPoint = new Cairo.PointD (x / Zoom, y / Zoom);
			IsPressed = false;
			IsMoving = false;
			if (SelectedTool != null) {
				SelectedTool.OnMouseUp ();
				
			}
			
			if (SelectedControl != null) {
				WorkspaceService.ShowInPropertyGrid (SelectedControl.ControlModel);
			}
			
			WorkspaceService.InvalidateDesignArea (); 			
		}

		 

		private void addSectionView (Section section)
		{
			Cairo.PointD sectionSpan;
			if (sectionViews.Count > 0) {
				var previousSection = sectionViews [sectionViews.Count - 1];
				sectionSpan = new Cairo.PointD (0, previousSection.AbsoluteBound.Y + previousSection.AbsoluteBound.Height);
			} else {
				sectionSpan = new Cairo.PointD (0, 0);
			}
			var sectionView = new SectionView (controlViewFactory, section, sectionSpan);
			sectionViews.Add (sectionView);
			Height = sectionView.AbsoluteBound.Y + sectionView.AbsoluteBound.Height;
		}

		public void ExportToPdf ()
		{
			
			Gtk.FileChooserDialog fc = new Gtk.FileChooserDialog ("Choose the pdf file to save", null, Gtk.FileChooserAction.Save, "Cancel", Gtk.ResponseType.Cancel, "Export", Gtk.ResponseType.Accept);
			var fileFilter = new Gtk.FileFilter { Name = "pdf file" };
			fileFilter.AddPattern ("*.pdf");
			
			fc.AddFilter (fileFilter);
			fc.CurrentName = string.IsNullOrEmpty( Report.Title) ? "untitled_report.pdf" : Report.Title ;
				
			 
			if (fc.Run () == (int)Gtk.ResponseType.Accept) {
				 
				Report.ExportToPdf(fc.Filename);
			}
		
			fc.Destroy ();
			
		}
 
	}
	
	
}

