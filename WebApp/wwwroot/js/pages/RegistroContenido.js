function RegistroContenidoController() {
    this.ViewName = "RegistroContenido";
    this.ApiService = "Contenido";

    this.InitView = function () {
        console.log("Contenido view init!");

        // Evento para el bot�n de registrar contenido
        $("#btnRegistrarContenido").click(() => {
            console.log("Clicked");

            // Llama directamente al m�todo RegistrarContenido en lugar de crear una nueva instancia
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

            // Redirige a la p�gina de "Contenido" despu�s de registrar el contenido
             // Cambia "Contenido" a la ruta correcta de tu p�gina de contenido
        });
    }
}

$(document).ready(() => {
    var uc = new RegistroContenidoController();
    uc.InitView();

    // Verifica si hay un usuario logueado
    var usuario = sessionStorage.getItem("Usuario");
    if (usuario == null) {
        // Si no hay sesi�n activa, redirige al usuario a la p�gina de inicio de sesi�n
        window.location.href = "HomePage"; // Cambia "HomePage" a la ruta correcta de tu p�gina de inicio de sesi�n
    }
    console.log(localStorage.getItem("Usuario"));
});
