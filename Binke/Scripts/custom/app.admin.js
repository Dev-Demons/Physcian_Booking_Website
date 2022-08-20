function loadRegisterForm(userType) {
    $("#modalContent").load("/Account/RegisterPartial?userType=" + userType, function () {
        $("#divModal .modal-dialog").addClass("modal-lg");
        $("#myModalLabel").text("Register " + userType);
        $("#divModal").modal("show");
    });
}
function defaultSelect2() {
    $('.select2').select2();
}
function datatableDefaultSettings() {
    /* // DOM Position key index //

        l - Length changing (dropdown)
        f - Filtering input (search)
        t - The Table! (datatable)
        i - Information (records)
        p - Pagination (paging)
        r - pRocessing
        < and > - div elements
        <"#id" and > - div with an id
        <"class" and > - div with a class
        <"#id.class" and > - div with an id and class

        Also see: http://legacy.datatables.net/usage/features
        */
    $.extend(true, $.fn.dataTable.defaults, {

        "sDom": "<'dt-toolbar'<'col-xs-12 col-sm-6'f><'col-sm-6 col-xs-12 hidden-xs'l>>" +
            //"r t" +
            "t" +
            "<'dt-toolbar-footer'<'col-sm-6 col-xs-12 hidden-xs'i><'col-xs-12 col-sm-6'p>>",
        //"oLanguage": {
        //    "sSearch": '<span class="input-group-addon"><i class="glyphicon glyphicon-search"></i></span>'
        //},
        "language": {
            "emptyTable": "No record found."
            //"processing": '<i class="fa fa-spinner fa-spin fa-3x fa-fw"></i><span class="sr-only">Loading...</span> '
        },
        "columnDefs": [
            { "className": "text-center", "targets": "_all" }
        ],
        "responsive": true,
        "searching": true,
        "autoWidth": true,
        "bServerSide": true,
        "bSearchable": true,
        "bProcessing": true,
        "ordering": true,
        "bDestroy": true
    });

    //var tables = $.fn.dataTable.tables(true);

    $(document).on("change", ".dataTables_filter input", function () {
        var searchValue = $(this).val();
        var parentTableId = $(this).parents(".dataTables_wrapper").find("table").attr("id");
        $('#' + parentTableId).DataTable().search(searchValue).draw();
    });
}

function formatDate(dateVal) {
    var newDate = new Date(dateVal);

    var sMonth = padValue(newDate.getMonth() + 1);
    var sDay = padValue(newDate.getDate());
    var sYear = newDate.getFullYear();
    var sHour = newDate.getHours();
    var sMinute = padValue(newDate.getMinutes());
    var sAmpm = "AM";

    var iHourCheck = parseInt(sHour);

    if (iHourCheck > 12) {
        sAmpm = "PM";
        sHour = iHourCheck - 12;
    }
    else if (iHourCheck === 0) {
        sHour = "12";
    }

    sHour = padValue(sHour);

    return sMonth + "-" + sDay + "-" + sYear + " " + sHour + ":" + sMinute + " " + sAmpm;
}

function padValue(value) {
    return (value < 10) ? "0" + value : value;
}

function isExistModal(callMe, id) {
    if ((id === 0 || id === "")
        && $("#modalContent").html() !== ""
        && ($("#modalContent").attr("customHidden").val() === 0 || $("#modalContent").attr("customHidden").val() === "")
    ) {
        $("#divModal").modal("show");
    } else {
        callMe();
    }

    //if ($("#modalContent").html() === "") {
    //    callMe();
    //} else {
    //    $("#divModal").modal("show");
    //}
}

function getRandomInt(min, max) {
    return Math.floor(Math.random() * (max - min + 1)) + min;
}

function componentToHex(c) {
    var hex = c.toString(16);
    return hex.length === 1 ? "0" + hex : hex;
}

function toSystemDateFormate(date) {
    var newDate = window.moment(date, "MM/DD/YYYY hh:mm A").toDate();
    var result = window.moment(newDate).format("MM/DD/YYYY hh:mm A");
    return date + "|" + newDate;
}
