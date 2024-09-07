function RegistroUsuarioController() {
    this.ApiService = "Usuario";

    this.InitView = function () {
        console.log("User view init!");

        $("#btnRegistrar").click(() => {
            console.log("Clicked");

            var uc = new RegistroUsuarioController();
            uc.RegistrarUsuario();
            window.location.href = "InicioSesion";
        });
    }

    this.RegistrarUsuario = () => {
        var usuario = {
            Nombre: $("#Nombre").val(),
            Email: $("#Email").val(),
            Password: $("#Password").val()
        };

        var ca = new ControlActions();
        var service = this.ApiService + "/Create";

        ca.PostToAPI(service, usuario, (data) => {
            console.log("Usuario registrado!");
            window.location.href = "InicioSesion";
        });
    }
}

$(document).ready(() => {
    var uc = new RegistroUsuarioController();
    uc.InitView();


});