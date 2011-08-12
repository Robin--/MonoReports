//ReportEngine.cs
//  
// Author:
//       Tomasz Kubacki <Tomasz.Kubacki (at) gmail.com>
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
using System.Linq;
using MonoReports.Model;
using System.Collections.Generic;
using MonoReports.Model.Controls;
using System.Collections;
using MonoReports.Model.Data;

namespace MonoReports.Model.Engine
{
	public class ReportEngine : IReportEngine
	{

		internal IReportRenderer ReportRenderer;
		Report Report;
		IDataSource source;
		internal ReportContext reportContext;
		internal Page currentPage = null;	
		List<Control> pageFooterControls = null;
		bool beforeFirstDetailSection = true;
		Section currentSection = null;
		SectionProcessingContext sectionContext;
		Dictionary<string, SectionProcessingContext> sectionContextDictionary = null;
		List<GroupInfo> groupInfos = null;
		bool afterReportHeader = false;
		double spanCorrection = 0;
		public bool IsSubreport { get; set; }
		bool dataSourceHasNextRow = true;
		bool stop = false;
		public static bool EvaluatorInitWasDone {get;set;}
		List<IDataControl> controlsToEvalAfterReportProcessing = null;
		
		public static void EvaluatorInit(){
			if (!MonoReports.Model.Engine.ReportEngine.EvaluatorInitWasDone) {
				Mono.CSharp.Evaluator.InitAndGetStartupFiles(new string[]{});				
				Mono.CSharp.Evaluator.LoadAssembly(System.Reflection.Assembly.GetExecutingAssembly().Location);
				Mono.CSharp.Evaluator.SetInteractiveBaseClass(typeof(MonoReports.Model.Data.MonoreportsInteractiveBase));
				Mono.CSharp.Evaluator.Run("using System;");
				Mono.CSharp.Evaluator.Run("using MonoReports.Model;");
				MonoReports.Model.Engine.ReportEngine.EvaluatorInitWasDone = true;
			}
		}

		public ReportEngine (Report report,IReportRenderer renderer)
		{
			Report = report;
			source = Report.DataSource;
			pageFooterControls = new List<Control> ();
			sectionContextDictionary = new Dictionary<string, SectionProcessingContext> ();
			controlsToEvalAfterReportProcessing = new List<IDataControl>();
			if (source == null)
				source = new DummyDataSource ();
			ReportRenderer = renderer;         
			groupInfos = new List<GroupInfo> ();
			for (int i = 0; i < report.Groups.Count; i++) {
				var efld = report.ExpressionFields.FirstOrDefault (ef => ef.Name == report.Groups [i].ExpressionFieledName);
				int index = (efld != null ? report.ExpressionFields.IndexOf (efld) : -1);
				groupInfos.Add (new GroupInfo () { ExpressionFieldIndex = index });
			}
			reportContext = new ReportContext(report) { RendererContext = renderer.RendererContext, CurrentPageIndex = 0, DataSource = source, ReportMode = ReportMode.Preview };
			Report.Pages = new List<Page> ();
			nextPage ();
			SelectCurrentStateByTemplateSection (Report.PageFooterSection);
			
			if (!ReportEngine.EvaluatorInitWasDone) {
				Mono.CSharp.Evaluator.InitAndGetStartupFiles(new string[]{});
				Mono.CSharp.Evaluator.LoadAssembly("MonoReports.Model.dll");
				Mono.CSharp.Evaluator.SetInteractiveBaseClass(typeof(MonoreportsInteractiveBase));
				Mono.CSharp.Evaluator.Run("using System;");
				Mono.CSharp.Evaluator.Run("using MonoReports.Model;");
				EvaluatorInitWasDone = true;
			} 
			
			foreach (var item in report.ExpressionFields) {
				if(item is ExpressionField){
					var expressionField = item as ExpressionField;
					if(expressionField.DataProvider == null)
						expressionField.DataProvider = new ExpressionFieldValueProvider(expressionField);
				}
				
			}
			
			MonoreportsInteractiveBase.ReportContext = reportContext;
		}

