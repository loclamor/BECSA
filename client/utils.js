
/**
 * 
 * @param {String} controller the controller to ask
 * @param {String} action the action to do
 * @param {Int} id the id of the element to change state
 * @returns {String} the URL
 */
function getControllerActionUrl( controller, action, id ){
    if( id )
        return baseUrl + "?controller=" + controller + "&action=" + action + "&id=" + id;
    return baseUrl + "?controller=" + controller + "&action=" + action;
}

/**
 * Add an On/Off switcher
 * @param {JQueryElement} elt the element where switcher is add
 * @param {String} name the name off the switcher
 * @param {String} text the text to show left of the switcher
 * @param {String} textOn the text for the left button (On)
 * @param {String} textOff the text for the right button (Off)
 * @param {String} urlOn
 * @param {String} urlOff
 * @param {String} state on|off|no
 * @returns {void}
 */
function addOnOffSwitcher( elt, name, text, textOn, textOff, urlOn, urlOff, state ){
    var hiddeOn = "", hiddeOff = "";
    var spanText = 9, spanBtn = 3;
    if( state != "no" ) {
        if( state == "off")
            hiddeOff = "hidden"
        else 
            hiddeOn = "hidden"
    }
    else {
        var spanText = 8, spanBtn = 2;
    }
    elt.append(
            '<div class="row switcher">'
                + '<div class="col-xs-'+spanText+'">' + text + '</div>'
                + '<button type="button" data-loading-text="'+textOn+'..." id="' + name + 'On" class="btn btn-success col-xs-'+spanBtn+' ' + hiddeOn + '">'
                    + textOn
                + '</button>'
                + '<button type="button" data-loading-text="'+textOff+'..." id="' + name + 'Off" class="btn btn-danger col-xs-'+spanBtn+' ' + hiddeOff + '">'
                    + textOff
                + '</button>'
//                + '<div class="btn-group col-xs-3" id="' + name + 'BtnGroup"data-toggle="buttons">'
//                    + '<label class="btn btn-success" id="' + name + 'On" data-loading-text="Loading..." >'
//                        + '<input type="radio" name="' + name + '" > ' + textOn
//                    + '</label>'
//                    + '<label class="btn btn-danger" id="' + name + 'Off" data-loading-text="Loading...">'
//                        + '<input type="radio" name="' + name + '" > ' + textOff
//                    +'</label>'
//                + '</div>'
            + '</div>'
    );
    //$('#' + name + 'BtnGroup').button()
    //attach events
    $('#' + name + 'On').on( "click", function(){
        $(this).button('loading');
        $.getJSON(urlOn, function( data ){
            if( state != "no" ) {
                $('#' + name + 'On').addClass("hidden");
                $('#' + name + 'Off').removeClass("hidden");
                $('#' + name + 'On').button("reset");
            }
            else {
                $('#' + name + 'On').button("reset");
            }
            //TODO : refresh with new state (or use a param function ? ) 
        });
    });
    $('#' + name + 'Off').on( "click", function(){
        $(this).button('loading');
        $.getJSON(urlOff, function( data ){
            if( state != "no" ) {
                $('#' + name + 'Off').addClass("hidden");
                $('#' + name + 'On').removeClass("hidden");
                $('#' + name + 'Off').button("reset");
            }
            else {
                $('#' + name + 'Off').button("reset");
            }
            //TODO : refresh with new state (or use a param function ? ) 
        });
    });
}