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
using System.Linq;
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
		
		Page currentPage;
		ProcessedSection currentSection;		
		
		Dictionary<string, ProcessedSection> dalayedSections;		
		bool dataSourceHasNextRecord;
		double pageHeight;
		double pageHeightLeft;		
		internal ReportContext reportContext;		

		
		public ReportEngine2 (Report report, IReportRenderer renderer)
		{
			this.renderer = renderer;
			this.report = report;
			dataSource = report.DataSource;			
			dalayedSections = new Dictionary<string, ProcessedSection>();			
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
			pageHeight= report.Height;
			pageHeightLeft = pageHeight;
			
			if (report.PageHeaderSection.IsVisible) {
				selectSection(report.PageHeaderSection);
				processSection();			
			}
			
			if (report.ReportHeaderSection.IsVisible) {
				selectSection(report.ReportHeaderSection);
				processSection();
			}
			
			
			if (report.PageFooterSection.IsVisible) {
				selectSection(report.PageFooterSection);
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
			
			double processingBar = 0;
			double span = 0;
			bool returnVal = true;
			double maxHeight = currentSection.Height;			
			
			
			for (int i = 0; i < currentSection.Controls.Count; i++) {
				ProcessedControl pc = currentSection.Controls[i];
				returnVal = pc.Process(renderer,maxHeight);				
			}			
			pageHeightLeft -= currentSection.Height;
			
			return returnVal;
		}
			
		void selectSection (Section sect) {
			if(dalayedSections.ContainsKey(sect.Name))
				currentSection = dalayedSections[sect.Name];
			else {
				var section = sect.CreateControl() as Section;
				currentSection = new ProcessedSection() {
					Controls = new List<ProcessedControl>(),
					Section = section,
					MarginBottom = section.Height,
					MarginTop = 0
				};
				
				var topOrderedControls = section.Controls.OrderBy(c=>c.Top).ToList();
				if(topOrderedControls.Count > 0)
				{
					currentSection.MarginTop = topOrderedControls[0].Top;
					foreach(var c in topOrderedControls) {
						ProcessedControl pc = null;
						if(c  is SubReport)
							pc = new SubreportControl() {Control = c, Section = currentSection };
						else
							pc = new ProcessedControl () {Control = c, Section = currentSection };
						currentSection.Controls.Add (pc);
						double marginBottom = section.Height - c.Bottom;
						if(currentSection.MarginBottom < marginBottom)
							currentSection.MarginBottom = marginBottom;
					}
				}
			}
			 

			
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
	
	
	public class ProcessedSection {
		
		public double Top {get;set;}
		
		public double Height {
			get { return Section.Height; }
			set { Section.Height = value; }
		}		
		
		public double MarginTop {get;set;}
		
		public double MarginBottom {get;set;}		
		
		public Section Section {get;set;}	
		
		public List<ProcessedControl> Controls {get;set;}
		
		public List<Control> Buffer {get;set;}
		
		public void AddControlToBuffer(Control c) {
			Buffer.Add(c);
		}
	}

	public class ProcessedControl {	
		
		public ProcessedSection Section {get;set;}

		public Control Control {get;set;}				
		
		public double Span {get;set;}
		
		public double BottomAfterGrow {get;set;}	
		
		void setNewSizeAndLocation() {
			
		}
		
		public virtual bool Process(IReportRenderer renderer, double maxHeight) {			
			Size s = renderer.MeasureControl(Control);
			BottomAfterGrow = Control.Bottom + (Control.Height - Control.Height) + Span;
			bool retVal = false;
			
			if(BottomAfterGrow <= maxHeight) {
				Control.Size = new Size(Control.Width,s.Height);
				Control.Location = new Point(Control.Location.X,Control.Location.Y + Span);
				Section.AddControlToBuffer(Control);
				retVal = true;
			}else{
				if(!Section.Section.KeepTogether){
					if(Control.Top + Span < maxHeight){
						Control[] brokenControlParts = renderer.BreakOffControlAtMostAtHeight(Control, maxHeight);
						if(brokenControlParts[0] != null)
							Section.AddControlToBuffer(Control);
						Control = brokenControlParts[1];							
					}
				}					
			}
				
			return retVal;	
		}
	}	
	
	public class SubreportControl : ProcessedControl {
		
		public ReportEngine2 Engine {get;set;}	
		
		public override bool Process (IReportRenderer renderer, double maxHeight)
		{
			return true;
		}
		
	}
	
}

