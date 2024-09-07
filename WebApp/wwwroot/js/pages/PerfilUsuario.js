function PerfilController() {
    this.ViewName = "Perfil";
    this.ApiService = "Review";

    this.initView = function () {
        console.log("PerfilController.initView");
        this.LoadUserInfo();
        this.LoadUserReviews();
    }

    this.LoadUserInfo = function () {
        var usuario = JSON.parse(sessionStorage.getItem("Usuario"));
        if (usuario) {
            $("#userName").text(usuario.nombre);
            $("#userEmail").text(usuario.email);
        } else {
            // Si no hay un usuario logueado, redirigir al inicio de sesión
            window.location.href = "HomePage";
        }
    }

    this.LoadUserReviews = function () {
        var usuario = JSON.parse(sessionStorage.getItem("Usuario"));
        if (usuario && usuario.id) {
            var usuarioId = usuario.id;
            console.log("PerfilController.LoadUserReviews userId:", usuarioId);
            var ca = new ControlActions();
            var urlService = ca.GetUrlApiService(this.ApiService + "/RetrieveByUserId?usuarioId=" + usuarioId);

            $.ajax({
                url: urlService,
                method: "GET",
                success: function (data) {
                    console.log("Datos recibidos:", data);  // Añade esto para verificar los datos.

                    var reviewCards = $("#reviewCards");
                    reviewCards.empty();

                    if (data && data.length > 0) {
                        data.forEach(function (review) {
                            console.log("Review individual:", review);  // Añade esto para verificar cada review.
                            var card = createReviewCard(review);
                            reviewCards.append(card);
                        });
                    } else {
                        reviewCards.append('<p>No has realizado ninguna reseña.</p>');
                    }
                },
                error: function (error) {
                    console.error("Error al cargar las reseñas:", error);
                    alert("Hubo un error al cargar las reseñas. Por favor, inténtalo de nuevo más tarde.");
                }
            });
        } else {
            console.error("No se pudo obtener el ID del usuario.");
        }
    }

    function createReviewCard(review) {
        return `
            <div class="col-md-4">
                <div class="card mb-4 shadow-sm" style="border-radius: 15px;">
                    <div class="card-body">
                        <h5 class="card-title text-primary">Contenido ID: ${review.contenidoId}</h5>
                        <p class="card-text">
                            <strong>Calificación:</strong> ${renderStars(review.rating)} <span class="text-muted">(${review.rating})</span>
                        </p>
                        <p class="card-text">${review.texto}</p>
                    </div>
                </div>
            </div>
        `;
    }

    function renderStars(rating) {
        let stars = '';
        for (let i = 0; i < 5; i++) {
            if (i < rating) {
                stars += '<i class="fas fa-star text-warning"></i>';
            } else {
                stars += '<i class="far fa-star text-warning"></i>';
            }
        }
        return stars;
    }
}

$(document).ready(function () {
    var pc = new PerfilController();
    pc.initView();
});
