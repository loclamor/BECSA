
SmartHome:{#mainpage}
==========

Description:
============

	4 Project:
		1. Core: Source code of SmartHome.dll
		2. TextViewClient: Sample of Core usage
		3. AudioViewClient: Sample of Core usage
		4. MainController: Sample of Core usage
		5. KinectController: Contains 3 Scenario of what smart home can do
		
		
Usage:
======

	1. Add as reference SmartHome.dll in your c# project.
	2. Instantiate SmartHome.Home with URI of your home server
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

	Description:
	~~~~~~~~~~~~

		Example project that enable the user to control the home with voice.

	Exhaustive list of vocal command:
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

		// The vocal command are listed in French and without any accent

		Allumer toutes les lumieres:
			. Maison, allume toutes les lumieres.
			. Maison, allume tous.
			. Maison, allumer toutes les lumieres.
			. Maison, allumer tous.
		Eteindre toutes les lumieres:
			. Maison, eteint toutes les lumieres.
			. Maison, eteint tous.
			. Maison, eteindre toutes les lumieres.
			. Maison, eteindre tous.
		Vérouiller toutes les portes:
			. Maison, verrouille toutes les portes.
			. Maison, verrouille tous.
			. Maison, verrouiller toutes les portes.
			. Maison, verrouiller tous.
			. Maison, ferme toutes les portes.
			. Maison, fermer toutes les portes.
		Déverouiller toutes les portes:
			. Maison, deverrouille toutes les portes.
			. Maison, deverrouille tous.
			. Maison, deverrouiller toutes les portes.
			. Maison, deverrouiller tous.
			. Maison, ouvre toutes les portes.
			. Maison, ouvrir toutes les portes.
		Ouvrir tous les volets:
			. Maison, ouvre tous les volets.
			. Maison, ouvre tous.
			. Maison, ouvrir tous les volets.
			. Maison, ouvrir tous.
		Fermer tous les volets:
			. Maison, ferme tous les volets.
			. Maison, ferme tous.
			. Maison, fermer tous les volets
			. Maison, fermer tous.
		
		
		Allumer la lumiere d'une pièce:
			. Maison, allume la lumiere de la/du $(PIECE.Nom).
			. Maison, allumer la lumiere de la/du $(PIECE.Nom).
			. Maison, allume $(PIECE.Nom).
			. Maison, allumer $(PIECE.Nom).
		Eteindre la lumiere d'une pièce:
			. Maison, eteint la lumiere de la/du $(PIECE.Nom).
			. Maison, eteindre la lumiere de la/du $(PIECE.Nom).
			. Maison, eteint $(PIECE.Nom).
			. Maison, eteindre $(PIECE.Nom).
		Vérouiller la porte d'une pièce:
			. Maison, verouille la porte de la/du $(PIECE.Nom).
			. Maison, verouille $(PIECE.Nom).
			. Maison, verouiller la porte de la/du $(PIECE.Nom).
			. Maison, verouiller $(PIECE.Nom).
			. Maison, fermer la porte $(PIECE.Nom).
			. Maison, ferme la porte $(PIECE.Nom).
		Déverouiller la porte d'une pièce:
			. Maison, deverouille la porte de la/du $(PIECE.Nom).
			. Maison, deverouille $(PIECE.Nom).
			. Maison, deverouiller la porte de la/du $(PIECE.Nom).
			. Maison, deverouiller $(PIECE.Nom).
			. Maison, ouvrir la porte de la/du $(PIECE.Nom).
			. Maison, ouvre la porte de la/du $(PIECE.Nom).
		Ouvrir les volets d'une pièce:
			. Maison, ouvre le/les volet de la/du $(PIECE.Nom).
			. Maison, ouvrir volet de la/du $(PIECE.Nom).
		Fermer les volets d'une pièce:
			. Maison, ferme le/les volet/volets de la/du $(PIECE.Nom).
			. Maison, fermer volet/volets de la/du $(PIECE.Nom).
		
		
		Hifi - Joue une musique aléatoire:
			. Maison, joue une musique aleatoire.
			. Maison, joue une chanson aleatoirement.
		Hifi - Pause:
			. Maison, met en pause la musique.
			. Maison, stop la musique.
			. Maison, pause.
		Hifi - Piste suivante:
			. Maison, musique suivante.
			. Maison, piste suivante.
			. Maison, chanson suivante.
		Hifi - Piste précedente:
			. Maison, musique precedente.
			. Maison, piste precedente.
			. Maison, chanson precedente.
		Hifi - Joue une chanson en particulier:
			. Maison, lance la musique $(SONG.Title).
			. Maison, lance la chanson $(SONG.Title).
			. Maison, lance le son $(SONG.Title).
			. Maison, ecouter la musique $(SONG.Title).
			. Maison, ecouter la chanson $(SONG.Title).
			. Maison, ecouter $(SONG.Title).
			. Maison, lance la musique $(SONG.Title) de $(SONG.Artist).
			. Maison, lance la chanson $(SONG.Title) de $(SONG.Artist).
			. Maison, lance le son $(SONG.Title) de $(SONG.Artist).
			. Maison, ecouter la musique $(SONG.Title) de $(SONG.Artist).
			. Maison, ecouter la chanson $(SONG.Title) de $(SONG.Artist).
			. Maison, ecouter $(SONG.Title) de $(SONG.Artist).
		HIfi - Listes les chanson:
			. Maison, liste les musiques disponibles.
			. Maison, liste les chansons disponibles.
		
		
		Météo:
			. Maison, (donne moi/qu'elle est/indique moi) la meteo d'aujourdui.
			. Maison, (donne moi/qu'elle est/indique moi) la meteo de demain.
			. Maison, (donne moi/qu'elle est/indique moi) la meteo du lendemain.
			. Maison, (donne moi/qu'elle est/indique moi) la meteo d'après demain.
			. Maison, (donne moi/qu'elle est/indique moi) la meteo du sur lendemain.
			. Maison, (donne moi/qu'elle est/indique moi) la meteo de lundi.
			. Maison, (donne moi/qu'elle est/indique moi) la meteo de mardi.
			. Maison, (donne moi/qu'elle est/indique moi) la meteo de mercredi.
			. Maison, (donne moi/qu'elle est/indique moi) la meteo de jeudi.
			. Maison, (donne moi/qu'elle est/indique moi) la meteo de vendredi.
			. Maison, (donne moi/qu'elle est/indique moi) la meteo de samedie.
			. Maison, (donne moi/qu'elle est/indique moi) la meteo de dimanche.
		
		
		Calcul d'Itinairaire:
			. Maison, (donne moi/qu'elle est/indique moi) l'itinéraire pour allez $(Directions)
			. Maison, (donne moi/qu'elle est/indique moi) le chemin pour allez $(Directions)
			. Maison, (donne moi/qu'elle est/indique moi) l'itinéraire pour se rendre $(Directions)
			. Maison, (donne moi/qu'elle est/indique moi) le chemin pour se rendre $(Directions)
		Traffic:
			. Maison, (donne moi/qu'elle est/indique moi) l'état du traffic pour allez $(Directions)
			. Maison, (donne moi/qu'elle est/indique moi) l'état du traffic pour se rendre $(Directions)
			
			
		Extra:
			Fermer tous les volets, eteindre toutes les lumieres et vérrouiller toutes les portes:
				. Maison, mode invasion zombie.
			Fermer tout les volets et allume la lumiere dans la pièce "courante" (en réalité la pièce est celle qui à pour Id 1):
				. Maison, ferme tout les volets et allume la lumiere.
			Ferme les volets et allume la lumiere de la pièce "courante" (en réalité la pièce est celle qui à pour Id 1):
				. Maison, ferme les volets et allume la lumiere.
			
			Couper net la synthese vocale + la musique:
				. Maison, silence.






