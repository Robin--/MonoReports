// 
// ReportSettingsEditor.cs
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
using MonoReports.Model;
using MonoReports.Extensions.PropertyGridEditors;
using MonoReports.Core;

namespace MonoReports.Gui.Widgets
{
	public partial class ReportSettingsEditor : Gtk.Dialog
	{
		public ReportSettingsEditor ()
		{
			this.Build ();
			reportPropertygrid.LoadMonoreportsExtensions();
			settingsPeportPropertygrid.LoadMonoreportsExtensions();
			settingsPeportPropertygrid.CurrentObject =  new MonoreportsSettings();
		}
		
		
		 
		public Report Report {
			get { return reportPropertygrid.CurrentObject  as Report; }
			set { 							
				reportPropertygrid.CurrentObject = value;			
			}
		}
	}
}

