function VerReviewsController() {
    this.ViewName = "VerReviews";
    this.ApiService = "Review";

    this.initView = function () {
        console.log("VerReviewsController.initView");
        this.LoadReviewsByContentId();
    }

    this.LoadReviewsByContentId = function () {
        var contentId = sessionStorage.getItem("ContentId");
        console.log("VerReviewsController.LoadReviewsByContentId contentId:", contentId);

        var ca = new ControlActions();
        var urlService = ca.GetUrlApiService(this.ApiService + "/RetrieveByContentId?contenidoId=" + contentId);

        $.ajax({
            url: urlService,
            method: "GET",
            success: function (data) {
                var reviewCards = $("#reviewCards");
                reviewCards.empty();

                if (data && data.length > 0) {
                    console.log("Reviews encontradas:", data);
                    data.forEach(function (review) {
                        var card = `
                            <div class="col-md-4">
                                <div class="card mb-4">
                                    <div class="card-body">
                                        <h5 class="card-title">Reseña de: ${review.usuarioId}</h5>
                                        <p class="card-text">Rating: ${review.rating}</p>
                                        <p class="card-text">Texto: ${review.texto}</p>
                                    </div>
                                </div>
                            </div>
                        `;
                        reviewCards.append(card);
                    });
                } else {
                    reviewCards.append('<p>No hay reseñas para este contenido.</p>');
                }
            },
            error: function (error) {
                console.error("Error al cargar las reseñas:", error);
            }
        });
    }
}

$(document).ready(function () {
    var vr = new VerReviewsController();
    vr.initView();

    var usuario = sessionStorage.getItem("Usuario");
    if (usuario == null) {
        window.location.href = "HomePage";
    }
});
