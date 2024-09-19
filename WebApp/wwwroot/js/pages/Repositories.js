
getData();


async function getData() {
    const url = "https://localhost:7269/api/repository/public/all/samuel";
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        
        const json = await response.json();                        
        
        for (let i of json) {
            displayRepository(i);  
            console.log(i);
        }
        
    } catch (error) {
        console.error(error.message);
    }
}



function displayRepository(value) {
    const table = $("#tableBodyRepo");
    var row = `
        <tr>
            <th>${value.name}</th>
            <td>${value.visibility} </td>
            <td>${value.tags}</td>
        </tr>
    `;
    table.append(row);

    table.appendChild
}
