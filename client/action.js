//liste des actions effectuées récement
var actionsFaites = Array();

//on écoute l'événement maison.refreshed, et on vas traiter les actions si il y en a.
$(document).ready( function(){
    $("body").on( "maison.refreshed", function(){
        $.each( state.actions, function( key, val ) {
            var action = val;
            //si l'action n'a pas ete effectué :
            if( !estEffectuee( action ) ){
                //on réalise l'action
                moteurEtAction( action );
            }
        });
    });
});


function estEffectuee( action ){
    
    if( !actionsFaites[action.id] ){
        //action jamais faite
        actionsFaites[action.id] = action;
        return false;
    }
    return true;
    
}

function sendAction(action) {
    $.post( getControllerActionUrl("action", "post"), action, function( data ){
        if( data.code < 300 ) {
			;
        }
        else {
            notify("danger", data.message, "Error", 5000);
        }
    }, 'json');
}

function moteurEtAction( action ) {
    console.log( action.action + " action" );
    switch ( action.action ) {
        case "joke" :
            notify( "info", "ric-rolled", "Joke", 5555 );
            break;
        case "meteo" :
            $("#functions #btn_meteo").trigger("click");
			$("body").on("meteo.retrieved", function() {
				$("body").off("meteo.retrieved");
				if ((action.params[0] >= 1) && (action.params[0] <= 7)) {
					/* Retrieve current day number */
					var d = new Date();
					currentWeekDay = d.getDay();
					if (currentWeekDay == 0) currentWeekDay = 7; /* Sunday in smarthome is not 0 but 7 */
					var dayId;
					if (action.params[0] >= currentWeekDay) {
						dayId = action.params[0] - currentWeekDay;
					} else {
						dayId = 7 - (currentWeekDay - action.params[0]); 
					}
					/* Treat action */
					if (dayId == 0) {
						/* Current day: send weather */ 
						sendAction({action: "meteoReponse", dest: "synthese", 
									/* DayId */ 		param0: action.params[0],
									/* Temperature */ 	param1: lastMeteoData.item.condition.temp, 
									/* Maximal */		param2: lastMeteoData.item.forecast[0].high,
									/* Minimal */		param3: lastMeteoData.item.forecast[0].low,
									/* Code */			param4: lastMeteoData.item.forecast[0].code
									});
					} else if ((dayId >= 1) && (dayId < lastMeteoData.item.forecast.length)) {
						/* Day found: send weather */
						/* Compute temp since weather plugin don't send temperature take moy(high,low)*/
						temp = ((parseInt(lastMeteoData.item.forecast[dayId].high) + 
									parseInt(lastMeteoData.item.forecast[dayId].low))/2
								); 
						sendAction({action: "meteoReponse", dest: "synthese", 
									/* DayId */ 		param0: action.params[0],
									/* Temperature */ 	param1: temp, 
									/* Maximal */		param2: lastMeteoData.item.forecast[dayId].high,
									/* Minimal */		param3: lastMeteoData.item.forecast[dayId].low,
									/* Code */			param4: lastMeteoData.item.forecast[dayId].code
									});
						
					} else {
						/* Day not found: send weather unvalaible*/
						sendAction({
							action: "meteoReponse", 
							dest: "synthese", 
							param0: action.params[0]
						});
						
					}
				}
			});
            break;
		case "trafic":
			/* In this action send directly an random percent (0-100) to indicate trafic status */ 
			sendAction({
				action: "traficReponse", 
				dest: "synthese", 
				param0: action.params[0], 
				param1: randomFromInterval(0,100)
			});
		break;
		case "itineraire":
            $("#functions #btn_itineraire").trigger("click");
			// <TODO> 
			//	1. Faire le calcul de l'itineraire
			//	2. Déclenché l'évenement "itineraire.calculer" quand le calcul est finis: $("body").trigger("itineraire.calculer"); 
			//	3. Décommenté les lignes ci-dessous 
			//$("body").on("itineraire.calculer", function() {
				$("body").off("itineraire.calculer");
				sendAction({
					action: "itineraireReponse", 
					dest: "synthese", 
					param0: action.params[0]
				});
			//});
			// </TODO>
			
		break;
        case "playSong" :
            if( !$('#fctBody').hasClass('hifi') ){
                //si pas sur la page
                $("#functions #btn_hifi").trigger("click");
                $("body").on("hifi.player.playble", function(){
                    $("body").off("hifi.player.playble");
                    $("#btnPlay").trigger("player.playrequested");
                });
            }
            else {
                //si deja sur al page, on clic
                $("#btnPlay").trigger("player.playrequested");
            }
            break;
        case "randomSong" :
            if( !$('#fctBody').hasClass('hifi') ){
                //si pas sur la page
                $("#functions #btn_hifi").trigger("click");
                $("body").on("hifi.page.ready", function(){
                    $("body").off("hifi.page.ready");
                    $("#btnRandom").trigger("click");
                });
            }
            else {
                //si deja sur al page, on clic
                $("#btnRandom").trigger("click");
            }
            break;
        case "nextSong" :
            $("#btnNext").trigger("player.nextrequested");
            break;
        case "previousSong" :
            $("#btnPrev").trigger("player.nextrequested");
            break;
        case "pauseSong" :
            $("#btnPlay").trigger("player.pauserequested");

            break;
        case "playSongId" :
            if( !$('#fctBody').hasClass('hifi') ){
                //si pas sur la page
                $("#functions #btn_hifi").trigger("click");
                $("body").on("hifi.page.ready", function(){
                    $("body").off("hifi.page.ready");
                    $("#btnPlay").trigger("player.playsongrequested", action.params[0]);
                });
            }
            else {
                //si deja sur al page, on clic
                $("#btnPlay").trigger("player.playsongrequested", action.params[0]);
            }
            break;
        default :
            notify( "warning", "action inconue : " + action.action, "Gestionnaire d'Actions", 5000 );
            console.log( action );
    }
}