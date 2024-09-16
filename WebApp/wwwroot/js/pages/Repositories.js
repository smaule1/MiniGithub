console.log("it woooooooorks");


getData();


async function getData() {
    const url = "https://localhost:7269/api/repositorory/public/all/samuel";
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }

        const json = await response.json();
        console.log(json);
    } catch (error) {
        console.error(error.message);
    }
}
