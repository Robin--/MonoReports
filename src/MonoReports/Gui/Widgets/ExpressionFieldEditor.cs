// 
// ExpressionFieldEditor.cs
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

namespace MonoReports.Gui.Widgets
{
	public partial class ExpressionFieldEditor : Gtk.Dialog
	{
		public ExpressionFieldEditor ()
		{
			this.Build ();
			buttonOk.Sensitive = false;	
		}
		
		public String PropertyName {
			get {return nameEntry.Text;  }
			set {nameEntry.Text = value;  }
		}
		
		public String ExpressionScript {
			get {return sciptTextview.Buffer.Text;  }
			set {sciptTextview.Buffer.Text = value;  }
		}
		
		protected virtual void OnNameEntryChanged (object sender, System.EventArgs e)
		{
			if(string.IsNullOrEmpty(nameEntry.Text)){
				buttonOk.Sensitive = false;		
			}else{
				buttonOk.Sensitive = true;		
			}
		}
	}
}

