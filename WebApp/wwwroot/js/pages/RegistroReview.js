function RegistroReviewController() {
    this.ApiService = "Review";

    this.InitView = function () {
        console.log("Review view init!");

        // Recupera el contentId desde sessionStorage
        var contentId = sessionStorage.getItem("ContentId");

        // Asegúrate de que el contentId se está recuperando correctamente
        if (contentId) {
            $("#contenidoId").val(contentId); // Asigna el valor al campo oculto
            console.log("ContentId recuperado y asignado: " + contentId);
        } else {
            console.error("No se encontró contentId en sessionStorage.");
        }

        $("#btnHacerReview").click(() => {
            console.log("Clicked");
            this.RegistrarReview();
            window.location.href = "Contenido";
        });
    }

    this.RegistrarReview = function () {
        // Obtén el usuario logueado desde sessionStorage
        var usuario = JSON.parse(sessionStorage.getItem("Usuario"));

        console.log("Usuario logueado:", usuario);

        // Verifica que el usuario esté definido
        if (!usuario || !usuario.id) {
            console.error("No se encontró un usuario logueado.");
            return;
        }

        // Obtén el contentId desde el campo oculto
        var contentId = $("#contenidoId").val();

        // Verifica que contentId sea un número entero
        if (!contentId || isNaN(contentId)) {
            console.error("El contenidoId no es válido.");
            alert("El ID del contenido es inválido o no está presente.");
            return;
        }

        var textoReview = $("#textoReview").val();
        var rating = $("#calificacion").val();

        console.log("ContentId:", contentId);
        console.log("TextoReview:", textoReview);
        console.log("Rating:", rating);

        var review = {
            contenidoId: parseInt(contentId),  // Convertir a entero
            usuarioId: parseInt(usuario.id),    // Convertir a entero
            texto: textoReview,
            rating: parseInt(rating)         // Convertir a entero
        };

        console.log("Review a enviar:", review);  // Verificar valores antes de enviar

        var ca = new ControlActions();
        var service = this.ApiService + "/Create";

        ca.PostToAPI(service, review, () => {
            console.log("Review registrada!");
            
        });
    }
}

$(document).ready(function () {
    var uc = new RegistroReviewController();
    uc.InitView();

    // Verifica si hay un usuario logueado
    var usuario = sessionStorage.getItem("Usuario");
    if (usuario == null) {
        window.location.href = "HomePage"; // Cambia "HomePage" a la ruta correcta de tu página de inicio de sesión
    }
});



