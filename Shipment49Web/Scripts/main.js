(function () {
    $('.FlowupLabels').FlowupLabels({
        /*
		 * These are all the default values
		 * You may exclude any/all of these options
		 * if you won't be changing them
		 */

        // Handles the possibility of having input boxes prefilled on page load
        feature_onInitLoad: true,

        // Class when focusing an input
        class_focused: 'focused',
        // Class when an input has text entered
        class_populated: 'populated'
    });
})();


; (function ($) {
    $(document).ready(function () {
        var $dropDown = $('.dropdown_menu'),
           $subMenuTrigger = $('.dropdown'),
           $subArrow = $('.sub-indicator');

        $subMenuTrigger.on('click', function () {
            // drop down element
            var $dropDownEl = $(this).find($dropDown);

            $dropDown.slideUp();
            $subArrow.removeClass('sub-arrow-open');
            // slidetoggle
            $dropDownEl.stop().slideToggle('slow', function () {
                $(this).next('.sub-indicator').toggleClass('sub-arrow-open', $dropDownEl.is(':visible'));
            });
        });

        jQuery('.menu-overlay').click(function () {
            jQuery('.slider_sidebar').removeClass('opened');
            jQuery('html,body').removeClass('slider-bar-opened');
        });
        jQuery(document).keyup(function (e) {
            if (e.keyCode === 27) {
                jQuery('.slider_sidebar').removeClass('opened');
                jQuery('html,body').removeClass('slider-bar-opened');
            }
        });
        /********************************
         Menu Sidebar
         ********************************/
        jQuery('.navbar-toggle').click(function (e) {
            e.stopPropagation();
            jQuery('.slider_sidebar').toggleClass('opened');
            jQuery('html,body').toggleClass('slider-bar-opened');
        });
        jQuery('.rightside_partial').click(function (e) {
            e.stopPropagation();
            jQuery('.mobile_loginPartial').toggleClass('opened');
        });
    });
})(jQuery);