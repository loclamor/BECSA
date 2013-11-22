function meteo() {
    $("#fctTitle").html("Météo");
    var body = initBodyPage('meteo');
    body.removeClass();
    body.html("<div id='meteo'></div>");
    $('#meteo').ready(function () {
	$('#meteo').weatherfeed(['FRXX0099'],{
		unit: 'c',
		image: true,
		country: true,
		highlow: true,
		wind: true,
		humidity: true,
		visibility: true,
		sunrise: true,
		sunset: true,
		forecast: true,
		link: false
	},translate());
});
}


function translate(){
    
}