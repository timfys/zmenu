/* Set the width of the side navigation to 250px */
function openNav() {
  $(".panel-screen-overlay").css("visibility","visible");
  $("#mySidenav").removeClass("sideNavClose").addClass("sideNavOpen");
}
function closeNav() {
  $(".panel-screen-overlay").css("visibility","hidden");
  $("#mySidenav").removeClass("sideNavOpen").addClass("sideNavClose");
}

$(document).ready(function () {

  if($( window ).width() <= 570)
  {
    $("#mySidenav .sidenav-box").css("width",$( document  ).width());
  }
  $(".wpcf7-not-valid-tip").click(function(){
    $(this).fadeOut(500);
  });
  $(".panel-screen-overlay").click(function(){
    closeNav();
  });
  // setTimeout(function () {
  //   $("footer").removeClass("d-none").fadeIn(1000);
  // }, 1000);

  $("#carouselBanner .wrap-btn a").mouseover(function(){
    $(this).find("img").attr("src",$(this).find("img").attr("data-hover"));
  });
  $("#carouselBanner .wrap-btn a").mouseout(function(){
    $(this).find("img").attr("src",$(this).find("img").attr("data-src"));
  });

  // Scroll to top
  $(window).scroll(function () {
      if ($(this).scrollTop() > 500) {
      $("#toTopBtn").fadeIn();
    } else {
      $("#toTopBtn").fadeOut();
    }
  });

  $("#toTopBtn").click(function () {
    $("html, body").animate(
      {
        scrollTop: 0,
      },
      1000
    );
    return false;
  });

  // menu sub
  $("#navbarDropdownMenuSub").hover(function () {
    $("#navbarDropdownMenuLink").css("color", "#0b2341");
  });
  $("#navbarNavDropdown .nav-link").hover(function () {
    $("#navbarDropdownMenuLink").css("color", "");
  });

  // search bar
  $("#searchBar .search-close").click(function () {
    $("#searchBar").collapse("hide");
  });
  $(".performance .accordion .accordion__item__header").click(function () {
    var objFa = $(this).children("img");
    if ($(this).hasClass("active")) {
      objFa.attr("src","/assets/img/icons8-subtract-24.png");
      // objFa.removeClass("fa-plus").addClass("fa-minus");
    } else {
      objFa.attr("src","/assets/img/icons8-plus-math-24.png");
      //objFa.removeClass("fa-minus").addClass("fa-plus");
    }
  });

  // // lazy load
  // $(".lazy").Lazy({
  //   effect: "fadeIn",
  //   effectTime: 2000,
  // });

  //scroll to animate
  if($(".kd-btn").length > 0){
    $(".kd-btn").each(function () {
      $(this).addClass("kd-int");
      new Waypoint({
        element: $(this),
        handler: function () {
          this.element.addClass(this.element.attr("data-animate"));
          this.destroy();
        },
        offset: "80%",
      });
    });
  }
  $("h1, h2, h3 , h4, h5, h6, img, a, i, nav, section #carouselBanner, div.accordion__item").each(function () {
    if($(this).length > 0)
    {
      new Waypoint({
        element: $(this),
        handler: function () {
          this.element.addClass("kd-fadeIn");
          this.destroy();
        },
        offset: "80%",
      });
    }
  });
  if($("section.download-form").length > 0){
    $("section.download-form").each(function () {
      $(this).addClass("kd-int");
      new Waypoint({
        element: $(this),
        handler: function () {
          this.element.addClass("kd-fadeIn");
          this.destroy();
        },
        offset: "100%",
      });
    });
  }
  if($(".box-sol-item").length > 0){
    var boxSolItem = new Waypoint({
      element: $(".box-sol-item"),
      handler: function () {
        $(".solution .sol-item").each(function () {
          $(this).addClass($(this).attr("data-animate"));
        });
        this.destroy();
      },
      offset: "80%",
    });
  }
  if($(".testimonial3").length > 0){
    var boxOwlItem = new Waypoint({
      element: $(".testimonial3"),
      handler: function () {
        $(".testimonial3 .owl-carousel .item").each(function () {
          $(this).addClass($(this).attr("data-animate"));
        });
        this.destroy();
      },
      offset: "80%",
    });
  }
  if($(".performance .rbox-2  .rbox-progress").length > 0){
    new Waypoint({
      element: $(".performance .rbox-2  .rbox-progress"),
      handler: function () {
        $(".input-progress").each(function () {
          i = 0;
          makeProgress(
            $(this).find(".progress-percent"),
            $(this).find(".progress-bar"),
            $(this).attr("data-max"),
            0
          );
        });
      },
      offset: "80%",
    });
  }
  if($("section #carouselBanner").length > 0){
    var footer = new Waypoint({
      element: $("section #carouselBanner"),
      handler: function () {
        this.element.addClass("kd-fadeIn");
        this.destroy();
      },
      offset: "80%",
    });
  }

  // Progress bar
  function makeProgress(percent, bar, max, i) {
    if (i < max) {
      i = i + 1;
      $(bar).css("width", i + "%");
      $(percent).text(i + " %");
    }
    // Wait for sometime before running this script again
    setTimeout(function () {
      makeProgress(percent, bar, max, i);
    }, 25);
  }

  // Stories
  if($(".testi3")){
    $(".testi3").owlCarousel({
      loop: true,
      margin: 20,
      nav: true,
      navText: [
        "<div class='nav-btn prev-slide'></div>",
        "<div class='nav-btn next-slide'></div>",
      ],
      dots: false,
      autoplay: true,
      responsiveClass: true,
      responsive: {
        0: {
          items: 1,
          nav: false,
        },
        1024: {
          items: 1,
        },
        1400: {
          items: 2,
        },
        1900: {
          items: 3,
        },
      },
    });
  }

  // DOWNLOAD PAGE PAGE
  if($('#frmDownload').length > 0){
    $('#frmDownload').find('#txtName').focus();
    // submit form download
    $("#frmDownload").submit(function(e){
      
      var name = $("#txtName");
      var company = $("#txtCompany");
      var phone = $("#txtPhone");
      var email = $("#txtEmail");
      if(name.val().trim() == "" )
      {
        alert(name.attr('data-error'));
        name.focus();
        return false;
      }
      if(company.val().trim() == "" )
      {
        alert(company.attr('data-error'));
        company.focus();
        return false;
      }
      if(phone.val().trim() == "" )
      {
        alert(phone.attr('data-error'));
        phone.focus();
        return false;
      }
      if(email.val().trim() == "" )
      {
        alert(email.attr('data-error'));
        email.focus();
        return false;
      }
    
      return true;
    
    });
  }
  // END DOWNLOAD PAGE

  // THANK YOU PAGE
  $('a[data-auto-download]').each(function(){
    var link = $(this).attr("href");
    var $remainSecond = $(".remain-second");
    var sec = 5;
    // var intRemain = setInterval(function(){
    //   sec = sec - 1;
    //   console.log(sec);
    //   $remainSecond.text(sec);
      
    // }, 1000);
    setTimeout(function() {
      //clearInterval(intRemain);
      window.location = link;
    }, 500);
  });
  // END THANK YOU PAGE

  // CONTACT US PAGE
  $("#btnContactSubmit").click(function(e){
    
    var name = $("#txtContactName");
    var email = $("#txtContactEmail");
    var phone = $("#txtContactPhone");
    var message = $("#txtContactMessage");
    var isValid = true;
    if(name.val().trim() == "" )
    {
      $(".alert-name").css("display","block")
      name.focus();
      isValid = false;
    }
    if(email.val().trim() == "" )
    {
      $(".alert-email").css("display","block")
      email.focus();
      isValid = false;
    }
    if(phone.val().trim() == "" )
    {
      $(".alert-phone").css("display","block")
      phone.focus();
      isValid = false;
    }
    // ajax
    if(isValid){
      jQuery.ajax({
        type: "POST",
        url: 'YOU_URL_TO_WHICH_DATA_SEND',
        data:'YOUR_DATA_TO_SEND',
        beforeSend: function() {
          $(".ajax-loader").css("visibility","visible");
        },
        complete: function(data) {
          $(".ajax-loader").css("visibility","hidden");;
        }
      });
    }
  });

  

  $("input[type=number]").keypress(function (e) {
    var maxlengthNumber = parseInt($(this).attr('maxlength'));
    var inputValueLength = $(this).val().length + 1;
    if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) {
      return false;
    }
    if(maxlengthNumber < inputValueLength) {
      return false;
    }
  });
  // END CONTACT US 
});

                        