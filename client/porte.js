function porte() {
    $("#fctTitle").html("Portes");
    var body = $("#fctBody");
    body.html("");
    body.removeClass();
    body.addClass("porte");
    addOnOffSwitcher( body, "all", "Toutes les portes", "Déverrouiller", "Verrouiller",
        getControllerActionUrl("porte", "deverrouillerTout"),
        getControllerActionUrl("porte", "verrouillerTout"),
        "no",
        refreshPiecePorte
    );
    
    $.getJSON( getControllerActionUrl("porte", "lister"), function( data ){
        $.each( data.pieces, function( key, val ) {
            addOnOffSwitcher( body, val.id, val.nom, "Déverrouiller", "Verrouiller",
                getControllerActionUrl("porte", "deverrouiller", val.id),
                getControllerActionUrl("porte", "verrouiller", val.id),
                val.porteVerrouillee ? "off" : "on",
                refreshPiecePorte
            );
        });
    });
    
    auto_refresh = setInterval(
        function (){
            //refresh
            $.getJSON( getControllerActionUrl("porte", "lister"), function( data ){
                $.each( data.pieces, function( key, val ) {
                    refreshPieceLPorte( val );
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
function refreshPiecePorte( piece ){
        //change imgState
        var eltImgState = $("#onOffRow_" + piece.id + " .imgState");
        if( !eltImgState.hasClass("no") ){
            eltImgState.removeClass( piece.porteVerrouillee ? "on" : "off" );
            eltImgState.addClass( piece.porteVerrouillee ? "off" : "on" );
        }
        
        var eltBtnOn = $("#onOff_"+piece.id+"On");
        var eltBtnOff = $("#onOff_"+piece.id+"Off");
        if( eltBtnOn.hasClass("hidden") || eltBtnOff.hasClass("hidden") ){
            eltBtnOn.removeClass("hidden");
            eltBtnOff.removeClass("hidden");
            eltBtnOn.addClass( piece.porteVerrouillee ? "" : "hidden" );
            eltBtnOff.addClass( piece.porteVerrouillee ? "hidden" : "" );
        }
    }