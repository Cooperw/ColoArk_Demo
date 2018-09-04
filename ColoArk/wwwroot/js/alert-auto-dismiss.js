$(document).ready(function () {

    autoRemoveAlert($('.alert-auto-remove'));

    function autoRemoveAlert(alertElement) {
        alertElement.fadeTo(2000, 800).slideUp(800, function () {
            alertElement.slideUp(800);
        });
    }
});