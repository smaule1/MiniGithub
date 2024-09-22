function ControlActions() {
    //Ruta base del API
    this.URL_API = "https://localhost:7269/api/";

    this.GetUrlApiService = function (service) {
        return this.URL_API + service;
    }

    this.GetTableColumsDataName = function (tableId) {
        var val = $('#' + tableId).attr("ColumnsDataName");

        return val;
    }

    this.FillTable = function (service, tableId, refresh) {
        if (!refresh) {
            columns = this.GetTableColumsDataName(tableId).split(',');
            var arrayColumnsData = [];

            $.each(columns, function (index, value) {
                var obj = {};
                obj.data = value;
                arrayColumnsData.push(obj);
            });

            $('#' + tableId).DataTable({
                "processing": true,
                "ajax": {
                    "url": this.GetUrlApiService(service),
                    dataSrc: ''
                },
                "columns": arrayColumnsData
            });
        } else {
            $('#' + tableId).DataTable().ajax.reload();
        }
    }

    this.GetSelectedRow = function (tableId) {
        var data = localStorage.getItem(tableId + '_selected');
        return data;
    };

    this.BindFields = function (formId, data) {
        console.log(data);
        $('#' + formId + ' *').filter(':input').each(function (input) {
            var columnDataName = $(this).attr("ColumnDataName");
            this.value = data[columnDataName];
        });
    }

    this.GetDataForm = function (formId) {
        var data = {};
        $('#' + formId + ' *').filter(':input').each(function (input) {
            var columnDataName = $(this).attr("ColumnDataName");
            data[columnDataName] = this.value;
        });
        console.log(data);
        return data;
    }

    this.PostToAPI = function (service, data, callBackFunction) {
        $.ajax({
            type: "POST",
            url: this.GetUrlApiService(service),
            data: JSON.stringify(data),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                if (callBackFunction) {
                    Swal.fire('Completado!', 'Transaction completed!');
                    callBackFunction(data);
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                var responseJson = jqXHR.responseJSON;
                var message = jqXHR.responseText;

                console.error("Error Status: ", textStatus);
                console.error("Error Thrown: ", errorThrown);
                console.error("Full Response: ", jqXHR);

                if (responseJson) {
                    var errors = responseJson.errors;
                    var errorMessages = Object.values(errors).flat();
                    message = errorMessages.join("<br/> ");
                }

                Swal.fire({
                    icon: 'error',
                    title: 'Oops...',
                    html: message,
                });
            }

        });
    };

    this.PostToAPICommit = function (service, formData, callBackFunction) {
        $.ajax({
            type: "POST",
            url: this.GetUrlApiService(service),
            data: formData,
            processData: false,
            contentType: false,
            success: function (data) {
                if (callBackFunction) {
                    Swal.fire('Completado!', 'Transaction completed!');
                    callBackFunction(data);
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                var responseJson = jqXHR.responseJSON;
                var message = jqXHR.responseText;

                console.error("Error Status: ", textStatus);
                console.error("Error Thrown: ", errorThrown);
                console.error("Full Response: ", jqXHR);

                if (responseJson) {
                    var errors = responseJson.errors;
                    var errorMessages = Object.values(errors).flat();
                    message = errorMessages.join("<br/> ");
                }

                Swal.fire({
                    icon: 'error',
                    title: 'Oops...',
                    html: message
                });
            }

        });
    };

    this.PutToAPI = function (service, data, callBackFunction) {
        $.put(this.GetUrlApiService(service), data, function (response) {
            Swal.fire('Completado!', 'Transaction completed!');
            if (callBackFunction) {
                callBackFunction(response);
            }
        }).fail(function (response) {
            var message = response.responseText;
            if (response.responseJSON) {
                var data = response.responseJSON;
                var errors = data.errors;
                var errorMessages = Object.values(errors).flat();
                message = errorMessages.join("<br/> ");
            }

            Swal.fire({
                icon: 'error',
                title: 'Oops...',
                html: message,

            });
        });
    };

    this.DeleteToAPI = function (service, data, callBackFunction) {
        $.delete(this.GetUrlApiService(service), data, function (response) {
            Swal.fire('Completado!', 'Transaction completed!');
            if (callBackFunction) {
                callBackFunction(response);
            }
        }).fail(function (response) {
            var message = response.responseText;
            if (response.responseJSON) {
                var data = response.responseJSON;
                var errors = data.errors;
                var errorMessages = Object.values(errors).flat();
                message = errorMessages.join("<br/> ");
            }

            Swal.fire({
                icon: 'error',
                title: 'Oops...',
                html: message,

            });
        });
    };

    this.GetToApi = function (service, callBackFunction) {
        $.get(this.GetUrlApiService(service), function (response) {
            console.log("Response " + response);
            if (callBackFunction) {
                callBackFunction(response);
            }
        });
    }
}

// Custom jQuery actions
$.put = function (url, data, callback) {
    if ($.isFunction(data)) {
        type = type || callback,
            callback = data,
            data = {}
    }
    return $.ajax({
        url: url,
        type: 'PUT',
        success: callback,
        data: JSON.stringify(data),
        contentType: 'application/json'
    });
}

$.delete = function (url, data, callback) {
    if ($.isFunction(data)) {
        type = type || callback,
            callback = data,
            data = {}
    }
    return $.ajax({
        url: url,
        type: 'DELETE',
        success: callback,
        data: JSON.stringify(data),
        contentType: 'application/json'
    });
}

document.convertFormToJSON = function (form) {
    const array = $(form).serializeArray();
    const json = {};
    $.each(array, function () {
        json[this.name] = this.value !== '' ? this.value : null;
    });
    return json;
};

document.setFormData = function (jsonData, formName) {
    var form = document.forms[formName];
    if (!form) {
        console.error("El formulario especificado no existe.");
        return;
    }

    for (var key in jsonData) {
        if (jsonData.hasOwnProperty(key)) {
            var value = jsonData[key];
            var element = null;

            for (var i = 0; i < form.elements.length; i++) {
                if (form.elements[i].name.toLowerCase() === key.toLowerCase()) {
                    element = form.elements[i];
                    break;
                }
            }

            if (element) {
                if (element.tagName === "INPUT") {
                    if (element.type === "radio") {
                        var radioButtons = form.querySelectorAll('input[type="radio"][name="' + element.name + '"]');
                        radioButtons.forEach(function (radioButton) {
                            if (radioButton.value === value.toString()) {
                                radioButton.checked = true;
                            }
                        });
                    } else if (element.type === "date") {
                        element.valueAsDate = new Date(value);
                    } else if (element.type === "number") {
                        element.value = isNaN(value) ? null : parseFloat(value);
                    } else {
                        element.value = value;
                    }
                } else if (element.tagName === "SELECT") {
                    for (var i = 0; i < element.options.length; i++) {
                        if (element.options[i].value === value.toString()) {
                            element.selectedIndex = i;
                            break;
                        }
                    }
                } else {
                    console.warn("El elemento", key, "no es un input o select.");
                }
            } else {
                console.warn("No se encontró un elemento con el nombre", key, "en el formulario.");
            }
        }
    }
};