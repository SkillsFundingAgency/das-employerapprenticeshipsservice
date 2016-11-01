// custom jquery scripts 
$(document).ready(function () {


    // Radio with outline on click
    $('input:radio').click(function () {
        $('input:radio').parent().removeClass('selected');
        $(this).parent(this).addClass('selected');
    });

    // checkbox with outline on click
    $('input:checkbox').click(function () {
        $('input:checkbox').parent().removeClass('selected');
        $(this).parent(this).addClass('selected');
    });


































});