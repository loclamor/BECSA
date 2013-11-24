// TODO : COMMENT IN ENGLISH
// WORK IN PROGRESS 

/**
* Commence ou stop une reconnaissance vocale
*/
function micro() {
	if(!record) {
		console.log('Debut enregistrement.');
		notify('info' , 'Que voulez-vous faire?',"",3000);
		notify('info' , 'Allumer la salle de bain. <BR>Fermer les volets du salon. <BR>Déverrouiller la cuisine.',"Exemple de commande",10000);
		recognition.start();
	}
	else {
		console.log('Fin enregistrement.');
		notify('info' , "Enregistrement annulé.","",3000);
		recognition.stop();
	}
	record = !record;
	refreshButton();
	
}

var dictionnaireJSON = {
	// TYPE A RECHERCHER 
	"init" : {
		"MENU" : {
			"mots" : [
				"menu",
				"afficher"
			],
			"suite" : ['all']
		},
		// IDENTIFIANT DE L'ACTION (LUMIERE 1 : POUR ALLUMER LA LUMIERE)
		"LUMIERE 1" : {
			// MOTS PERMETTANT De CHOISIR CETTE ACTION
			"mots" :[
				"mettre", // Mettre la lumière à ...
				"met", 
				"allumer", // Allumer la ....
				"allume"
			],
			// TYPES DANS LESQUEL EFFECTUER LA RECHERCHER POUR LA SUITE DE LA PHRASE
			"suite" : ['all']
		},
		"LUMIERE 0" : {
			"mots" :[
				"éteindre", // Eteindre la ...
				"enlever", // Enlever la lumière à ...
				"enlève",
				"éteins"
			],
			"suite" : ['all']
		},
		"PORTE 0" : {
			"mots" :[
				"verrouiller",
				"verrouille"
			],
			"suite" : ['all']
		},
		"PORTE 1" : {
			"mots" :[
				"déverrouille",
				"déverrouiller"
			],
			"suite" : ['all']
		},
		"ACCES 1" : {
			"mots" :[
				"ouvrir",
				"ouvre"
			],
			"suite" : ["porteOuVolet"]
		},
		"ACCES 0" : {
			"mots" :[
				"fermer",
				"ferme"
			],
			// Plusieurs entrées dans suite indique que l'on effectuera la recherche dans la suite de la phrase dans ces types
			"suite" : ["porteOuVolet"]
		},
		"MAP GOTO" : {
			"mots" : [
				"aller",
				"itinéraire"
			],
			"suite" : ['all'] // Indique que l'on garde la reste de la phrase
		},
		"MAP HOME" : {
			"mots" : [
				"habiter"
			],
			"suite" : ['all'] // Indique que l'on garde la reste de la phrase
		},
		"REVEIL" : {
			"mots" : [
				"réveil"
			],
			"suite" : ['all']
		}
	},
	"porteOuVolet" : {
		"PORTE" : {
			"mots" :[
				"porte",
				"portes",
			],
		"suite" : ['all']
		},
		"VOLET" : {
			"mots" :[
				"volet",
				"volets",
				"fenêtre",
				"fenêtres"
			],
		"suite" : ['all']
		},
	}
};

// Vrai si on enregistre, daux sinon
var record = false;
// Moteur reconnaissance vocale
var recognition = new webkitSpeechRecognition();
recognition.lang = "fr";

// Fonction appélée si la reconnaissance échoue
recognition.onerror = function (event) {
	console.log('Erreur : ');
	console.log(event);
	recognition.stop();
	record = false;
	notify('error' , 'Erreur lors de la reconnaissance vocale.',"",3000);
	refreshButton();
};
		
// Fonction appelée si la reconnaissance réussie
recognition.onresult = function (event) {
	// On arrête la reconnaissance par précaution
	recognition.stop();
	record = false;
	// On rafraichit l'affichage
	refreshButton();
	// Si on a trouvé quelquechose
	var trouve = false;
	console.log('Résultat de la reconnaissance :');
	// On parcour les résultats
	for (var i = event.resultIndex; i < event.results.length; ++i) {
		// Si on trouve la chaîne retenue
		if (event.results[i].isFinal) {
			trouve = true;
			// On récupère la phrase
			var sentence = event.results[i][0].transcript;
			notify('info' , 'Vous avez demandé : ' + sentence + ".","",3000);
			console.log(sentence);
			// On la traite
			commandeVocale(sentence);
		}
	}
	if(!trouve) {
		notify('info' , "Je n\'ai rien entendu.","",3000);
	}
};

