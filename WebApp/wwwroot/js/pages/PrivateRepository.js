
const queryString = window.location.search;
const urlParams = new URLSearchParams(queryString);


const repoName = document.getElementById("repoName");
const repoVisibility = document.getElementById("repoVisibility");
const repoTags = document.getElementById("repoTags");
const branchSelect = document.getElementById("branchSelect");
const branchNameInput = document.getElementById("branchNameInput");

let repository = null;

loadRepo(urlParams.get("id"));

async function loadRepo(id) {
    const url = `https://localhost:7269/api/repository/private/${id}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }

        repository = await response.json();

        displayRepository(repository);

    } catch (error) {
        console.error(error.message);
    }
}


function displayRepository(repoObject) {
    repoName.innerHTML = repoObject.name;
    repoVisibility.innerHTML += repoObject.visibility;
    repoTags.innerHTML += repoObject.tags;

    let branches = repoObject.branches;


    for (let branch of branches) {
        branchSelect.innerHTML += `<option value="${branch.name}">${branch.name}</option>`;
        branchSelect.lastChild.addEventListener("change", displayBranch(branch));
    }
}


document.getElementById("modifyBtn").addEventListener("click", (e) => { 
    window.location.href = `/ModifyRepository?id=${repository.id}&v=${repository.visibility}`;;
});

document.getElementById("deleteBtn").addEventListener("click", async (e) => {
    const url = `https://localhost:7269/api/repository/${repository.id}`;    
    try {
        const response = await fetch(url, {method: "DELETE"});
        if (!response.ok) {
            alert("Unexpected error");
            return;
        }

        window.location.pathname = "/UserPage";

    } catch (error) {
        console.error(error.message);        
    }       
});

document.getElementById("createBranchBtn").addEventListener("click", async (e) => {
    e.preventDefault();
    const url = `https://localhost:7269/api/repository/${repository.id}/branch`;
    const myHeaders = new Headers();
    myHeaders.append("Content-Type", "application/json");

    let selectedBranchCommit = null;

    for (let branch of repository.branches) {
        if (branch.name == branchSelect.value) {
            selectedBranchCommit = branch.latestCommit;
            break;
        }
    }    

    if (selectedBranchCommit == null) {
        alert("No se puede crear un branch a partir de un branch sin commits");
        return;
    }

    //Name
    let name = branchNameInput.value;
    name.trim();
    if (name == "") {              
        return;
    } else if (!isAlphanumeric(name)) {        
        alert("El nombre del repositorio solo debe contener caracteres alfanumericos");        
    }
    name = name.replaceAll(" ", "-");

    console.log(selectedBranchCommit)
    try {
        const response = await fetch(url, {
            method: "POST",
            body: JSON.stringify({ name:name , latestCommit:selectedBranchCommit}),
            headers: myHeaders
        });

        if (!response.ok) {
            let obj = await response.text();
            alert(obj)         
        }
        location.reload()

    } catch (error) {
        console.error(error.message);
        alert("Ocurrió un error inesperado");
    }
});

document.getElementById("deleteBranchBtn").addEventListener("click", async (e) => {
    if (branchSelect.value == "Master") { alert("El Branch Master no puede ser borrado"); return; } 
    const url = `https://localhost:7269/api/repository/${repository.id}/branch/${branchSelect.value}`;
    try {
        const response = await fetch(url, { method: "DELETE" });
        if (!response.ok) {
            alert("Unexpected error");
            return;
        }        
    } catch (error) {
        console.error(error.message);
    }
});

function isAlphanumeric(text) {
    const alphanumericRegex = /^[a-z0-9 -]+$/i;
    return alphanumericRegex.test(text);
}







function displayBranch(branch) {
    $("#branchDiv").append(`<h5>${branch.latestCommit}: poner datos del commit o algo así</h5>`);
}