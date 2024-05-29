toastr.options = {
    "closeButton": true,
    "positionClass": "toast-top-center"
};

walletId = document.getElementById("walletId");

walletId.addEventListener("click",
    () => {
        if (window.isSecureContext) {
            var text = walletId.textContent;
            navigator.clipboard.writeText(text).then(() => {
                console.log("Text copied to clipboard:", text);
                toastr.success("Wallet ID copied!");
            }).catch(err => {
                console.error("Failed to copy text: ", err);
                toastr.error("Failed to copy text");
            });
        }
    });