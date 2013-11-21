
Description:
============

	4 Project:
		1. Core: Source code of SmartHome.dll
		2. TextViewClient: Sample of Core usage
		3. AudioViewClient: Sample of Core usage
		4. MainController: Sample of Core usage
		
		
Usage:
======

	1. Add as reference SmartHome.dll in your c# project.
	2. Instanciate SmartHome.Home with URI of your home server
	3. (Optional) Register room update event and home update event
	4. Use the method SmartHome.Home.Refresh()
		// This method change/add room and home global state
	
	
Core:
=====

	Complete class library which communicate and interact with home server.


Clients:
========
	
	TextViewClient:
	---------------
	
	This client is an Observer of home events. When the home or when one of his room are updated then this client show in the console the event done.
		
		
	AudioViewClient:
	----------------

	This client is an Observer of home events. When the home or when one of his room are updated then this client show in the console and with audio the event done.
	


Controller:
===========

	HomeController:
	---------------

	Example project that enable the user to control the home with voice.



