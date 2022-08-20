
// Global variables to be called in all functions, to update the map status when call variant functions
var map;
var placesService;
var geocoderService;
var myLocation;
var myAreaName;
var infoWindow;
var markers = [];
var directionsService = new google.maps.DirectionsService();
var directionsDisplay = new google.maps.DirectionsRenderer();
var geocoder = new google.maps.Geocoder();

//var geoCodeReuest = {
//    //address: "200 Lothrop St.",
//    componentRestrictions: { postalCode: "15213-2582" }
//};

// Create the map object, 
// focus current user position if he / she allowed,
// and instantiate PlacesService object
function initializeMap(mapElementId) {
    var centerPoint;
    if (myLocation)
        centerPoint = new google.maps.LatLng(myLocation.lat, myLocation.lng);
    else
        centerPoint = new google.maps.LatLng(39.305, -76.617);

    infoWindow = new google.maps.InfoWindow();
    map = new google.maps.Map(
        document.getElementById(mapElementId), {
            center: centerPoint,
            zoom: 12,
            mapTypeId: google.maps.MapTypeId.ROADMAP,
            position: window.google.maps.ControlPosition.LEFT_TOP,
            mapTypeControl: false,
            streetViewControl: false,
            fullscreenControl: false
        });
    getUserPosition();
    placesService = new google.maps.places.PlacesService(map);
    geocoderService = new google.maps.Geocoder(map);

    //directionsDisplay.setMap(map);
    //var request = {
    //    location: myLocation,
    //    rankBy: google.maps.places.RankBy.DISTANCE,
    //    types: ['bar', 'cafe', 'food', 'liquor_store', 'lodging', 'meal_delivery', 'meal_takeaway', 'night_club', 'restaurant'],
    //    keyword: ['bar', 'pub']
    //};
    //nearbySearch(request); // stopped because it return the focus on current user location instead of doctors list locations
}

function testCalculateDistance() {
    // Define two points using LatLng objects
    var point_a = new google.maps.LatLng(44.037307, -123.110091);
    var point_b = new google.maps.LatLng(44.034851, -123.099449);

    // Get the distance between the two points in miles
    var distance_miles = getDistanceInMiles(point_a, point_b);

    // Show the distance, rounded to 2 decimals
    alert("The distance between point_a and point_b is: \n\n" + distance_miles.toFixed(2) + " miles");

    // Get distance in miles. The API returns distance in meters, and we can easily convert that with multiplication.
    function getDistanceInMiles(point_a, point_b) {
        var distance_in_meters = google.maps.geometry.spherical.computeDistanceBetween(point_a, point_b);
        return distance_in_meters * 0.000621371; // convert meters to miles
    }
}

function getCurrentLatLong(callback, error) {
    if (navigator.geolocation == undefined) {
        if (error)
            error();
        if (map)
            handleLocationError(true, map.getCenter());
    }

    if (myLocation == undefined) {
        navigator.geolocation.getCurrentPosition(function (position) {
            var pos = {
                lat: position.coords.latitude,
                lng: position.coords.longitude
            };

            myLocation = pos;
            if (callback)
                callback(pos);
        }, function () {
            if (error)
                error();
        }, {
                enableHighAccuracy: true
            });
    }
}

// Get current user position and set it into myLocation variable
function getUserPosition() {
    getCurrentLatLong(function (pos) {
        map.setCenter(pos);
    }, function (error) {
        handleLocationError(true, map.getCenter());
    });
}

// handle getting current user position's error
function handleLocationError(browserHasGeolocation, pos) {
    infoWindow.setPosition(pos);
    infoWindow.setContent(browserHasGeolocation ?
        'Error: The Geolocation service failed.' :
        'Error: Your browser doesn\'t support geolocation.');
    infoWindow.open(map);
}

// Search palces by address, city, or location object and add marker on the result found
function searchPlace_AddMarker(palceReuest, docotrInfo) {
    placesService.findPlaceFromQuery(palceReuest, function (results, status) {
        if (status === google.maps.places.PlacesServiceStatus.OK) {
            for (var i = 0; i < results.length; i++) {
                createMarker(results[i], docotrInfo);
                if (i === results.length - 1)
                    map.setCenter(results[i].geometry.location);
            }
        }
    });
}