/**
* Permet d'effectuer les action correspondantes à la commande vocale
* @param phrase Phrase à traiter
*/
function commandeVocale( phrase ) {
	// On calcule la commande effectuée
	var computedCommand = computeCommand( phrase );
	// On récupère l'action à effectuer
	var commande = computedCommand.commande;
	// On récupèe les paramètres
	var parametresCommande = computedCommand.parametres;
	// On check si on a demandé l'itinaire
	console.log(parametresCommande);
	console.log(commande);
	if(commande.indexOf('MAP') > -1) {
		if(commande.indexOf('GOTO') > -1) {
			if($("#fctTitle").html() == "Itinéraire") {
				newDestination(parametresCommande,'FR');
			}
			else {
				itineraire(newDestination(parametresCommande,'FR'));
			}
		}
		else if(commande.indexOf('HOME') > -1) {
			if($("#fctTitle").html() == "Itinéraire") {
				changeHome(parametresCommande,'FR');
			}
			else {
				itineraire(changeHome(parametresCommande,'FR'));
			}
		}
	}
	else if(commande.indexOf('MENU') > -1) {
		var parametreSimplifie = simplifierPhrase(parametresCommande);
		if(parametreSimplifie.indexOf(simplifierPhrase("lumiere")) > -1) {
                lumiere();
		}
        else if(parametreSimplifie.indexOf(simplifierPhrase("volet"))> -1) {
                volet();
		}
        else if(parametreSimplifie.indexOf(simplifierPhrase("porte"))> -1) {
                porte();
		}
        else if(parametreSimplifie.indexOf(simplifierPhrase("hifi"))> -1) {
                hifi();
		}
        else if(parametreSimplifie.indexOf(simplifierPhrase("réveil"))> -1) {
                reveil();
		}
        else if(parametreSimplifie.indexOf(simplifierPhrase("itinéraire"))> -1) {
                itineraire();
		}
        else if(parametreSimplifie.indexOf(simplifierPhrase("météo"))> -1) {
                meteo();
		}
        else if(parametreSimplifie.indexOf(simplifierPhrase("recapitulatif"))> -1) {
                recap();
		}
        else if(parametreSimplifie.indexOf(simplifierPhrase("paramètres"))> -1) {
                param();
		}
	}
	else if(commande.indexOf('REVEIL') > -1) {
		var heure = -1;
		var minute = -1;
		var regHeureMinutes = new RegExp('([0-9]+)h ([0-9]*)');
		var regHeure = new RegExp('([0-9]+)h');
		var match = parametresCommande.match(regHeureMinutes);
		if(!match) {
			match = parametresCommande.match(regHeure);
			if(match) {
				var heure = parseInt(match[1]);
				var minute = 0;
			}
		}
		else{
			heure = parseInt(match[1]);
			minute = parseInt(match[2]);
		}
		if(heure != -1) {
			var jour;
			if((new Date().getHours() == heure && new Date().getMinutes() < minute)
			|| (new Date().getHours() > heure)) {
				jour = (new Date().getDay() + 6)%7;
			}
			else {
				jour = (new Date().getDay() + 7)%7;
			}
			var jours = [0,0,0,0,0,0,0];
			jours[jour] = 1;
			if(minute < 10) {
				minute = "0" + minute;
			}
			if(heure < 10) {
				heure = "0" + heure;
			}
			var postData = {
				"reveil": "Nouveau réveil à " + heure + ":" + minute,
				"heure": match[1] + ":" + minute + ":00",
				"jours": jours,
				"repetition":1
			};
			console.log(postData);
			$.post( getControllerActionUrl("reveil","creer"), postData, function(data) {console.log(data);});
		}
	}	
	else {
		commandeVocalePiece(parametresCommande);
	}
}


