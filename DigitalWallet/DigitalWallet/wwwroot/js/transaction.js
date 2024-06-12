$('#submit').on('submit', function () {
    if ($('#submit').valid()) {
        $('#startTransaction').prop('disabled', true);
        $('#startTransaction').html('<div class="spinner-border text-secondary" role="status"></div>');
    }
});

$(document).ready(function () {
    $('#startTransaction').prop('disabled', false);
})