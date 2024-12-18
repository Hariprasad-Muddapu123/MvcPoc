let map, directionsService, directionsRenderer;

function initMap() {
    if (!map) {
        map = new google.maps.Map(document.getElementById('map'), {
            zoom: 12,
            center: { lat: 0, lng: 0 }
        });

        directionsService = new google.maps.DirectionsService();
        directionsRenderer = new google.maps.DirectionsRenderer({ map: map });
    }
}

function loadRoute(bikeLat, bikeLng) {
    initMap();
    const mapContainer = document.getElementById('map');
    const closeButton = document.getElementById('close-map-btn');
    mapContainer.style.display = 'block';
    closeButton.style.display = 'block';

    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function (position) {
            const userLocation = {
                lat: position.coords.latitude,
                lng: position.coords.longitude
            };

            const bikeLocation = {
                lat: parseFloat(bikeLat),
                lng: parseFloat(bikeLng)
            };

            map.setCenter(userLocation);

            const request = {
                origin: userLocation,
                destination: bikeLocation,
                travelMode: google.maps.TravelMode.DRIVING
            };

            directionsService.route(request, function (response, status) {
                if (status === google.maps.DirectionsStatus.OK) {
                    directionsRenderer.setDirections(response);
                } else {
                    alert('Route not available: ' + status);
                }
            });
        }, function () {
            alert('Unable to retrieve your location. Please enable location access.');
        });
    } else {
        alert('Geolocation is not supported by your browser.');
    }
}

function closeMap() {
    const mapContainer = document.getElementById('map');
    const closeButton = document.getElementById('close-map-btn');
    mapContainer.style.display = 'none';
    closeButton.style.display = 'none';
    if (directionsRenderer) {
        directionsRenderer.set('directions', null);
    }
}