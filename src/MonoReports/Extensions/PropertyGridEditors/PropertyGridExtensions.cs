// 
// PropertyGridExtensions.cs
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
using PropertyGrid;

namespace MonoReports.Extensions.PropertyGridEditors
{
	public static class PropertyGridExtensions
	{
		public static void LoadMonoreportsExtensions (this PropertyGrid.PropertyGrid pg)
		{
			pg.AddPropertyEditor (typeof(MonoReports.Model.Point), typeof(MonoReports.Extensions.PropertyGridEditors.PointEditorCell));
			pg.AddPropertyEditor (typeof(MonoReports.Model.Border), typeof(MonoReports.Extensions.PropertyGridEditors.BorderEditorCell));
			pg.AddPropertyEditor (typeof(MonoReports.Model.Thickness), typeof(MonoReports.Extensions.PropertyGridEditors.ThicknessEditorCell));
			pg.AddPropertyEditor (typeof(MonoReports.Model.Color), typeof(MonoReports.Extensions.PropertyGridEditors.MonoreportsColorEditorCell));
			pg.AddPropertyEditor (typeof(double), typeof(MonoReports.Extensions.PropertyGridEditors.DoubleEditorCell));
		
		}
	}
}

