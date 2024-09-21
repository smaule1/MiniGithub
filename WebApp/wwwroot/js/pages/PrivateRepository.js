
const queryString = window.location.search;
const urlParams = new URLSearchParams(queryString);


const repoName = document.getElementById("repoName");
const repoVisibility = document.getElementById("repoVisibility");
const repoTags = document.getElementById("repoTags");
const branchSelect = document.getElementById("branchSelect");

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

document.getElementById("createBranchBtn").addEventListener("click", (e) => {
    e.preventDefault();
    window.location.pathname = "/CreateRepository";
});

document.getElementById("deleteBranchBtn").addEventListener("click", (e) => {
    e.preventDefault();
    window.location.pathname = "/CreateRepository";
});





function displayBranch(branch) {
    $("#branchDiv").append(`<h5>${branch.latestCommit}: poner datos del commit o algo así</h5>`);
}