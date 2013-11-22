var baseUrl = "../server/";
var auto_refresh = 0;
var joursArray = Array("Lun","Mar","Mer","Jeu","Ven","Sam","Dim");
var state = {};

$(document).ready(function(){
    main();
    //auto refresh alarm
    $("body").on( "maison.refreshed", function(){
        $.each( state.reveils, function( key, val ) {
            if( val.sonne ) {
                notify("danger", "<img src='./img/bell.png'>&nbsp;" + val.heure, val.nom, 0);
                //refresh alarm state
                $("#listeReveils #row_"+val.id+" .nom").removeClass("on off");
                $("#listeReveils #row_"+val.id+" .nom").addClass( (val.actif ? "on" : "off") );
            }
        });
    });
        
    setInterval( function(){
        $.getJSON( getControllerActionUrl("maison", "getState")+"&dest=webapp", function( data ){
            if( data.code < 300 ){
                state = data.state;
                $("body").trigger("maison.refreshed");
            }
            else {
                console.error( data.message );
            }
        });
    }, 1000 );
    
    /**
     * clic sur une fonction
     */
    $("button.btn-function").click(function(){
        clearInterval(auto_refresh);
        $("button.btn-function").removeClass("active");
        $(this).addClass("active");
        switch ( $(this).data("function") ) {
            case "lumiere":
                lumiere();
            break;
            case "volet":
                volet();
            break;
            case "porte":
                porte();
            break;
            case "hifi":
                hifi();
            break;
            case "reveil":
                reveil();
            break;
            case "itineraire":
                itineraire();
            break;
            case "meteo":
                meteo();
            break;
            case "recap":
                recap();
            break;
            case "param":
                param();
            break;
            default :
                main();
        }
    });
    
});

function main(){
    $("#fctTitle").html("Bonjour");
    $("#fctBody").html("Pour commencer, choisissez une fonctionnalité.");
}