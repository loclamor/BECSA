function hifi() {
    $("#fctTitle").html("HiFi");
    var body = initBodyPage('hifi');
        
    var list = initPageList('hifi');
    
    body.html("<iframe id='hifi' src='http://toma.hk/embed.php?artist=Grouplove&title=Tongue+Tied' width='200' scrolling='no' height='200' frameborder='0' allowtransparency='true'></iframe>");

//    $.getJSON( getControllerActionUrl("hifi", "lister"), function( data ){
//        $.each( data.songs, function( key, val ) {
//        	print val.id + ' : ' + val.artist + ' - ' + val.title;
//        });
//        list.slideDown(500);
//    });
    

}
