// 
// ReportEngine2.cs
//  
// Author:
// Tomasz Kubacki <tomasz.kubacki@gmail.com>
// 
// Copyright (c) 2011 tomek
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

namespace MonoReports.Model.Engine
{
	public class ReportEngine2
	{
		IReportRenderer renderer;
		Report report;
		ProcessingContext procContext;
		
		
		public ReportEngine2 (Report report, IReportRenderer renderer)
		{
			this.renderer = renderer;
			this.report = report;
		}
		
	 	public bool ProcessPageUpToTreshold (double breakTreshold) {
			bool result = false;
			double height = breakTreshold;

			while (height < breakTreshold )
			{
				
			}
			
			return result;
		}

		bool processCurrentControl (double heightLeft) {
			
			renderer.MeasureControl (procContext.Control);
			
			return false;
		}
	
		void selectNextSetion () {
			
		}
		
	}

	public class ProcessingContext {				

		public Section Section {get;set;}

		public Control Control {get;set;}

		public List<ProcessedControl> ProcessedControls {get;set;}
		
		public bool WasInitialized { get; set; }
	}
	
	
	public class ProcessedControl {		

		public Control Control {get;set;}
		
		public double Span {get;set;}

		public bool WasProcessed {get;set;}
	}
}

