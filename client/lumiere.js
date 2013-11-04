function lumiere() {
    $("#fctTitle").html("Lumières");
    var body = $("#fctBody");
    body.html("");
    addOnOffSwitcher( body, "allumerTout", "Toutes les lumières", "Allumer", "Eteindre",
        getControllerActionUrl("lumiere", "allumerTout"),
        getControllerActionUrl("lumiere", "eteindreTout"),
        "no"
    );
    
    $.getJSON( getControllerActionUrl("lumiere", "lister"), function( data ){
        $.each( data.pieces, function( key, val ) {
            addOnOffSwitcher( body, "allumer"+val.nom, val.nom, "Allumer", "Eteindre",
                getControllerActionUrl("lumiere", "allumer", val.id),
                getControllerActionUrl("lumiere", "eteindre", val.id),
                val.lumiereAllumee ? "on" : "off"
            );
        });
    });
}