		public void Process ()
		{

			nextRecord ();

			while (!ProcessReportPage ()) {
				nextPage ();
			}
			 
			foreach (var dc in controlsToEvalAfterReportProcessing) {
				try {
					if (reportContext.ExpressionFieldsDict.ContainsKey (dc.FieldName)) 
						dc.Text = reportContext.ExpressionFieldsDict [dc.FieldName].GetStringValue ("",dc.FieldTextFormat);
				} catch {}						
			}
			 
			if (source != null)
				source.Reset ();

			 Report.FireOnAfterReportProcessing(reportContext);
		}

		void storeSectionContextForNextPage ()
		{
			if (!sectionContextDictionary.ContainsKey (currentSection.Name)) {                
				sectionContextDictionary.Add (currentSection.Name, sectionContext);               
			}
		}

		T SelectCurrentStateByTemplateSection<T> (T s) where T : Section
		{
			T newSection = null;
			if (sectionContextDictionary.ContainsKey (s.Name)) {
				sectionContext = sectionContextDictionary [s.Name];
				sectionContext.CanProcess = true;
				if (!sectionContext.CurrentPageBackgroundSectionControl.KeepTogether)
                {
					
                    sectionContext.CurrentPageBackgroundSectionControl = sectionContext.CurrentPageBackgroundSectionControl.CreateControl() as Section;
					sectionContext.CurrentPageBackgroundSectionControl.Top = 0;
					sectionContext.CurrentPageBackgroundSectionControl.WasProcessed = true;					
					sectionContext.CurrentPageBackgroundSectionControl.WasFullyProcessed = true;	
					sectionContext.ProcessedControlsBuffer.Clear();
                    sectionContext.ProcessedControlsBuffer.Add(sectionContext.CurrentPageBackgroundSectionControl);																			 
					sectionContext.SetFirstNotFullyProcessedControl();
					for (int i = sectionContext.BrokenControlsBuffer.Count-1; i >= 0;i--) {
						sectionContext.BrokenControlsBuffer[i].Top = 0;						
						sectionContext.TopOrderedControls.Insert(sectionContext.CurrentControlIndex,sectionContext.BrokenControlsBuffer[i]);
					}
					sectionContext.BrokenControlsBuffer.Clear();	
					sectionContext.SetFirstNotFullyProcessedControl();											
					double minTop = sectionContext.TopOrderedControls.Where(ctrl=> !ctrl.WasFullyProcessed).Min(tt=>tt.Top);
					minTop = Math.Min(sectionContext.RealBreak,minTop);
					if(minTop > 0) {
						foreach (var item in sectionContext.TopOrderedControls) {	
							if(!item.WasFullyProcessed)
 								item.Top -= minTop;
						}
					}					
					sectionContext.MaxHeight = 0;					
                }
				sectionContextDictionary.Remove (s.Name); 				
				newSection = sectionContext.Section as T;				
			} else {
				newSection = s.CreateControl () as T;
				newSection.Format ();
				sectionContext = new SectionProcessingContext (newSection);				
				sectionContext.TopOrderedControls = newSection.Controls.OrderBy (ctrl => ctrl.Top).ToList ();
			}                    
			newSection.Location = new Point (s.Location.X, 0);
			currentSection = newSection;
			return newSection;
		}

