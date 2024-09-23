


const publicList = document.getElementById("publicList");
const privateList = document.getElementById("privateList");

const userId = sessionStorage.getItem("_User");

document.getElementById("repoName").textContent = sessionStorage.getItem("_UserName");

document.getElementById("repoName").textContent = sessionStorage.getItem("_UserName");

getPublicRepositories(userId);
getPrivateRepositories(userId);

async function getPublicRepositories(userId) {
    const url = `https://localhost:7269/api/repository/public/all/${userId}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }

        const json = await response.json();

        for (let i of json) {
            displayPublicRepository(i);
        }

    } catch (error) {
        console.error(error.message);
    }
}

async function getPrivateRepositories(userId) {    
    const url = `https://localhost:7269/api/repository/privado/all/${userId}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }

        const json = await response.json();

        for (let i of json) {
            displayPrivateRepository(i);
        }

    } catch (error) {
        console.error(error.message);
    }
}


function displayPublicRepository(value) {    
    publicList.innerHTML += `<a href="/PublicRepository?id=${value.id}" class="list-group-item list-group-item-action">${value.name}</a>`;
}

function displayPrivateRepository(value) {
    privateList.innerHTML += `<a href="/PrivateRepository?id=${value.id}" class="list-group-item list-group-item-action">${value.name}</a>`;
}

document.getElementById("createBtn").addEventListener("click", (e) => {
    e.preventDefault();
    window.location.pathname = "/CreateRepository";
});

document.getElementById("sessionBtn").addEventListener("click", async (e) => {
    e.preventDefault();

    logout();
});

async function logout() {
    const url = new URL(`https://localhost:7269/api/usersession`);
    var session = sessionStorage.getItem("_SessionId");

    if (session == null || session == "") {
        console.log("No existe una sesión activa.");
        return;
    }

    const params = { sessionId: session };
    Object.keys(params).forEach(key => url.searchParams.append(key, params[key]));

    try {
        const response = await fetch(url, {
            method: 'DELETE'
        });

        if (!response.ok) {
            let obj = await response.json();
            if (obj.error === "Not Auth" || obj.error === "Not Found") {
                clearSession();
                window.location.pathname = "/Homepage";
                console.error(obj.message);
            }
        } else {
            clearSession();
            window.location.pathname = "/Homepage";
        }
    } catch (error) {
        clearSession();
        window.location.pathname = "/Homepage";
        console.error(error.message);
    }
}

function clearSession() {    
    sessionStorage.clear();
}

const nameInput = document.getElementById("newUsername");
const curInput = document.getElementById("currentPassword");
const newInput = document.getElementById("newPassword");
const confInput = document.getElementById("confirmPassword");

document.getElementById("changeDetailsBtn").addEventListener("click", function () {
    const changeDetailsDiv = document.getElementById("changeDetails");

    if (changeDetailsDiv.classList.contains("d-none")) {
        changeDetailsDiv.classList.remove("d-none");
    } else {
        changeDetailsDiv.classList.add("d-none");
        removeWarningClasses(nameInput);
        removeWarningClasses(curInput);
        removeWarningClasses(newInput);
        removeWarningClasses(confInput);
        cleanAlerts();
    }
});

document.getElementById("changeDetailsForm").addEventListener("submit", function (event) {
    event.preventDefault();

    removeWarningClasses(nameInput);
    removeWarningClasses(curInput);
    removeWarningClasses(newInput);
    removeWarningClasses(confInput);
    cleanAlerts();

    const newName = nameInput.value;
    const curPassword = curInput.value;
    const newPassword = newInput.value;
    const confPassword = confInput.value;

    console.log("nase");

    if (curPassword == "" || curPassword == null) {
        setWarningClasses(document.getElementById("currentPassword"));
        addAlert("Debe introducir su contraseña actual.");
        return;
    }

    if (newPassword != confPassword) {
        setWarningClasses(document.getElementById("confirmPassword"));
        addAlert("Ambas constraseñas deben ser iguales.");
        return;
    }

    update(newName, newPassword, curPassword)
});

async function update(name, password, oldPassword) {
    const url = `https://localhost:7269/api/usuario`;
    const myHeaders = new Headers();
    myHeaders.append("Content-Type", "application/json");
    const email = sessionStorage.getItem("_UserEmail");

    try {
        const response = await fetch(url, {
            method: "PUT",
            body: JSON.stringify({ password: password, email: email, name: name, oldPassword: oldPassword }),
            headers: myHeaders
        });

        if (!response.ok) {
            let obj = await response.json();
            if (obj.error == "Invalid Input") {
                if (obj.type == "Nombre") {
                    setWarningClasses(nameInput);
                    addAlert(obj.message);
                }
                if (obj.type == "Wrong Current") {
                    setWarningClasses(curInput);
                    addAlert(obj.message);
                }
                if (obj.type == "Wrong New") {
                    setWarningClasses(newInput);
                    addAlert(obj.message);
                }
            } else if (obj.error == "Invalid Operation") {
                addAlert(obj.message);
            }
        } else {
            logout();
        }

    } catch (error) {
        console.error(error.message);
        addAlert("Ocurrió un error inesperado.");
    }
}

document.getElementById("deleteAccountBtn").addEventListener("click", async () => {
    const confirmed = confirm("¿Estás seguro de que deseas eliminar tu cuenta? Esta acción no se puede deshacer.");
    if (confirmed) {
        const url = new URL(`https://localhost:7269/api/usuario`);
        var user = sessionStorage.getItem("_UserEmail");

        if (user == null || user == "") {
            console.log("No existe una sesión activa.");
            return;
        }

        const params = { userId: user };
        Object.keys(params).forEach(key => url.searchParams.append(key, params[key]));

        try {
            const response = await fetch(url, {
                method: 'DELETE'
            });

            if (!response.ok) {
                let obj = await response.json();
                if (obj.error === "Invalid Operation" || ob.error === "Invalid Input") {
                    addAlert2(obj.message);
                }
            } else {
                logout();
            }
        } catch (error) {
            addAlert2(obj.message);
            console.error(error.message);
        }
    } else {
        console.log("Eliminación de cuenta cancelada.");
    }
});

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

function addAlert2(message) {
    text = `Advertencia: ${message}`;
    alertDiv2.innerHTML += `<div class="alert alert-danger row h-20" role="alert">${text}</div>`;
}

function cleanAlerts2() {
    alertDiv2.innerHTML = "";
}