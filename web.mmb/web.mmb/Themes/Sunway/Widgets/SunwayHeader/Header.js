var kenwoodHeaderJs = new function () {
    //=============  mobile menu

    var mobileMenuBtn = $('.mobile-menu-btn');
    var topNav = $('#topnav');
    var currentPosition;

    mobileMenuBtn.click(function () {
        var OpeningTimes = document.getElementById('opening-times-mobile');
        topNav.slideToggle(300);
        $('.phone-and-time').toggle();
        mobileMenuBtn.toggleClass('opened');

        if (mobileMenuBtn.hasClass('opened')) {
            currentPosition = $(window).scrollTop();
            $("html, body").animate({ scrollTop: 0 }, 0);
        } else {
            $("html, body").animate({ scrollTop: currentPosition }, 0);
        }

        if (!mobileMenuBtn.hasClass('opened')) {
            OpeningTimes.style.display = "none";
        }


    });

    /**
    *
    * @returns {boolean}
    */
    var menuIsCollapsed = function () {

        return mobileMenuBtn.is(':visible');
    };

    topNav.on('click',
        'a',
        function (ev) {

            if (menuIsCollapsed()) {
                var target = ev.target;
                if (target.nodeName.toLowerCase() === 'a') {
                    var parent = $(target.parentNode);
                    var submenu = parent.children('ul');
                    if (submenu.length) {
                        //if has only one child don't open drawer, open directly
                        if (parent.find('ul > li:not(".arrow_box")').length === 1) {
                            location.href = parent.find('ul a').attr('href');
                            return false
                        }
                        if (parent.hasClass('header')) {
                            parent.parent().find('ul').not(submenu).removeClass('opened');
                        } else {
                            topNav.find('.ddDestinations').not(submenu).removeClass('opened');
                            topNav.find('.ddDestinations ul').removeClass('opened');
                            topNav.find('.ddDestinations .header').removeClass('opened');
                        }
                        parent.siblings().removeClass('opened');
                        parent.toggleClass('opened');
                        submenu.toggleClass('opened');
                        return false;
                    }
                }
            }

            return true;
        });

    var isHandheld = window.innerWidth < 1024;
    if (isHandheld) {

        var OpeningTime = document.getElementById('CurrentOpeningTime');

        //opening times dropdown
        var $OpeningTimesDD = $('#opening-times-mobile');

        OpeningTime.onclick = function () {
            $OpeningTimesDD.slideToggle(100);
        };
    };

}