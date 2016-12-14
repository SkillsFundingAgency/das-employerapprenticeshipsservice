(function () {

    var sfa = sfa || {};

    var ShowHideContent = function () {
        this.selectedClass = 'selected';
    };

    ShowHideContent.prototype.init = function () {
        console.log('hello');
    };

    sfa.ShowHideContent = ShowHideContent();

})(window);
