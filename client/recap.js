function recap() {
    $("#fctTitle").html("Récapitulatif");
    var body = initBodyPage('recap');
    body.append(
        '<div class="row title">'
            + '<div class="col-xs-6" >Piece</div>'
            + '<div class="col-xs-2" >Lumiere</div>'
            + '<div class="col-xs-2" >Volet</div>'
            + '<div class="col-xs-2" >Porte</div>'
        + '</div>'
    );
    var list = initPageList('recap');
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
            list.append(
                '<div class="row" id="row_' + val.id + '">'
                    + '<div class="col-xs-6 nom" >' + val.nom + '</div>'
                    + '<div class="col-xs-2 lumiere ' + lum + '" ></div>'
                    + '<div class="col-xs-2 volet ' + volet + '" ></div>'
                    + '<div class="col-xs-2 porte ' + porte + '" ></div>'
                + '</div>'
            );
        });
        list.slideDown(500);
    });
    
    //listen refresh
    $("body").on( "maison.refreshed", function(){
        if( $("#fctBody").hasClass('recap') ){
            $.each( state.pieces, function( key, val ) {
                refreshPiece( val );
            });
        }
    });
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