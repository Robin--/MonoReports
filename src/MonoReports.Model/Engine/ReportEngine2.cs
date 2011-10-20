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
		public ProcessingState ProcessingState { get; set; }
		
		IReportRenderer renderer;
		Report report;
		IDataSource dataSource;
		int pageNumber = 0;
		Page currentPage;
		ProcessedSection currentSection;
		Dictionary<string, ProcessedSection> dalayedSections;		
		bool dataSourceHasNextRecord;
		bool pageBreak;
		double pageHeight;
		double pageHeightLeft;
		double pageHeightUsed;
		List<Tuple<double,double>> spanTable;
		internal ReportContext reportContext;
		bool reportHeaderFished = false;
		
		public ReportEngine2 (Report report, IReportRenderer renderer)
		{
			this.renderer = renderer;
			this.report = report;
			dataSource = report.DataSource;			
			dalayedSections = new Dictionary<string, ProcessedSection> ();
			spanTable = new List<Tuple<double, double>>();
			ProcessingState = ProcessingState.BeforeProcessing;
			reportContext = new ReportContext (report) 
			{ 
				RendererContext = renderer.RendererContext,
				CurrentPageIndex = 0,
				DataSource = report.DataSource,
				ReportMode = ReportMode.Preview 
			};
			
		}
		
		void initProcessing ()
		{	
			report.Pages.Clear();
			nextRecord ();			
			ProcessingState = ProcessingState.Processing;
		}
		
		void finishProcessing ()
		{
			if (dataSource != null)
				dataSource.Reset ();
			report.FireOnAfterReportProcessing (reportContext);			
		}
		
		public void Process ()
		{
			try {
			initProcessing ();			
			while (ProcessingState != ProcessingState.Finished) {				
				ProcessPage ();				
			}			 
			finishProcessing ();
			}catch (Exception exp) {
				Console.WriteLine(exp);
			}
		}
		
		public void ProcessPage ()
		{
			spanTable.Clear();
			currentPage = new Page (){ PageNumber = ++pageNumber };			
			report.FireOnBeforePageProcessing (reportContext,currentPage);
			pageHeight = report.Height;						
			pageHeightLeft = pageHeight;
				
			if(report.PageHeaderSection.IsVisible)
				pageHeightLeft -= report.PageHeaderSection.Height;
			if(report.PageFooterSection.IsVisible)
				pageHeightLeft -= report.PageFooterSection.Height;
			
			pageHeightUsed = 0;
			pageBreak = false;
			
			if (report.ReportHeaderSection.IsVisible && !pageBreak && !reportHeaderFished) {
				selectSection (report.ReportHeaderSection);
				reportHeaderFished = processSection ();				
			}	
			
			if (report.PageHeaderSection.IsVisible) {
				pageHeightLeft += report.PageHeaderSection.Height;
				selectSection (report.PageHeaderSection);
				processSection ();				
			}
 
			if (report.PageFooterSection.IsVisible) {
				pageHeightLeft += report.PageFooterSection.Height;
				selectSection (report.PageFooterSection);
				currentSection.AttachBottom = true;
				processSection ();
			}			
						
			while (pageHeightLeft > 0 && !pageBreak && (dataSourceHasNextRecord || dalayedSections.ContainsKey(report.DetailSection.Name))) {
					selectSection (report.DetailSection);
					processSection();
					nextRecord ();
			}			
									
			if (!dataSourceHasNextRecord && !pageBreak) {
				selectSection (report.ReportFooterSection);												
				processSection ();
			}
			
			if (!dataSourceHasNextRecord && dalayedSections.Count == 0)
				ProcessingState = ProcessingState.Finished;
			
			report.Pages.Add(currentPage);
		}

		bool  processSection ()
		{	
			ProcessedControl bottomMostAfterProcessing = null;
			double maxHeight = currentSection.Section.CanGrow ? pageHeightLeft : currentSection.Section.Height;			
			bool allControlsFitInSection = true;
			bool sectionSplitted = false;
				
			for (int i = 0; i < currentSection.Controls.Count; i++) {
				ProcessedControl pc = currentSection.Controls [i];	
				if(!pc.WasProcessed) {
					currentSection.Section.TemplateControl.FireBeforeControlProcessing(reportContext,pc.Control);
					double span = 0;
					
					//get span for control
					for (int j = 0; j < spanTable.Count; j++) {
						if(pc.Control.Top >= spanTable[j].Item1)
							span = span > spanTable[j].Item2 ? span : spanTable[j].Item2;
						else
							break;
					}
					bool controllFullyProcessed = pc.Process (renderer,span, maxHeight);
					
					if (!controllFullyProcessed) {
						allControlsFitInSection = false;
						dalayedSections [currentSection.Name] = currentSection;
						if(!currentSection.Section.KeepTogether)
							sectionSplitted = true;
					}
 
					if (pc.Grow > 0 ) {
											
						Tuple<double, double> spanToUpdate = null;
						int k;
						//a little tricky - should be simplified
						for (k = 0; k < spanTable.Count; k++){
							if(spanTable[k].Item1 == pc.BottomBeforeSpanAndGrow)
								spanToUpdate = spanTable[k];
							else if(spanTable[k].Item1 > pc.BottomBeforeSpanAndGrow)
								break;
						}
						
						if(spanToUpdate == null) {
							spanToUpdate = new Tuple<double, double>(pc.BottomBeforeSpanAndGrow,pc.Grow + pc.Span);
						}
						
						spanTable.Insert(k,spanToUpdate);
					}
					
					
					if(bottomMostAfterProcessing == null || bottomMostAfterProcessing.BottomAfterSpanAndGrow < pc.BottomAfterSpanAndGrow) {
						bottomMostAfterProcessing = pc;
					}
				}
				
			}
 
			/* compute section height */
			if (allControlsFitInSection) {
				if(bottomMostAfterProcessing != null) {
					double newHeight = bottomMostAfterProcessing.Control.Bottom + currentSection.MarginBottom;
					
					if(newHeight > currentSection.Height && currentSection.Section.CanGrow)
						currentSection.Height = newHeight;
					else if(newHeight < currentSection.Height && currentSection.Section.CanShrink) {
						currentSection.Height = newHeight;
					}					
				} else if(currentSection.Section.CanShrink)
					currentSection.Height = 0;
				
			}else {
				if(!currentSection.Section.KeepTogether)
					currentSection.Height = maxHeight;
			}
								
			if (allControlsFitInSection || !currentSection.Section.KeepTogether) {
				
				pageHeightUsed += currentSection.Height;
				pageHeightLeft -= currentSection.Height;
				
				if(currentSection.AttachBottom)
					currentSection.SectionSpan = pageHeight - currentSection.Height;
												
				foreach(var c in currentSection.PageBuffer)
					c.Top += currentSection.SectionSpan;
				
				var sectionBacground = currentSection.Section.CreateControl();				
				sectionBacground.Height = currentSection.Height;
				sectionBacground.Top = currentSection.SectionSpan;
				currentSection.PageBuffer.Add(sectionBacground);
 
				currentPage.Controls.AddRange (currentSection.PageBuffer);
				
				if(sectionSplitted) {
					currentSection.PageBuffer.Clear();
					foreach(var ctrl in currentSection.Controls) {
						if(ctrl.Control.Top >= maxHeight) {
							ctrl.Control.Top -= maxHeight;
							ctrl.Span = 0;
						}
					}
					currentSection.Height = 0;
				}
			}
			return allControlsFitInSection;
 
		}
			
		void selectSection (Section sect)
		{
			if (dalayedSections.ContainsKey (sect.Name)) {
				currentSection = dalayedSections [sect.Name];				
				dalayedSections.Remove(sect.Name);
			}
			else {			
				var section = sect.CreateControl () as Section;
				currentSection = new ProcessedSection () {
					Controls = new List<ProcessedControl> (),
					Section = section,
					Height = section.Height,
					MarginBottom = section.Height,
					MarginTop = 0
				};
				
				var topOrderedControls = section.Controls.OrderBy (c => c.Top).ToList ();
				if (topOrderedControls.Count > 0) {
					currentSection.MarginTop = topOrderedControls [0].Top;
					foreach (var c in topOrderedControls) {
						ProcessedControl pc = null;
						if (c  is SubReport)
							pc = new SubreportControl () {Control = c, Section = currentSection };
						else
							pc = new ProcessedControl () {Control = c, Section = currentSection };
						
						double marginBottom = section.Height - c.Bottom;
						if (marginBottom < currentSection.MarginBottom)
							currentSection.MarginBottom = marginBottom;
						currentSection.AddProcessingControl(pc);
					}
				}
			}
			currentSection.SectionSpan = pageHeightUsed;
		
		}
		
		
		void nextRecord ()
		{
			if (dataSource == null)
				dataSourceHasNextRecord = false;
			else
				dataSourceHasNextRecord = dataSource.MoveNext ();
		}		
		
	}
 
	public enum ProcessingState
	{
		BeforeProcessing, 
		Processing,
		Suspended,
		Finished
	}
	
	public class MonoReportsEngineException : Exception
	{
		
		public MonoReportsEngineException (): base()
		{
		}
		
		public MonoReportsEngineException (string message): base(message)
		{
		}
		
		public MonoReportsEngineException (string message, Exception innerException): base(message,innerException)
		{
		}
		
	}
	
	public class ProcessedSection
	{
		public ProcessedSection () {
			PageBuffer = new List<Control>();			
			Controls = new List<ProcessedControl>();			
		}
		
		public bool AttachBottom {
			get;
			set;
		}

		public double Top { get; set; }

		public string Name {
			get { return Section.Name; }			
		}
		
		public double Height {
			get;
			set;
		}
		
		public double MarginTop { get; set; }
		
		public double MarginBottom { get; set; }
		
		public Section Section { get; set; }
		
		public double SectionSpan { get; set; }
		
		public List<ProcessedControl> Controls { get; set; }
		
		public List<Control> PageBuffer { get; set; }
		
		public void AddProcessingControl (ProcessedControl pc)
		{
			 Controls.Add(pc);
		}
		
		public void AddControlToPageBuffer (Control c)
		{
			PageBuffer.Add (c);
		}
	}

	public class ProcessedControl
	{	
		
		public ProcessedSection Section { get; set; }

		public Control Control { get; set; }
		
		public double Span { get; set; }
		
		public bool WasProcessed { get; set; }
		
		public double Grow { get; set; }				
		
		public double BottomBeforeSpanAndGrow { get; set; }
		
		public double BottomAfterSpanAndGrow { get; set; }

		public virtual bool Process (IReportRenderer renderer, double span, double maxHeight)
		{
			Span = span;
			BottomBeforeSpanAndGrow = Control.Bottom;
			Size s = renderer.MeasureControl (Control);
			
			BottomAfterSpanAndGrow = Span + Control.Location.Y + s.Height;			
			bool retVal = false;
			Control.Top += span;
			if (Control.Top < maxHeight) {
				Grow = s.Height - Control.Height;
				Control.Size = new Size (Control.Width, s.Height);			
				if (BottomAfterSpanAndGrow <= maxHeight) {				
					Section.AddControlToPageBuffer (Control);
					WasProcessed = true;
					retVal = true;
				} else {
					if (!Section.Section.KeepTogether ) {
						if (Control.Top < maxHeight) {
							Control[] brokenControlParts = renderer.BreakOffControlAtMostAtHeight (Control, maxHeight-Control.Top);
							if (brokenControlParts [0] != null) {
								Section.AddControlToPageBuffer (brokenControlParts [0]);
								brokenControlParts [1].Top = maxHeight;
								double g  = (maxHeight-Control.Top) -  brokenControlParts [0].Height;
								Grow += g;
							}
							Control = brokenControlParts [1];
						}
					}					
				}
			}
			
			return retVal;	
		}
	}
	
	public class SubreportControl : ProcessedControl
	{
		
		public ReportEngine2 Engine { get; set; }
		
		public override bool Process (IReportRenderer renderer, double span, double maxHeight)
		{
			return true;
		}
		
	}
	
}

