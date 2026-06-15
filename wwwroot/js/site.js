// site.js - Donora UI Interaction Script

document.addEventListener("DOMContentLoaded", () => {
    // 1. Password Visibility Toggles
    const setupPasswordToggle = (btnId, inputId, iconId) => {
        const toggleBtn = document.getElementById(btnId);
        const passwordInput = document.getElementById(inputId);
        const eyeIcon = document.getElementById(iconId);

        if (toggleBtn && passwordInput && eyeIcon) {
            toggleBtn.addEventListener("click", () => {
                const type = passwordInput.getAttribute("type") === "password" ? "text" : "password";
                passwordInput.setAttribute("type", type);
                
                // Toggle classes
                if (type === "text") {
                    eyeIcon.classList.remove("bi-eye-slash");
                    eyeIcon.classList.add("bi-eye");
                } else {
                    eyeIcon.classList.remove("bi-eye");
                    eyeIcon.classList.add("bi-eye-slash");
                }
            });
        }
    };

    setupPasswordToggle("btnTogglePassword", "txtPassword", "eyeIcon");
    setupPasswordToggle("btnToggleConfirmPassword", "txtConfirmPassword", "confirmEyeIcon");

    // 2. Click Ripple Effect on all Buttons
    const buttons = document.querySelectorAll(".btn");
    buttons.forEach(button => {
        button.addEventListener("click", function(e) {
            // Get click coordinates relative to button
            const rect = this.getBoundingClientRect();
            const x = e.clientX - rect.left;
            const y = e.clientY - rect.top;

            // Create ripple span element
            const rippleSpan = document.createElement("span");
            rippleSpan.classList.add("ripple");
            rippleSpan.style.left = `${x}px`;
            rippleSpan.style.top = `${y}px`;

            // Append to button
            this.appendChild(rippleSpan);

            // Remove ripple span after animation runs
            setTimeout(() => {
                rippleSpan.remove();
            }, 600);
        });
    });
});
