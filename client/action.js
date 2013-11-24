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

function moteurEtAction( action ) {
    console.log( action.action + " action" );
    switch ( action.action ) {
        case "joke" :
            notify( "info", "ric-rolled", "Joke", 5555 );
            break;
        case "meteo" :
            $("#functions #btn_meteo").trigger("click");
            break;
        default :
            notify( "warning", "action inconue : " + action.action, "Gestionnaire d'Actions", 5000 );
            console.log( action );
    }
}