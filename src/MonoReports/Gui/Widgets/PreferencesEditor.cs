// 
// PreferencesEditor.cs
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
using MonoReports.Model;
using MonoReports.Extensions.PropertyGridEditors;
using MonoReports.Core;
using Gtk;

namespace MonoReports.Gui.Widgets
{
	public partial class PreferencesEditor : Gtk.Dialog
	{
		public PreferencesEditor ()
		{
			this.Build ();
			referencesNodeView.AppendColumn ("Reference name", new Gtk.CellRendererText (), "text", 0);
			usingsNodeview.AppendColumn ("Using Name", new Gtk.CellRendererText (), "text", 0);
			AppSettings = new MonoreportsSettings();									
			IsInEditMode = true; 	
			addUsingButton.Sensitive = false;
		}
		
		
		Report report;
		 
		public Report Report {
			get {return report;}
			set {
				
				report = value;
				referencesStore = new Gtk.NodeStore(typeof(SimpleTreeNode));				
			 	foreach(string s in report.References) {
					referencesStore.AddNode(new SimpleTreeNode(s));
				}
				
				usingsStore = new Gtk.NodeStore(typeof(SimpleTreeNode));
				
				foreach(string s in report.Usings) {
					usingsStore.AddNode(new SimpleTreeNode(s));
				}
				
				this.referencesNodeView.NodeStore = referencesStore;
				this.usingsNodeview.NodeStore = usingsStore;
				
				IsInEditMode = false;
				leftMarginEntry.Text = report.Margin.Left.ToString();
				topMarginEntry.Text = report.Margin.Top.ToString();
				rightMarginEntry.Text = report.Margin.Right.ToString();
				bottomMarginEntry.Text = report.Margin.Bottom.ToString();
				IsInEditMode = true;
			}
		}
		
		
		Gtk.NodeStore referencesStore;
		
		Gtk.NodeStore usingsStore;
		
		MonoreportsSettings appSettings;
		public MonoreportsSettings AppSettings {
			get { return appSettings; }
			set { 
				appSettings = value; 
				generalSettingsPropertygrid.CurrentObject = appSettings;
			}
		}
		
		public bool IsInEditMode {
			get;
			set;
		}
		
		
		double newVal;
 
		protected virtual void OnAddReferenceFilechooserbuttonSelectionChanged (object sender, System.EventArgs e)
		{
		    string s = addReferenceFilechooserbutton.Filenames[0];
			if(!Report.References.Contains(s)) {			
				Report.References.Add(s);
				referencesStore.AddNode(new SimpleTreeNode(s));
				Mono.CSharp.Evaluator.LoadAssembly(s);			
			}
		}
		
		SimpleTreeNode selectedReferencesNode = null;
		
		SimpleTreeNode selectedUsingsNode = null;
		
		
		protected virtual void OnRemoveReferenceButtonClicked (object sender, System.EventArgs e)
		{
			if(selectedReferencesNode != null) {
				referencesStore.RemoveNode(selectedReferencesNode);	
				report.References.Remove(selectedReferencesNode.Name);
			}			 
		}
		
		protected virtual void OnLeftMarginEntryChanged (object sender, System.EventArgs e)
		{
			if(IsInEditMode) {			 
				newVal =  UnitExtensions.FromString(leftMarginEntry.Text);
				if(!double.IsNaN(newVal)) {
					Report.Margin = new Thickness(newVal,Report.Margin.Top,Report.Margin.Right,Report.Margin.Bottom);					
					buttonOk.Sensitive = true;
				}else {
					buttonOk.Sensitive = false;
				}
			}
		}
		
		
		
		protected virtual void OnTopMarginEntryChanged (object sender, System.EventArgs e)
		{
			 if(IsInEditMode) {			 
				newVal =  UnitExtensions.FromString(topMarginEntry.Text);
				if(!double.IsNaN(newVal)) {
					Report.Margin = new Thickness(Report.Margin.Left,newVal,Report.Margin.Right,Report.Margin.Bottom);					
					buttonOk.Sensitive = true;
				}else {
					buttonOk.Sensitive = false;
				}
			 }
		}
		
		protected virtual void OnRightMarginEntryChanged (object sender, System.EventArgs e)
		{
			if(IsInEditMode) {			 
				newVal =  UnitExtensions.FromString(rightMarginEntry.Text);
				if(!double.IsNaN(newVal)) {
					Report.Margin = new Thickness(Report.Margin.Left,Report.Margin.Top,newVal,Report.Margin.Bottom);					
					buttonOk.Sensitive = true;
				}else {
					buttonOk.Sensitive = false;
				}
			}
		}
		
		protected virtual void OnBottomMarginEntryChanged (object sender, System.EventArgs e)
		{
			 if(IsInEditMode) {			 
				newVal =  UnitExtensions.FromString(bottomMarginEntry.Text);
				if(!double.IsNaN(newVal)) {
					Report.Margin = new Thickness(Report.Margin.Left,Report.Margin.Top,Report.Margin.Right,newVal);					
					buttonOk.Sensitive = true;
				}else {
					buttonOk.Sensitive = false;
				}
			}
		}
 
		protected virtual void OnAddUsingButtonClicked (object sender, System.EventArgs e)
		{	
			
			if(!report.Usings.Contains(usingEntry.Text)){
				string text = usingEntry.Text.Replace(";",String.Empty);
 				Mono.CSharp.Evaluator.Run("using " + text + ";");			
				report.Usings.Add(text);
				usingsStore.AddNode( new SimpleTreeNode(text));
			}
		}
		
		protected virtual void OnUsingEntryChanged (object sender, System.EventArgs e)
		{			
			if(!string.IsNullOrEmpty( usingEntry.Text)) {
			  	var text = usingEntry.Text.Replace(";",String.Empty);
				if(text.Length > 0) {
					addUsingButton.Sensitive = true;
				}else {
					addUsingButton.Sensitive = false;
				}
			} else {
				addUsingButton.Sensitive = false;
			}
		}
		
		protected virtual void OnRemoveUsingButtonClicked (object sender, System.EventArgs e)
		{
			if(selectedUsingsNode != null) {
				usingsStore.RemoveNode(selectedUsingsNode);	
				report.Usings.Remove(selectedUsingsNode.Name);
			}	
		}
		[GLib.ConnectBefore]
		protected virtual void OnReferencesNodeViewButtonPressEvent (object o, Gtk.ButtonPressEventArgs args)
		{
			TreePath path;
			referencesNodeView.GetPathAtPos ((int)args.Event.X, (int)args.Event.Y, out path);
			if (path != null) {
				selectedReferencesNode = referencesStore.GetNode(path) as SimpleTreeNode;				 
			}else {
				selectedReferencesNode = null;
			}
		}
		[GLib.ConnectBefore]
		protected virtual void OnUsingsNodeviewButtonPressEvent (object o, Gtk.ButtonPressEventArgs args)
		{
			TreePath path;
			usingsNodeview.GetPathAtPos ((int)args.Event.X, (int)args.Event.Y, out path);
			if (path != null) {
				selectedUsingsNode = usingsStore.GetNode(path) as SimpleTreeNode;
			}else {
				selectedUsingsNode = null;
			}
		}
		
		
		
		
		
		
		
		
		
		
	}
	
	
	[Gtk.TreeNode (ListOnly=true)]
    public class SimpleTreeNode : Gtk.TreeNode {
 
    	public SimpleTreeNode (string name)
        {
        	Name = name;
        }
 
        [Gtk.TreeNodeValue (Column=0)]
        public string Name;
 
               
    }
	
	
		
		
}