// Search palces by zip code and add marker on the result found
function searchZipCode_AddMarker(geoCodeReuest, docotrInfo) {
    geocoderService.geocode(geoCodeReuest, function (results, status) {
        if (status === google.maps.GeocoderStatus.OK) {
            for (var i = 0; i < results.length; i++) {
                createMarker(results[i], docotrInfo);
                if (i === results.length - 1)
                    map.setCenter(results[i].geometry.location);
            }
        }
    });

}

// Add marker on sent location parameter
function createMarker(place, docotrInfo) {
    console.log(docotrInfo);
    var marker = new google.maps.Marker({
        map: map,
        position: place.geometry.location
    });
    google.maps.event.addListener(marker, 'click', function () {
        infoWindow.setContent(docotrInfo.fullName + " - " + docotrInfo.phoneNumber);
        infoWindow.open(map, this);
    });
}

function getInfoButtonHTML(mapModel) {
    var ratingHtmlStr = mapModel.reviewHtml.replace(new RegExp('"', 'g'), '\'');
    ratingHtmlStr = ratingHtmlStr.replace(new RegExp('text-white', 'g'), 'text-grey');
    var finalRatingStr = $(ratingHtmlStr).append("<li>(" + mapModel.reviewCount + " Reviews)</li>")[0].outerHTML;

    var contentHtml =
        "<div class='card map-container'>" +
        "	<div class='row no-gutters'>" +
        "		<div class='col-auto'>" +
        "			<img src='" + mapModel.profileImage + "' class='img-fluid' alt=''>" +
        "		</div>" +
        "		<div class='col'>" +
        "			<div class='card-block px-2'>" +
        "				<h4 class='card-title'><a href='" + mapModel.profileUrl.split(' ').join('').split('&').join('') + "'>" + mapModel.name + "</a></h4>" +
        "               " + finalRatingStr + "" +
        "				<p class='card-text'>" + mapModel.fullAddress + "</p>" +
        "			</div>" +
        "           ##ButtonHTML##" +
        "		</div>" +
        "	</div>" +
        //"	<div class='card-footer w-100 text-muted'>" +
        //"		" + ratingHtmlStr + "" +
        //"	</div> " +
        "</div> ";

    if (mapModel.isAcceptButton) {
        var buttonString =
            "			<div class='card-block px-2 card-button'>" +
            "				<a href='" + mapModel.profileUrl.split(' ').join('').split('&').join('') + "' class='btn btn-primary'>Accepting New Patient</a>" +
            "			</div>";

        contentHtml = contentHtml.replace(new RegExp('##ButtonHTML##', 'g'), buttonString);
    } else {
        contentHtml = contentHtml.replace(new RegExp('##ButtonHTML##', 'g'), "");
    }
    return contentHtml;
}
var latlngbounds = new google.maps.LatLngBounds();

function LoadMap() {
    var mapOptions = {
        center: new google.maps.LatLng(markers[0].lat, markers[0].lng),
        // zoom: 8, //Not required.
        mapTypeId: google.maps.MapTypeId.ROADMAP
    };
    var infoWindow = new google.maps.InfoWindow();
    var map = new google.maps.Map(document.getElementById("dvMap"), mapOptions);

    //Create LatLngBounds object.

    for (var i = 0; i < markers.length; i++) {
        var data = markers[i]
        var myLatlng = new google.maps.LatLng(data.lat, data.lng);
        var marker = new google.maps.Marker({
            position: myLatlng,
            map: map,
            title: data.title
        });
        (function (marker, data) {
            google.maps.event.addListener(marker, "click", function (e) {
                infoWindow.setContent("<div style = 'width:200px;min-height:40px'>" + data.description + "</div>");
                infoWindow.open(map, marker);
            });
        })(marker, data);

        //Extend each marker's position in LatLngBounds object.
        latlngbounds.extend(marker.position);
    }

    //Get the boundaries of the Map.
    var bounds = new google.maps.LatLngBounds();

    //Center map and adjust Zoom based on the position of all markers.
    map.setCenter(latlngbounds.getCenter());
    map.fitBounds(latlngbounds);
}

function createMarkerBasedOnLatLong(mapModel) {
    var contentHtml = getInfoButtonHTML(mapModel);
    if (mapModel.latitude && mapModel.longitude) {
        var point = new google.maps.LatLng(parseFloat(mapModel.latitude), parseFloat(mapModel.longitude));
        var marker = new google.maps.Marker({
            map: map,
            position: point
        });
        google.maps.event.addListener(marker, 'click', function () {
            infoWindow.setContent(contentHtml);
            infoWindow.open(map, this);
        });
    }
}

