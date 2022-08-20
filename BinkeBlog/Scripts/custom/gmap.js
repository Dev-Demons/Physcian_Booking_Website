var gMarker = [];
var markers = [];
var pagefunction = function () {
    /*
     * Google Maps Initialize
     */
    var lat = 27.964157;
    var lng = -82.452606;

    //var $this = $("#googleMap");
    var $zoomLevel = ($("#googleMap").data("gmap-zoom") || 12);
    var $dataLat = ($("#googleMap").data("gmap-lat") || lat);
    var $dataLng = ($("#googleMap").data("gmap-lng") || lng);
    var $xmlSrc = ($("#googleMap").data("gmap-src") || "xml/gmap/pins.xml");

    function initializeMap() {
        var centerLatLng;
        if (jQuery.isEmptyObject(markers)) {
            centerLatLng = new window.google.maps.LatLng($dataLat, $dataLng);
        } else {
            centerLatLng = new window.google.maps.LatLng(markers[0].Lat, markers[0].Lng);
        }
        var mapOptions = {
            zoom: $zoomLevel,
            center: centerLatLng,
            mapTypeId: window.google.maps.MapTypeId.ROADMAP,
            position: window.google.maps.ControlPosition.LEFT_TOP,
            mapTypeControl: false,
            streetViewControl: false,
            fullscreenControl: false,
        };
        var map = new window.google.maps.Map(document.getElementById("googleMap"), mapOptions);
        var infoWindow = new window.google.maps.InfoWindow();


        var data;
        var myLatlng;
        var marker;
        if (jQuery.isEmptyObject(markers)) {
            var gMarker1 = [];
            data = [{ "lat": lat, "lng": lng, 'description': "Your Location" }];
            myLatlng = new window.google.maps.LatLng(data[0].lat, data[0].lng);
            marker = new window.google.maps.Marker({ position: myLatlng, map: map });
            gMarker1.push(marker);
            //Attach click event to the marker.
            (function (marker, data) {
                window.google.maps.event.addListener(marker, "click", function (e) {
                    //Wrap the content inside an HTML DIV in order to set height and width of InfoWindow.
                    infoWindow.setContent(data[0].description);
                    infoWindow.open(map, marker);
                });
            })(marker, data);
        } else {
            for (var i = 0; i < markers.length; i++) {
                data = markers[i];
                myLatlng = new window.google.maps.LatLng(data.lat, data.lng);
                marker = new window.google.maps.Marker({
                    position: myLatlng,
                    map: map
                });
                gMarker.push(marker);
                //Attach click event to the marker.
                (function (marker, data) {
                    window.google.maps.event.addListener(marker, "click", function (e) {
                        //Wrap the content inside an HTML DIV in order to set height and width of InfoWindow.
                        infoWindow.setContent(data.description);
                        infoWindow.open(map, marker);
                    });
                })(marker, data);
            }
        }

        var showMe = function (index) {
            gMarker[index].setAnimation(window.google.maps.Animation.BOUNCE);
            setTimeout(function () {
                gMarker[index].setAnimation(null);
            },
                4000);
        }

    } // end demoLoadAttribute

    initializeMap();
};

var geocoder;
var map;
var searchService;
var myLocation;
var directionsService = new google.maps.DirectionsService();
var directionsDisplay = new google.maps.DirectionsRenderer();

function initialize() {
    var map = new google.maps.Map(
        document.getElementById("googleMap"), {
            center: new google.maps.LatLng(37.4419, -122.1419),
            zoom: 12,
            mapTypeId: google.maps.MapTypeId.ROADMAP,
            position: window.google.maps.ControlPosition.LEFT_TOP,
            mapTypeControl: false,
            streetViewControl: false,
            fullscreenControl: false,
        });
    myLocation = map.getCenter();
    var marker = new google.maps.Marker({
        position: myLocation,
        map: map
    });
    searchService = new google.maps.places.PlacesService(map);
    directionsDisplay.setMap(map);
    var request = {
        location: myLocation,
        rankBy: google.maps.places.RankBy.DISTANCE,
        types: ['bar', 'cafe', 'food', 'liquor_store', 'lodging', 'meal_delivery', 'meal_takeaway', 'night_club', 'restaurant'],
        keyword: ['bar', 'pub']
    };
    searchService.nearbySearch(request, function (bars, status) {
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
                if (status == google.maps.DirectionsStatus.OK) {
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
google.maps.event.addDomListener(window, "load", initialize);

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