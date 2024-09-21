const emailInput = document.getElementById("Email");
const passwordInput = document.getElementById("Password");
const btn = document.getElementById("btnIniciarSesion");

btn.addEventListener("click", (event) => {
    event.preventDefault();

    removeWarningClasses(emailInput);
    removeWarningClasses(passwordInput);
    cleanAlerts();

    let password = passwordInput.value;
    let email = emailInput.value;

    logIn(email, password);
});

async function logIn(email, password) {
    const url = `https://localhost:7269/api/usersession`;
    const myHeaders = new Headers();
    myHeaders.append("Content-Type", "application/json");

    try {
        const response = await fetch(url, {
            method: "POST",
            body: JSON.stringify({ password: password, email: email }),
            headers: myHeaders
        });

        if (!response.ok) {
            let obj = await response.json();
            if (obj.error == "Invalid Input") {
                if (obj.type == "Email") {
                    setWarningClasses(emailInput);
                    addAlert(obj.message);
                }
                if (obj.type == "Password") {
                    setWarningClasses(passwordInput);
                    addAlert(obj.message);
                }
            }
            if (obj.error == "Not Auth") {
                addAlert(obj.message);
            }
            if (obj.error == "Not Found") {
                addAlert(obj.message);
            }
        } else {
            let sessionData = await response.json();
            sessionStorage.getItem("_User", sessionData.UserId);
            sessionStorage.getItem("_UserName", sessionData.Name);
            window.location.pathname = "/Homepage"; // creo que es homepage?
        }
    } catch (error) {
        console.error(error.message);
        addAlert("Ocurrió un error inesperado.");
    }
}

function setWarningClasses(element) {
    element.classList.add("border-danger");
    element.classList.add("border-2");
}

function removeWarningClasses(element) {
    element.classList.remove("border-danger");
    element.classList.remove("border-2");
}

function addAlert(message) {
    text = `Advertencia: ${message}`;
    alertDiv.innerHTML += `<div class="alert alert-danger row h-20" role="alert">${text}</div>`;
}

function cleanAlerts() {
    alertDiv.innerHTML = "";
}