		public bool ProcessReportPage ()
		{
			bool result = false;
			stop = false;

			do {
				currentSection.TemplateControl.FireBeforeControlProcessing (reportContext, currentSection);
				result = ProcessSectionUpToHeightTreshold (reportContext.HeightLeftOnCurrentPage);   

				if (!currentSection.KeepTogether || result){
					addControlsToCurrentPage (reportContext.HeightUsedOnCurrentPage);
					sectionContext.ProcessedControlsBuffer.Clear();
				}
				
				reportContext.HeightLeftOnCurrentPage -= currentSection.Height;
				reportContext.HeightUsedOnCurrentPage += currentSection.Height;

				if (result) {
					nextSection ();
				} else {
					return false;
				}
			} while (!stop);

			return result;
		}
 
		
		/// <summary>
		/// Processes the section up to heightTreshold.
		/// </summary>
		/// <returns>
		///  returns <c>true</c> if finished processig section and <c>false</c> while not
		/// </returns>
		/// <param name='pageBreakTreshold'>
		/// maxiumum height (starting from current section Location.Y) after which page will break
		/// </param>
		public bool ProcessSectionUpToHeightTreshold (double heightTreshold)
		{
			bool result = true;
			
			if (sectionContext.State == ProcessingState.BeforeProcessing) {
				sectionContext.InitSection(heightTreshold);
			}
			

			while (!sectionContext.AllControlsProcessed) {
				sectionContext.TmpSpan = double.MinValue;				

				if (!sectionContext.Control.IsVisible || sectionContext.Control.WasFullyProcessed) {
					sectionContext.NextControl ();
					continue;
				}

				if (sectionContext.Control is Line && (sectionContext.Control as Line).ExtendToBottom) {
					var line = sectionContext.Control as Line;
					sectionContext.ExtendedLines.Add (line);
				}
				#region assign data
				if (source != null && sectionContext.Control is IDataControl && !sectionContext.Control.WasProcessed) {
					IDataControl dataControl = sectionContext.Control as IDataControl;
					if (!string.IsNullOrEmpty (dataControl.FieldName)) {

						switch (dataControl.FieldKind) {
						case FieldKind.Parameter:
							if (reportContext.ParameterFieldsDict.ContainsKey (dataControl.FieldName)) {
								var parameter = reportContext.ParameterFieldsDict [dataControl.FieldName];
								if(this.Report.ParameterValues.ContainsKey(dataControl.FieldName))
									dataControl.Text = parameter.GetStringValue (this.Report.ParameterValues, dataControl.FieldTextFormat);
								else
									dataControl.Text = parameter.GetStringValue (null, dataControl.FieldTextFormat);
							}
							break;
						case FieldKind.Expression:
							if (reportContext.ExpressionFieldsDict.ContainsKey (dataControl.FieldName)) {
							 
								var expression = reportContext.ExpressionFieldsDict [dataControl.FieldName] as ExpressionField;
								
								if(!expression.IsEvaluatedAfterProcessing)
									dataControl.Text = expression.GetStringValue (dataControl.FieldName, dataControl.FieldTextFormat);
								else
									controlsToEvalAfterReportProcessing.Add(dataControl);
							}
							break;
						case FieldKind.Data:
							if (source.ContainsField (dataControl.FieldName))
								dataControl.Text = source.GetValue (dataControl.FieldName, dataControl.FieldTextFormat);
							break;
						default:
							break;
						}
					}
				}
				#endregion

				sectionContext.Y = sectionContext.Control.Top + sectionContext.Span;
				Size controlSize = ReportRenderer.MeasureControl (sectionContext.Control);

				foreach (SpanInfo item in sectionContext.Spans) {
					if (sectionContext.Y > item.Treshold) {
						sectionContext.TmpSpan =  Math.Max (sectionContext.TmpSpan, item.Span);
					}
				}

				sectionContext.Span = sectionContext.TmpSpan == double.MinValue ? 0 : sectionContext.TmpSpan;
				sectionContext.Control.Top += sectionContext.Span;
				sectionContext.HeightBeforeGrow = sectionContext.Control.Height;
				sectionContext.BottomBeforeGrow = sectionContext.Control.Bottom;
				sectionContext.Control.Size = controlSize;
				sectionContext.Control.WasProcessed = true;


				if (sectionContext.Control.Bottom <= heightTreshold) {
					sectionContext.ProcessedControlsBuffer.Add (sectionContext.Control);   
					sectionContext.Control.WasFullyProcessed = true;
				} else {
					result = false;
					storeSectionContextForNextPage ();
					if (!currentSection.KeepTogether) {

						sectionContext.BreakControlMax = sectionContext.Control.Height - ((sectionContext.Control.Top + sectionContext.Control.Height) - heightTreshold);
						if (sectionContext.RealBreak == 0)
							sectionContext.RealBreak = heightTreshold;

						if (sectionContext.Control.Top < heightTreshold) {
	
							Control[] brokenControlParts = ReportRenderer.BreakOffControlAtMostAtHeight (sectionContext.Control, sectionContext.BreakControlMax);
							sectionContext.RealBreak = heightTreshold - (sectionContext.BreakControlMax - brokenControlParts [0].Height);																
							sectionContext.ProcessedControlsBuffer.Add (brokenControlParts [0]);
							brokenControlParts [0].WasFullyProcessed = true;			
							sectionContext.Control.WasFullyProcessed = true;
							sectionContext.BrokenControlsBuffer.Add(brokenControlParts [1]);
						}
					}

				}

				if (currentSection.CanGrow && sectionContext.MaxHeight <= sectionContext.Control.Bottom) {
					sectionContext.MaxHeight = Math.Max (sectionContext.Control.Bottom, sectionContext.MaxHeight);
				}

				if (!result) {
					if (sectionContext.RealBreak > 0) {
						sectionContext.MaxHeight = Math.Max (sectionContext.RealBreak, sectionContext.MaxHeight);
					}
				}

				sectionContext.Spans.Add (
				new SpanInfo
				{
					Treshold = sectionContext.BottomBeforeGrow,
					Span = sectionContext.Span + sectionContext.Control.Bottom - sectionContext.BottomBeforeGrow
				});
				if (result){
					sectionContext.NextControl ();  				
				} else if (!currentSection.KeepTogether) {
					sectionContext.NextControl ();					
					if(!sectionContext.AllControlsProcessed && sectionContext.Control.Top < heightTreshold && !sectionContext.Control.WasProcessed) {						
						continue;
					}
					else
						break;
				}else {
					break;
				}
			}

			if(result || (!result && !currentSection.KeepTogether)){
				double sectionHeightWithMargin = currentSection.CanShrink ? sectionContext.MaxHeight : sectionContext.MaxHeight + sectionContext.MarginBottom;
				if (!result) {
					currentSection.Height = heightTreshold;
					
				} else if ((currentSection.CanGrow && currentSection.Height < sectionHeightWithMargin) || 
					(currentSection.CanShrink && currentSection.Height > sectionHeightWithMargin)) {
					currentSection.Height = sectionHeightWithMargin;
				} else {
					currentSection.Height = Math.Min (currentSection.Height, heightTreshold);
				}
				
				sectionContext.CurrentPageBackgroundSectionControl.Height = currentSection.Height;
				
				foreach (Line lineItem in sectionContext.ExtendedLines) {
					if (lineItem.Location.Y == lineItem.End.Y) {
						lineItem.Location = new Point (lineItem.Location.X, currentSection.Height - lineItem.LineWidth / 2);
						lineItem.End = new Point (lineItem.End.X, currentSection.Height - lineItem.LineWidth / 2);
					} else if (lineItem.Location.Y > lineItem.End.Y) {
						lineItem.Location = new Point (lineItem.Location.X, currentSection.Height);
					} else {
						lineItem.End = new Point (lineItem.End.X, currentSection.Height);
					}

					if (!result) {
						var newCtrl = lineItem.CreateControl ();

						if (lineItem.Location.Y == lineItem.End.Y)
							lineItem.IsVisible = false;
						newCtrl.Top = 0;         
						//controlsFromPreviousSectionPage[currentSection.Name].Insert(1, newCtrl);
					}
				}
	
				if (!currentSection.CanGrow) {
					sectionContextDictionary.Remove (currentSection.Name);
					result = true;
				}
			}
			return result;
		}