function setMarkerByAddress(mapModel, recordCount) {
    var contentHtml = getInfoButtonHTML(mapModel);
    geocoder.geocode({
        'address': mapModel.fullAddress
    }, function (results, status) {
        if (status == google.maps.GeocoderStatus.OK) {
            if (status != google.maps.GeocoderStatus.ZERO_RESULTS) {
                map.setCenter(results[0].geometry.location);

                var locationInfoWindow = new google.maps.InfoWindow({
                    content: contentHtml,
                    size: new google.maps.Size(150, 50)
                });

                var marker = new google.maps.Marker({
                    position: results[0].geometry.location,
                    map: map,
                    title: mapModel.fullAddress,
                    infowindow: locationInfoWindow
                });
                markers.push(marker);

                google.maps.event.addListener(marker, 'click', function () {
                    hideAllInfoWindows(map);
                    this.infowindow.open(map, this);
                });
                latlngbounds.extend(marker.position);

                setTimeout(function () {
                    map.setCenter(latlngbounds.getCenter());
                    map.fitBounds(latlngbounds);
                });
            } else {
                console.log("No results found");
            }
        } else {
            console.log("Geocode was not successful for the following reason: " + status);
        }
    });
}

function hideAllInfoWindows(map) {
    markers.forEach(function (marker) {
        marker.infowindow.close(map, marker);
    });
}

function setMapOnAll(map) {
    for (var i = 0; i < markers.length; i++) {
        markers[i].setMap(map);
    }
}

function clearMarkers() {
    setMapOnAll(null);
}

function setMarkerToCurrentLocation() {
    if (myLocation) {
        var myOptions = {
            zoom: 15,
            center: myLocation,
            mapTypeId: google.maps.MapTypeId.ROADMAP
        };
        var map = new google.maps.Map(document.getElementById("googleMapTop"), myOptions);
        var marker = new google.maps.Marker({
            position: myLocation
        });
        marker.setMap(map);
    }
}

function nearbySearch(request) {
    placesService.nearbySearch(request, function (bars, status) {
        if (status === google.maps.places.PlacesServiceStatus.OK) {
            var barMark = new google.maps.Marker({
                position: bars[0].geometry.location,
                map: map,
                icon: {
                    url: "https://maps.gstatic.com/intl/en_us/mapfiles/markers2/measle.png",
                    size: new google.maps.Size(7, 7),
                    anchor: new google.maps.Point(3.5, 3.5)
                }
            });
            var request = {
                origin: myLocation,
                destination: bars[0].geometry.location,
                travelMode: google.maps.TravelMode.WALKING
            };
            directionsService.route(request, function (response, status) {
                if (status === google.maps.DirectionsStatus.OK) {
                    directionsDisplay.setDirections(response);
                    directionsDisplay.setOptions({
                        suppressMarkers: true,
                        preserveViewport: true
                    });
                    var polyline = getPolyline(response);
                    map.setCenter(polyline.getPath().getAt(polyline.getPath().getLength() - 1));
                    map.setZoom(19);

                    var lineLength = google.maps.geometry.spherical.computeDistanceBetween(bars[0].geometry.location, polyline.getPath().getAt(polyline.getPath().getLength() - 1));
                    var lineHeading = google.maps.geometry.spherical.computeHeading(bars[0].geometry.location, polyline.getPath().getAt(polyline.getPath().getLength() - 1));
                    var markerO = new google.maps.Marker({
                        position: google.maps.geometry.spherical.computeOffset(bars[0].geometry.location, lineLength * 0.1, lineHeading)
                    });
                    var markerD = new google.maps.Marker({
                        position: google.maps.geometry.spherical.computeOffset(bars[0].geometry.location, lineLength * 0.9, lineHeading)
                    });

                    var markerA = new google.maps.Marker({
                        position: google.maps.geometry.spherical.computeOffset(markerO.getPosition(), lineLength / 3, lineHeading - 40)
                    });
                    var markerB = new google.maps.Marker({
                        position: google.maps.geometry.spherical.computeOffset(markerD.getPosition(), lineLength / 3, lineHeading - 140)
                    });

                    var curvedLine = new GmapsCubicBezier(markerO.getPosition(), markerA.getPosition(), markerB.getPosition(), markerD.getPosition(), 0.01, map);

                    var line = new google.maps.Polyline({
                        path: [bars[0].geometry.location, polyline.getPath().getAt(polyline.getPath().getLength() - 1)],
                        strokeOpacity: 0,
                        icons: [{
                            icon: {
                                path: 'M 0,-1 0,1',
                                strokeOpacity: 1,
                                scale: 4
                            },
                            offset: '0',
                            repeat: '20px'
                        }],
                        // map: map
                    });
                } else {
                    console.log("directionsService : " + status);
                }
            });
        }
    });
}

