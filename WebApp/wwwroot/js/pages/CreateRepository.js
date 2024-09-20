
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


    let input;
    let isValid = true;

    //Name
    input = nameInput.value;
    if (input == "") {
        setWarningClasses(nameInput);
        addAlert("El nombre del repositorio no puede estar vacío");
        isValid = false;
    }else if (!isAlphanumeric(input)) {
        setWarningClasses(nameInput);
        addAlert("El nombre del repositorio solo debe contener caracteres alfanumericos");
        isValid = false;
    }

    //Branch
    input = branchInput.value;
    if (input == "") {
        setWarningClasses(branchInput);
        addAlert("El nombre del branch no puede estar vacío");
        isValid = false;
    } else if (!isAlphanumeric(input)) {
        setWarningClasses(branchInput);
        addAlert("El nombre del branch solo debe contener caracteres alfanumericos");
        isValid = false;
    }

    //Tags

    input = tagInput.value;
    if (input!="") {
        input = input.split(",");
        for (let tag of input) {
            if (!isAlphanumeric(tag)) {
                setWarningClasses(tagInput);
                addAlert("El nombre del branch solo debe contener caracteres alfanumericos o comas");
                isValid = false;
            }
        }
    }
    
        
    if (!isValid) return;

    createRepository();
    
}, false);


function createRepository() {

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