/**
*
*
*/
function commandeVocalePiece(parametresCommande) {
	var piecesId = [];
	// On simplifie les paramètres pour faciliter les comparaison
	// (supprime articles, lettre silencieuse, accents, ponctuation, etc)
	parametresCommandes = simplifierPhrase(parametresCommande).split(' ');
	//console.log('Pieces demandées simplifiées : ' + parametresCommandes.join(' '));
	// On récupère la liste des pièces
	$.getJSON( getControllerActionUrl("piece", "lister"), function( data ){
		//console.log(data);
		// Pour chaque piece
		$.each( data.pieces, function( key, val ) {
			// On simplifie le nom pour faciliter la comparaison
			var piece = simplifierPhrase(val.nom).split(' ');
			// Passe à faux si la pièce ne correspond pas
			var pieceReconnue = true;
			//console.log('Pieces disponible simplifiées : ' + piece.join(' '));
			// Pour chaque mot dans le nomde la piece
			for(var i = 0; i < piece.length; i++) {
				// Si le mot n'existe pas dans la phrase, on ne garde pas la piece
				if(parametresCommandes.indexOf(piece[i]) == -1) {
					pieceReconnue = false;
				}
			}
			// On garde la piece si son nom se trouve dans la phrase
			if(pieceReconnue) {
				piecesId.push(val.id);
			}
		})
		//console.log(piecesId);
		// Si il y a des pièce dans la phrase
		if(piecesId != []) {
			// Pour chacune, on applique la commande trouvée
			for(var i = 0; i < piecesId.length; i++) {
				var id = piecesId[i];
				appliqueCommandeDansPiece(commande, id);
			}
		}
	});
}
/**
*
* @param commande Action à effectuer (voir dictionnaireJSON en début de code)
* @param piece Id de la pièce ou s'applique l'action
*/
function appliqueCommandeDansPiece(commande, piece) {
	if(commande.indexOf('LUMIERE 1') > -1) {
		var url = getControllerActionUrl("lumiere", "allumer", piece);
	}
	if(commande.indexOf('LUMIERE 0') > -1) {
		var url = getControllerActionUrl("lumiere", "eteindre", piece);
	}
	if(commande.indexOf('PORTE 1') > -1) {
		var url = getControllerActionUrl("porte", "deverrouiller", piece);
	}
	if(commande.indexOf('PORTE 0') > -1) {
		var url = getControllerActionUrl("porte", "verrouiller", piece);
	}
	if(commande.indexOf('ACCES 1') > -1) {
		if(commande.indexOf('PORTE') > -1) {
			var url = getControllerActionUrl("porte", "deverrouiller", piece);
		}
		else if(commande.indexOf('VOLET') > -1) {
			var url = getControllerActionUrl("volet", "ouvrir", piece);
		}
		else {
		//TODO
		}
	}
	if(commande.indexOf('ACCES 0') > -1) {
		if(commande.indexOf('PORTE') > -1) {
			var url = getControllerActionUrl("porte", "verrouiller", piece);
		}
		else if(commande.indexOf('VOLET') > -1) {
			var url = getControllerActionUrl("volet", "fermer", piece);
		}
		else {
		//TODO
		}
	}
	if(url){
		$.getJSON(url, function( data ){
			notify( data.code < 300 ? 'success' : 'warning', data.message, "", 4000);
		});
	}
}
		

/**
* Rafraichit l'affichage du bouton
*/
function refreshButton() {
	if(record) {
		$("#btnMicro").attr("class", 'btn-micro-record');
	}
	else {
		$("#btnMicro").attr("class", 'btn-micro');
	}
}

/**
* Permet de calculer l'action à effectuer à partir de la phrase en paramètre
* @param phrase Phrase de laquelle l'on doit deviner l'action à effectuer
* @return commande {commande : Commande effectué, parametres : Paramètres de la commande (généralement le reste dela phrase)}
*/
function computeCommand( phrase ) {
	// Function parcourant le dictionnaireJson afin de trouver la commande demandée
	parsePhraseRecurrent = function(phrase, typeIdRecherche, resultatParser) {
		//console.log('parsePhraseRecurrent('+phrase+', '+typeIdRecherche+')');
		// Fin de la récurrence
		if(phrase.length == 0 || typeIdRecherche == []) {
			return resultatParser;
		}
		// Fin de récurrence avec récupérationdu reste de la phrase
		if(typeIdRecherche[0] == 'all') {
			resultatParser.parametres += phrase.join(' ');
			return resultatParser;
		}
		// Recurrence
		// Pour chaque type dans laquelle on recherche une correspondance
		for(var i = 0; i < typeIdRecherche.length; i++) {
			// On récupère le premier mot
			var mot = phrase[0];
			// On récupère l'ID si cela correspond
			var resultatRecherche = retrouverIdDepuisMotEtTypesDansJson(mot, typeIdRecherche[i]);
			if(resultatRecherche) {
				resultatParser.commande += resultatRecherche.id + ' ';
				return parsePhraseRecurrent(phrase, resultatRecherche.suite, resultatParser);
			}
		}
		return parsePhraseRecurrent(phrase.slice(1, phrase.lenght), typeIdRecherche, resultatParser);
	};
	// Nettoyage de la phrase et conversion en tableau
	//console.log("parsephrase( " + phrase + " )");
	phrase = phrase.toLowerCase().split(' ');;
	
	// Chaîne résultante de la recherhce de l'action
	var resultatParser = '';
	
	// Initialise de la recherhce
	var typeIdRecherche = ["init"];
	
	// Appel fonction récurrente permettant de parcourir le fichier JSON
	var resultatParser = {
		"commande" : "",
		"parametres" : ""
	};
	// Construction de la commande
	parsePhraseRecurrent(phrase, typeIdRecherche, resultatParser);
	
	// Affichage du résultat
	// if(resultatParser != '') {
		// console.log(resultatParser);
	// }
	// else {
		// console.log('AUCUNE CORRESPONDANCE');
	// }
	return resultatParser;
}

