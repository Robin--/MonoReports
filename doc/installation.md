Installing Monoreports
======================

Monoreports can be installed on Linux and Windows 
(and probably MacOs - i don't have Mac machine )

System Requirements
-------------------
Monoreports should run on Linux (with mono), Windows (.net or mono) and MacOs (mono)

Dependencies
------------

To build MonoReports you need to have:
     mono 2.6 or higher
     Gtk-sharp 2.12 or higher
     Json.net library Newtonsoft.Json.dll (src/lib)

Binary version
------------------
I've found that on Windows and Ubuntu 10.10 binary version works out of box. 
(Miguel De Icaza reporteted, that it also works on MacOs) so if you want to play
with the designer simply grab binary package at:
[http://github.com/downloads/tomaszkubacki/monoreports/monoreports_pre.zip](http://github.com/downloads/tomaszkubacki/monoreports/monoreports_pre.zip)

Building from source
----------------------

1. install git

2. clone repository

git clone git://github.com/tomaszkubacki/monoreports.git

3. enter cloned directory and run:

	./configure
	make



 

