/*********************************************************************************

	Template Name: Binke - Health & Medical Bootstrap4 Template
	Description: A perfect template to build beautiful and unique medical websites. It comes with nice and clean design.
	Version: 1.0

    Note: This is scripts js. All custom scripts here.

**********************************************************************************/

/*===============================================================================
			[ INDEX ]
=================================================================================

    Header Style 1 Sticky Header
    Header Style 2 Sticky Header
    Progress Bar Effect
    Breadcrumb Padding
    Portfolio Load More

=================================================================================
			[ END INDEX ]
================================================================================*/

(function ($) {
    'use strict';

    /* Header Style 1 Sticky Header */
    if($('.sticky-header').length){
        
        $(window).on('scroll', function () {

            var scrollPos = $(this).scrollTop();
              
            if (scrollPos > 300) {
                $('.sticky-header').addClass('is-sticky');
            } else {
                $('.sticky-header').removeClass('is-sticky');
            }

        });

    }

    
    /* Service Effect */
    $('.bk-service').each(function(){

        $('.bk-service').on('mouseenter', function(){
            $(this).find('p').slideDown(300);
            $(this).find('span').slideUp(300);
            $(this).addClass('is-hover');
        });
        $('.bk-service').on('mouseleave', function(){
            $(this).find('span').slideDown(300);
            $(this).find('p').slideUp(300);
            $(this).removeClass('is-hover');
        });

    });



    /* Progress Bar Effect */
    $(window).on('scroll', function () {

        function winScrollPosition() {
            var scrollPos = $(window).scrollTop(),
                winHeight = $(window).height();
            var scrollPosition = Math.round(scrollPos + (winHeight / 1.2));
            return scrollPosition;
        }

        var trigger = $('.progress-bar');
        if (trigger.length) {
            var triggerPos = Math.round(trigger.offset().top);
            if (triggerPos < winScrollPosition()) {
                trigger.each(function () {
                    $(this).addClass('fill');
                });
            }
        }

    });



    $('.bk-navigation ul li').on('mouseenter mouseleave', function (e) {
        if ($('ul', this).length) {
            var elm = $('ul:first', this);
            var off = elm.offset();
            var l = off.left - $('body').offset().left;
            var w = elm.width();

            if( elm.find('ul').length ){
                w = elm.width() * 2;
            }

            var docW = $('body').width();
            var isEntirelyVisible = (l + w <= docW);

            if (!isEntirelyVisible) {
                $(this).addClass('overflow-element');
            } else {
                $(this).removeClass('overflow-element');
            }
        }
    });


})(jQuery);