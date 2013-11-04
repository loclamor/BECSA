$(document).ready(function(){
    /**
     * clic sur une fonction
     */
    $("button.btn-function").click(function(){
        $("button.btn-function").removeClass("active");
        $(this).addClass("active");
    });
});
