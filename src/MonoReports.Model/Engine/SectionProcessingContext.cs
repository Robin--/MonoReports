// 
// SectionProcessingState.cs
//  
// Author:
//       Tomasz Kubacki <tomasz (dot) kubacki (at) gmail (dot ) com>
// 
// Copyright (c) 2011 Tomasz Kubacki
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
using MonoReports.Model.Controls;
using System.Linq;

namespace MonoReports.Model.Engine
{
	public class SectionProcessingContext
	{			
		public SectionProcessingContext(Section section){
			this.Section = section;
			CurrentControlIndex = -1;
			ExtendedLines = new List<Line>();
			TopOrderedControls = new List<Control>();
			ProcessedControlsBuffer = new List<Control>();
			BrokenControlsBuffer = new List<Control>();
			Spans = new List<SpanInfo>();			
		}
		
		Section section;

		public Section Section {
			get {
				return section;	
			}
			set {
				section =  value;
			}
		}
		
		public ProcessingState State {get;set;}
		
		public double MaxControlBottom {
			get;
			set;
		}
		
		public double MarginBottom {
			get;
			set;
		}
		
		public double Span {
			get;
			set;
		}
		
		public double TmpSpan {
			get;
			set;
		}
		
		public Section CurrentPageBackgroundSectionControl {get;set;}
		
		public double Y {get;set;}
		
		public int CurrentControlIndex {get;set;}
		
		public List<Control> ProcessedControlsBuffer  {get;set;}
		
		public List<Control> BrokenControlsBuffer  {get;set;}
		
		public List<Control> TopOrderedControls {get;set;}
		
		public List<Line> ExtendedLines{get;set;} 
		
		public List<SpanInfo> Spans {get;set;}
				
        public double HeightBeforeGrow{get;set;}
		
        public double BottomBeforeGrow {get;set;}
		
		public double BreakControlMax {get;set;}
		
		public double MaxHeight {get;set;}
		
		public double RealBreak {get;set;}
						
		public double HeightTresholdIncludingBottomMargin {get;set;}
		
		public Control Control {get;set;}
		
		public bool CanProcess {get;set;}			
		
		public bool AllControlsProcessed {get;set;}
		
		public void InitSection(double heightTreshold) {
			if (TopOrderedControls.Count > 0) {
					MaxControlBottom = TopOrderedControls.Max (ctrl => ctrl.Bottom);				
			}
				MarginBottom = section.Height - MaxControlBottom;
				HeightTresholdIncludingBottomMargin = heightTreshold - MarginBottom;
				State = ProcessingState.Processing;
				var s = Section.CreateControl() as Section;				
				s.Controls.Clear();
				s.TemplateControl = section.TemplateControl;
				ProcessedControlsBuffer.Add (s); 
				CurrentPageBackgroundSectionControl = s;
				NextControl ();
		}
		
		
		public void SetFirstNotFullyProcessedControl() {			
			CurrentControlIndex = 0;
			AllControlsProcessed = false;
			if(!AllControlsProcessed && TopOrderedControls[CurrentControlIndex].WasFullyProcessed)
				NextControl();
		}
		
		
		
		
		public void NextControl() {
			CurrentControlIndex++;
			if(CurrentControlIndex < TopOrderedControls.Count){
				Control = TopOrderedControls[CurrentControlIndex];
				AllControlsProcessed = false;
			}else {
				Control = null;
				AllControlsProcessed = true;
			}
		}
		
		
	}
	
	
	public enum ProcessingState {Init, Processing, Suspended}
}

