(function () {

    // https://select2.github.io/examples.html
    var init = function () {
        if ($("#TrainingId")) {
            $("#TrainingId").select2();
        }
    };
    init();

    // open dropdownon on focus
    $(document).on('focus', '.select2', function () {
        $(this).siblings('select').select2('open');
    });

        // add focus to span element for accessibility while using tabs 
       // $('span.select2').attr('tabindex', 0), 
       // keydown: function(e) { // spacebar press to behave like click
       //     var code = e.which;
       //     if ((code === 13) || (code === 32)) {
       //         $(this).click();
        //    }

}());