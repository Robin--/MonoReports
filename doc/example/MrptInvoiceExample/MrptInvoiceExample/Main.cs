// Main.cs
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
using MonoReports.Model;
using MonoReports.Model.Controls;
using System.Collections.Generic;
using MonoReports.Model.Data;
using System.Linq;

namespace MrptInvoiceExample
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Report r = new Report ();			
			//----------------------
			// report header section
			//----------------------
			
			var invTextTb = new TextBlock (){ 				 
				Text = "Invoice",
				Width = r.Width,
				FontWeight = FontWeight.Bold,
				FontSize = 14,
				HorizontalAlignment = HorizontalAlignment.Center,				
			};		
			r.ReportHeaderSection.Controls.Add (invTextTb);
			
			var invNumberTb = new TextBlock (){ 
				FieldName ="invoice.Number", 
				FieldKind = FieldKind.Parameter, 
				Text = "Invoice",
				Width = r.Width,
				FontSize = 14,
				Top = 5.mm(),
				HorizontalAlignment = HorizontalAlignment.Center,
				FontColor = Color.White
			};				
			r.ReportHeaderSection.Height = 30.mm();
			r.ReportHeaderSection.Controls.Add (invNumberTb);
			r.ReportHeaderSection.BackgroundColor = Color.Silver;
			
			
			//-------------------
			//page header section
			//-------------------
			
			var phLine = new Line (){ 
				Location = new Point(0,10.mm()), End = new Point(r.Width,10.mm()), ExtendToBottom = true};
 
			r.PageHeaderSection.Controls.Add (phLine);
			
			
			//index label
			var indhTb = new TextBlock () {FontWeight = FontWeight.Bold, Text = "Ind", Width = 10.mm()};			
			r.PageHeaderSection.Controls.Add (indhTb);
			
			// description label
			var deschTb = new TextBlock () {FontWeight = FontWeight.Bold,Text = "Description", Left = 12.mm(), Width = 20.mm()};			
			r.PageHeaderSection.Controls.Add (deschTb);
			
			// quantity label
			var qnthTb = new TextBlock () {FontWeight = FontWeight.Bold, Text = "Quantity", Left = 42.mm()};			
			r.PageHeaderSection.Controls.Add (qnthTb);
			
			// price field
			var prthTb = new TextBlock () {FontWeight = FontWeight.Bold,Text = "Price", Left = 60.mm(),Width = 30.mm()};			
			r.PageHeaderSection.Controls.Add (prthTb);
			
			
			
			//---------------
			//details section
			//---------------
			
			//do not allow break detail section across page
			r.DetailSection.KeepTogether = true;			
			r.DetailSection.Height = 6.mm();
				
			//index field
			var indTb = new TextBlock () { FieldName = "Index",  FieldKind = FieldKind.Data, Text = "00", Left = 1.2.mm(), Width = 10.mm()};			
			r.DetailSection.Controls.Add (indTb);
			
			// description field
			var descTb = new TextBlock () { FieldName = "Description",  FieldKind =  FieldKind.Data, Text = "Desc", Left = 12.mm(), Width = 35.mm()};			
			r.DetailSection.Controls.Add (descTb);
			
			// quantity field
			var qntTb = new TextBlock () { FieldName = "Quantity",  FieldKind =  FieldKind.Data, Text = "0", Left = 47.mm(), Width = 5.mm(), };			
			r.DetailSection.Controls.Add (qntTb);
			
			// price field
			var prtTb = new TextBlock () { FieldName = "PricePerUnitGross", FieldTextFormat = "{0:C}", FieldKind =  FieldKind.Data, Text = "0", Left = 62.mm(),Width = 20.mm()};			
			r.DetailSection.Controls.Add (prtTb);
			
			
			var line = new Line (){ Location = new Point(0,2.mm()), End = new Point(r.Width,2.mm()), ExtendToBottom = true};
			r.DetailSection.Controls.Add (line);
 
			//just before processing we can change section properties
			r.DetailSection.OnBeforeControlProcessing += delegate(ReportContext rc, Control c) {
				if(rc.RowIndex % 2 == 0) {
					c.BackgroundColor = Color.LightGray;					
				}
				else {
					( (TextBlock) (c as Section).Controls[1]).FontColor = Color.PaleVioletRed;				 
				}
			};
			
			var lv0 = new Line (){
				Location = new Point(1,0),
				End = new Point(1,2.mm()), 
				ExtendToBottom = true};
			r.DetailSection.Controls.Add (lv0);
			
			var lineV = new Line (){ Location = new Point(r.Width,2.mm()), End = new Point(r.Width,2.mm()), LineType = LineType.Dash, ExtendToBottom = true};
			r.DetailSection.Controls.Add (lineV);
			
			
			//---------------
			//Report footer
			//---------------

		 	// price field
			
			var prtTotalLabelTb = new TextBlock () { 
				FontWeight = FontWeight.Bold, 
			  	HorizontalAlignment = HorizontalAlignment.Right,			 
				FontSize = 12,
				FieldKind =  FieldKind.Parameter, 
				Text = "Total: ", 
				Left = 50.mm(),
				Width = 10.mm()
				};	
			
			r.ReportFooterSection.Controls.Add (prtTotalLabelTb);
			
			
			
			var prtTotalTb = new TextBlock () { 
				FontWeight = FontWeight.Bold, 
				FieldName = "invoice.TotalGross", 
				FieldTextFormat = "{0:C}", 
				FontSize = 12,
				FieldKind =  FieldKind.Parameter, 
				Text = "0", 
				Left = 62.mm(),
				Width = 40.mm()
				};	
			
			r.ReportFooterSection.Controls.Add (prtTotalTb);
			
			
			//---------------
			//Page footer
			//---------------
			var fl = new Line (){ Location = new Point(0,1), End = new Point(r.Width,1)};
			r.PageFooterSection.Controls.Add (fl);
			
			var pnTb = new TextBlock () {
				FieldName = "#PageNumber",
				FieldTextFormat = "{0:C}",
				FieldKind =  FieldKind.Expression, 
				Text = "0",
				Left = (r.Width-30).mm(),
				Width = 10.mm(), 
				HorizontalAlignment = HorizontalAlignment.Right,
				Top = 2.mm()};	
			
			r.PageFooterSection.Controls.Add (pnTb);
			r.PageFooterSection.BackgroundColor = Color.LightBlue;
			
			#region example invoice datasource	
			
			//example invoice class... 
			Invoice invoice = new Invoice () { 
				Number = "01/12/2010",
				CreationDate = DateTime.Now,
				Positions = new List<InvoicePosition>()					
			};
			
			for (int i = 0; i < 82; i++) {
				invoice.Positions.Add (
					new InvoicePosition () 
					{ 
						Index = i+1,
						Quantity = 1, 
						Description = "Reporting services " + (i + 1).ToString(),
						PricePerUnitGross = ((i * 50) / (i + 1)) + 1
					}
				);								
			}
			
			
			invoice.Positions[4].Description = "here comes longer position text to see if position will extend section height";
			
			invoice.Positions[11].Description = "another longer position text to see if position will extend section height";
			
			
			//Total gross ...
			invoice.TotalGross = invoice.Positions.Sum (p => p.PricePerUnitGross * p.Quantity);
			#endregion
	
			r.DataSource = invoice.Positions;			
			r.ExportToPdf ("invoice.pdf", new Dictionary<string,object>{ {"invoice",invoice}});
			r.Save("report.mrp");
			
				
		}

		public class Invoice
		{
			
			public string Number {get; set;}

			public DateTime CreationDate {get; set;}

			public List<InvoicePosition> Positions {
				get;
				set;
			}

			public double TotalGross {get; set;}
		}

		public class InvoicePosition
		{
		
			public int Index {
				get;
				set;
			}

			public string Description {
				get;
				set;
			}

			public double  Quantity {
				get;
				set;
			}

			public double PricePerUnitGross {
				get;
				set;
			}
			
		}
	}
}
