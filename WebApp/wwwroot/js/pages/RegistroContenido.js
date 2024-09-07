function RegistroContenidoController() {
    this.ViewName = "RegistroContenido";
    this.ApiService = "Contenido";

    this.InitView = function () {
        console.log("Contenido view init!");

        // Evento para el botón de registrar contenido
        $("#btnRegistrarContenido").click(() => {
            console.log("Clicked");

            // Llama directamente al método RegistrarContenido en lugar de crear una nueva instancia
            this.RegistrarContenido();
            window.location.href = "Contenido";
        });
    }

    this.RegistrarContenido = () => {
        var contenido = {
            Titulo: $("#titulo").val(),
            Autor: $("#Autor").val(),
            TipoContenido: $("#TipoContenido").val()
        };
        console.log(contenido);

        var ca = new ControlActions();
        var service = this.ApiService + "/Create";

        ca.PostToAPI(service, contenido, () => {
            console.log("Contenido registrado!");

            // Redirige a la página de "Contenido" después de registrar el contenido
             // Cambia "Contenido" a la ruta correcta de tu página de contenido
        });
    }
}

$(document).ready(() => {
    var uc = new RegistroContenidoController();
    uc.InitView();

    // Verifica si hay un usuario logueado
    var usuario = sessionStorage.getItem("Usuario");
    if (usuario == null) {
        // Si no hay sesión activa, redirige al usuario a la página de inicio de sesión
        window.location.href = "HomePage"; // Cambia "HomePage" a la ruta correcta de tu página de inicio de sesión
    }
    console.log(localStorage.getItem("Usuario"));
});
