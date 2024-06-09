$('#submit').on('submit', function () {
    $('#startTransaction').prop('disabled', true);
    $('#startTransaction').html('<div class="spinner-border text-secondary" role="status"></div>');
});