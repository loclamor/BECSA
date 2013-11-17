function volet() {
    $("#fctTitle").html("Volets");
    var body = initBodyPage('volet');
    addOnOffSwitcher( body, "all", "Tous les volets", "Ouvrir", "Fermer",
        getControllerActionUrl("volet", "ouvrirTout"),
        getControllerActionUrl("volet", "fermerTout"),
        "no",
        refreshPieceVolet
    );
    
    var list = initPageList('volet');
    
    $.getJSON( getControllerActionUrl("volet", "lister"), function( data ){
        $.each( data.pieces, function( key, val ) {
            addOnOffSwitcher( list, val.id, val.nom, "Ouvrir", "Fermer",
                getControllerActionUrl("volet", "ouvrir", val.id),
                getControllerActionUrl("volet", "fermer", val.id),
                val.voletOuvert ? "on" : "off",
                refreshPieceVolet
            );
        });
        list.slideDown(500);
    });
    
    auto_refresh = setInterval(
        function (){
            //refresh
            $.getJSON( getControllerActionUrl("volet", "lister"), function( data ){
                $.each( data.pieces, function( key, val ) {
                    refreshPieceVolet( val );
                });
            });
        },
        1000
    ); // refresh every 1000 milliseconds
}

/**
 * Refresh a onOffSwitcher for a Piece added with addOnOffSwitcher(...)
 * @param {Piece} piece
 * @returns {void}
 */
function refreshPieceVolet( piece ){
        //change imgState
        var eltImgState = $("#onOffRow_" + piece.id + " .imgState");
        if( !eltImgState.hasClass("no") ){
            eltImgState.removeClass( piece.voletOuvert ? "off" : "on" );
            eltImgState.addClass( piece.voletOuvert ? "on" : "off" );
        }
        
        var eltBtnOn = $("#onOff_"+piece.id+"On");
        var eltBtnOff = $("#onOff_"+piece.id+"Off");
        if( eltBtnOn.hasClass("hidden") || eltBtnOff.hasClass("hidden") ){
            eltBtnOn.removeClass("hidden");
            eltBtnOff.removeClass("hidden");
            eltBtnOn.addClass( piece.voletOuvert ? "hidden" : "" );
            eltBtnOff.addClass( piece.voletOuvert ? "" : "hidden" );
        }
    }