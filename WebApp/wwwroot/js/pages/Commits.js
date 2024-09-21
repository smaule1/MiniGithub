sessionStorage.setItem("Branch", "Master");
var testBranches = [
    {
        Name: "Master",
        Latest_commit: "66ecf4bf1741bfca3151ed7f"
    },
    {
        Name: "Commit",
        Latest_commit: "66ed2bf9a75d4957b228bb22"
    }
];

sessionStorage.setItem("AllBranches", JSON.stringify(testBranches));

function changeDropdown(text) {
    //Se mete la branch actual dentro del storage local
    sessionStorage.setItem("Branch", text);

    document.getElementById('dropdownButton').innerHTML = text;

    commitController();
}

//Listens the "click" event to create a Commit
function createCommit() {
    $("#acceptModalButton").click(() => {
        //registrarCommit();
        multipleFiles();
    });
}

function multipleFiles() {
    //Necesita traer elementos guardados en el SessionStorage

    var message = $("#commitMsg").val();
    var file = document.getElementById('file').files;
    console.log(file);

}

//This function create a commit
function registrarCommit() {
    //Necesita traer elementos guardados en el SessionStorage
    var lastVersion = Number(sessionStorage.getItem("Version"));
    lastVersion += 1;
    console.log(lastVersion);

    var formData = new FormData(); // Crear un objeto FormData

    var message = $("#commitMsg").val();
    var file = document.getElementById('file').files;
    console.log(file);

    formData.append('repoName', "MiniGithub");
    formData.append('branchName', sessionStorage.getItem("Branch"));
    formData.append('version', lastVersion);
    formData.append('message', message);
    formData.append('file', file);

    var ca = new ControlActions();
    var service = "commit/create";

    ca.PostToAPICommit(service, formData, () => {
        console.log("Contenido registrado!");
        commitController();

        // Redirige a la p�gina de "Contenido" despu�s de registrar el contenido
        // Cambia "Contenido" a la ruta correcta de tu p�gina de contenido
    });
}

//This function fetchs data from Mongo
function commitController() {
    var currentBranch = sessionStorage.getItem("Branch");
    var ca = new ControlActions();
    var urlService = ca.GetUrlApiService("commit/retrieveall?currentBranch=" + currentBranch)

    $.ajax({
        url: urlService,
        method: "GET",
        success: function (data) {
            const commitList = $("#commitList");
            commitList.empty();

            if (data && data.length > 0) {
                console.log("Commits encontrados:", data);

                const selectedBranch = document.getElementById(currentBranch);
                selectedBranch.dataset.commit = data[data.length-1].id;
                console.log(selectedBranch.dataset.commit);

                sessionStorage.setItem("Version", data[data.length - 1].version);

                data.forEach(function (commit) {
                    var listElement = ` 
                                    <a id="${commit.id}" data-version="${commit.version}" href="https://localhost:7269/api/commit/download/${commit.fileId}" class="list-group-item list-group-item-action">${commit.message}</a>
                                    `;
                    commitList.append(listElement);
                });
            } else {
                sessionStorage.setItem("Version", 0);
                commitList.append('<p>No hay commits aún.</p>');
            }
        },
        error: function (error) {
            console.error("Error al cargar los commits:", error);
        }
    });
}

function branchController() {
    //var currentBranch = sessionStorage.getItem("RepoName");
    var currentBranch = JSON.parse(sessionStorage.getItem("AllBranches"));

    const dropDown = $("#dropDownBranch");
    dropDown.empty();

    for (var ele of currentBranch) {
        var listElement = ` 
                        <li><button id="${ele.Name}" data-commit="${ele.Latest_commit}" class="dropdown-item" onclick="changeDropdown('${ele.Name}')">${ele.Name}</button></li>
                        `;
        dropDown.append(listElement);
    }
}


$(document).ready(function () {
    branchController();
    var commitCreator = new createCommit();
    commitController();

    /*
    var usuario = sessionStorage.getItem("Usuario");
    if (usuario == null) {
        window.location.href = "HomePage";
    }
    */
});