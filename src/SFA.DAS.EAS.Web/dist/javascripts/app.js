var sfa = sfa || {};

sfa.homePage = {
    init: function () {
        this.startButton();
        this.toggleRadios();
    },
    startButton: function () {
        var that = this;
        $('#submit-button').on('click touchstart', function (e) {
            var isYesClicked = $('#have-everything').prop('checked'),
                errorShown = $('body').data('shownError') || false;
            if (!isYesClicked && !errorShown) {
                e.preventDefault();
                that.showError();
            }
        });
    }, 
    showError: function() {
        $('.error-message').removeClass("js-hidden").attr("aria-hidden");
        $('#what-you-need-form').addClass("error");
        $('body').data('shownError', true);
    },
    toggleRadios: function () {
        var radios = $('input[type=radio][name=everything-you-need]');
        radios.on('change', function () {

            radios.each(function () {
                if ($(this).prop('checked')) {
                    var target = $(this).parent().data("target");
                    $("#" + target).removeClass("js-hidden").attr("aria-hidden");
                } else {
                    var target = $(this).parent().data("target");
                    $("#" + target).addClass("js-hidden").attr("aria-hidden", "true");
                }
            });

        });
    }
}

sfa.navigation = {
    elems: {
        userNav: $('nav#user-nav > ul'),
        levyNav: $('ul#global-nav-links')
    },
    init: function () {
        this.setupMenus(this.elems.userNav);
        this.setupEvents(this.elems.userNav);
    },
    setupMenus: function (menu) {
       menu.find('ul').addClass("js-hidden").attr("aria-hidden", "true");
    },
    setupEvents: function (menu) {
        var that = this;
        menu.find('li.has-sub-menu > a').on('click', function (e) {
            var $that = $(this);
            that.toggleMenu($that, $that.next('ul'));
            e.stopPropagation();
            e.preventDefault();
        });
        // Focusout event on the links in the dropdown menu
        menu.find('li.has-sub-menu > ul > li > a').on('focusout', function (e) {
            // If its the last link in the drop down menu, then close
            var $that = $(this);
            if ($(this).parent().is(':last-child')) {
                that.toggleMenu($that, $that.next('ul'));
            }
        });

    },
    toggleMenu: function (link, subMenu) {
        var $li = link.parent();
        if ($li.hasClass("open")) {
            // Close menu
            $li.removeClass("open");
            subMenu.addClass("js-hidden").attr("aria-hidden", "true");
        } else {
            // Open menu
            this.closeAllOpenMenus();         
            $li.addClass("open");
            subMenu.removeClass("js-hidden").attr("aria-hidden", "false");
        }
    },
    closeAllOpenMenus: function () {
        this.elems.userNav.find('li.has-sub-menu.open').removeClass('open').find('ul').addClass("js-hidden").attr("aria-hidden", "true");
        this.elems.levyNav.find('li.open').removeClass('open').addClass("js-hidden").attr("aria-hidden", "true");
    },
    linkSettings: function () {
        var $settingsLink = $('a#link-settings'),
            that = this;
        this.toggleUserMenu();
        $settingsLink.attr("aria-hidden", "false");
        $settingsLink.on('click touchstart', function (e) {
            var target = $(this).attr('href');
            $(this).toggleClass('open');
            that.toggleUserMenu();
            e.preventDefault();
        });
    },
    toggleUserMenu: function () {
        var $userNavParent = this.elems.userNav.parent();
        if ($userNavParent.hasClass("close")) {
            //open it
            $userNavParent.removeClass("close").attr("aria-hidden", "false");
        } else {
            // close it 
            $userNavParent.addClass("close").attr("aria-hidden", "true");
        }
    }
}

sfa.forms = {
    init: function () {
        this.preventDoubleSubmit();
    },
    preventDoubleSubmit: function () {
        var forms = $('form').not('.has-client-side-validation');
        forms.on('submit', function (e) {
            var button = $(this).find('.button');
            button.attr('disabled', 'disabled');
            setTimeout(function () {
                button.removeAttr('disabled');
            }, 5000);
        });
    },
    removeDisabledAttr: function () {
        var btns = $('form').not('.has-client-side-validation').find('.button');
        btns.removeAttr('disabled');
    }
}

window.onunload = function () {
    sfa.forms.removeDisabledAttr();
};

sfa.forms.init();
sfa.navigation.init();
$('ul#global-nav-links').collapsableNav();

var selectionButtons = new GOVUK.SelectionButtons("label input[type='radio'], label input[type='checkbox'], section input[type='radio']");
var selectionButtonsOrgType = new GOVUK.SelectionButtons("section input[type='radio']", { parentElem: 'section' });


// cohorts bingo balls - clickable block
$(".clickable").on('click touchstart', (function () {
    window.location = $(this).find("a").attr("href");
    return false;
}));
