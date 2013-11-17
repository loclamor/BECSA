function initBodyPage( page ) {
    var body = $("#fctBody");
    body.html("");
    body.removeClass();
    body.addClass( page );
    body = $("#fctBody."+page);
    console.log(body);
    return body;
}

function initPageList( page ) {
    $("#fctBody."+page).append('<div class="list" style="display: none;"></div>');
    var list = $("#fctBody."+page+" .list");
    return list;
}
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
 * @param {String} eltId the id of the element to switch (all or piece.id)
 * @param {String} text the text to show left of the switcher
 * @param {String} textOn the text for the left button (On)
 * @param {String} textOff the text for the right button (Off)
 * @param {String} urlOn
 * @param {String} urlOff
 * @param {String} state on|off|no
 * @param {function} refreshPiece a function to refresh ONE piece : function refresh( Piece ){...}
 * @returns {void}
 */
function addOnOffSwitcher( elt, eltId, text, textOn, textOff, urlOn, urlOff, state, refreshPiece ){
    console.log(elt);
    var name = "onOff_" + eltId;
    var hiddeOn = "", hiddeOff = "";
    var spanText = 9, spanBtn = 3;
    if( state !== "no" ) {
        if( state == "off")
            hiddeOff = "hidden"
        else 
            hiddeOn = "hidden"
    }
    else {
        spanText = 8;
        spanBtn = 2;
    }
    elt.append(
            '<div class="row switcher" id="onOffRow_' + eltId + '">'
                + '<div class="col-xs-'+spanText+' nomElt">'
                    + '<div class="imgState '+state+'"></div>'
                    + '<span>' + text + '</span>'
                + '</div>'
                + '<button type="button" data-loading-text="'+textOn+'..." id="' + name + 'On" class="btn btn-success col-xs-'+spanBtn+' onBtn_' + eltId + ' ' + hiddeOn + '">'
                    + textOn
                + '</button>'
                + '<button type="button" data-loading-text="'+textOff+'..." id="' + name + 'Off" class="btn btn-danger offBtn_' + eltId + ' col-xs-'+spanBtn+' ' + hiddeOff + '">'
                    + textOff
                + '</button>'
            + '</div>'
    );
    //attach events
    $('#' + name + 'On').on( "click", function(){
        $(this).button('loading');
        $.getJSON(urlOn, function( data ){
            if( state !== "no" && data.code < 300 ) {
                $('#' + name + 'On').addClass("hidden");
                $('#' + name + 'Off').removeClass("hidden");
                $('#' + name + 'On').button("reset");
            }
            else {
                $('#' + name + 'On').button("reset");
            }
            notify( data.code < 300 ? 'success' : 'warning', data.message, "", 4000);
            //refresh
            refresh( data );
        });
    });
    $('#' + name + 'Off').on( "click", function(){
        $(this).button('loading');
        $.getJSON(urlOff, function( data ){
            if( state !== "no" && data.code < 300 ) {
                $('#' + name + 'Off').addClass("hidden");
                $('#' + name + 'On').removeClass("hidden");
                $('#' + name + 'Off').button("reset");
            }
            else {
                $('#' + name + 'Off').button("reset");
            }
            notify( data.code < 300 ? 'success' : 'warning', data.message, "", 4000);
            //refresh
            refresh( data );
        });
    });
    
    function refresh( data ) {
        if( eltId === "all" ){
            if( data.pieces ){
                //refresh all pieces
                $.each( data.pieces, function( key, val ) {
                    refreshPiece( val )
                });
            }
        }
        else {
            if( data.piece ){
                //refresh the piece
                refreshPiece( data.piece )
            }
        }
    }
}

/**
 * display a notification in the notification bar 
 * @param {String} type type of the message, in [success|info|warning|danger]
 * @param {String} message message of the notification
 * @param {String} title title of the notification
 * @param {Int} timeout timeout in ms
 * @returns {void}
 */
function notify( type, message, title, timeout ) {
    var _timeout = 0;
    var _title = "";
    var notifbar = $("#notif-row");
    if( timeout )
        _timeout = timeout;
    if( title && title !== "" )
        _title = '<h4>' + title + '</h4>';
    var notifbox = $('<div class="alert alert-' + type + ' fade in">'
                + '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>'
                + _title
                + message
            + '</div>');
    notifbar.append( notifbox );
    //set timeout if needed
    if( _timeout > 0 ) {
        notifbox.slideDown(1000).delay(_timeout).queue(function() { $(this).slideUp(1000).alert("close"); });
    }
}