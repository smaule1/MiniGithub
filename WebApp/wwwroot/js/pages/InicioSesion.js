function InicioSesionController() {
    this.ApiService = "Usuario";

    this.InitView = function () {
        console.log("Login view init!");

        // Evento para el bot�n de iniciar sesi�n
        $("#btnIniciarSesion").click(() => {
            console.log("Clicked");
            this.IniciarSesion();
        });
    };

    this.IniciarSesion = () => {
        var loginData = {
            Email: $("#Email").val(),
            Password: $("#Password").val()
        };

        var ca = new ControlActions();

        var service = this.ApiService + "/Login?Email=" + loginData.Email + "&Password=" + loginData.Password;

        ca.PostToAPI(service, loginData, (data) => {

            console.log(data);

            if (data != null) {
                sessionStorage.setItem("Usuario", JSON.stringify(data)); // Cambiado a sessionStorage
                window.location.href = "Contenido"; // Redirecciona a la p�gina de contenido despu�s de iniciar sesi�n.
            } else {
                Swal.fire({
                    icon: 'error',
                    title: 'Oops...',
                    text: 'Usuario o contrase�a incorrectos',
                });
            }
        });
    };
}

$(document).ready(() => {
    var ic = new InicioSesionController();
    ic.InitView();
});
