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

    // retain tabbed order after selection
    $('#TrainingId').on('select2:select', function () {
        $("#StartMonth").focus();
    });

    // retain tabbed order on close without selection
    $('#TrainingId').on('select2:close', function () {
        $("#StartMonth").focus();
    });

}());