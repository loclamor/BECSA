/**
* Initialize the itineraire panel
*/
function itineraire(callBack){
	$("#fctTitle").html("Itinéraire");
    var body = $("#fctBody");
    body.removeClass();
	body.html("<div id='ajout'></div>");


    var ajout = $("#fctBody #ajout");
    $.get("./carteParametre.html", function(data){
        ajout.append(data);
		initialize('map');
		if(callBack) callBack;
    });
}

// Home position 
var homePosition = new google.maps.LatLng(46.5,2.5);
// Displayed map
var map;
// Home marker
var homeMarker;
// Direction and traffic display
var directionsDisplay;
var trafficLayer;

/**
* Initialize the map in the element having the id
* @param id : Id of the element that will contain the map	
*/
function initialize(elementId) {
	// Home position (center of France by default)
	var adresseSaved;
	var latSaved, lngSaved;
	if(readCookie) {
		latSaved = readCookie("lat");
		lngSaved = readCookie("lng");
	}
	if(!latSaved || !lngSaved) {
		homePosition = new google.maps.LatLng(46.5,2.5);
	}
	else {
		homePosition = new google.maps.LatLng(latSaved,lngSaved);
	}
	// Default map option
	var mapOptions = {
		center: homePosition,
		zoom: 6,
		mapTypeId: google.maps.MapTypeId.ROADMAP
	};
	// Generate the map
	map = new google.maps.Map(document.getElementById(elementId),
								  mapOptions
	);
	
	map.fitBounds(new google.maps.LatLngBounds(
		new google.maps.LatLng(41.5,-5.0),
		new google.maps.LatLng(51.0,10.0)
	));
	// Add a marqueur on the home position
	homeMarker = new google.maps.Marker({
		map : map,
		position : homePosition
	});
}

/**
* Change the adress of the home
* @param adresse The new adress string
* @param stateCode Code of the state of the home
* @param callback a calback function that can use date from adress
*/
function changeHome(adress, stateCode, callBack, saveCallback) {
	// Recenter on the user adress
	var urlAdress = "http://maps.google.com/maps/api/geocode/json?address="+adress+","+stateCode+"&sensor=false";
	// Get the adress information
	$.getJSON(urlAdress, function( data ){
		console.log('Change gome data : ');
		console.log(data);
		// If adress found
		if(data.status == "OK") {
			// Get location coordinate
			var location = data.results[0].geometry.location;
			if(createCookie) {
				if(data.results[0].formatted_address) {
					createCookie("adress",data.results[0].formatted_address);
				}
				else {
					console.log("MAP : Save " + adress);
					createCookie("adress",adress);
				}
				createCookie("lat",location.lat);
				createCookie("lng",location.lng);
			}

			// Change the home position
			homePosition = new google.maps.LatLng(location.lat, location.lng);
			console.log(homePosition);
			// Recenter the map
			map.panTo(homePosition);
			map.setZoom(10);
			homeMarker.setPosition(homePosition);
			if(callBack) callBack(data);
		}
	});
}

/**
* Display the path to the destination
* @param adress Adressof the destination
* @param stateCode Code of the state of the destination
* @param callback a calback function that can use date from adress
*/
function newDestination(adress, stateCode, callBack) {
	// Direction display
	if(directionsDisplay) {
		directionsDisplay.setMap();
	}
	directionsDisplay = new google.maps.DirectionsRenderer();
	// Direction service
	var directionsService = new google.maps.DirectionsService();
	// URL of the adress
	var urlAdress = "http://maps.google.com/maps/api/geocode/json?address="+adress+","+stateCode+"&sensor=false";
	// Retrieve the destination coordinate
	$.getJSON(urlAdress, function( data ){
		console.log(data);
		if(data.status == "OK") {
			// Compute and display the path
			var location = data.results[0].geometry.location;
			var request = {
				origin : homePosition,
				destination : new google.maps.LatLng(location.lat, location.lng),
				travelMode : google.maps.TravelMode.DRIVING
			};
			directionsService.route(request, function(result, status) {
				if (status == google.maps.DirectionsStatus.OK) {
					directionsDisplay.setDirections(result);
					directionsDisplay.setMap(map);
					try {
						var distance = result.routes[0].legs[0].distance.text;
						var duree = result.routes[0].legs[0].duration.text;
						notify('info',"Temps de trajet : " + duree + "<BR>Distance du trajet : " + distance,"",3000);
					}
					catch(e) {
						notify('info',"Informations sur le trajet indisponibles.","",3000);
					}
				}
			});
		}
		if(callBack) callBack(data);
	});
}

function geolocation(callback) {
	if (navigator.geolocation) {
		navigator.geolocation.getCurrentPosition(function(position) {
			console.log('Position : ');
			console.log(position);
			if(position.coords) {
				callback({lat : position.coords.latitude, lng : position.coords.longitude});
			}
			else {
				notify('error' ,"Impossible de retrouver votre position.","",3000);
			}
		});
	}
	else{
		notify('error' ,"Geolocalisation impossible sur votre navigateur.","",3000);
	}	
}

function traffic(displayOrNot) {
	if(displayOrNot === undefined) {
		displayOrNot = !trafficLayer;
	}
	if(trafficLayer) {
		trafficLayer.setMap();
		trafficLayer = false;
	}
	if(displayOrNot) {
		console.log("MAP : Traffic ON");
		trafficLayer = new google.maps.TrafficLayer();
		trafficLayer.setMap(map);
	}
}

function setZoom(zoom) {
	map.setZoom(zoom);
}

function retrieveAdress(location, callback) {
	var url = "http://maps.googleapis.com/maps/api/geocode/json?latlng="+location.lat+","+location.lng+"&sensor=false";
	$.getJSON(url, function( data ){
		console.log('Adresse :');
		console.log(data);
		if(data.status == "OK") {
			notify('info' , "Adresse mise à jour.","",3000);
			callback(data.results[0].formatted_address);
		}
		else {
			notify('error' ,"Impossible de retrouver votre adresse.","",3000);
		}
	});
}