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
			"suite" : ["piece"]
		},
		"LUMIERE 0" : {
			"mots" :[
				"éteindre", // Eteindre la ...
				"enlever", // Enlever la lumière à ...
				"enlève",
				"éteins"
			],
			"suite" : ["piece"]
		},
		"PORTE 0" : {
			"mots" :[
				"verrouiller",
				"verrouille"
			],
			"suite" : ["piece"]
		},
		"PORTE 1" : {
			"mots" :[
				"déverrouille",
				"déverrouiller"
			],
			"suite" : ["piece"]
		},
		"ACCES 1" : {
			"mots" :[
				"ouvrir",
				"ouvre"
			],
			"suite" : [
				"porteOuVolet",
				"piece"
			]
		},
		"ACCES 0" : {
			"mots" :[
				"fermer",
				"ferme"
			],
			// Plusieurs entrées dans suite indique que l'on effectuera la recherche dans la suite de la phrase dans ces types
			"suite" : [ 
				"porteOuVolet",
				"piece"
			]
		},
		"MAP GOTO" : {
			"mots" : [
				"aller",
				"itinéraire"
			],
			"suite" : ['all'] // Indique que l'on garde la reste de la phrase
		}
	},
	"piece" : {
		"SALON" : {
			"mots" :[
				"salon",
				"manger", // Salle à manger
			],
		"suite" : []
		},
		"BAIN" : {
			"mots" :[
				"bain",
				"salle de bain",
				"eau" // Salle d'eau pour les vieux
			],
		"suite" : [] 
		},
		"CUISINE" : {
			"mots" :[
				"cuisine"
			],
			"suite" : []
		},
	},
	"porteOuVolet" : {
		"PORTE" : {
			"mots" :[
				"porte",
				"portes",
			],
		"suite" : ["piece"]
		},
		"VOLET" : {
			"mots" :[
				"volet",
				"volets",
				"fenêtre",
				"fenêtres"
			],
		"suite" : ["piece"]
		},
	}
};

console.log('micro added');
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
	var commande = computeCommand( phrase );
	var commandeSplitted = commande.split(' ');
	var pieceDansCommande = commandeSplitted[commandeSplitted.length-2];
	var pieceId = -1;
	$.getJSON( getControllerActionUrl("lumiere", "lister"), function( data ){
		$.each( data.pieces, function( key, val ) {
			console.log(pieceDansCommande + ' ' + val.nom);
			console.log(jsonReconnaissance.piece[pieceDansCommande].mots);
			console.log(jsonReconnaissance.piece[pieceDansCommande].mots.indexOf(val.nom));
			if(jsonReconnaissance.piece[pieceDansCommande].mots.indexOf(val.nom) > -1) {
				pieceId =  val.id;
			}
		})
		if(pieceId > -1) {
			if(commande.indexOf('LUMIERE 1') > -1) {
				var url = getControllerActionUrl("lumiere", "allumer", pieceId);
			}
			if(commande.indexOf('LUMIERE 0') > -1) {
				var url = getControllerActionUrl("lumiere", "eteindre", pieceId);
			}
			if(commande.indexOf('PORTE 1') > -1) {
				var url = getControllerActionUrl("porte", "deverrouiller", pieceId);
			}
			if(commande.indexOf('PORTE 0') > -1) {
				var url = getControllerActionUrl("porte", "verrouiller", pieceId);
			}
			if(commande.indexOf('ACCES 1') > -1) {
				if(commande.indexOf('PORTE') > -1) {
					var url = getControllerActionUrl("porte", "deverrouiller", pieceId);
				}
				else if(commande.indexOf('VOLET') > -1) {
					var url = getControllerActionUrl("volet", "ouvrir", pieceId);
				}
				else {
				//TODO
				}
			}
			if(commande.indexOf('ACCES 0') > -1) {
				if(commande.indexOf('PORTE') > -1) {
					var url = getControllerActionUrl("porte", "verrouiller", pieceId);
				}
				else if(commande.indexOf('VOLET') > -1) {
					var url = getControllerActionUrl("volet", "fermer", pieceId);
				}
				else {
				//TODO
				}
			}
			console.log(url);
			if(url){
				$.getJSON(url, function( data ){
					notify( data.code < 300 ? 'success' : 'warning', data.message, "", 4000);
				});
			}
		}
	});
}
		


function refreshButton() {
	if(record) {
		//$("#btnMicro").attr("class", 'btn-function btnMicroOn');
	}
	else {
		//$("#btnMicro").attr("class", 'btn-function btnMicro');
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
	resultatParser = parsePhraseRecurrent(phrase, typeIdRecherche);
	
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
function parsePhraseRecurrent(phrase, typeIdRecherche) {
	//console.log('parsePhraseRecurrent('+phrase+', '+typeIdRecherche+')');
	// Fin de la récurrence
	if(phrase.length == 0) {
		return '';
	}
	// Fin de récurrence avec récupérationdu reste de la phrase
	if(typeIdRecherche[0] == 'all') {
		return phrase.join(' ');
	}
	for(var i = 0; i < typeIdRecherche.length; i++) {
		var mot = phrase[0];
		var resultatRecherche = retrouverIdDepuisMotEtTypesDansJson(mot, typeIdRecherche[i]);
		if(resultatRecherche) {
			return resultatRecherche.id + ' ' + parsePhraseRecurrent(phrase.slice(1, phrase.lenght), resultatRecherche.suite);
		}
	}
	return parsePhraseRecurrent(phrase.slice(1, phrase.lenght), typeIdRecherche);
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

/**
 * Refresh a onOffSwitcher for a Piece added with addOnOffSwitcher(...)
 * @param {Piece} piece
 * @returns {void}
 */
function refreshPieceLumiere( piece ){
   
}