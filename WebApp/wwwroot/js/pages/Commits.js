sessionStorage.setItem("Branch", "Master");

function changeDropdown(text) {
    //Se mete la branch actual dentro del storage local
    sessionStorage.setItem("Branch", text);

    document.getElementById('dropdownButton').innerHTML = text;

    commitController();
}

//Listens the "click" event to create a Commit
function createCommit() {
    $("#acceptModalButton").click(() => {
        registrarCommit();
    });
}

//This function create a commit
function registrarCommit() {
    //Necesita traer elementos guardados en el SessionStorage

    var formData = new FormData(); // Crear un objeto FormData

    var message = $("#commitMsg").val();
    var file = document.getElementById('file').files[0];

    formData.append('repoName', "MiniGithub");
    formData.append('branchName', sessionStorage.getItem("Branch"));
    formData.append('version', 10);
    formData.append('message', message);
    formData.append('file', file);

    var ca = new ControlActions();
    var service = "commit/create";

    ca.PostToAPICommit(service, formData, () => {
        console.log("Contenido registrado!");

        // Redirige a la p�gina de "Contenido" despu�s de registrar el contenido
        // Cambia "Contenido" a la ruta correcta de tu p�gina de contenido
    });

    const commitList = $("#commitList");
    var listElement = `<a href="#" class="list-group-item list-group-item-action">${message}</a>`;

    commitList.append(listElement);
}

//This function fetchs data from Mongo
function commitController() {
    var currentBranch = sessionStorage.getItem("Branch");
    var ca = new ControlActions();
    var urlService = ca.GetUrlApiService("commit/retrieveall?currentBranch=" + currentBranch);

    $.ajax({
        url: urlService,
        method: "GET",
        success: function (data) {
            const commitList = $("#commitList");
            commitList.empty();

            if (data && data.length > 0) {
                console.log("Commits encontrados:", data);
                data.forEach(function (commit) {
                    var listElement = ` 
                                    <a href="#" class="list-group-item list-group-item-action">${commit.message}</a>
                                    `;
                    commitList.append(listElement);
                });
            } else {
                commitList.append('<p>No hay commits aún.</p>');
            }
        },
        error: function (error) {
            console.error("Error al cargar los commits:", error);
        }
    });
}

$(document).ready(function () {
    commitController("Master");
    var commitCreator = new createCommit();

    /*
    var usuario = sessionStorage.getItem("Usuario");
    if (usuario == null) {
        window.location.href = "HomePage";
    }
    */
});