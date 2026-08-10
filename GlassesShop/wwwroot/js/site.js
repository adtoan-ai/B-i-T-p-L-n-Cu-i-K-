
document.addEventListener('click', function (e) {
    var btn = e.target.closest('.password-toggle-btn');
    if (!btn) return;

    var wrapper = btn.closest('.password-wrapper');
    if (!wrapper) return;

    var input = wrapper.querySelector('input');
    var icon = btn.querySelector('i');
    if (!input) return;

    if (input.type === 'password') {
        input.type = 'text';
        icon.classList.remove('bi-eye');
        icon.classList.add('bi-eye-slash');
        btn.setAttribute('aria-label', 'Ẩn mật khẩu');
    } else {
        input.type = 'password';
        icon.classList.remove('bi-eye-slash');
        icon.classList.add('bi-eye');
        btn.setAttribute('aria-label', 'Hiện mật khẩu');
    }
});