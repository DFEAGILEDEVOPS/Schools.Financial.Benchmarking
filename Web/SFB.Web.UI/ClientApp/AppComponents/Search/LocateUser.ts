import jsonp from 'jsonp';

export default class LocateUser {
  
  constructor() {
    this.init();
  }
  
  init() {
    const locateButton = document.getElementById('FindCurrentPosition');
    locateButton?.addEventListener('click', this.findCurrentLocation)
  }
  
  findCurrentLocation(e: Event) {
    e.preventDefault();
    
    const getCurrentPositionSuccessHandler = function(position: GeolocationPosition) {
      let coords = position.coords;
      
      jsonp(`https://api.postcodes.io/postcodes?lat=${coords.latitude}&lon=${coords.longitude}&widesearch=true`, function(error, data) {
        if (data.result) {
          const locationInput = document.getElementById('FindSchoolByTown');
          if (locationInput instanceof HTMLInputElement){
            locationInput.removeAttribute('placeholder');
            locationInput.value = data.result[0].postcode;
          }
          
        }
      });    
    }

    const getCurrentPositionErrorHandler = function (err: any) {
      const errorTarget: string = '#finderSection';
      const errorsList = [];
      
      switch (err.code) {
        case err.UNKNOWN_ERROR:
          errorsList.push(
            {
              text: 'Unable to find your location',
              target: errorTarget
            }
          );
          break;
        case err.PERMISSION_DENIED:
          errorsList.push(
            {
              text: 'Your location is blocked by your browser, enter your location manually',
              target: errorTarget
            }
          );
          break;
        case err.POSITION_UNAVAILABLE:
          errorsList.push({
              text: 'Your location is currently unknown',
              target: errorTarget
            });
          break;
        case err.TIMEOUT:
          errorsList.push({
              text: 'Attempt to find location took too long',
              target: errorTarget
            });
          break;
        default:
          errorsList.push({
              text: 'Location detection not supported in browser',
              target: errorTarget
            });
      }
      
      const input = document.getElementById('FindSchoolByTown');
      input?.removeAttribute('placeholder');
      const errorContainer = document.querySelector('.location-error-message-placeholder');
      const span = document.createElement('span');
      span.classList.add('govuk-error-message');
      span.appendChild(document.createTextNode(errorsList[0].text));
      errorContainer?.appendChild(span);
    }
       
    if (navigator.geolocation) {
      const input = document.getElementById('FindSchoolByTown');
      input?.setAttribute('placeholder', "Locating...");

      navigator.geolocation.getCurrentPosition(
        getCurrentPositionSuccessHandler,
        getCurrentPositionErrorHandler, {timeout: 60000});
    } else getCurrentPositionErrorHandler(-1);
  }
  
}

