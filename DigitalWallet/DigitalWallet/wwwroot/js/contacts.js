$(document).ready(function () {
    showSelected();
});

$('#contacts').on('change',
    function () {
        $('.content').each(function () {
            $(this).hide();
        });

        showSelected();
    });

function showSelected() {
    const value = $('#contacts').find(":selected").val();
    $(`#content-${value}`).show();
}