(function(){
    jQuery('#theCarousel').carousel();

      }());

      (function(){
        jQuery('.carousel-showmanymoveone .item').each(function(){
          var itemToClone = jQuery(this);

          for (var i=1;i<3;i++) {
            itemToClone = itemToClone.next();

            // wrap around if at end of item collection
            if (!itemToClone.length) {
              itemToClone = jQuery(this).siblings(':first');
            }

            // grab item, clone, add marker class, add to collection
            itemToClone.children(':first-child').clone()
              .addClass("cloneditem-"+(i))
              .appendTo(jQuery(this));
          }
        });
  }());
  
(function() {
jQuery('.carousel-showmanymoveone').carousel({
  interval: false
}); 
}());