// 
// ReportEngineTest.cs
//  
// Author:
//       Tomasz Kubacki <tomasz (dot ) kubacki (at) gmail (dot) com>
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
using NUnit.Framework;
using MonoReports.Model.Engine;
using MonoReports.Model;
using System.Linq;
using MonoReports.Model.Controls;

namespace MonoReports.Tests
{
	[TestFixture]
	public class ReportEngineTest
	{

		public IReportEngine CreateEngine (Report report,IReportRenderer renderer) {
			return new ReportEngine2(report,renderer);
		}
		
		[Test]
		public void ProcessSection_WithNoDatasource_HasAtLeastOnePage ()
		{
			Report r = new Report();
			RendererMock m = new RendererMock();
			IReportEngine re = CreateEngine(r,m);	
			re.Process();	
			Assert.IsNotEmpty(r.Pages);
		}


		[Test]
		public void ProcessSection_BeforeDetailsProcess_HeightLeftIsReportHeightMinusHeadersAndFooters()
		{
			
			Report r = new Report();	
			
			double heightBeforeDetails = 0;
			
			r.ReportHeaderSection.Height = 50;
			r.PageHeaderSection.Height = 10;
			r.PageFooterSection.Height = 25;
			r.DetailSection.Height = 15;
			
			r.DetailSection.OnBeforeControlProcessing += delegate(ReportContext rc, Control c) {
				//before first detail processing
				if( heightBeforeDetails == 0 ) {
					heightBeforeDetails = rc.HeightLeftOnCurrentPage;
				}
			};
			
			RendererMock m = new RendererMock();
			IReportEngine re = CreateEngine(r,m);			
			re.Process();	
			double pageHeaderAndPageFooterHeight =  r.Height - ( r.ReportHeaderSection.Height + r.PageHeaderSection.Height + r.PageFooterSection.Height);
			
			Assert.AreEqual(pageHeaderAndPageFooterHeight,heightBeforeDetails);
		}
		
		
		
		public class RendererMock : IReportRenderer {
	
			public Size MeasureControl (Control control)
			{
				return new Size(control.Size);
			}

			public void RenderControl (Control control)
			{
				
			}

			public Control[] BreakOffControlAtMostAtHeight (Control control, double height)
			{
				return new Control[]{ control.CreateControl(), control.CreateControl() };				
			}

			public void RenderPage (Page p)
			{
				
			}
		
			public void NewPage ()
			{
				
			}
			
			public object RendererContext { get; set; }

		}
	}
}

