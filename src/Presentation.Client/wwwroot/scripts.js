window.initPlaceAutocomplete = function(id){
    if (!window.google || !google.maps || !google.maps.places){
        return;
    }
    var input = document.getElementById(id);
    if(input){
        input._autocomplete = new google.maps.places.Autocomplete(input);
    }
}
window.disposePlaceAutocomplete = function(id){
    var input = document.getElementById(id);
    if(input && input._autocomplete){
        google.maps.event.clearInstanceListeners(input._autocomplete);
        delete input._autocomplete;
    }
}
