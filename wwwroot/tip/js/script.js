jQuery(document).ready(function(){

  jQuery('html,body').click(function(e){

    if ( jQuery('.login-bottom-wrapper').hasClass('is-show') ) {
      jQuery('.login-bottom-wrapper').removeClass('is-show');
    }

  });
  
  jQuery('.carousel-privilege').carousel({
    dist: -50,
  });

  jQuery('.carousel-prev').click(function(e){
    e.preventDefault();
    e.stopPropagation();
    jQuery('.carousel-privilege').carousel('prev');
  });

  jQuery('.carousel-next').click(function(e){
    e.preventDefault();
    e.stopPropagation();
    jQuery('.carousel-privilege').carousel('next');
  });

  jQuery('.slideshow-main').slick({
    arrows: false,
    dots: true,
     adaptiveHeight: true,
     autoplay: true,
     autoplaySpeed: 3000,
  });

  jQuery(".tooltipped").tooltip();

  jQuery('.mobile-menu-toggle').click(function(e){
    e.preventDefault();

    jQuery('body').addClass('popup-is-active');
    jQuery('.mobile-sidebar').addClass('is-show');
  });

  jQuery('.mobile-sidebar-close').click(function(e){
    e.preventDefault();

    jQuery('body').removeClass('popup-is-active');
    jQuery('.mobile-sidebar').removeClass('is-show');
  });

  jQuery('.stick-nav .stick-nav-more .stick-nav-link').click(function(e){
    e.preventDefault();

    jQuery('body').toggleClass('popup-is-active');
    jQuery('.privilege-popup-menu').toggleClass('is-show');
  });

  jQuery('.login-top .logged-username').click(function(e){
    e.preventDefault();

    jQuery('.login-bottom-wrapper').toggleClass('is-show');
  });

  jQuery('.login-top').click(function(e){
    e.stopPropagation();
  });

  jQuery('.input-block select').select2({
      minimumResultsForSearch: Infinity
    });
  jQuery('.input-block .disable-search').select2({
    minimumResultsForSearch: Infinity
  });

  $(window).on('resize', function() {
    jQuery('.input-block select').select2({
      minimumResultsForSearch: Infinity
    });
    jQuery('.input-block .disable-search').select2({
      minimumResultsForSearch: Infinity
    });    
  });



   jQuery('[data-toggle="datepicker"]').datepicker({
      format: 'dd/mm/yyyy',     
      language: 'th-th'
   });

  jQuery('.faq-main-menu-link').click(function(e){
    e.preventDefault();

    let targetElement = jQuery(this).attr('href');
    jQuery([document.documentElement, document.body]).animate({
      scrollTop: jQuery(targetElement).offset().top
    }, 2000);    
  });
});


