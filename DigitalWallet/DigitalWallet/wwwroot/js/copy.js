toastr.options = {
    "closeButton": true,
    "positionClass": "toast-top-center"
};

copiableElements = document.getElementsByClassName("copy-content");
Array.from(copiableElements).forEach(e =>
    e.addEventListener("click",
        () => {
            const text = e.innerText;
            navigator.clipboard.writeText(text).then(() => {
                console.log("Text copied to clipboard:", text);
                toastr.success("Text copied!");
            }).catch(err => {
                console.error("Failed to copy text: ", err);
                toastr.error("Failed to copy text");
            });
        }));

