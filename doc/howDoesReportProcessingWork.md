How does report processing work
==================================

What is report processing?
-------------------------
Report processing is to produce report pages from report template (called report). In Monoreports, report is processed section by section, starting with Report Header, every control in that section is processed (measured, growed, spaned  etc.), then next section is processed and so on.

It's not possible to start process next section before previous one is not finished.

Section processing sequence
---------------------------

In most cases (see: Exceptions to processing sequence) report processing squence is like this:

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
	

It may seem odd that page footer is processed before detail section, but reporting engine has to know how much space left before
is shall break page.

Exceptions to processing sequence
---------------------------------
- some sections could have IsVisible = true, 
- ReportHeader could have BreakPageAfter set to True, it that's the case, then report footer (if visible) is processed just after Report Header

Breaking Off Sections
--------------------

If section's KeepTogether flag is set to false and in the same time section will not fit in a current page. Section break will happen. Monoreports's engine is looking how much space left on current space and breake (or move to next page) every control exceeding page height threashold.





