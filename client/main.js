var baseUrl = "../server/";
var auto_refresh = 0;
var joursArray = Array("Lun","Mar","Mer","Jeu","Ven","Sam","Dim");
var state = {};
var reveilsRinged = Array();

$(document).ready(function(){
    main();
    
    //menu toggle small display
    $("#btn-menu").click(function(){
        if( $("#functions").hasClass("displayed") ) {
            $("#btn-menu").animate({
                left: 0
            }, 500, "linear", function() { 
                $("#functions").removeClass("displayed");
            });
            $("#functions").animate({
                width: 0
            }, 500, "linear", function() { 
                $("#btn-menu").removeClass("displayed");
                $("#functions").removeClass("done");
                $("#functions").attr("style","");
            });
        }
        else {
            $("#functions").addClass("displayed");
            $("#btn-menu").addClass("displayed");
            $("#btn-menu").animate({
                left: 199
            }, 500, "linear", function() {});
            $("#functions").animate({
                width: 200
            }, 500, "linear", function() {
                $("#functions").addClass("done");
                $("#functions").attr("style","");
            });
            
        }
    });
    
    
    var stateUrl = getControllerActionUrl("maison", "getState")+"&dest=webapp";
    function getHouseState() {
        $.getJSON( stateUrl, function( data ){
            if( data.code < 300 ){
                if( data.code != 204 ) {
                    state = data.state;
                    $("body").trigger("maison.refreshed");
                    console.log( "maison.refreshed emmited" );
                }
                stateUrl = getControllerActionUrl("maison", "getState")+"&dest=webapp&timestamp="+data.timestamp;
            }
            else {
                console.error( data.message );
            }
            getHouseState()
        }).fail(function() {
            console.log( "An error occured while getting house state" );
            getHouseState();
          });
    }
    getHouseState();
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