		void nextRecord ()
		{
			dataSourceHasNextRow = source.MoveNext ();
			reportContext.RowIndex++;
			for (int i = 0; i < Report.Groups.Count; i++) {
				var gi = groupInfos [i];
				gi.PreviousVal = groupInfos [i].CurrentVal;
				gi.CurrentVal = Report.ExpressionFields [gi.ExpressionFieldIndex].GetStringValue (source.Current, null);
				gi.ValHasChanged = (gi.CurrentVal != gi.PreviousVal ? true : false);
			}
		}

		void nextSection ()
		{

			switch (currentSection.SectionType) {
			case SectionType.PageHeader:
				if (reportContext.CurrentPageIndex > 1) {
					SelectCurrentStateByTemplateSection (Report.PageFooterSection);
				} else {
					setDetailsOrGroup ();
				}
				break;
			case SectionType.PageFooter:
				if (!afterReportHeader) {
					SelectCurrentStateByTemplateSection (Report.ReportHeaderSection);
				} else {
					setDetailsOrGroup ();
				}
				break;


			case SectionType.ReportHeader:
				if (Report.ReportHeaderSection.BreakPageAfter) {
					nextPage ();
					stop = true;
				} else {
					if (reportContext.CurrentPageIndex == 1) {
						SelectCurrentStateByTemplateSection (Report.PageHeaderSection);
					} else {
						setDetailsOrGroup ();
					}
				}

				afterReportHeader = true;
				break;



			case SectionType.Details:

				setDetailsOrGroup ();
				break;


			case SectionType.ReportFooter:
				addControlsToCurrentPage (Report.Height - Report.PageFooterSection.Height, pageFooterControls);
				stop = true;
				break;
			default:
				break;
			}

			if (!currentSection.IsVisible)
				nextSection ();
		}

