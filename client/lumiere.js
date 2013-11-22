function lumiere() {
    $("#fctTitle").html("Lumières");
    var body = initBodyPage('lumiere');
    addOnOffSwitcher( body, "all", "Toutes les lumières", "Allumer", "Eteindre",
        getControllerActionUrl("lumiere", "allumerTout"),
        getControllerActionUrl("lumiere", "eteindreTout"),
        "no",
        refreshPieceLumiere
    );
    
    var list = initPageList('lumiere');
    
    $.getJSON( getControllerActionUrl("lumiere", "lister"), function( data ){
        $.each( data.pieces, function( key, val ) {
            addOnOffSwitcher( list, val.id, val.nom, "Allumer", "Eteindre",
                getControllerActionUrl("lumiere", "allumer", val.id),
                getControllerActionUrl("lumiere", "eteindre", val.id),
                val.lumiereAllumee ? "on" : "off",
                refreshPieceLumiere
            );
        });
        list.slideDown(500);
    });
    
    //listen refresh
    $("body").on( "maison.refreshed", function(){
        if( $("#fctBody").hasClass('lumiere') ){
            $.each( state.pieces, function( key, val ) {
                if( val.aLumiere )
                    refreshPieceLumiere( val );
            });
        }
    });
}

/**
 * Refresh a onOffSwitcher for a Piece added with addOnOffSwitcher(...)
 * @param {Piece} piece
 * @returns {void}
 */
function refreshPieceLumiere( piece ){
    //change imgState
    var eltImgState = $("#onOffRow_" + piece.id + " .imgState");
    if( !eltImgState.hasClass("no") ){
        eltImgState.removeClass( piece.lumiereAllumee ? "off" : "on" );
        eltImgState.addClass( piece.lumiereAllumee ? "on" : "off" );
    }

    var eltBtnOn = $("#onOff_"+piece.id+"On");
    var eltBtnOff = $("#onOff_"+piece.id+"Off");
    if( eltBtnOn.hasClass("hidden") || eltBtnOff.hasClass("hidden") ){
        eltBtnOn.removeClass("hidden");
        eltBtnOff.removeClass("hidden");
        eltBtnOn.addClass( piece.lumiereAllumee ? "hidden" : "" );
        eltBtnOff.addClass( piece.lumiereAllumee ? "" : "hidden" );
    }
}