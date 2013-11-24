
//auto refresh alarm
$(document).ready(function(){
    $("body").on( "maison.refreshed", function(){
        $.each( state.reveils, function( key, val ) {
            if( val.sonne && !reveilASonne(val.id)) {
                notify("danger", "<img src='./img/bell.png'>&nbsp;" + val.heure, val.nom, 0);
                //refresh alarm state
                $("#listeReveils #row_"+val.id+" .nom").removeClass("on off");
                $("#listeReveils #row_"+val.id+" .nom").addClass( (val.actif ? "on" : "off") );
            }
        });
    });
});

function reveilASonne( reveil ){
    var dateM = Date();
    
    if( !reveilsRinged[reveil.id] ){
        reveilsRinged[reveil.id] = reveil;
        return false;
    }
    //le reveil est présent en mémoire.
    //a t-il sonne aujourd'hui (celui en mémoire) ?
    var dateReveil = reveilsRinged[reveil.id].lastRing;
    var dateReveilArray = dateReveil.split("-");
    if( dateM.getDate() == parseInt(dateReveilArray[2]) && ( dateM.getMonth() +1 ) == parseInt(dateReveilArray[1]) ){
        reveilsRinged[reveil.id] = reveil;
        return true;
    }
    else {
        reveilsRinged[reveil.id] = reveil;
        return false;
    }
    
}

function reveil() {
    $("#fctTitle").html("Réveils");
    var body = initBodyPage('reveil');
    
    body.append(
        '<div id="listeReveils" style="display:none;"><div class="row title">'
            + '<div class="pull-left status" ></div>'
            + '<div class="col-xs-4" >Reveil</div>'
            + '<div class="col-xs-1" >Heure</div>'
            + '<div class="col-xs-5" >Jours</div>'
        + '</div></div><div id="ajout"></div>'
    );
    
    //get reveil list
    $.getJSON( getControllerActionUrl("reveil", "lister"), function( data ){
        $.each( data.reveils, function( key, val ) {
            var actif = ( val.actif ? "on" : "off" );
            var nom = val.nom;
            var heure = val.heure;
            var joursBrut = val.jour;
            var joursBrutArray = joursBrut.split(",");
            var jours = "";
            for (var i = 0; i<7; i++){
                if( joursBrutArray[i] == "1" ) {
                    if (jours != ""){
                        jours += ", ";
                    }
                    jours += joursArray[i];
                }
            }
            
            $("#listeReveils").append(
                '<div class="row" id="row_' + val.id + '">'
                    + '<div class="pull-left status ' + actif + '" ></div>'
                    + '<h4 class="col-xs-4 nom " >' + nom + '</h4>'
                    + '<div class="col-xs-1 heure" >' + heure + '</div>'
                    + '<div class="col-xs-5 jours" >' + jours + '</div>'
                    + '<div class="pull-right delete" ></div>'
                + '</div>'
            );
            
            //attach click listener
            $('#fctBody.reveil .row#row_' + val.id + ' .status').on('click', function(){reveilActifListener( $(this), val.id );});
            $('#fctBody.reveil .row#row_' + val.id + ' .delete').on('click', function(){reveilSupprListener( $(this), val.id );});

        });
        $("#listeReveils").slideDown(500);
    });
    
    // creation reveil
    var ajout = $("#fctBody #ajout");
    $.get("./ajoutReveil.html", function(data){
        ajout.append(data);
    });

}

function reveilActifListener( elt, id ){
    var action = "";
    if( elt.hasClass('on') ) {
        //on desactive
        action = "desactiver";
    }
    else {
        //on active
        action = "activer";
    }
    elt.removeClass('on off');
    elt.addClass('wait');
    //requete
    $.getJSON( getControllerActionUrl("reveil", action, id), function( data ){
        $('#fctBody.reveil .row#row_' + data.reveil.id + ' .status').removeClass( 'wait' );
        $('#fctBody.reveil .row#row_' + data.reveil.id + ' .status').addClass( (data.reveil.actif ? "on" : "off" ) );
        notify( (data.code < 300 ? 'success' : 'warning') , data.message, '', 4000);
    });
}

function reveilSupprListener( elt, id ){
    elt.parent('.row').remove();
    //requete
    $.getJSON( getControllerActionUrl("reveil", "supprimer", id), function( data ){
        console.log(elt + " : reveil " + id + " supprime : " + data);
        notify( (data.code < 300 ? 'info' : 'warning') , data.message, '', 4000);
    });
}