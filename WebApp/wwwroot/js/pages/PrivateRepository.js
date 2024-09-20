
const queryString = window.location.search;
const urlParams = new URLSearchParams(queryString);


const repoName = document.getElementById("repoName");
const repoVisibility = document.getElementById("repoVisibility");
const repoTags = document.getElementById("repoTags");
const branchSelect = document.getElementById("branchSelect");


loadRepo(urlParams.get("id"));

async function loadRepo(id) {
    const url = `https://localhost:7269/api/repository/private/${id}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }

        const repoObj = await response.json();

        displayRepository(repoObj);

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

function displayBranch(branch) {
    $("#branchDiv").append(`<h1>${branch.latestCommit}: poner datos del commit o algo así</h1>`);
}