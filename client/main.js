$(document).ready(function(){
    /**
     * clic sur une fonction
     */
    $("button.function").click(function(){
        $("button.function").removeClass("active");
        $(this).addClass("active");
    });
});
