
function openNav() {
    document.querySelectorAll(".qr-menu-open")[0].style.display = "none";
    document.querySelectorAll(".qr-menu-close")[0].style.display = "block";
    document.getElementById("qr-sidebar-menu").style.top = "10px";
    document.getElementById("menuContainerrtl").style.top = "10px";
}

function closeNav() {
    document.querySelectorAll(".qr-menu-close")[0].style.display = "none";
    document.querySelectorAll(".qr-menu-open")[0].style.display = "block";
    document.getElementById("qr-sidebar-menu").style.top = `-480px`;
    document.getElementById("menuContainerrtl").style.top = `-480px`;
}
$(document).ajaxStop(function () {
    $("body").removeClass("loading");
});
var width = $(window).width();




$(document).ready(function () {
    var parameters = window.location.pathname.split('/');
    if (parameters === undefined) return;
    var culture = 'eng';
    if (parameters.find(x => x.toLowerCase() == 'account') != undefined)
        culture = parameters[3];
    else
        if (parameters.find(x => x.toLowerCase() == 'mybenefits') != undefined) {
            culture = parameters[2];
        }
        else {
            culture = (parameters.length > 3) ? parameters[1] : 'eng';
        }
        
    culture = culture.toLowerCase();
   var businessId = 0;
    var entityId = 0;

    if (parameters.length > 3) {
        businessId = (parameters.length >= 3) ? parameters[2] : null;
        entityId = (parameters.length >= 4) ? parameters[3] : null;
    } else {
        businessId = (parameters.length >= 2) ? parameters[1] : null;
        entityId = (parameters.length >= 3) ? parameters[2] : null;
    }
    if (businessId === null || entityId === null) {
        $("#error").show();
        return false;
    }

    if (culture === 'heb' || culture === 'he') {
        $("#rtlhamburger").css("display", "flex");
        if (width <= 992) {
            if ($("#menuContainer").hasClass("show"))
                $("#rtlNavbar").css("display", "flex");
            else
                $("#rtlNavbar").css("display", "block");
        }
        else
            $("#rtlNavbar").css("display", "flex");
        $("#ltrNavbar").css("display", "none");
        $("#ltrhamburger").css("display", "none");
    }
    else {
        $("#ltrhamburger").css("display", "flex");
        if (width <= 992) {
            if ($("#menuContainer").hasClass("show"))
                $("#ltrNavbar").css("display", "flex");
            else
                $("#ltrNavbar").css("display", "block");
        }
        else
            $("#ltrNavbar").css("display", "flex");
        $("#rtlNavbar").css("display", "none");
        $("#rtlhamburger").css("display", "none");
    }

    $("#btnHambugerltr").addClass("offset-8");

    $("#ltren").attr("href", "/eng/" + businessId + "/" + entityId);
    $("#ltrhe").attr("href", "/heb/" + businessId + "/" + entityId);
    $("#ltrru").attr("href", "/rus/" + businessId + "/" + entityId);
    $("#rtlen").attr("href", "/eng/" + businessId + "/" + entityId);
    $("#rtlhe").attr("href", "/heb/" + businessId + "/" + entityId);
    $("#rtlru").attr("href", "/rus/" + businessId + "/" + entityId);
    if (culture === 'eng') {
        $("#ltren").css("font-weight", "Bold");
        $("#rtlen").css("font-weight", "Bold");
    }
    if (culture === 'heb' ||  culture === 'he') {
        $("#ltrhe").css("font-weight", "Bold");
        $("#rtlhe").css("font-weight", "Bold");
    }
    if (culture === 'rus') {
        $("#rtlru").css("font-weight", "Bold");
        $("#ltrru").css("font-weight", "Bold");
    }
    $('#navlist').hScroll(100);
    //// Cache selectors
    var lastId,
        topMenu = $("#navlist"),
        topMenuHeight = topMenu.outerHeight() + 1,
        // All list items
        menuItems = topMenu.find("a"),
        // Anchors corresponding to menu items
        scrollItems = menuItems.map(function () {
            var item = $($(this).attr("href"));
            if (item.length) { return item; }
        });

    //Bind click handler to menu items
    //so we can get a fancy scroll animation
    menuItems.click(function (e) {
        window.removeEventListener('scroll', {});
        var href = $(this).attr("href"),
            offsetTop = href === "#" ? 0 : $(href).offset().top - topMenuHeight + 1;
        $('html, body').stop().animate({
            scrollTop: offsetTop
        }, 850);
        $('#navlist a').find('button').removeClass("active");        
        $(this).find('button').addClass('active');
       
        e.preventDefault();
    });
   

    var $navigationLinks = $('#navlist > a');
    // cache (in reversed order) the sections
    var $sections = $($(".allprod").get().reverse());

    // map each section id to their corresponding navigation link
    var sectionIdTonavigationLink = {};
    $sections.each(function () {
        var id = $(this).attr('id');
        sectionIdTonavigationLink[id] = $('#navlist > a[href=\\#' + id + ']');
    });

    // Bind to scroll
    $(window).scroll(function () {
        var navlist = document.getElementById("navlist");
        var sticky = navlist.offsetTop;
        var position = $(this).scrollTop();
        if ($(window).scrollTop() > sticky) {
            $('#navlist').addClass('sticky');
        } else {
            $('#navlist').removeClass('sticky');
        }
        highlightNavigation();
        if ($(window).scrollTop() + $(window).height() == $(document).height()) {
            var $navigationLink = $("#navlist").find('a').last();
            // remove .active class from all the links
            $navigationLinks.find('button').removeClass('active');
            // add .active class to the current link
            $navigationLink.find('button').addClass('active');
            $navigationLink[0].scrollIntoView();
        }
    });

   
    function highlightNavigation() {
       
        // get the current vertical position of the scroll bar
        var scrollPosition = $(window).scrollTop() + 80;

        // iterate the sections
        $sections.each(function () {
            var currentSection = $(this);
            // get the position of the section
            var sectionTop = currentSection.offset().top;
            var content = document.getElementById('navlist');
            // if the user has scrolled over the top of the section
            if (scrollPosition >= sectionTop) {
                // get the section id
                var id = currentSection.attr('id');
                // get the corresponding navigation link
                var $navigationLink = sectionIdTonavigationLink[id];
                // if the link is not active
                if (!$navigationLink.find('button').hasClass('active')) {
                    // remove .active class from all the links
                    $navigationLinks.find('button').removeClass('active');
                    // add .active class to the current link
                    $navigationLink.find('button').addClass('active');
                    $navigationLink[0].scrollIntoView();
                }
                // we have found our section, so we return false to exit the each loop
                return false;
            }
        });
    }

    return false;
});
$(document).on("keyup", "#SearchContent", function () {
    const value = this.value.toLowerCase().trim();
    setTimeout(function () {
        if (value == "" || value == '') {
            $('.prod').show();
            $('.catheading').show();
            $('#navlist').find('a').show();
            return false;
        }
        $('.catheading').hide();
        //$('#navlist').show();
        var catHeadtxt = [];
        $('.prod').each(function () {
            if ($(this).find('div').text().search(value) > -1) {
                $(this).show();
                $(this).prevAll('.catheading').first().show();
                catHeadtxt.push($(this).prevAll('.catheading').first().text());
            }
            else {
                $(this).hide();
            }
        });
        $('#navlist button').each(function () {
            if (catHeadtxt.indexOf($(this).text()) > -1) {
                $(this).parent().show();
            }
            else
                $(this).parent().hide();
        });        
    }, 300);
});
$.fn.hScroll = function (amount) {
    amount = amount || 120;
    $(this).bind("DOMMouseScroll mousewheel", function (event) {
        var oEvent = event.originalEvent,
            direction = oEvent.detail ? oEvent.detail * -amount : oEvent.wheelDelta,
            position = $(this).scrollLeft();
        position += direction > 0 ? -amount : amount;
        $(this).scrollLeft(position);
        event.preventDefault();
    })
};