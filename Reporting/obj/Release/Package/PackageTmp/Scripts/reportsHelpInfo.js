$(document).ready(function () {
    $(".helpInfoShow").click(function () {
        $(this).nextAll(".helpInfo").slideToggle();
    });
});