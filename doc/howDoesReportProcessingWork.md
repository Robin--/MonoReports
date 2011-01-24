How does report processing work
===============================

What is report generation?
--------------------------------------
Report processing is a procedure that generates report pages from a report template (later called `report`). 
There are two main phases of report generation:

1. Processing - in this phase the report will be fully generated. All pages will be filled with controls. After this phase OnAfterReportProcesing will be fired. Handling this event allows change report look before converting it to output form.

2. Rendering - this phase is simple conversiongenerated pages into output format (e.g. pdf)

Processing
----------------------------
In Monoreports, report is processed section by section, starting with Report Header, every control (textblock, image, line etc.) in that section is processed (measured, resized, spaned  etc.).

Each section must be completed in order to proceed to the next section. It is not possible to start processing the next section before the previous one has been finished.

The report processing sequence
------------------------------

In most cases (see: Exceptions to the above processing sequence) the report processing sequence looks this:

	1 Report Header
	2 Page Header
	3 Page Footer  
	4...n Deatail section
	[new page]
	n+1 Page Header
	n+2 Page Footer
	n+3 Detail section
	...
	n+x Report Footer
	n+x+1 Page Footer
	

It may seem odd that the page footer is processed before the detail section, but this reporting engine has to know how much space left before it breaks page.

Exceptions to the above processing sequence
-------------------------------------------
- some sections may have IsVisible set to True, the section is not processed then and it's not visible on the output report
- ReportHeader may have BreakPageAfter set to True, it that's the case, then the report footer (if visible) is processed just after the report header

Breaking Off Sections
---------------------

If section's KeepTogether flag is set to False and at same time the section does not fit into current page, the section will be broken. Monoreports's engine is detecting how much space left on the current page and breaks (or move to the next page) every control exceeding current page height threshold.


