$("#MnuFacility").click(function () {
    window.location.href = "/Facility";
}); 
$("#FacilityMain").click(function () {
    window.location.href = "/Facility";
});

function AddSubmenu(orgId, isDashboardAdd) {
    if (isDashboardAdd)
        return;
    localStorage.setItem("organisationId", orgId);
    if (localStorage.getItem("submenuAdded") !== true) {
        AddSubmenuItems('Profile', isDashboardAdd);
        var url = "/FacilityBasicInformation/" + orgId;
        localStorage.setItem("submenuAdded", true);
        window.location.href = url;
    }
}

function AddSubmenuItems(menuName, isDashboardAdd) {
    if (isDashboardAdd)
        return;
    var linksToBeAdded = '<ul class="nav nav-treeview">';
    var organisationId = localStorage.getItem("organisationId") ? +localStorage.getItem("organisationId") : 0;
    if (menuName === "Profile") {
        linksToBeAdded += '<li class="nav-item"><a href="/FacilityBasicInformation/' + organisationId + '" class="nav-link"><i class="far fa-dot-circle-o nav-icon"></i><p>Profile</p></a></li>';
    } else {
        linksToBeAdded += '<li class="nav-item"><a href="/FacilityBasicInformation/' + organisationId + '" class="nav-link"><i class="far fa-circle nav-icon"></i><p>Profile</p></a></li>';
    }
    if (menuName === "Amenity") {
        linksToBeAdded += '<li class="nav-item"><a href="/Facility/AmenityOptions/' + organisationId + '" class="nav-link"><i class="far fa-dot-circle-o nav-icon"></i><p>Amenity/Options</p></a></li>';
    } else {
        linksToBeAdded += '<li class="nav-item"><a href="/Facility/AmenityOptions/' + organisationId + '" class="nav-link"><i class="far fa-circle nav-icon"></i><p>Amenity/Options</p></a></li>';
    }

    if (organisationId) {
        if (menuName === "Booking") {
            linksToBeAdded += '<li class="nav-item"><a href="/Facility/Booking" class="nav-link"><i class="far fa-dot-circle-o nav-icon"></i><p>Booking</p></a></li>';
        } else {
            linksToBeAdded += '<li class="nav-item"><a href="/Facility/Booking" class="nav-link"><i class="far fa-circle nav-icon"></i><p>Booking</p></a></li>';
        }

        if (menuName === "Reviews") {
            linksToBeAdded += '<li class="nav-item"><a href="/Facility/Reviews" class="nav-link"><i class="far fa-dot-circle-o nav-icon"></i><p>Reviews</p></a></li>';
        } else {
            linksToBeAdded += '<li class="nav-item"><a href="/Facility/Reviews" class="nav-link"><i class="far fa-circle nav-icon"></i><p>Reviews</p></a></li>';
        }

        if (menuName === "Specialities") {
            linksToBeAdded += '<li class="nav-item"><a href="/Facility/Specialities" class="nav-link"><i class="far fa-dot-circle-o nav-icon"></i><p>Specialities</p></a></li>';
        } else {
            linksToBeAdded += '<li class="nav-item"><a href="/Facility/Specialities" class="nav-link"><i class="far fa-circle nav-icon"></i><p>Specialities</p></a></li>';
        }

        if (menuName === "Summary") {
            linksToBeAdded += '<li class="nav-item"><a href="/Facility/Summary" class="nav-link"><i class="far fa-dot-circle-o nav-icon"></i><p>Summary</p></a></li>';
        } else {
            linksToBeAdded += '<li class="nav-item"><a href="/Facility/Summary" class="nav-link"><i class="far fa-circle nav-icon"></i><p>Summary</p></a></li>';
        }
        
        if (menuName === "StateLicense") {
            linksToBeAdded += '<li class="nav-item"><a href="/Facility/StateLicense" class="nav-link"><i class="far fa-dot-circle-o nav-icon"></i><p>State License</p></a></li>';
        } else {
            linksToBeAdded += '<li class="nav-item"><a href="/Facility/StateLicense" class="nav-link"><i class="far fa-circle nav-icon"></i><p>State License</p></a></li>';
        }

        if (menuName === "InsurancePlan") {
            linksToBeAdded += '<li class="nav-item"><a href="/Facility/InsurancePlan" class="nav-link"><i class="far fa-dot-circle-o nav-icon"></i><p>Insurance Plan</p></a></li>';
        } else {
            linksToBeAdded += '<li class="nav-item"><a href="/Facility/InsurancePlan" class="nav-link"><i class="far fa-circle nav-icon"></i><p>Insurance Plan</p></a></li>';
        }

        if (menuName === "Image") {
            linksToBeAdded += '<li class="nav-item"><a href="/Facility/Image" class="nav-link"><i class="far fa-dot-circle-o nav-icon"></i><p>Image</p></a></li>';
        } else {
            linksToBeAdded += '<li class="nav-item"><a href="/Facility/Image" class="nav-link"><i class="far fa-circle nav-icon"></i><p>Image</p></a></li>';
        }

        if (menuName === "OpeningHours") {
            linksToBeAdded += '<li class="nav-item"><a href="/Facility/OpeningHours" class="nav-link"><i class="far fa-dot-circle-o nav-icon"></i><p>Opening Hours</p></a></li>';
        } else {
            linksToBeAdded += '<li class="nav-item"><a href="/Facility/OpeningHours" class="nav-link"><i class="far fa-circle nav-icon"></i><p>Opening Hours</p></a></li>';
        }

        if (menuName === "SocialMedia") {
            linksToBeAdded += '<li class="nav-item"><a href="/Facility/SocialMedia" class="nav-link"><i class="far fa-dot-circle-o nav-icon"></i><p>Social Media</p></a></li>';
        } else {
            linksToBeAdded += '<li class="nav-item"><a href="/Facility/SocialMedia" class="nav-link"><i class="far fa-circle nav-icon"></i><p>Social Media</p></a></li>';
        }

        if (menuName === "Taxonomy") {
            linksToBeAdded += '<li class="nav-item"><a href="/Facility/Taxonomy" class="nav-link"><i class="far fa-dot-circle-o nav-icon"></i><p>Taxonomy</p></a></li>';
        } else {
            linksToBeAdded += '<li class="nav-item"><a href="/Facility/Taxonomy" class="nav-link"><i class="far fa-circle nav-icon"></i><p>Taxonomy</p></a></li>';
        }

        if (menuName === "Addresses") {
            linksToBeAdded += '<li class="nav-item"><a href="/Facility/Addresses" class="nav-link"><i class="far fa-dot-circle-o nav-icon"></i><p>Addresses</p></a></li>';
        } else {
            linksToBeAdded += '<li class="nav-item"><a href="/Facility/Addresses" class="nav-link"><i class="far fa-circle nav-icon"></i><p>Addresses</p></a></li>';
        }
        if (menuName === "Cost") {
            linksToBeAdded += '<li class="nav-item"><a href="/Facility/Cost/' + organisationId + '" class="nav-link"><i class="far fa-dot-circle-o nav-icon"></i><p>Cost</p></a></li>';
        } else {
            linksToBeAdded += '<li class="nav-item"><a href="/Facility/Cost/' + organisationId + '" class="nav-link"><i class="far fa-circle nav-icon"></i><p>Cost</p></a></li>';
        }

        //if (isDashboardAdd) {
        //    if (menuName == "Dashboard") { linksToBeAdded += '<li class="nav-item"><a href="/Dashboard/Facility/" class="nav-link"><i class="far fa-dot-circle-o nav-icon"></i><p>Dashboard</p></a></li>'; }
        //    else { linksToBeAdded += '<li class="nav-item"><a href="/Dashboard/Facility/" class="nav-link"><i class="far fa-circle nav-icon"></i><p>Dashboard</p></a></li>'; }
        //}
    }

    linksToBeAdded += '</ul>';
    $("#MnuFacility").append(linksToBeAdded);
    $('#MnuFacility').addClass("active");
    $('#MnuFacility').addClass("open");
    $('#MnuFacility').addClass("menu-open");
}