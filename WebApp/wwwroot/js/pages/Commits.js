function changeDropdown(text) {
    document.getElementById('dropdownButton').innerHTML = text;
    commitController(text);
}
function commitController(branch) {
    var ca = new ControlActions();
    var urlService = ca.GetUrlApiService("commit/retrieveall?currentBranch=" + branch);

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

    /*
    var usuario = sessionStorage.getItem("Usuario");
    if (usuario == null) {
        window.location.href = "HomePage";
    }
    */
});