var sfa = sfa || {};

    var ShowHideContent = function () {
        var selectors = {
            radio: '.block-label[data-target] input[type=radio]'
        }
        this.populateEvents = function () {
            var getElem = this.getElements(selectors.radio);
            if (getElem.length > 0) {

                var radios = getElem.closest('form').find('input[type=radio][name=' + getElem.attr('name') + ']');   
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
        this.getElements = function (selector) {
            var findElems = $(selector);
            return findElems;
        }
    }

    ShowHideContent.prototype.init = function () {
        this.populateEvents();
    }

    var shc = new ShowHideContent();
    shc.init();

