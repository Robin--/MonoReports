Installing Monoreports
======================

Monoreports can be installed on Linux and Windows 
(Miguel De Icaza reported, that it also does work on MacOs).

System Requirements
-------------------
Monoreports should run on Linux (with mono), Windows (.net or mono) and MacOs (mono)

Dependencies
------------

To build MonoReports you need to have:

- Mono 2.6.7 or higher or .Net Framework >= 3.5 on Windows
- Gtk-sharp 2.12 or higher  - windows installer here: 
[gtk-sharp-2.12.10.win32.msi](http://ftp.novell.com/pub/mono/gtk-sharp/gtk-sharp-2.12.10.win32.msi)
- Json.net library Newtonsoft.Json.dll (included in src/lib)

Binary version
------------------
I've found that on Windows and Ubuntu 10.10 binary version works out of box (as long as you have gtk-sharp installed). 
so if you want to play with the designer simply grab a binary package at:
[https://github.com/downloads/tomaszkubacki/monoreports/monoreports_0_1rc.zip](https://github.com/downloads/tomaszkubacki/monoreports/monoreports_0_1rc.zip)

Building from source
----------------------

1. install git

2. clone repository

git clone git://github.com/tomaszkubacki/monoreports.git

3. enter cloned directory and run:

	./configure
	make




 

