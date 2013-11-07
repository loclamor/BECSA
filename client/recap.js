function recap() {
    $("#fctTitle").html("Récapitulatif");
    var body = $("#fctBody");
    body.html("");
    body.removeClass();
    body.addClass("recap");
    body.append(
        '<div class="row title">'
            + '<div class="col-xs-3" >Piece</div>'
            + '<div class="col-xs-2" >Lumiere</div>'
            + '<div class="col-xs-2" >Volet</div>'
            + '<div class="col-xs-2" >Porte</div>'
        + '</div>'
    );
    $.getJSON( getControllerActionUrl("piece", "lister"), function( data ){
        $.each( data.pieces, function( key, val ) {
            var lum = "no";
            if( val.aLumiere ) {
                lum = ( val.lumiereAllumee ? "on" : "off" );
            }
            var volet = "no";
            if( val.aVolet ) {
                volet = ( val.voletOuvert ? "on" : "off" );
            }
            var porte = "no";
            if( val.aPorte ) {
                porte = ( val.porteVerrouillee ? "off" : "on" );
            }
            body.append(
                '<div class="row" id="row_' + val.id + '">'
                    + '<div class="col-xs-3 nom" >' + val.nom + '</div>'
                    + '<div class="col-xs-2 lumiere ' + lum + '" ></div>'
                    + '<div class="col-xs-2 volet ' + volet + '" ></div>'
                    + '<div class="col-xs-2 porte ' + porte + '" ></div>'
                + '</div>'
            );
        });
    });
    
    auto_refresh = setInterval(
        function (){
            //refresh
            $.getJSON( getControllerActionUrl("piece", "lister"), function( data ){
                $.each( data.pieces, function( key, val ) {
                    refreshPiece( val );
                });
            });
        },
        1000
    ); // refresh every 1000 milliseconds
}


function refreshPiece( piece ){
    //refresh lumiere state
    var eltLum = $("#row_" + piece.id + " .lumiere");
    if( !eltLum.hasClass("no") ){
        eltLum.removeClass( piece.lumiereAllumee ? "off" : "on" );
        eltLum.addClass( piece.lumiereAllumee ? "on" : "off" );
    }
    
    //refresh volet state
    var eltVolet = $("#row_" + piece.id + " .volet");
    if( !eltVolet.hasClass("no") ){
        eltVolet.removeClass( piece.voletOuvert ? "off" : "on" );
        eltVolet.addClass( piece.voletOuvert ? "on" : "off" );
    }
    
    //refresh volet state
    var eltPorte = $("#row_" + piece.id + " .porte");
    if( !eltPorte.hasClass("no") ){
        eltPorte.removeClass( piece.porteVerrouillee ? "on" : "off" );
        eltPorte.addClass( piece.porteVerrouillee ? "off" : "on" );
    }
}