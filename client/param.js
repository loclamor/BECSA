function param() {
    $("#fctTitle").html("Paramètres");
    var body = $("#fctBody");
    body.html("");
    body.removeClass();
    body.addClass("param");
    $.get("./ajout.html", function(data){
        body.append(data);
    });
}