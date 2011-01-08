Units in monoreports
==================================

Device independent units
-------------------------
Monoreports uses device independent units for control measures. 
Default unit is pixel (px) which is equal to 1/96 of inch, except font size where default unit is point (pt) which is equal to 72/96 of inch.

Using other units
-----------------
There are a couple of extension methods defined (Monoreport.Model namespace), to make it easier using other than default unit, e.g to make TextBlock 15 milimeters wide and 2 inches high:

	using Monoreports.Model
	[..]
	TextBlock textBlock =new TextBlock();
	textBlock.Width = 15.mm();
	textBlock.Height = 2.in();

If you want to know what's measure of something in more human readable form you can use:

	textBlock.Width.Tomm(); // width in milimeters
	textBlock.Width.Toin(); // ---//--- inches
	textBlock.Width.Tocm(); // ---//--- centimeters


Units in designer
----------------
As of today (pre 0.1) designer supports only default units for input. It is however possible to display mouse distance (in windows status) from point 0,0 in non default unit using
Edit -> Report Settings menu
Also debug information will be displayed in choosen unit.

