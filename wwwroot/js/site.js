$('#rememberMe, #btnLogin').click(function () {
    CreateCookie();
});
function CreateCookie() {
    let email = $('#email').val();
    let password = $('#password').val();
    let rememberMe = $('#rememberMe').is(':checked');
    let date = new Date();
    let checkTillDate = new Date(date.setDate(date.getDate() + 30)).toGMTString();
    let checkTillExpire = new Date(date.setDate(date.getDate() + -1)).toGMTString();
    if (rememberMe) {
        document.cookie = `username=${btoa(email)}; expires=${checkTillDate}`;
        document.cookie = `password=${btoa(password)}; expires=${checkTillDate}`;
    } else {
        document.cookie = `username=; expires=`;
        document.cookie = `password=; expires=}`;
    }
}
function GetCookie(name) {
    let cookie = document.cookie.split(';');
    for (let i = 0; i < cookie.length; i++) {
        let c = cookie[i].trim();
        if (c.startsWith(name + '=')) {
            return c.substring(name.length + 1);
        }
    }
    return null;
}
window.onload = function () {
    let email = GetCookie('username');
    let password = GetCookie('password');
    if (email && password) {
        $('#email').val(atob(email));
        $('#password').val(atob(password));
        $('#rememberMe').prop('checked', true);
    } else {
        $('#rememberMe').prop('checked', false);
    }
}
