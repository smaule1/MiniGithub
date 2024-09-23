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

//Functions that merges the branch
function merge() {
    mergeController();
}

function mergeController(commitId) {
    //Get the last commit from the current branch
    var currentBranch = sessionStorage.getItem("Branch");

    const selectedBranch = document.getElementById(currentBranch);
    var lastCurrentCommit = selectedBranch.dataset.commit;

    changeDropdown("Master");

    const masterBranch = document.getElementById('Master');
    var lastMasterCommit = masterBranch.dataset.commit;

    var data = lastCurrentCommit;

    var ca = new ControlActions();
    var service = "commit/merge/" + lastMasterCommit;

    console.log(lastMasterCommit);
    console.log(lastCurrentCommit);

    ca.PostToAPI(service, data, () => {
        console.log("Contenido registrado!");
        commitController();

    });
}

//Functions that does a rollback
function rollback(commitId) {

    rollbackController(commitId);
}

function rollbackController(commitId) {
    //Necesita traer elementos guardados en el SessionStorage
    var lastVersion = Number(sessionStorage.getItem("Version"));
    lastVersion += 1;

    console.log(lastVersion);

    var data = lastVersion;

    var ca = new ControlActions();
    var service = "commit/rollback/" + commitId;

    ca.PostToAPI(service, data, () => {
        console.log("Contenido registrado!");
        commitController();

    });
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

    for (let i = 0; i < file.length; i++) {
        formData.append('file', file[i]);
    }

    console.log(formData);

    var ca = new ControlActions();
    var service = "commit/create";

    ca.PostToAPICommit(service, formData, () => {
        console.log("Contenido registrado!");
        commitController();

    });
}

//This function fetchs data from Mongo
function commitController() {
    var currentBranch = sessionStorage.getItem("Branch");

    if (currentBranch == "Master") 
        document.getElementById("mergeButton").hidden = true;
    else
        document.getElementById("mergeButton").hidden = false;
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
                console.log(sessionStorage.getItem("Version"));


                for (var i = data.length-1; i >= 0; i--) {
                    var commit = data[i];
                    var listElement = ` 
                                    <li class="hoverList list-group-item d-flex justify-content-between align-items-center">
                                        <a data-version="${commit.version}" href="https://localhost:7269/api/commit/download/${commit.id}" class="commits list-group-item list-group-item-action">${commit.message}</a>
                                        <div class="dropdown">
                                            <button class="btn btn-link dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                                                <i class="fas fa-ellipsis-h"></i>
                                            </button>
                                            <ul id="dropDownBranch" class="dropdown-menu">
                                                <li><button data-commit="Master" class="dropdown-item" onclick="rollback('${commit.id}')">Rollback</button></li>  
                                            </ul>
                                        </div>  
                                    </li>
                                    `;
                    commitList.append(listElement);
                }
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