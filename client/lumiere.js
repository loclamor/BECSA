function lumiere() {
    $("#fctTitle").html("Lumières");
    var body = $("#fctBody");
    body.html("");
    body.removeClass();
    body.addClass("lumiere");
    addOnOffSwitcher( body, "all", "Toutes les lumières", "Allumer", "Eteindre",
        getControllerActionUrl("lumiere", "allumerTout"),
        getControllerActionUrl("lumiere", "eteindreTout"),
        "no",
        refreshPieceLumiere
    );
    
    $.getJSON( getControllerActionUrl("lumiere", "lister"), function( data ){
        $.each( data.pieces, function( key, val ) {
            addOnOffSwitcher( body, val.id, val.nom, "Allumer", "Eteindre",
                getControllerActionUrl("lumiere", "allumer", val.id),
                getControllerActionUrl("lumiere", "eteindre", val.id),
                val.lumiereAllumee ? "on" : "off",
                refreshPieceLumiere
            );
        });
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