		void setDetailsOrGroup ()
		{

			if (!sectionContextDictionary.ContainsKey (Report.DetailSection.Name) && !beforeFirstDetailSection) {
				nextRecord ();
			}

			if (dataSourceHasNextRow || beforeFirstDetailSection) {
				SelectCurrentStateByTemplateSection (Report.DetailSection);
			} else {
				SelectCurrentStateByTemplateSection (Report.ReportFooterSection);
			}
			beforeFirstDetailSection = false;
		}

		void addControlsToCurrentPage (double span)
		{            
			if (currentSection.SectionType != SectionType.PageFooter) {
				addControlsToCurrentPage (span + spanCorrection, sectionContext.ProcessedControlsBuffer);            
			} else {
				pageFooterControls = new List<Control> (sectionContext.ProcessedControlsBuffer);  	
				spanCorrection -= currentSection.Height;
			}
		}

		void addControlsToCurrentPage (double span, List<Control> controls)
		{
			foreach (var control in controls) {
				control.Top += span;
				currentPage.Controls.Add (control);
			}
		}

		void nextPage ()
		{
			addControlsToCurrentPage (Report.Height - Report.PageFooterSection.Height, pageFooterControls);
			spanCorrection = 0;			
			reportContext.CurrentPageIndex++;
			currentPage = new Page { PageNumber = reportContext.CurrentPageIndex };
			reportContext.HeightLeftOnCurrentPage = Report.Height;
			reportContext.HeightUsedOnCurrentPage = 0;
			Report.Pages.Add (currentPage);						
			SelectCurrentStateByTemplateSection (Report.PageHeaderSection);
		}	

		public void RenderPages(IReportRenderer renderer, Report report){
			reportContext.RendererContext = renderer.RendererContext;
			reportContext.CurrentPageIndex = 1;
			for (int i = 0; i < report.Pages.Count; i++) {					
				Page p = report.Pages [i];
				report.FireOnBeforePageRender(reportContext,p);
				renderer.RenderPage (p);									
				report.FireOnAfterPageRender(reportContext,p);
				if(reportContext.CurrentPageIndex < report.Pages.Count) {
					renderer.NewPage();
					reportContext.CurrentPageIndex++;
				}
			}	
		}
		
 
	}

	internal class GroupInfo
	{
		internal string PreviousVal;
		internal string CurrentVal;
		internal bool ValHasChanged;
		internal int ExpressionFieldIndex;
	}

	static internal class SectionExtensions
	{

		public static IEnumerable<Control> GetCrossSectionControls (this Section section, Section endSection)
		{

			foreach (var c in section.Controls.Where (ctrl => ctrl is ICrossSectionControl)) {

				var csc = c as ICrossSectionControl;
				csc.StartSection = section;
				csc.EndSection = endSection;
				yield return c;
			}
		}

	}
}

