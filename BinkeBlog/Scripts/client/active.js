/*********************************************************************************

	Template Name: Binke - Health & Medical Bootstrap4 Template
	Description: A perfect template to build beautiful and unique medical websites. It comes with nice and clean design.
	Version: 1.0

	Note: This is active js. Plugins activation code here.

**********************************************************************************/


/*===============================================================================
			[ INDEX ]
=================================================================================

	Scroll Up Activation
	Fake Loader
	Banner Slider Active
	Events Slider Active
	Testimonial Slider
	Brand Logo Slider
	Popular Post SLider Active
	Countdown Activation
	Portfolio Popup Gallery
	Mobilemenu Activation
	Odometer Activation
	Instafeed Activation
	Nice Select Activation
	Twitterfeed Activation
	Sticky Sidebar Activation
	Parallax Activation

=================================================================================
			[ END INDEX ]
================================================================================*/

(function ($) {
    'use strict';


    /* Scroll Up Activation */
    $.scrollUp({
        scrollText: '<i class="fa fa-angle-up"></i>',
        easingType: 'linear',
        scrollSpeed: 900
    });

    /* Fake Loader */
    $('.fakeloader').fakeLoader({
        timeToHide: 1200,
        bgColor: '#de2532',
        spinner: 'spinner4'
    });

    /* Banner Slider Active */
    $('.banner-slider-active').slick({
        autoplay: false,
        arrows: false,
        adaptiveHeight: true,
        dots: false,
        dotsClass: 'cr-slider-dots'
    });

    $('button.banner-navigation-prev').click(function () {
        $('.banner-slider-active').slick('slickPrev');
    });
    $('button.banner-navigation-next').click(function () {
        $('.banner-slider-active').slick('slickNext');
    });


    /* Team Member Slider */
    $('.team-member-silder-active').slick({
        slidesToShow: 3,
        autoplay: false,
        adaptiveHeight: true,
        dots: false,
        dotsClass: 'cr-slider-dots',
        arrows: true,
        centerMode: true,
        prevArrow: '<span class="slider-navigation slider-navigation-prev"><i class="fa fa-angle-left"></i></span>',
        nextArrow: '<span class="slider-navigation slider-navigation-next"><i class="fa fa-angle-right"></i></span>',
        responsive: [{
            breakpoint: 992,
            settings: {
                slidesToShow: 2,
                slidesToScroll: 1,
                centerMode: false,
            }
        },
        {
            breakpoint: 768,
            settings: {
                slidesToShow: 1,
                slidesToScroll: 1
            }
        }]
    });


    /* Blog Slider Active */
    $('.blog-slider-active').slick({
        slidesToShow: 3,
        autoplay: true,
        adaptiveHeight: true,
        dots: false,
        arrows: true,
        prevArrow: '<span class="slider-navigation slider-navigation-prev"><i class="fa fa-angle-left"></i></span>',
        nextArrow: '<span class="slider-navigation slider-navigation-next"><i class="fa fa-angle-right"></i></span>',
        responsive: [{
            breakpoint: 992,
            settings: {
                slidesToShow: 2,
                slidesToScroll: 1
            }
        },
        {
            breakpoint: 768,
            settings: {
                slidesToShow: 1,
                slidesToScroll: 1
            }
        }]
    });


    /* Testimonial Slider */
    $('.testimonial2-wrapper:not(.testimonial-2-items)').slick({
        autoplay: true,
        autoplaySpeed: 5000,
        arrows: false,
        adaptiveHeight: true,
        dots: true,
        dotsClass: 'cr-slider-dots',
        fade: false,
    });


    /* Testimonial Slider */
    $('.testimonial2-wrapper.testimonial-2-items').slick({
        autoplay: true,
        autoplaySpeed: 5000,
        arrows: false,
        adaptiveHeight: true,
        dots: true,
        dotsClass: 'cr-slider-dots',
        fade: false,
        slidesToShow: 2,
        responsive: [
            {
                breakpoint: 768,
                settings: {
                    slidesToShow: 1,
                    slidesToScroll: 1
                }
            }]
    });


    /* Testimonial Slider */
    $('.testimonial-wrap').slick({
        autoplay: true,
        autoplaySpeed: 5000,
        arrows: false,
        prevArrow: '<span class="cr-navigation cr-navigation-prev"><i class="fa fa-angle-left"></i></span>',
        nextArrow: '<span class="cr-navigation cr-navigation-next"><i class="fa fa-angle-right"></i></span>',
        adaptiveHeight: true,
        dots: true,
        dotsClass: 'cr-slider-dots',
        fade: false,
    });

    /* Brand Logo Slider */
    $('.brand-logos-slider').slick({
        slidesToShow: 6,
        infinite: true,
        autoplay: true,
        autoplaySpeed: 5000,
        arrows: true,
        prevArrow: '<span class="cr-navigation cr-navigation-prev"><i class="fa fa-angle-left"></i></span>',
        nextArrow: '<span class="cr-navigation cr-navigation-next"><i class="fa fa-angle-right"></i></span>',
        dots: false,
        responsive: [{
            breakpoint: 1199,
            settings: {
                slidesToShow: 5,
                slidesToScroll: 1
            }
        },
        {
            breakpoint: 992,
            settings: {
                slidesToShow: 4,
                slidesToScroll: 1
            }
        },
        {
            breakpoint: 768,
            settings: {
                slidesToShow: 2,
                slidesToScroll: 1
            }
        },
        {
            breakpoint: 576,
            settings: {
                slidesToShow: 1,
                slidesToScroll: 1
            }
        }
        ]
    });


    /* Popular Post Slider */
    $('.polular-post-slider-active').slick({
        slidesToShow: 4,
        infinite: true,
        autoplay: true,
        autoplaySpeed: 5000,
        arrows: true,
        prevArrow: '<span class="cr-navigation cr-navigation-prev"><i class="fa fa-angle-left"></i></span>',
        nextArrow: '<span class="cr-navigation cr-navigation-next"><i class="fa fa-angle-right"></i></span>',
        dots: false,
        responsive: [{
            breakpoint: 1199,
            settings: {
                slidesToShow: 3,
                slidesToScroll: 1
            }
        },
        {
            breakpoint: 992,
            settings: {
                slidesToShow: 2,
                slidesToScroll: 1
            }
        },
        {
            breakpoint: 768,
            settings: {
                slidesToShow: 1,
                slidesToScroll: 1
            }
        }
        ]
    });


    /* Popular Post Slider for Sidebar */
    $('.polular-post-slider-active-3-items').slick({
        slidesToShow: 3,
        infinite: true,
        autoplay: true,
        autoplaySpeed: 5000,
        arrows: true,
        prevArrow: '<span class="cr-navigation cr-navigation-prev"><i class="fa fa-angle-left"></i></span>',
        nextArrow: '<span class="cr-navigation cr-navigation-next"><i class="fa fa-angle-right"></i></span>',
        dots: false,
        responsive: [{
            breakpoint: 1199,
            settings: {
                slidesToShow: 3,
                slidesToScroll: 1
            }
        },
        {
            breakpoint: 992,
            settings: {
                slidesToShow: 2,
                slidesToScroll: 1
            }
        },
        {
            breakpoint: 768,
            settings: {
                slidesToShow: 1,
                slidesToScroll: 1
            }
        }
        ]
    });

    /* Mobilemenu Activation */
    $('nav.bk-navigation').meanmenu({
        meanMenuClose: '<img src="images/icons/icon-close.png" alt="close icon">',
        meanMenuCloseSize: '18px',
        meanScreenWidth: '991',
        meanExpandableChildren: true,
        meanMenuContainer: '.mobile-menu',
        onePage: true
    });

    /* Odometer Activation */
    if ($('.odometer').length) {

        $(window).on('scroll', function () {

            function winScrollPosition() {
                var scrollPos = $(window).scrollTop(),
                    winHeight = $(window).height();
                var scrollPosition = Math.round(scrollPos + (winHeight / 1.2));
                return scrollPosition;
            }

            var scrollPos = $(this).scrollTop();
            var elemOffset = $('.odometer').offset().top;
            var winHeight = $(window).height();

            if (elemOffset < winScrollPosition()) {

                $('.odometer').each(function () {
                    $(this).html($(this).data('count-to'));
                });

            }

        });

    }

    /* Instafeed Activation */
    if ($('#sidebar-instagram-feed').length) {

        var userFeed = new Instafeed({
            get: 'user',
            userId: 6665768655,
            accessToken: '6665768655.1677ed0.313e6c96807c45d8900b4f680650dee5',
            target: 'sidebar-instagram-feed',
            resolution: 'standard_resolution',
            limit: 4,
            template: '<div class="sidebar-instafeed-single"><a href="{{link}}" target="_new"><img src="{{image}}" /><span>{{caption}}</span></a></div>',

            after: function () {

                $('#sidebar-instagram-feed').slick({
                    slidesToShow: 1,
                    infinite: true,
                    autoplay: true,
                    autoplaySpeed: 5000,
                    arrows: false,
                    dots: true,
                    adaptiveHeight: true,
                });

            },
        });
        userFeed.run();

    }

    /* Nice Select Activation */
    $('select').niceSelect();


    /* Twitterfeed Activation */
    if ($('#sidebar-twitter-feed').length) {

        var configProfile = {
            "profile": { "screenName": 'devitemsllc' },
            "domId": 'sidebar-twitter-feed',
            "maxTweets": 2,
            "enableLinks": true,
            "showUser": false,
            "showTime": true,
            "showImages": true,
            "lang": 'en'
        };
        twitterFetcher.fetch(configProfile);

    }

    /* Sticky Sidebar Activation */
    $('.sticky-sidebar-active').theiaStickySidebar({
        additionalMarginTop: 30
    });

    /* Parallax Activation */
    $('.bk-parallax').jarallax();

    /* Match Height */
    $('.timetable-block').matchHeight();
    $('.bk-service-2').matchHeight();





    /* Facebook Feed */
    //(function (d, s, id) {
    //    var js, fjs = d.getElementsByTagName(s)[0];
    //    if (d.getElementById(id)) return;
    //    js = d.createElement(s); js.id = id;
    //    js.src = 'https://connect.facebook.net/en_US/sdk.js#xfbml=1&version=v3.0';
    //    fjs.parentNode.insertBefore(js, fjs);
    //}(document, 'script', 'facebook-jssdk'));




})(jQuery);
