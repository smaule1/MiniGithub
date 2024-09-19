

const searchForm = document.getElementById("searchForm");
const searchField = document.getElementById("searchField");
const tableRepos = document.getElementById("tableRepos");


searchForm.addEventListener("submit", (e) => {
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
            <th> <a href="/Repository?id=${value.id}"> ${value.name} </a> </th>
            <td>${value.visibility} </td>
            <td>${value.tags}</td>
        </tr>
    `;
    tableRepos.innerHTML += row;
}