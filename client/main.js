var baseUrl = "../server/";

$(document).ready(function(){
    main();
    /**
     * clic sur une fonction
     */
    $("button.btn-function").click(function(){
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
            default :
                main();
        }
    });
    
});

function main(){
    $("#fctTitle").html("Bonjour");
    $("#fctBody").html("Pour commencer, choisissez une fonctionnalité.");
}