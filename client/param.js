function param() {
    $("#fctTitle").html("Paramètres");
    var body = initBodyPage('param');
    $.get("./ajout.html", function(data){
        body.append(data);
    });
}