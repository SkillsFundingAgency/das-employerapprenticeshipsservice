
    var sfa = sfa || {};

    var ShowHideContent = function () {
        this.headerSelector = '.accordion-tab';
        this.headers = $(this.headerSelector);
        this.panelSelector = '.accordion-panel';
        this.panels = $(this.panelSelector);
    };

    ShowHideContent.prototype.init = function () {
        this.setUpEvents();
        this.addAttributes();
    };

    ShowHideContent.prototype.setUpEvents = function () {

        var that = this;
        this.headers.attr('tabindex', 0);
        this.headers.on('click', function () {
            var target = $(this).attr('aria-controls');
            that.togglePanel(target, $(this));
        });

        this.headers.on('keydown', function (e) {
            if (e.which === 13 || e.which === 32) {
                var target = $(this).attr('aria-controls');
                that.togglePanel(target, $(this));
            }
        });
    };

    ShowHideContent.prototype.addAttributes = function () {
        this.headers.each(function () {

            var $this = $(this),
                 target = $this.attr('aria-controls'),
                 headerId = $this.attr('id'),
                 panel = $('#' + target);

            $this.attr({'aria-expanded': false});
            panel.attr({'aria-labelledby': headerId, 'aria-hidden': true}).hide();

        });
      
    };

    ShowHideContent.prototype.togglePanel = function (target, header) {
        var panel = $('#' + target),
            panelVisible = panel.attr('aria-hidden');

        if (panelVisible === 'true') {
            header.addClass('open').attr({ 'aria-expanded': true });
            panel.attr({'aria-hidden': false }).show();
        } else {
            header.removeClass('open').attr({ 'aria-expanded': false });
            panel.attr({'aria-hidden': true }).hide();
        }
    };

    sfa.ShowHideContent = ShowHideContent;

    var showHide = new sfa.ShowHideContent;
    showHide.init();