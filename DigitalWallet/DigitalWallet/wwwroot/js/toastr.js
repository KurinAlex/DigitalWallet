toastr.options = {
    "closeButton": true,
    "positionClass": 'toast-top-center'
};

$('.copy-content').each(function() {
    $(this).on('click',
        () => {
            const text = $(this).prop('innerText');
            navigator.clipboard.writeText(text).then(() => {
                console.log('Text copied to clipboard:', text);
                toastr.success('Text copied!');
            }).catch(err => {
                console.error('Failed to copy text: ', err);
                toastr.error('Failed to copy text');
            });
        }
    );
});
