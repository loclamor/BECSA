var baseUrl = "../server/";
var auto_refresh = 0;

$(document).ready(function(){
    main();
    //auto refresh alarm
    setInterval(
        function (){
            //get reveils
            $.getJSON( getControllerActionUrl("reveil", "lister"), function( data ){
                $.each( data.reveils, function( key, val ) {
                    if( val.sonne ) {
                        notify("danger", "<img src='./img/bell.png'>&nbsp;" + val.heure, val.nom, 0);
                        //refresh alarm state
                        $("#listeReveils #row_"+val.id+" .nom").removeClass("on off");
                        $("#listeReveils #row_"+val.id+" .nom").addClass( (val.actif ? "on" : "off") );
                    }
                });
            });
        },
        60000
    ); // refresh every minutes
    
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
			case "micro" :
				micro();
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