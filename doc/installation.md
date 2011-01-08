Installing Monoreports
======================

Monoreports can be installed on Linux and Windows 
(Miguel De Icaza reporteted, that it also works on MacOs).

System Requirements
-------------------
Monoreports should run on Linux (with mono), Windows (.net or mono) and MacOs (mono)

Dependencies
------------

To build MonoReports you need to have:

- Mono 2.6 or higher or .Net Framework >= 3.5 on Windows
- Gtk-sharp 2.12 or higher  - windows installer here: [http://ftp.novell.com/pub/mono/gtk-sharp/gtk-sharp-2.12.10.win32.msi](http://ftp.novell.com/pub/mono/gtk-sharp/gtk-sharp-2.12.10.win32.msi
- Json.net library Newtonsoft.Json.dll (included in src/lib)

Binary version
------------------
I've found that on Windows and Ubuntu 10.10 binary version works out of box. 
so if you want to play with the designer simply grab binary package at:
[http://github.com/downloads/tomaszkubacki/monoreports/monoreports_pre.zip](http://github.com/downloads/tomaszkubacki/monoreports/monoreports_pre.zip)

Building from source
----------------------

1. install git

2. clone repository

git clone git://github.com/tomaszkubacki/monoreports.git

3. enter cloned directory and run:

	./configure
	make



 

