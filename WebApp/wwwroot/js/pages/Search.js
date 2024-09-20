

const searchButton = document.getElementById("searchButton");
const searchField = document.getElementById("searchField");
const tableRepos = document.getElementById("tableRepos");


searchButton.addEventListener("click", (e) => {
    e.preventDefault();    
    tableRepos.innerHTML = null;
    let value = searchField.value;
    if (value == "") {
        return;
    } else {
        search(value);    
    }
    
});


async function search(name) {        
    const url = `https://localhost:7269/api/repository/public/name/${name}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }

        const json = await response.json();

        for (let i of json) {
            displayRepository(i);            
        }

    } catch (error) {
        console.error(error.message);
    }
}


function displayRepository(value) {
    var row = `
        <tr>
            <td> <a href="/PublicRepository?id=${value.id}" class="link-primary"> ${value.name} </a> </td>
            <td>${value.visibility} </td>
            <td>${value.tags}</td>
        </tr>
    `;
    tableRepos.innerHTML += row;
}