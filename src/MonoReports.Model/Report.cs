// Report.cs
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
using System.Collections.Generic;
using MonoReports.Model.Controls;
using System.Linq;
using System.Collections;
using MonoReports.Model.Data;

namespace MonoReports.Model
{
	public class Report
	{

		public Report ()
		{			
			Width = 190.mm (); 
			Height = 277.mm ();		
			Margin = new Thickness (10.mm (),10.mm (),10.mm (),10.mm ());
			Groups = new List<Group> ();			
			Parameters = new List<Field> ();
			DataFields = new List<Field> ();
			References = new List<string>();
			Usings = new List<string>();
			ExpressionFields = new List<Field> ();
			GroupHeaderSections = new List<GroupHeaderSection> ();
			GroupFooterSections = new List<GroupFooterSection> ();
			Totals = new List<Total>();
			Pages = new List<Page> ();
			ResourceRepository = new Dictionary<string,byte[]> ();
			ReportHeaderSection = new Controls.ReportHeaderSection { Location = new Point (0, 0), Size = new Model.Size (Width, 10.mm()) };
			PageHeaderSection = new Controls.PageHeaderSection { Location = new Point (0, 0), Size = new Model.Size (Width, 10.mm()) };
			DetailSection = new Controls.DetailSection { Location = new Point (0, 9), Size = new Model.Size (Width, 10.mm()) };
			PageFooterSection = new Controls.PageFooterSection { Location = new Point (0, 13), Size = new Model.Size (Width, 10.mm()) };
			ReportFooterSection = new Controls.ReportFooterSection { Location = new Point (0, 16), Size = new Model.Size (Width, 10.mm()) };
			
			var ef = new MonoReports.Model.Data.ExpressionField() { Name = "#RowNumber"};
			ef.ExpressionScript = "RowIndex;";
			ef.FieldKind = MonoReports.Model.Data.FieldKind.Expression;
			ef.DataProvider = new MonoReports.Model.Data.ExpressionFieldValueProvider(ef);
			ExpressionFields.Add(ef);
			
			ef = new MonoReports.Model.Data.ExpressionField() { Name = "#PageNumber"};
			ef.ExpressionScript = "CurrentPageIndex;";
			ef.FieldKind = MonoReports.Model.Data.FieldKind.Expression;
			ef.DataProvider = new MonoReports.Model.Data.ExpressionFieldValueProvider(ef);
			ExpressionFields.Add(ef);
			
			ef = new MonoReports.Model.Data.ExpressionField() { Name = "#NumberOfPages"};
			ef.ExpressionScript = "CurrentPageIndex;";
			ef.FieldKind = MonoReports.Model.Data.FieldKind.Expression;
			ef.IsEvaluatedAfterProcessing = true;
			ef.DataProvider = new MonoReports.Model.Data.ExpressionFieldValueProvider(ef);
			ExpressionFields.Add(ef);
		}
						

		public event AfterPageRender OnAfterPageRender;
		
		public event BeforePageRender OnBeforePageRender;
		
		public event AfterReportProcessing OnAfterReportProcessing;

		public string Title { get; set; }

		public string DataScript {get; set;}

		public PageHeaderSection PageHeaderSection { get; set; }

		public PageFooterSection PageFooterSection { get; set; }

		public ReportHeaderSection ReportHeaderSection { get; set; }

		public ReportFooterSection ReportFooterSection { get; set; }

		public DetailSection DetailSection { get; internal set; }
		
		public List<Total> Totals { get; set; }

		public List<GroupHeaderSection> GroupHeaderSections { get; set; }

		public List<GroupFooterSection> GroupFooterSections { get; set; }

		public List<Page> Pages { get; internal set; }

		public List<Field> Parameters { get; private set; }

		public List<Field> DataFields { get; private set; }
		
		public List<string> References { get; private set; }
		
		public List<string> Usings { get; private set; }

		public List<Field> ExpressionFields { get; private set; }		

		public List<Group> Groups { get; internal set; }

		public Dictionary<string,byte[]> ResourceRepository { get; set; }

		public double Height { get; set; }

		public double HeightWithMargins {
			get { return Height + Margin.Top + Margin.Bottom;}
		}		

		double width;

		public double Width { 
			get { return width; } 
			set {
				if (width != value) {
					width = value;
					if (ReportHeaderSection != null) {
						ReportHeaderSection.Width = width;
						ReportFooterSection.Width = width;
						PageHeaderSection.Width = width;
						PageFooterSection.Width = width;
						DetailSection.Width = width;

						foreach (var item in GroupHeaderSections) {
							item.Width = width;

						}

						foreach (var item in GroupFooterSections) {
							item.Width = width;
						}
					}
				}
			} 
		
		}

