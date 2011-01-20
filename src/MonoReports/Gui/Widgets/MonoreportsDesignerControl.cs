// 
// MainWindow.cs
//  
// Author:
//       Tomasz Kubacki <Tomasz.Kubacki(at)gmail.com>
// 
// Copyright (c) 2010 Tomasz Kubacki 2010
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
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using Cairo;
using MonoReports.Gui.Widgets;
using MonoReports.ControlView;
using MonoReports.Tools;
using MonoReports.Gui;
using MonoReports.Services;
using Newtonsoft.Json;
using System.IO;
using System.Reflection;
using MonoReports.Renderers;
using Engine = MonoReports.Model.Engine;
using Controls = MonoReports.Model.Controls;
using Model = MonoReports.Model;
using MonoReports.Core;
using MonoReports.Model;
using MonoReports.Extensions.JsonNetExtenssions;
using MonoReports.Extensions.PropertyGridEditors;

namespace MonoReports.Gui.Widgets {

[System.ComponentModel.ToolboxItem(true)]
public partial class MonoreportsDesignerControl : Gtk.Bin
{

	DesignService designService;

		public DesignService DesignService {
			get {
				return this.designService;
			}
			set {
				designService = value;
			}
		}

	ToolBoxService toolBoxService;
	WorkspaceService workspaceService;
	CompilerService compilerService;	 
	PixbufRepository pixbufRepository;
	string lastFileName = String.Empty;

