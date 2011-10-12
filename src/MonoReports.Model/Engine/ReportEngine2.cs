// 
// ReportEngine2.cs
//  
// Author:
// Tomasz Kubacki <tomasz.kubacki@gmail.com>
// 
// Copyright (c) 2011 tomasz kubacki
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
using System.Collections.Generic;
using MonoReports.Model.Data;

namespace MonoReports.Model.Engine
{
	public class ReportEngine2 : IReportEngine
	{
		public ProcessingState ProcessingState {get;set;}
		
		IReportRenderer renderer;
		Report report;
		IDataSource dataSource;
		int pageNumber = 0;
		double currentPageSpan;
		Page currentPage;
		Section currentSection;		
		ProcessedControl currentControl;		
		List<ProcessedControl> controls;
		bool dataSourceHasNextRecord;
		double height;
		internal ReportContext reportContext;
		
		public ReportEngine2 (Report report, IReportRenderer renderer)
		{
			this.renderer = renderer;
			this.report = report;
			dataSource = report.DataSource;
			controls = new List<ProcessedControl> ();	
			ProcessingState = ProcessingState.BeforeProcessing;
			reportContext = new ReportContext(report) 
			{ 
				RendererContext = renderer.RendererContext,
				CurrentPageIndex = 0,
				DataSource = report.DataSource,
				ReportMode = ReportMode.Preview 
			};
		}
		
		void initProcessing() {			
			nextRecord ();			
			ProcessingState = ProcessingState.Processing;
		}
		
		void finishProcessing () {
			if (dataSource != null)
				dataSource.Reset ();
						 
//			foreach (var dc in controlsToEvalAfterReportProcessing) {
//				try {
//					if (reportContext.ExpressionFieldsDict.ContainsKey (dc.FieldName)) 
//						dc.Text = reportContext.ExpressionFieldsDict [dc.FieldName].GetStringValue ("",dc.FieldTextFormat);
//				} catch {}						
//			}
			ProcessingState = ProcessingState.Finished;
			report.FireOnAfterReportProcessing(reportContext);
			
		}
		
		public void Process ()
		{
			initProcessing();			
			while (ProcessingState != ProcessingState.Finished) {				
				ProcessPage();				
			}			 
			finishProcessing();
		}
		
		public void ProcessPage () {
			currentPage = new Page(){ PageNumber = ++pageNumber };
			height = report.Height;			
			if (report.PageHeaderSection.IsVisible) {
				selectSection(report.PageHeaderSection);
				processSection();
			}
			
			if (report.ReportHeaderSection.IsVisible) {
				selectSection(report.ReportHeaderSection);					
			}
			
			
			if (report.PageFooterSection.IsVisible) {
				currentSection = report.PageFooterSection;
				processSection ();
			}
			
			
			if (dataSourceHasNextRecord) {
				
				selectSection(report.DetailSection);
				while (processSection())
				{
					selectSection(report.DetailSection);
					nextRecord();
				}
			
			}
			
			if(!dataSourceHasNextRecord) {
				selectSection(report.ReportFooterSection);												
				processSection();
			}

		}

		bool processSection () {
			
			for (int i = 0; i < controls.Count; i++) {
				ProcessedControl pc = controls[i];
				Control c = pc.Control;				
				Size size = renderer.MeasureControl(c);
				pc.GrowHeight = size.Height - c.Height;
			}
			//Size size = renderer.MeasureControl ();
			//double bottom = size.Height + proce	
			
			return false;
		}
			
		void selectSection (Section sect) {
			currentSection = sect;
			foreach(var c in sect.Controls) {
				controls.Add (new ProcessedControl () { Control = c });
			}
		}
		
		Section getBackgroundSectionControl () {
			var c = currentSection.CreateControl () as Section;
			c.Controls.Clear ();
			c.TemplateControl = currentSection.TemplateControl;
			return c;
		}
		
		void addControlsToCurrentPage (List<Control> controls)
		{
			foreach (var control in controls) {				
				currentPage.Controls.Add (control);
			}
		}
		
		void nextRecord() {
			if(dataSource == null)
				dataSourceHasNextRecord = false;
			else
				dataSourceHasNextRecord = dataSource.MoveNext();
		}		
		
	}	 
	
 
	public enum ProcessingState {
		BeforeProcessing, 
		Processing,
		Suspended,
		Finished
	}

	public class ProcessedControl {		

		public Control Control {get;set;}
		
		public double Span {get;set;}
		
		public double GrowHeight {get;set;}

		public bool WasProcessed {get;set;}
		
		public virtual void Process () {
			WasProcessed = true;
		}
	}	
	
	public class SubreportControl : ProcessedControl {
		
		public ReportEngine2 Engine {get;set;}
		
		public override void Process ()
		{
			Engine.ProcessPage();
		}
		
	}
	
}

