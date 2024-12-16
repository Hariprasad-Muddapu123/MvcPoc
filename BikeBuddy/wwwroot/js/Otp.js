
document.addEventListener("DOMContentLoaded", function () {
    const expirationCountDown = document.getElementById("expirationCountDown");
    let timeRemaining = parseInt(expirationCountDown.dataset.otpCount, 10);// 10 minutes in seconds
    const timerElement = document.getElementById("otp-timer");
    const resendButton = document.getElementById("resendOtpButton");
    const resendMessage = document.getElementById("resendMessage");
    const otpInput = document.getElementById("otp");
    const verifyButton = document.getElementById("verifyButton");

    // Timer function to show countdown
    function updateTimer() {
                const minutes = Math.floor(timeRemaining / 60);
    const seconds = timeRemaining % 60;

    timerElement.textContent = `OTP expires in: ${minutes}m ${seconds}s`;

                if (timeRemaining > 0) {
                        timeRemaining--;
                    setTimeout(updateTimer, 1000);
                }
                else {
                    timerElement.textContent = "OTP has expired. Please request a new one.";
                    otpInput.disabled = true;
                    verifyButton.disabled = true;
                    verifyButton.classList.add("button-disabled");
                    resendButton.disabled = false; // Allow resend
                }
            }

    // Initialize timer
    updateTimer();

    // Resend OTP functionality
        resendButton.addEventListener("click", function () {
            clearTimeout(timerId);
            const email = document.getElementById("emailInput").value;

    if (!email) {
        resendMessage.textContent = "Email is required to resend OTP.";
    resendMessage.classList.remove("text-green-500");
    resendMessage.classList.add("text-danger");
    return;
                }

    resendButton.disabled = true; // Temporarily disable the button
    resendMessage.textContent = "Resending OTP...";
    resendMessage.classList.remove("text-danger", "text-green-500");
    resendMessage.classList.add("text-gray-600");

    // Send the email as part of the request
    fetch('/Account/ResendOtp', {
        method: 'POST',
    headers: {
        'Content-Type': 'application/x-www-form-urlencoded'
                    },
    body: `email=${encodeURIComponent(email)}`
                })
                    .then(response => response.json())
                    .then(data => {
                        if (data.success) {
        resendMessage.textContent = "OTP resent successfully.";
    resendMessage.classList.remove("text-danger");
    resendMessage.classList.add("text-green-500");

    // Reset timer and enable input fields
    timeRemaining = 600; // Reset to 10 minutes
    otpInput.disabled = false;
    verifyButton.disabled = false;
    verifyButton.classList.remove("button-disabled");
    updateTimer();
                        } else {
        resendMessage.textContent = data.message || "Error resending OTP.";
    resendMessage.classList.add("text-danger");
                        }
                    })
                    .catch(error => {
        console.error("Error:", error);
    resendMessage.textContent = "An error occurred while resending OTP.";
    resendMessage.classList.add("text-danger");
                    })
                    .finally(() => {
        resendButton.disabled = false; // Re-enable the button
                    });
            });
        });