function getAreaName(lat, lng) {
    var latlng = new google.maps.LatLng(lat, lng);
    geocoder.geocode({ 'latLng': latlng }, function (results, status) {
        if (status == google.maps.GeocoderStatus.OK) {
            console.log(results)
            if (results[1]) {
                //formatted address
                console.log(results[0].formatted_address);
                //find country name
                for (var i = 0; i < results[0].address_components.length; i++) {
                    for (var b = 0; b < results[0].address_components[i].types.length; b++) {
                        //there are different types that might hold a city admin_area_lvl_1 usually does in come cases looking for sublocality type will be more appropriate
                        if (results[0].address_components[i].types[b] == "administrative_area_level_1") {
                            //this is the object you are looking for
                            myAreaName = results[0].address_components[i];
                            break;
                        }
                    }
                }
                console.log(myAreaName.short_name + " " + myAreaName.long_name);
            } else {
                console.log("No results found");
            }
        } else {
            console.log("Geocoder failed due to: " + status);
        }
    });
}

function getPolyline(result) {
    var polyline = new google.maps.Polyline({
        path: []
    });
    var path = result.routes[0].overview_path;
    var legs = result.routes[0].legs;
    for (i = 0; i < legs.length; i++) {
        var steps = legs[i].steps;
        for (j = 0; j < steps.length; j++) {
            var nextSegment = steps[j].path;
            for (k = 0; k < nextSegment.length; k++) {
                polyline.getPath().push(nextSegment[k]);
            }
        }
    }
    return polyline;
}

var GmapsCubicBezier = function (latlong1, latlong2, latlong3, latlong4, resolution, map) {
    var lat1 = latlong1.lat();
    var long1 = latlong1.lng();
    var lat2 = latlong2.lat();
    var long2 = latlong2.lng();
    var lat3 = latlong3.lat();
    var long3 = latlong3.lng();
    var lat4 = latlong4.lat();
    var long4 = latlong4.lng();

    var points = [];

    for (it = 0; it <= 1; it += resolution) {
        points.push(this.getBezier({
            x: lat1,
            y: long1
        },
            {
                x: lat2,
                y: long2
            }, {
                x: lat3,
                y: long3
            }, {
                x: lat4,
                y: long4
            }, it));
    }
    var path = [];
    for (var i = 0; i < points.length - 1; i++) {
        path.push(new google.maps.LatLng(points[i].x, points[i].y));
        path.push(new google.maps.LatLng(points[i + 1].x, points[i + 1].y, false));
    }

    var Line = new google.maps.Polyline({
        path: path,
        geodesic: true,
        strokeOpacity: 0.0,
        icons: [{
            icon: {
                path: 'M 0,-1 0,1',
                strokeOpacity: 1,
                scale: 4
            },
            offset: '0',
            repeat: '20px'
        }],
        strokeColor: 'grey'
    });

    Line.setMap(map);

    return Line;
};

GmapsCubicBezier.prototype = {
    B1: function (t) {
        return t * t * t;
    },
    B2: function (t) {
        return 3 * t * t * (1 - t);
    },
    B3: function (t) {
        return 3 * t * (1 - t) * (1 - t);
    },
    B4: function (t) {
        return (1 - t) * (1 - t) * (1 - t);
    },
    getBezier: function (C1, C2, C3, C4, percent) {
        var pos = {};
        pos.x = C1.x * this.B1(percent) + C2.x * this.B2(percent) + C3.x * this.B3(percent) + C4.x * this.B4(percent);
        pos.y = C1.y * this.B1(percent) + C2.y * this.B2(percent) + C3.y * this.B3(percent) + C4.y * this.B4(percent);
        return pos;
    }
};