/**
* Parcour le dictionnaire pour trouver l'action à effectuer
* @param mot Mot dont on veut trouver la commande correspondante
* @param type Type dans lequel on doit cherche la commande correspondante
*/
function retrouverIdDepuisMotEtTypesDansJson(mot, type) {
	var motSimplifie = simplifierPhrase(mot);
	var jsonTmp = dictionnaireJSON[type];
	for(var i in jsonTmp) {
		var mots = jsonTmp[i]['mots'];
		for(var j in mots) {
			var motJson = simplifierPhrase(mots[j]);
			if(motJson == motSimplifie && ! motJson.match(new RegExp("^[ ]*$", "gi"))) {
				console.log('ORDRE TROUVE : '+ i +' car '+ motJson +' == '+ motSimplifie);
				return {
					'id' : i,
					'suite' : jsonTmp[i]['suite']
				};
			}
		}
	}
	return false;
}

/**
*
* @param phrase Phrase à simplifier afin de transformer une chaîne de caractère
*				pour eviter les erreurs de comparaison dues à des erreur d'orthogaphe et autres
* @return Une chaîne simplifier (plus d'acents, plus de ponctuation, plus de lettre en doubles
								 et de lettre non prononcées)
*/
function simplifierPhrase(phrase) {
	// Passe en minuscule et ajoutd'espace avant et après pour simplifier expression régulière
	var res = ' ' + phrase.toLowerCase() + ' '; 
	
	// Supprimer les accents (de finalclap.com)
	var accent = [
        /[\300-\306]/g, /[\340-\346]/g, // A, a
        /[\310-\313]/g, /[\350-\353]/g, // E, e
        /[\314-\317]/g, /[\354-\357]/g, // I, i
        /[\322-\330]/g, /[\362-\370]/g, // O, o
        /[\331-\334]/g, /[\371-\374]/g, // U, u
        /[\321]/g, /[\361]/g, // N, n
        /[\307]/g, /[\347]/g, // C, c
    ];
    var noaccent = ['A','a','E','e','I','i','O','o','U','u','N','n','C','c'];
     
    for(var i = 0; i < accent.length; i++){
        res = res.replace(accent[i], noaccent[i]);
    }
	
	//Ensemble d'expession régulière pour la simplification
	var regSimplifier = {
		// Suppression des articles
		"article" 		: new RegExp(" l' | d' | de | des | la | le | les | un | une | à | dans | et  | au " , "gi"),
		// Suppression des ponctuations
		"ponctuation" 	: new RegExp(",|\\.|:|!|\\?|'|\\(|\\)" , "g"),
		// Suppression des lettre silencieuse
		"silence"		: new RegExp("t |r |s |x |e |p |z " , "gi"),
		// Suppression des lettre seules
		"seule"			: new RegExp(" [a-z] " , "gi"),
		// Simplification des lettre doublée
		"double"		: new RegExp("([a-zA-Z])\\1", "gi")
	};
	// On vérifie que la chaîne à besoin d'être simplifié
	var phraseEstComplexe =  res.match(regSimplifier.article) 
						  || res.match(regSimplifier.ponctuation) 
						  || res.match(regSimplifier.silence) 
						  || res.match(regSimplifier.seule);
	// Tant que la chaîne n'est pas simplifié
	while(phraseEstComplexe){
		res = res.replace(regSimplifier.article," ")
				 .replace(regSimplifier.ponctuation," ")
				 .replace(regSimplifier.silence," ")
				 .replace(regSimplifier.seule," ");
		// On vérifie que la chaîne à besoin d'être encore simplifié
		phraseEstComplexe =  res.match(regSimplifier.article) 
						  || res.match(regSimplifier.ponctuation) 
						  || res.match(regSimplifier.silence) 
						  || res.match(regSimplifier.seule);
	}
	
	// On nettoie les lettre doubles
	var aLettresDouble = res.match(regSimplifier.double);
	while(aLettresDouble) {
		res = res.replace(aLettresDouble[0],aLettresDouble[0].charAt(0));
		aLettresDouble = res.match(regSimplifier.double);
	}
     
	return res;
}