function GetCityList(id) {
    return $.ajax({
        url: "/Home/GetCityList/" + id,
        type: "GET",
        async: false,
        dataType: "json",
        traditional: true,
        contentType: "application/json; charset=utf-8"
    });
}

function DisplayErrors(errors) {
    for (var i = 0; i < errors.length; i++) {
        $("<label for='" + errors[i].Key + "' class='error'></label>")
            .html(errors[i].Value[0]).appendTo($("input#" + errors[i].Key).parent());
    }
}

function remove(array, element) {
    var index = array.indexOf(element);
    array.splice(index, 1);
}

function showAlert(alert) {
    window.toastr.options = {
        closeButton: false,
        debug: false,
        newestOnTop: true,
        positionClass: "toast-top-right",
        preventDuplicates: true,
        onclick: null,
        showDuration: 200,
        hideDuration: 1000,
        timeOut: 2000,
        extendedTimeOut: 1000,
        showEasing: "swing",
        hideEasing: "linear",
        showMethod: "fadeIn",
        hideMethod: "fadeOut"
    };
    window.toastr[alert.AlertType](alert.Message, alert.Title !== "" ? alert.Title : ""); // Wire up an event handler to a button in the toast, if it exists
}

function closeModal() {
    $("#modalContent").html("");
    $("#divModal").modal("hide");
}

function GetAlertResponse(Status, Message, Title = "", Data = []) {
    var alert = { AlertType: Status, Message: Message, Title: Title, Data: Data };
    return HandleResponse(alert);
}

function HandleResponse(result) {
    if (result.Status === 4) {
        DisplayErrors(result.Data);
        return false;
    }
    var alert = { AlertType: result.Status, Message: result.Message, Title: "" };
    switch (result.Status) {
        case 0:
            alert.AlertType = "error";
            break;
        case 1:
            alert.AlertType = "success";
            break;
        case 2:
            alert.AlertType = "warning";
            break;
        case 3:
            alert.AlertType = "info";
            break;
        default:
            alert.AlertType = "error";
    }
    showAlert(alert);
    return result.Data;
}

function getCookie(cname) {
    var name = cname + "=";
    var decodedCookie = decodeURIComponent(document.cookie);
    var ca = decodedCookie.split(";");
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) === " ") {
            c = c.substring(1);
        }
        if (c.indexOf(name) === 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}

function geturlId(url) {
    return url.substring(url.lastIndexOf("/") + 1);
}

function getUrlFields(url, noOf) {
    var all = url.split("/");
    if (all.length >= noOf) {
        return all[noOf];
    } else {
        return "";
    }
}

function returnValue(arrayList, value) {
    var result = 0;
    $.each(arrayList, function (key, val) {
        if (val.Text === value)
            result = parseInt(val.Value);
    });
    return result;
}

function validateUrl(value) {
    return /^(ftp|https?):\/\/+(www\.)?[a-z0-9\-\.]{3,}\.[a-z]{3}$/.test(value);
    //return /^(?:(?:(?:https?|ftp):)?\/\/)(?:\S+(?::\S*)?@)?(?:(?!(?:10|127)(?:\.\d{1,3}){3})(?!(?:169\.254|192\.168)(?:\.\d{1,3}){2})(?!172\.(?:1[6-9]|2\d|3[0-1])(?:\.\d{1,3}){2})(?:[1-9]\d?|1\d\d|2[01]\d|22[0-3])(?:\.(?:1?\d{1,2}|2[0-4]\d|25[0-5])){2}(?:\.(?:[1-9]\d?|1\d\d|2[0-4]\d|25[0-4]))|(?:(?:[a-z\u00a1-\uffff0-9]-*)*[a-z\u00a1-\uffff0-9]+)(?:\.(?:[a-z\u00a1-\uffff0-9]-*)*[a-z\u00a1-\uffff0-9]+)*(?:\.(?:[a-z\u00a1-\uffff]{2,})))(?::\d{2,5})?(?:[/?#]\S*)?$/i.test(value);
}

function blockElement(selector) {
    $(selector).block({
        message: '<img src = "/Content/client/images/ajax-loader.gif" class="block-image" /><span class="block-message">Processing...</span>',
        css: {
            width: "auto",
            border: "none !important",
            "border-radius": "5px",
            padding: "6px",
            left: "45.2%"
        }
    });
}

function unblockElement(selector) {
    $(selector).unblock();
}

function blockPage() {
    $.blockUI(
        {
            baseZ: 9999,
            message: '<img src = "/Content/client/images/ajax-loader.gif" class="block-image"/><span class="block-message">Processing...</span>',
            css: {
                width: "auto",
                border: "none !important", "border-radius": "5px",
                padding: "6px",
                left: "45.2%"
            }
        });
}

function unblockPage() {
    $.unblockUI();
}

function getRandomInt(min, max) {
    return Math.floor(Math.random() * (max - min + 1)) + min;
}

function componentToHex(c) {
    var hex = c.toString(16);
    return hex.length === 1 ? "0" + hex : hex;
}

function getRandomColor() {

    //generate random red, green and blue intensity
    var r = getRandomInt(0, 255);
    var g = getRandomInt(0, 255);
    var b = getRandomInt(0, 255);

    var color = "#" + componentToHex(r) + componentToHex(g) + componentToHex(b);
    //alert(color);
    return color;

}

String.prototype.ucfirst = function () {
    return this.charAt(0).toUpperCase() + this.substr(1);
}

String.prototype.CapitalizeFirstLetter = function () {
    return this.charAt(0).toUpperCase() + this.slice(1);
}

$.fn.hasAnyClass = function () {
    for (var i = 0; i < arguments.length; i++) {
        if (this.hasClass(arguments[i])) {
            return true;
        }
    }
    return false;
}

function IsNullOrEmpty(value) {
    return (!value || value == undefined || value == "" || value.length == 0);
}