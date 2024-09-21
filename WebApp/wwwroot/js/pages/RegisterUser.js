const nameInput = document.getElementById("Nombre");
const emailInput = document.getElementById("Email");
const passwordInput = document.getElementById("Password");
const confInput = document.getElementById("Conf_Password");
const btn = document.getElementById("btnRegistrar");

btn.addEventListener("click", (event) => {
    event.preventDefault();

    removeWarningClasses(nameInput);
    removeWarningClasses(emailInput);
    removeWarningClasses(passwordInput);
    removeWarningClasses(confInput);
    cleanAlerts();

    let name = nameInput.value;
    let password = passwordInput.value;
    let email = emailInput.value;
    let conf = confInput.value;

    if (password != conf) {
        setWarningClasses(confInput);
        addAlert("Both passwords must match.");
        return;
    }

    registerUser(email, password, name);
});

async function registerUser(email, password, name) {
    const url = `https://localhost:7269/api/usuario`;
    const myHeaders = new Headers();
    myHeaders.append("Content-Type", "application/json");

    try {
        console.log("here 1");
        const response = await fetch(url, {
            method: "POST",
            body: JSON.stringify({ password: password, email: email, name: name }),
            headers: myHeaders
        });

        console.log("here 2");
        if (!response.ok) {
            console.log("here 3");
            let obj = await response.json();
            if (obj.error == "Invalid Input") {
                if (obj.type == "nombre") {
                    console.log("here 4");
                    setWarningClasses(nameInput);
                    addAlert(obj.message);
                }
                if (obj.type == "email") {
                    console.log("here 5");
                    setWarningClasses(emailInput);
                    addAlert(obj.message);
                }
                if (obj.type == "contraseña") {
                    console.log("here 6");
                    setWarningClasses(passwordInput);
                    addAlert(obj.message);
                }
            } else if (obj.error == "Invalid Operation") {
                console.log("here 8", response.status);
                setWarningClasses(emailInput);
                addAlert(obj.message);
            }
        } else {
            console.log("here 7", response.status);
            window.location.pathname = "/Iniciosesion";
        }

    } catch (error) {
        console.error(error.message);
        addAlert("An unexpected error has occured.");
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