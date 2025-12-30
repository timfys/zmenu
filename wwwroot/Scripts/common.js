$(document).ready(function () {
    $(".OnlyNumeric").keypress(function(e) {
        var regex = new RegExp("^[0-9\u0000\u0008]+$");

        var str = String.fromCharCode(!e.charCode ? e.which : e.charCode);
        if (regex.test(str)) {
            return true;
        }
        e.preventDefault();
        return false;
    });

    $(".OnlyAlphabatic").keypress(function(e) {
        var regex = new RegExp("^[a-zA-Z\\s]+$");
        var str = String.fromCharCode(!e.charCode ? e.which : e.charCode);
        if (regex.test(str)) {
            return true;
        }

        e.preventDefault();
        return false;
    });

    $(".OnlyAlphaNumeric").keypress(function(e) {
        var regex = new RegExp("^[a-zA-Z0-9\\s]+$");
        var str = String.fromCharCode(!e.charCode ? e.which : e.charCode);
        if (regex.test(str)) {
            return true;
        }

        e.preventDefault();
        return false;
    });

    $('input[type="text"]').focus(function () {
        $(this).addClass("focus");
    });

    $('input[type="text"]').blur(function () {
        $(this).removeClass("focus");
    });

    $('input[type="password"]').focus(function () {
        $(this).addClass("focus");
    });

    $('input[type="password"]').blur(function () {
        $(this).removeClass("focus");
    });
})

function LoadingSubmitStart(className) {
    $('form').submit(function (e) {
        if (!$(this).hasClass('needs-validation')) {
            //$(className).attr("disabled", true);
            setTimeout(function () {
                var isErrorAbsent = true;
                var allInputs = $(':input');
                $(':input').each(function () {
                    if ($(this).hasClass('input-validation-error')) {
                        isErrorAbsent = false;
                    }
                });
                if (isErrorAbsent) {
                    $('#loading').modal({ backdrop: 'static', keyboard: false });
                }
                else {
                    //$(className).attr("disabled", false);
                }
            }, 100);
        }
        return true;
    });
}

function LoadingStart() {
    setTimeout(function () {
        $('#loading').modal({ backdrop: 'static', keyboard: false });
    }, 100);
}

function LoadingEnd() {
    setTimeout(function () {
        $('#loading').modal("hide");
    }, 100);
}

var loadTime = function(){
    setTimeout(function(){
        var perfData = window.performance.timing;
        var EstimatedTime = (perfData.loadEventEnd - perfData.navigationStart)/1000;

        color = (EstimatedTime >= 5)? 'red': 'black';
        $('#loadingTimeInfo')
            .text('Loading time: ' + EstimatedTime + ' seconds')
            .css('color', color);
})}

