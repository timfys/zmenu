window.addEventListener('keydown', function (e) { if (e.keyIdentifier == 'U+000A' || e.keyIdentifier == 'Enter' || e.keyCode == 13) { if (e.target.nodeName == 'INPUT' && e.target.type == 'text') { e.preventDefault(); return false; } } }, true);

var geocoder;
var map;
var infoWindow;

$(document).ready(function () {
    $("#btnBackSignUp").click(function () {
        window.location.href = window.location.href.substring(0, location.href.lastIndexOf("/") + 1).replace("Customers/", "") + "Login";
    });

    $("#btnBackSignUp2").click(function () {
        var entityId = $("#EntityId").val();
        window.location.href = window.location.href.substring(0, location.href.lastIndexOf("/") + 1) + "SignUp?eID=" + entityId;
    });

    $("#btnBackSignUp3").click(function () {
        var entityId = $("#EntityId").val();
        window.location.href = window.location.href.substring(0, location.href.lastIndexOf("/") + 1) + "SignUp2?eID=" + entityId;
    })

    $('#img').change(function () {
        $("#step-1-danger").addClass("d-none");
        $("#step-1-danger-size").addClass('d-none');
        $("#step-1-danger-extension").addClass('d-none');

        var file = document.getElementById("img").files[0];
        var validExtensions = ['jpeg', 'png', 'jpg', 'JPEG', 'PNG', 'JPG'];
        var fileName = file.name;
        var fileNameExt = fileName.substr(fileName.lastIndexOf('.') + 1);
        if ($.inArray(fileNameExt, validExtensions) == -1) {
            $("#step-1-danger-extension").removeClass("d-none");
            $('#profileImage').attr('src', "../img/user.png");
            return;
        }

        if (file.size > (10 * 1024 * 1024)) {
            $("#step-1-danger-size").removeClass("d-none");
            $('#profileImage').attr('src', "../img/user.png");
            return;
        }

        $("#ProfilePicture").val(fileName);
        if (file) {
            var reader = new FileReader();
            reader.onload = function (e) {
                $('#profileImage').attr('src', e.target.result);
            }
            reader.readAsDataURL(file);
        }

        $("#IsChangeImage").val(true);
    });

    $('#btnNext').click(function () {
        $("#step-1-danger").addClass("d-none");
        $("#step-1-danger-size").addClass('d-none');
        $("#step-1-danger-extension").addClass('d-none');

        if ($("#IsImageUploaded").val() == "False") {
            if (document.getElementById("img").files[0] == undefined) {
                $("#step-1-danger").removeClass('d-none');
                $('html, body').animate({
                    scrollTop: parseInt($("#step-1-danger").offset().top - 10)
                }, 250);
                $('#profileImage').attr('src', "../img/user.png");
                return false;
            }
        }
    });

    $("#btnClearPhoto").click(function () {
        $.ajax({
            type: "POST",
            beforeSend: function () {
                LoadingStart();
            },
            url: "/Customers/ClearProfileImage",
            data: "{ entityId:" + $("#EntityId").val() + "}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                if (data.ResultCode == 1) {
                    $('#profileImage').attr('src', "../img/user.png");
                    //getNewImage();
                }
            },
            error: function (error) { alert(error.statusText); },
            complete: function () {
                LoadingEnd();
            }
        });
    })
})


var componentForm = {
    locality: 'long_name',
    administrative_area_level_1: 'long_name',
    administrative_area_level_2: 'long_name',
    postal_code: 'short_name',
    country: 'long_name'
};

function initMap() {
    geocoder = new google.maps.Geocoder;
    map = new google.maps.Map(document.getElementById('map'), {
        center: { lat: -34.397, lng: 150.644 },
        zoom: 15
    });
    infoWindow = new google.maps.InfoWindow({ map: map });

    search();
    codeAddress();
}