	public MonoreportsDesignerControl ()  
	{
		Build ();
  
		
		Report startReport = newReportTemplate();
		
		
string template = @"
using System;
using System.Collections.Generic;
{0}

public sealed class GenerateDataSource {{
    public object Generate()
    {{ 
		object datasource = null;
		Dictionary<string,object> parameters = new Dictionary<string,object>();
		 {1}
        return new object[] {{datasource,parameters}};
    }}
}}

";		
		
		double resolutionX = ((double)  Gdk.Screen.Default.Width) / ((double) Gdk.Screen.Default.WidthMm) * 25.4;
		
		compilerService = new CompilerService(template);
			compilerService.References.Add("Newtonsoft.Json.dll");
		pixbufRepository = new PixbufRepository () { Report = startReport };			
		workspaceService = new WorkspaceService (this,maindesignview1.DesignDrawingArea,maindesignview1.PreviewDrawingArea,mainPropertygrid, StatusBarLabel);
		var reportRenderer = new ReportRenderer(){ ResolutionX =  resolutionX};
		reportRenderer.RegisterRenderer(typeof(Controls.TextBlock), new TextBlockRenderer());
        reportRenderer.RegisterRenderer(typeof(Controls.Line), new LineRenderer());
		reportRenderer.RegisterRenderer(typeof(MonoReports.Model.Controls.Image),
				new ImageRenderer(){ PixbufRepository = pixbufRepository});
		SectionRenderer sr = new SectionRenderer();
		reportRenderer.RegisterRenderer(typeof(Controls.ReportHeaderSection), sr);
		reportRenderer.RegisterRenderer(typeof(Controls.ReportFooterSection), sr);
		reportRenderer.RegisterRenderer(typeof(Controls.DetailSection), sr);
		reportRenderer.RegisterRenderer(typeof(Controls.PageHeaderSection), sr);
		reportRenderer.RegisterRenderer(typeof(Controls.PageFooterSection), sr);	
		designService = new DesignService (workspaceService,reportRenderer,pixbufRepository,compilerService, startReport);
		
		toolBoxService = new ToolBoxService ();
		designService.ToolBoxService = toolBoxService;
		maindesignview1.DesignService = designService;
		maindesignview1.WorkSpaceService = workspaceService;		
		maindesignview1.ReportRenderer = reportRenderer;
		workspaceService.InvalidateDesignArea ();		
		reportExplorer.DesignService = designService;
		reportExplorer.Workspace = workspaceService;
		toolBoxService.AddTool (new ZoomTool (designService));		
		toolBoxService.AddTool (new LineTool (designService));		
		toolBoxService.AddTool (new LineToolV (designService));		
		toolBoxService.AddTool (new LineToolH (designService));				
		toolBoxService.AddTool (new TextBlockTool (designService));
		//TODO 3tk: currently not supported
		//toolBoxService.AddTool (new SubreportTool (designService));
		toolBoxService.AddTool (new SectionTool (designService));
		toolBoxService.AddTool (new ImageTool (designService));
		toolBoxService.AddTool (new RectTool (designService));
		toolBoxService.BuildToolBar (mainToolbar);
 		
		
		ToolBarButton exportPdfToolButton = new ToolBarButton ("pdf.png","exportPdf","export to pdf");
		exportPdfToolButton.Clicked += delegate(object sender, EventArgs e) {
			designService.ExportToPdf();
		};
		
		
	    var sep = new Gtk.SeparatorToolItem();
		
		mainToolbar.Insert (sep,mainToolbar.NItems);		
		mainToolbar.Insert (exportPdfToolButton,mainToolbar.NItems);	
			
		//ToolBarButton btn = new ToolBarButton("gtk-media-play","execute","Execute report");
		//mainToolbar.Insert (btn,mainToolbar.NItems);	
		
		mainPropertygrid.LoadMonoreportsExtensions();
 
	}
	 
	
	Report newReportTemplate(){
		return new Report(){ 
			DataScript = @"
datasource = new [] {
     new { Name=""Alfred"", Surname = ""Tarski"", Age = ""82"" },
     new { Name=""Saul"", Surname = ""Kripke"", Age = ""70"" },
     new { Name=""Gotlob"", Surname = ""Frege"", Age = ""85"" },
     new { Name=""Kurt"", Surname = ""Gödel"", Age = ""72"" }, 
};

parameters.Add(""Title"",new { Title = ""The Logicans"", SubTitle = ""...and philosophers...""});

"};
	}
		
	protected virtual void OnMainPropertygridChanged (object sender, System.EventArgs e)
	{
		workspaceService.InvalidateDesignArea();
	}

	public void Status (string message)
	{
		bottomStatusbar.Push (1, message);
	}

	public void ShowInPropertyGrid (object o)
	{
		mainPropertygrid.CurrentObject = o;
	}

	public void SetCursor (Gdk.CursorType cursorType)
	{
		this.GdkWindow.Cursor = new Gdk.Cursor (cursorType);
	}

	protected virtual void OnEditActionActivated (object sender, System.EventArgs e)
	{
		toolBoxService.SetToolByName ("LineTool");
	}
		
	public void SaveAs () {
		lastFileName = String.Empty;
		Save();
	}

	public void Save ()
	{
		Gtk.FileChooserDialog fc = new Gtk.FileChooserDialog ("Choose Monoreports file to save", ((Gtk.Window)this.Toplevel), FileChooserAction.Save, "Cancel", ResponseType.Cancel, "Save", ResponseType.Accept);
		var fileFilter = new FileFilter { Name = "Monoreports project" };
		fileFilter.AddPattern ("*.mrp");
		fc.AddFilter (fileFilter);
		fc.CurrentName =  string.IsNullOrEmpty(designService.Report.Title) ? "untiteled.mrp" : designService.Report.Title + ".mrp";
		
		designService.Report.DataSource = null;
		if(string.IsNullOrEmpty(lastFileName)) {
			if (fc.Run () == (int)ResponseType.Accept) {				
				designService.Save(fc.Filename);
				lastFileName = fc.Filename;
			}				
			fc.Destroy ();		
		}else {
			designService.Save(lastFileName);	
		}
	}
		
	public void ProcessAndShowPreview() {		
		SetCursor (Gdk.CursorType.Watch);
		
		DesignService.ProcessReport ();
		ShowPreview();
		
		SetCursor (Gdk.CursorType.LeftPtr);	
	}
		
	public void ShowPreview() {	
		maindesignview1.ShowPreview();
	}
		
	public void ShowDesign() {
 
		maindesignview1.ShowDesign();
	}
		
	public void New ()
	{		
		lastFileName = string.Empty;
	 	Report r = newReportTemplate();
		ShowInPropertyGrid (null);
		designService.Report = r;			
		workspaceService.InvalidateDesignArea ();
	}

	public void Open ()
	{
		Gtk.FileChooserDialog fc = new Gtk.FileChooserDialog ("Choose the Monoreports file to open", ((Gtk.Window)this.Toplevel), FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);
		var fileFilter = new FileFilter { Name = "Monoreports project" };
		fileFilter.AddPattern ("*.mrp");
		fc.AddFilter (fileFilter);
		fc.CurrentName =  string.Empty;
		if (fc.Run () == (int)ResponseType.Accept) {
			ShowInPropertyGrid (null);
			designService.Load(fc.Filename);
			lastFileName = fc.Filename;
		}
		
		fc.Destroy ();
		workspaceService.InvalidateDesignArea ();
	}


	
	public void About()
	{
		AboutDialog about = new AboutDialog();
	
		about.ProgramName = "Monoreports - report designer tool";
		about.Authors = new string[]{"Tomasz Kubacki"};
		about.WrapLicense = true;
		about.License = @"
Copyright (c) 2010 Tomasz Kubacki

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the ""Software""), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.

";
 
		about.Response += delegate(object o, ResponseArgs args) {
			about.Destroy ();
		};
		
		about.Show();
	}
	
	public void Settings()
	{
		
		ReportSettingsEditor reportSettingsEditor = new ReportSettingsEditor();
	
		reportSettingsEditor.Report = designService.Report;
 
		reportSettingsEditor.Response += delegate(object o, ResponseArgs args) {
			reportSettingsEditor.Destroy ();
		};
		
		reportSettingsEditor.Show();
		
	}
 
	
}

}

