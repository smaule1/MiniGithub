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
    var message = $("#commitMsg").val();
    var contenido = {
        RepoName: "MiniGithub",
        BranchName: sessionStorage.getItem("Branch"),
        Version: 1,
        Message: message
    };
    console.log(contenido);

    var ca = new ControlActions();
    var service = "commit/create";

    ca.PostToAPI(service, contenido, () => {
        console.log("Contenido registrado!");

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