function handleLocationError(browserHasGeolocation, infoWindow, pos) {
    infoWindow.setPosition(pos);
    infoWindow.setContent(browserHasGeolocation ?
        'Error: The Geolocation service failed.' :
        'Error: Your browser doesn\'t support geolocation.');
    document.getElementById('btnHomeAddress').className = 'hidden';
}

function homeAddress() {
    // Try HTML5 geolocation.
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function (position) {
            var pos = {
                lat: position.coords.latitude,
                lng: position.coords.longitude
            };

            infoWindow.setPosition(pos);
            infoWindow.setContent('Location found.');
            geocodeLatLng(pos);
            map.setCenter(pos);
        }, function () {
            handleLocationError(true, infoWindow, map.getCenter());
        });
    } else {
        // Browser doesn't support Geolocation
        handleLocationError(false, infoWindow, map.getCenter());
    }
}

function search() {
    var input = document.getElementById('adr');
    var searchBox = new google.maps.places.SearchBox(input);
    //map.controls[google.maps.ControlPosition.TOP_LEFT].push(input);

    // Bias the SearchBox results towards current map's viewport.
    map.addListener('bounds_changed', function () {
        searchBox.setBounds(map.getBounds());
    });

    var markers = [];
    // Listen for the event fired when the user selects a prediction and retrieve
    // more details for that place.
    searchBox.addListener('places_changed', function () {
        var places = searchBox.getPlaces();

        if (places.length == 0) {
            return;
        }

        // Clear out the old markers.
        markers.forEach(function (marker) {
            marker.setMap(null);
        });
        markers = [];

        // For each place, get the icon, name and location.
        var bounds = new google.maps.LatLngBounds();
        places.forEach(function (place) {
            if (!place.geometry) {
                console.log("Returned place contains no geometry");
                return;
            }
            var icon = {
                url: place.icon,
                size: new google.maps.Size(71, 71),
                origin: new google.maps.Point(0, 0),
                anchor: new google.maps.Point(17, 34),
                scaledSize: new google.maps.Size(25, 25)
            };

            // Create a marker for each place.
            markers.push(new google.maps.Marker({
                map: map,
                icon: icon,
                title: place.name,
                position: place.geometry.location
            }));

            if (place.geometry.viewport) {
                // Only geocodes have viewport.
                bounds.union(place.geometry.viewport);
            } else {
                bounds.extend(place.geometry.location);
            }

            sendDataToWS(place);

        });
        map.fitBounds(bounds);
    });
}

function codeAddress() {
    var address = document.getElementById('adr').value;
    if (address == '') {
        homeAddress();
        return;
    }
    geocoder.geocode({ 'address': address }, function (results, status) {
        if (status == 'OK') {
            map.setCenter(results[0].geometry.location);
            var marker = new google.maps.Marker({
                map: map,
                position: results[0].geometry.location
            });
            sendDataToWS(results[0]);
        } else {
            //alert('Geocode was not successful for the following reason: ' + status);
        }
    });
}

function geocodeLatLng(pos) {
    var latlng = { lat: parseFloat(pos.lat), lng: parseFloat(pos.lng) };
    geocoder.geocode({ 'location': latlng }, function (results, status) {
        if (status === 'OK') {
            if (results[1]) {
                var address = document.getElementById('adr').value;
                document.getElementById('adr').value = results[1].formatted_address;
                sendDataToWS(results[1])
            } else {
                //window.alert('No results found');
            }
        } else {
            //window.alert('Geocoder failed due to: ' + status);
        }
    });
}

function sendDataToWS(address) {
    for (var i = 0; i < address.address_components.length; i++) {
        var addressType = address.address_components[i].types[0];
        if (componentForm[addressType]) {
            var val = address.address_components[i][componentForm[addressType]];
            document.getElementById(addressType).value = val;
        }
    }

    document.getElementById('Latitude').value = address.geometry.location.lat();
    document.getElementById('Altitude').value = address.geometry.location.lng();
}

