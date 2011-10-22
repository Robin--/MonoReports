// 
// LineProcessedControl.cs
//  
// Author:
// Tomasz Kubacki <tomasz (dot) kubacki (at) gmail.com>
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
using MonoReports.Model.Data;

namespace MonoReports.Model.Engine
{
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
		
		void ProcessData () {
			if( ! WasProcessed ) {
				IDataControl dataControl = Control as IDataControl;
				if (dataControl != null) {
					if(!string.IsNullOrWhiteSpace(dataControl.FieldName)) {					
					 	ScriptEvaluator.EvalDataControl(dataControl);						
					}
				}
			}
			
			
			
			
		}
		
	}
}

