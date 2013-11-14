function reveil() {
    $("#fctTitle").html("Réveils");
    var body = $("#fctBody");
    body.html("");
    body.removeClass();
    body.addClass("reveil");
    
    body.append(
        '<div id="listeReveils"><div class="row title">'
            + '<div class="col-xs-3" >Reveil</div>'
            + '<div class="col-xs-2" >Heure</div>'
            + '<div class="col-xs-4" >Jours</div>'
        + '</div></div>'
    );
    
    //get reveil list
    $.getJSON( getControllerActionUrl("reveil", "lister"), function( data ){
        $.each( data.reveils, function( key, val ) {
            var actif = ( val.actif ? "on" : "off" );
            var nom = val.nom;
            var heure = val.heure;
            var joursBrut = val.jour;
            var joursArray = Array("Lun","Mar","Mer","Jeu","Ven","Sam","Dim");
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
                    + '<div class="col-xs-3 nom ' + actif + '" >' + nom + '</div>'
                    + '<div class="col-xs-2 heure" >' + heure + '</div>'
                    + '<div class="col-xs-4 jours" >' + jours + '</div>'
                + '</div>'
            );

        });
    });
    
    //TODO : creation reveil

}