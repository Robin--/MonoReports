// 
// MainWindow.cs
//  
// Author:
//       Tomasz Kubacki <tomasz (dot) kubacki (at) gmail (dot ) com>
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
using Gtk;

public partial class MainWindow: Gtk.Window
{	
	public MainWindow (): base (Gtk.WindowType.Toplevel)
	{
		Build ();
        Maximize();
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();	
		a.RetVal = true;
	}
	
	protected virtual void OnQuitAction1Activated (object sender, System.EventArgs e)
	{
		Application.Quit ();	
	}
	
	protected virtual void OnAboutActionActivated (object sender, System.EventArgs e)
	{
		monoreportsdesignercontrol1.About();
	}
	
	protected virtual void OnOpenActionActivated (object sender, System.EventArgs e)
	{
	}
	
	protected virtual void OnSaveActionActivated (object sender, System.EventArgs e)
	{
		monoreportsdesignercontrol1.Save();
	}
 
	protected virtual void OnCopyAction1Activated (object sender, System.EventArgs e)
	{
		monoreportsdesignercontrol1.DesignService.Copy();
		
	}
	
	protected virtual void OnPasteAction1Activated (object sender, System.EventArgs e)
	{
		monoreportsdesignercontrol1.DesignService.Paste();
	}
	
	protected virtual void OnMediaPlayActionActivated (object sender, System.EventArgs e)
	{
		monoreportsdesignercontrol1.DesignService.ProcessReport ();
	}
	
	protected virtual void OnOpenAction2Activated (object sender, System.EventArgs e)
	{
		monoreportsdesignercontrol1.Open();
	}
	
	protected virtual void OnZoomInActionActivated (object sender, System.EventArgs e)
	{
		monoreportsdesignercontrol1.DesignService.Zoom += 0.1;
		monoreportsdesignercontrol1.DesignService.WorkspaceService.InvalidateDesignArea();
	}
	
	protected virtual void OnZoomOutActionActivated (object sender, System.EventArgs e)
	{
		monoreportsdesignercontrol1.DesignService.Zoom -= 0.1;
		monoreportsdesignercontrol1.DesignService.WorkspaceService.InvalidateDesignArea();
	}
	
	protected virtual void OnZoom100ActionActivated (object sender, System.EventArgs e)
	{
		monoreportsdesignercontrol1.DesignService.Zoom = 1;
		monoreportsdesignercontrol1.DesignService.WorkspaceService.InvalidateDesignArea();
	}
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
}
