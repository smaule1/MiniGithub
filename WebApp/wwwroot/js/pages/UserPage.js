


const publicList = document.getElementById("publicList");
const privateList = document.getElementById("privateList");

const userId = "todo";

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