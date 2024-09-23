function ContenidoController() {
    this.ViewName = "Contenido";
    this.ApiService = "Contenido";

    this.initView = function () {
        console.log("ContenidoController.initView");
        this.LoadContent();
    }

    this.LoadContent = function () {
        var ca = new ControlActions();
        var urlService = ca.GetUrlApiService(this.ApiService + "/RetrieveAll");

        $.ajax({
            url: urlService,
            method: "GET",
            success: function (data) {
                var contenidoCartas = $("#contenidoCartas");
                contenidoCartas.empty();

                if (data.length > 0) {
                    data.forEach(function (item) {
                        var card = `
                            <div class="col-md-4 mb-4">
                                <div class="card shadow-sm border-light">
                                    <div class="card-body">
                                        <h5 class="card-title">${item.titulo}</h5>
                                        <p class="card-text">Autor: ${item.autor}</p>
                                        <p class="card-text">Tipo: ${item.tipoContenido}</p>
                                        <div class="d-flex justify-content-between">
                                            <button class="btn btn-primary btn-hacer-review" data-id="${item.id}">Hacer Review</button>
                                            <button class="btn btn-secondary btn-ver-reviews" data-id="${item.id}">Ver Reviews</button>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        `;
                        contenidoCartas.append(card);
                    });

                    // A�adir eventos a los botones
                    this.addEventListeners();
                } else {
                    contenidoCartas.html('<p>No hay contenido disponible en este momento.</p>');
                }
            }.bind(this), // Mantener el contexto correcto
            error: function (error) {
                console.error("Error al cargar el contenido:", error);
                $("#contenidoCartas").html('<p>Ocurri� un error al cargar el contenido.</p>');
            }
        });
    }

    this.addEventListeners = function () {
        // A�adir evento click a los botones "Hacer Review"
        $(".btn-hacer-review").off("click").on("click", function () {
            var contentId = $(this).data("id");
            sessionStorage.setItem("ContentId", contentId);
            console.log("ContentId almacenado en sessionStorage: " + sessionStorage.getItem("ContentId"));

            setTimeout(function () {
                window.location.href = "RegistroReview";
            }, 500); // Reducir el tiempo de espera si es necesario
        });

        // A�adir evento click a los botones "Ver Reviews"
        $(".btn-ver-reviews").off("click").on("click", function () {
            var contentId = $(this).data("id");
            sessionStorage.setItem("ContentId", contentId);
            console.log("ContentId almacenado en sessionStorage para ver reviews: " + sessionStorage.getItem("ContentId"));

            setTimeout(function () {
                window.location.href = "VerReview";
            }, 500); // Reducir el tiempo de espera si es necesario
        });
    }
}

$(document).ready(function () {
    var vc = new ContenidoController();
    vc.initView();

    var usuario = sessionStorage.getItem("Usuario");
    if (!usuario) {
        // Si no hay sesi�n activa, redirige al usuario a la p�gina de inicio de sesi�n
        window.location.href = "HomePage"; // Cambia "HomePage" a la ruta correcta de tu p�gina de inicio de sesi�n
    }
});
