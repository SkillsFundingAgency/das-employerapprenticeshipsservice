(function () {

    // https://select2.github.io/examples.html
    var init = function () {
        if ($("#TrainingId")) {
            $("#TrainingId").select2();
        }
    };
    init();
    // add focus to span element for accessibility while using tabs 
    $('span.select2').attr('tabindex', 0);
}());