//  Register
document.addEventListener('DOMContentLoaded', function () {
    const themeToggle = document.getElementById('themeToggle');
    const icon = themeToggle.querySelector('i');

    // Check for saved theme preference or use system preference
    const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
    const savedTheme = localStorage.getItem('theme');
    let currentTheme = savedTheme || (prefersDark ? 'dark' : 'light');

    // Apply the initial theme
    applyTheme(currentTheme);

    // Toggle theme on button click
    themeToggle.addEventListener('click', function () {
        currentTheme = currentTheme === 'dark' ? 'light' : 'dark';
        applyTheme(currentTheme);
        localStorage.setItem('theme', currentTheme);
    });

    function applyTheme(theme) {
        document.documentElement.setAttribute('data-bs-theme', theme);

        // Update icon
        if (theme === 'dark') {
            icon.classList.remove('fa-sun');
            icon.classList.add('fa-moon');

            // Update background gradient for dark theme
            document.body.style.background = 'linear-gradient(0deg, #00013d, #001631)';
        } else {
            icon.classList.remove('fa-moon');
            icon.classList.add('fa-sun');

            // Update background gradient for light theme
            document.body.style.background = 'linear-gradient(0deg, #e6f0ff, #cce0ff)';
        }
    }
});



////Login Page
function togglePassword() {
    const passwordInput = document.getElementById('password');
    const toggleIcon = document.querySelector('.password-toggle i');

    if (passwordInput.type === 'password') {
        passwordInput.type = 'text';
        toggleIcon.classList.remove('fa-eye');
        toggleIcon.classList.add('fa-eye-slash');
    } else {
        passwordInput.type = 'password';
        toggleIcon.classList.remove('fa-eye-slash');
        toggleIcon.classList.add('fa-eye');
    }
}

// Add animation to form elements
document.addEventListener('DOMContentLoaded', function () {
    const formElements = document.querySelectorAll('.sso-login-form .form-floating, .sso-login-form button');
    formElements.forEach((el, index) => {
        el.style.opacity = '0';
        el.style.transform = 'translateY(20px)';
        el.style.transition = 'opacity 0.3s ease, transform 0.3s ease';
        setTimeout(() => {
            el.style.opacity = '1';
            el.style.transform = 'translateY(0)';
        }, 100 * index);
    });
});
