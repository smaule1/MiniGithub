
const nameInput = document.getElementById("nameInput");
const visibilityInput = document.getElementById("visibilityInput");
const tagInput = document.getElementById("tagInput");
const branchInput = document.getElementById("branchInput");
const alertDiv = document.getElementById("alertDiv");
const submitBtn = document.getElementById("submitBtn");

 

submitBtn.addEventListener("click", (event) => {
    event.preventDefault();    

    removeWarningClasses(nameInput);
    removeWarningClasses(branchInput);
    removeWarningClasses(tagInput);
    cleanAlerts();

    let isValid = true;

    //Name
    let name = nameInput.value;
    if (name == "") {
        setWarningClasses(nameInput);
        addAlert("El nombre del repositorio no puede estar vacío");
        isValid = false;
    }else if (!isAlphanumeric(name)) {
        setWarningClasses(nameInput);
        addAlert("El nombre del repositorio solo debe contener caracteres alfanumericos");
        isValid = false;
    }

    //Branch
    let branch = branchInput.value;
    if (branch == "") {
        setWarningClasses(branchInput);
        addAlert("El nombre del branch no puede estar vacío");
        isValid = false;
    } else if (!isAlphanumeric(branch)) {
        setWarningClasses(branchInput);
        addAlert("El nombre del branch solo debe contener caracteres alfanumericos");
        isValid = false;
    }

    //Tags
    let tags = tagInput.value;
    if (tags != "") {
        tags = tags.split(",");
        for (let tag of tags) {
            if (!isAlphanumeric(tag)) {
                setWarningClasses(tagInput);
                addAlert("El nombre las etiquetas solo debe contener caracteres alfanumericos o comas");
                isValid = false;
            }
        }
    } else {
        tags = [];
    }

    
    let visibility = document.querySelector('input[name="visibility"]:checked').value;
    
        
    if (!isValid) return;

    createRepository(name, branch, tags, visibility);
    
}, false);


async function createRepository(name, branch, tags, visibility) {    
    

    const url = `https://localhost:7269/api/repository`;
    const myHeaders = new Headers();
    myHeaders.append("Content-Type", "application/json");

    try {
        const response = await fetch(url, {
            method: "POST",
            body: JSON.stringify({ userId: "todo", name: name, tags: tags, visibility:visibility, branches: [{name:branch, latestCommit:null}]}),
            headers: myHeaders
        });

        if (!response.ok) {
            let obj = await response.json()            
            if (obj.error = "Duplicate Key Error") {
                addAlert(`Este usuario ya tiene un repositorio llamado ${name}`);
            }
        }

        window.location.pathname = "/Homepage";  //TODO: cambiar por pagina de usuario
       
    } catch (error) {
        console.error(error.message);
        addAlert("Ocurrió un error inesperado");
    }
}




function isAlphanumeric(text) {
    const alphanumericRegex = /^[a-z0-9]+$/i;
    return alphanumericRegex.test(text);
}


function setWarningClasses(element) {    
    element.classList.add("border-danger");
    element.classList.add("border-2");
}

function removeWarningClasses(element){    
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