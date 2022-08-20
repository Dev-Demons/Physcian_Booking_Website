$("#MnuPatient").click(function () {
    window.location.href = "/Patient";
});
$("#PatientMain").click(function () {
    window.location.href = "/Patient";
});

function AddPatientSubMenu(activeMenuName) {
    var linksToBeAdded = '<ul class="nav nav-treeview">';
    var activePatinetId = sessionStorage.getItem("CurrentPatientId");
    if (activeMenuName === "Profile") {
        linksToBeAdded += '<li class="nav-item"><a href="/Patient/Profile/' + activePatinetId + '" class="nav-link"><i class="far fa-dot-circle-o nav-icon"></i><p>Profile</p></a></li>';
    } else {
        linksToBeAdded += '<li class="nav-item"><a href="/Patient/Profile/' + activePatinetId + '" class="nav-link"><i class="far fa-circle nav-icon"></i><p>Profile</p></a></li>';
    }

    if (activeMenuName === "Booking") {
        linksToBeAdded += '<li class="nav-item"><a href="/Patient/Booking/" class="nav-link"><i class="far fa-dot-circle-o nav-icon"></i><p>Booking</p></a></li>';
    } else {
        linksToBeAdded += '<li class="nav-item"><a href="/Patient/Booking/" class="nav-link"><i class="far fa-circle nav-icon"></i><p>Booking</p></a></li>';
    }

    if (activeMenuName === "Orders") {
        linksToBeAdded += '<li class="nav-item"><a href="/Patient/Orders/" class="nav-link"><i class="far fa-dot-circle-o nav-icon"></i><p>Orders</p></a></li>';
    } else {
        linksToBeAdded += '<li class="nav-item"><a href="/Patient/Orders/" class="nav-link"><i class="far fa-circle nav-icon"></i><p>Orders</p></a></li>';
    }

    linksToBeAdded += '</ul>';
    $("#MnuPatient").append(linksToBeAdded);
    $('#MnuPatient').addClass("active");
    $('#MnuPatient').addClass("open");
    $('#MnuPatient').addClass("menu-open");
}