		public IEnumerable<Control> GetAllReportControls ()
		{
			yield return ReportHeaderSection;
			foreach (var item in ReportHeaderSection.Controls) {
				yield return item;
			}
			foreach (var item in PageHeaderSection.Controls) {
				yield return item;
			}

			foreach (var group in GroupHeaderSections) {
				foreach (var item in group.Controls) {
					yield return item;
				}
			}
			foreach (var item in DetailSection.Controls) {
				yield return item;
			}

			foreach (var group in GroupFooterSections) {
				foreach (var item in group.Controls) {
					yield return item;
				}
			}

			foreach (var item in PageFooterSection .Controls) {
				yield return item;
			}
			foreach (var item in ReportFooterSection.Controls) {
				yield return item;
			}
		}

		public double WidthWithMargins {
			get { return Width + Margin.Left + Margin.Right;}
		}

		public Thickness Margin {get; set;}		

		public void AddGroup (Group gr)
		{
			
			Groups.Add (gr);
			GroupHeaderSection gh = new GroupHeaderSection { Name = "Group header " + gr.ExpressionFieledName, Size = new Model.Size (Width, 20), Location = new Point (0, 150) };
			GroupHeaderSections.Add (gh);
			GroupFooterSection gf = new GroupFooterSection { Name = "Group footer " + gr.ExpressionFieledName, Size = new Model.Size(Width, 20), Location = new Point(0, 250) };
			GroupFooterSections.Add (gf);
		}

		public void RemoveGroup (Group gr)
		{
			int index = Groups.IndexOf (gr);
			if (index != -1) {
				Groups.RemoveAt (index);
				GroupHeaderSections.RemoveAt (index);
				GroupFooterSections.RemoveAt (index);
			}
		}

		object dataSource;

		public object DataSource { 
			get { return dataSource; } 
			set {
				dataSource = value;
				if (dataSource != null) {

					Type dsType = dataSource.GetType ();
					Type elementType = dsType.GetElementType ();
					if (elementType == null) {
						Type genericArgumentTypes = dsType.GetGenericArguments () [0];
						Type genericType = typeof(ObjectDataSource<>); 
						var genericDatasourceType = genericType.MakeGenericType (new Type[]{genericArgumentTypes});
						_dataSource = (Activator.CreateInstance (genericDatasourceType, dataSource))  as IDataSource;
					} else {
						Type genericType = typeof(ObjectDataSource<>); 
						var realGeneric = genericType.MakeGenericType (new Type[]{elementType});
						_dataSource = (Activator.CreateInstance (realGeneric, dataSource))  as IDataSource;
					}
					FillFieldsFromDataSource ();
				} else {
					_dataSource = null;
				}
				
			} 
		
		}

		internal void FireOnAfterPageRender (ReportContext rc, Page p)
		{
			if (OnAfterPageRender != null) {
				OnAfterPageRender (rc, p);
			}
		}

		internal void FireOnBeforePageRender (ReportContext rc, Page p)
		{
			if (OnBeforePageRender != null) {
				OnBeforePageRender (rc, p);
			}
		}

		internal void FireOnAfterReportProcessing (ReportContext rc)
		{
			if (OnAfterReportProcessing != null) {
				OnAfterReportProcessing (rc);
			}
		}

		internal IDataSource _dataSource {get; set;}

		public void FillFieldsFromDataSource ()
		{
						
			if (DataSource != null) {				
				foreach (var field in _dataSource.DiscoverFields ()) {
				
					field.FieldKind = FieldKind.Data;
					var oldField = DataFields.FirstOrDefault (f => f.Name == field.Name);
					if (oldField != null) {
						oldField.DataProvider = field.DataProvider;
						oldField.FieldType = field.FieldType;
						oldField.DefaultValue  = field.DefaultValue;
					} else
						DataFields.Add (field);				 					
				}
			} else 
				throw new InvalidOperationException ("Datasource can't be null while discovering data fields");
				
		}

		public void CopyToReport (Report r)
		{
			r.Title = Title;
			r.ResourceRepository = ResourceRepository;
			r.PageHeaderSection = PageHeaderSection;
			r.PageFooterSection = PageFooterSection;
			r.ReportHeaderSection = ReportHeaderSection;
			r.ReportFooterSection = ReportFooterSection;
			r.DataScript = DataScript;
			r.DetailSection = DetailSection;
			r.GroupHeaderSections = GroupHeaderSections;
			r.GroupFooterSections = GroupFooterSections;
			r.Height = Height;
			r.Width = Width;
			r.Margin = Margin;	
			r.Parameters.AddRange(Parameters);
			r.DataFields.AddRange(DataFields);
			r.Totals.AddRange(Totals);
			r.References.AddRange(References);
			r.Usings.AddRange(Usings);
			 
			 
			foreach (var ef in ExpressionFields) {
				if(!ef.Name.StartsWith("#"))
					r.ExpressionFields.Add (ef);
			}
		}

	
	}
}
