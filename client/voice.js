/**
*
*
*/
function micro() {
	if(!record) {
		console.log('Debut enregistrement.');
		notify('info' , 'Que voulez-vous faire?',"",3000);
		notify('info' , 'Allumer la salle de bain. <BR>Fermer les volets du salon. <BR>Déverrouiller la cuisine.',"Exemple de commande",10000);
		recognition.start();
		//commandeVocale("allumer la cuisine");
	}
	else {
		console.log('Fin enregistrement.');
		notify('info' , "Enregistrement annulé.","",3000);
		recognition.stop();
	}
	record = !record;
	refreshButton();
}

// TODO : COMMENT AND REFACTORING
// WORK IN PROGRESS 

var jsonReconnaissance = {
	// TYPE A RECHERCHER 
	"init" : {
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

// Permet de savoir si oui ou non on enregistre
var record = false;
// Moteur de reconnaissance vocale
var recognition = new webkitSpeechRecognition();
recognition.lang = "fr";

// Fonction appelée en cas d'erreur de reconnaissance
recognition.onerror = function (event) {
	console.log('Erreur : ');
	console.log(event);
	console.log('Fin enregistrement.');
	recognition.stop();
	record = false;
	notify('error' , 'Erreur lors de la reconnaissance vocale.',"",3000);
	refreshButton();
};
		
// Fonction appélée en cas de réussite
recognition.onresult = function (event) {
	recognition.stop();
	record = false;
	refreshButton();
	var trouve = false;
	console.log('Résultat de la reconnaissance :');
	var text = '';
	for (var i = event.resultIndex; i < event.results.length; ++i) {
		if (event.results[i].isFinal) {
			trouve = true;
			var sentence = event.results[i][0].transcript;
			notify('info' , 'Vous avez demandé : ' + sentence + ".","",3000);
			console.log(sentence);
			commandeVocale(sentence);
		}
	}
	if(!trouve) {
		notify('info' , "Je n\'ai rien entendu.","",3000);
	}
};

function commandeVocale( phrase ) {
	// On calcule la commande effectuée
	var computedCommand = computeCommand( phrase );
	var commande = computedCommand.commande;
	var parametresCommande = computedCommand.parametres;
	var piecesId = [];
	parametresCommandes = simplifierPhrase(parametresCommande).split(' ');
	console.log('Pieces demandées simplifiées : ' + parametresCommandes.join(' '));
	$.getJSON( getControllerActionUrl("lumiere", "lister"), function( data ){
		$.each( data.pieces, function( key, val ) {
			var piece = simplifierPhrase(val.nom).split(' ');
			var pieceReconnue = true;
			console.log('Pieces disponible simplifiées : ' + piece.join(' '));
			for(var i = 0; i < piece.length; i++) {
				if(parametresCommandes.indexOf(piece[i]) == -1) {
					pieceReconnue = false;
				}
			}
			if(pieceReconnue) {
				piecesId.push(val.id);
			}
		})
		console.log(piecesId);
		if(piecesId != []) {
			for(var i = 0; i < piecesId.length; i++) {
				var id = piecesId[i];
				appliqueCommandeDansPiece(commande, id);
			}
		}
	});
}

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
	console.log(url);
	switch($("#fctBody").attr("class")) {
		case "lumiere":
			lumiere();
		break;
		case "volet":
			volet();
		break;
		case "porte":
			porte();
		break;
		case "micro" :
			micro();
		break;
		case "hifi":
			hifi();
		break;
		case "reveil":
			reveil();
		break;
		case "itineraire":
			itineraire();
		break;
		case "meteo":
			meteo();
		break;
		case "recap":
			recap();
		break;
		case "param":
			param();
		break;
		default :
			main();
	}
	if(url){
		$.getJSON(url, function( data ){
			notify( data.code < 300 ? 'success' : 'warning', data.message, "", 4000);
		});
	}
}
		


function refreshButton() {
	if(record) {
		$("#btnMicro").attr("class", 'btn-function btn-micro-record');
	}
	else {
		$("#btnMicro").attr("class", 'btn-function btn-micro');
	}
}

/**
*	Permet de calculer l'action à effectuer à partir de la phrase en paramètre
*	@param phrase Phrase de laquelle l'on doit deviner l'action à effectuer
*/
function computeCommand( phrase ) {
	// Nettoyage de la phrase et conversion en tableau
	console.log("parsephrase( " + phrase + " )");
	
	phrase = phrase.toLowerCase();
	phrase = phrase.replace("\n"," ");
	phrase = phrase.replace("."," ");
	phrase = phrase.replace(","," ");
	phrase = phrase.replace("l'"," ");
	phrase = phrase.replace(";"," ");
	phrase = phrase.split(' ');
	
	// Chaîne résultante de la recherhce de l'action
	var resultatParser = '';
	
	// Initialise de la recherhce
	var typeIdRecherche = ["init"];
	
	// Appel fonction récurrente permettant de parcourir le fichier JSON
	var resultatParser = {
		"commande" : "",
		"parametres" : ""
	};
	parsePhraseRecurrent(phrase, typeIdRecherche, resultatParser);
	
	// Affichage du résultat
	if(resultatParser != '') {
		console.log(resultatParser);
	}
	else {
		console.log('AUCUNE CORRESPONDANCE');
	}
	return resultatParser;
}

/**
*	Permet de parcourir la phrase et de calculer la commande par récurrence à partir des types recherchés
*	@param phrase Phrase à partir de laquelle on doit calculer la commande
*	@param typeIdRecherche types recherchés
*/
function parsePhraseRecurrent(phrase, typeIdRecherche, resultatParser) {
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
	for(var i = 0; i < typeIdRecherche.length; i++) {
		var mot = phrase[0];
		var resultatRecherche = retrouverIdDepuisMotEtTypesDansJson(mot, typeIdRecherche[i]);
		if(resultatRecherche) {
			resultatParser.commande += resultatRecherche.id + ' ';
			return parsePhraseRecurrent(phrase, resultatRecherche.suite, resultatParser);
		}
	}
	return parsePhraseRecurrent(phrase.slice(1, phrase.lenght), typeIdRecherche, resultatParser);
}

function retrouverIdDepuisMotEtTypesDansJson(mot, type) {
	console.log('retrouverIdDepuisMotEtTypesDansJson('+mot+', '+type+')');
	mot = mot.toLowerCase();
	var jsonTmp = jsonReconnaissance[type];
	for(var id in jsonTmp) {
		var mots = jsonTmp[id]['mots'];
		if(mots.indexOf(mot) > -1) {
			return {
				'id' : id,
				'suite' : jsonTmp[id]['suite']
			};
		}
	}
	return false;
}

function simplifierPhrase(phrase) {
	var res = phrase;
	
	// Supprimer les accents 
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
		"article" 		: new RegExp(" de | des | la | le | les | un | une | à | dans | et  | au " , "gi"),
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