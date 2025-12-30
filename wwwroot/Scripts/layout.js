$(document).ready(function () {
    if ((window.location.pathname == "/")) {
        var objA = $(".sidebar a:first");
        //$(objA).parent().addClass('active');
    }

    $(".sidebar a").each(function () {
        if ((window.location.pathname.indexOf($(this).attr('href'))) > -1) {
            $(this).parent().addClass('active');

            if ($(this).parent().parent().hasClass("list-unstyled")) {
                $(this).parent().parent().css("display", "block");
            }
        }
    });
})