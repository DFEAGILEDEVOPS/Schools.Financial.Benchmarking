import {IAnalyticsPayload} from '../Interfaces';


const isAvailable = function(): boolean {
  // @ts-ignore
  return window.ga && window.ga.hasOwnProperty('loaded');
}


const trackClick = function(this: HTMLElement, e: Event): void {
  if (isAvailable()) {
    const element: HTMLElement = this;
    if (element instanceof  HTMLAnchorElement) {
      const isHyperLinking: boolean = element && element.hostname !== null;
      const isExternalLink: boolean = isHyperLinking && location.hostname !== element.hostname;
      const isTargetBlank: boolean = element.getAttribute('target') === '_blank';
      const targetHref: string | null = isHyperLinking ? element.href : null;
      const navigateHitCallback = function(): void {
        window.location.href = targetHref as string;
      };
      const hasJsTrackClass: boolean = element.classList.contains('js-track');
      let gaActionName: string | null = null; 
      let gaCategoryName: string | null = null;
      let gaEventLabel: string | null = null;
      
      if (isExternalLink || hasJsTrackClass) {
        if (isExternalLink && !hasJsTrackClass) {
          gaActionName = e.type;
          gaCategoryName = 'outbound';
          gaEventLabel = targetHref as string
        } else {
          const trackData: string | undefined = element.dataset.track;
          if (typeof trackData !== 'undefined') {
            const parts: string[] = trackData.split('|');
            
            if (parts.length === 3) {
              gaCategoryName = parts[0];
              gaEventLabel = parts[1];
              gaActionName = parts[2];

              if (gaEventLabel === "[use-path]"){
                gaEventLabel = window.location.toString();
              }
            }
          }
        }
        
        if (gaCategoryName && gaActionName) {
          let gaHitCallback: Function | null = null;
          if (isHyperLinking && !isTargetBlank && !navigator.sendBeacon) {
            gaHitCallback = navigateHitCallback;
            e.preventDefault();
          }
          trackEvent(gaCategoryName, gaEventLabel, gaActionName, gaHitCallback);
        }
      }
    }
    
  }
}

const trackEvent = function(category: string,
                            label: string | null, 
                            action: string, 
                            callback: Function | null): void {
  if (isAvailable() && category && action) {
    let callbackFired: boolean = false;
    let safeCallback: Function | null = null;
    
    if (typeof callback === 'function') {
      safeCallback = function() {
        if (callbackFired) {
          return;
        }
        callbackFired = true;
        callback();
      }
    }
    
    category = category.trim();
    label = label? label.trim() : null;
    action = action.trim();
    
    try {
      const payload: IAnalyticsPayload = {
        eventCategory: category,
        eventAction: action,
        eventLabel: label
      };
      
      if (typeof safeCallback === 'function') {
        payload.transport = 'beacon';
        payload.hitCallback = safeCallback;
        
        window.setTimeout(function (): void {
          safeCallback && safeCallback();
        },1000);
      }

      // @ts-ignore
      window.ga('send', 'event', payload);
    }
    catch (error) {
      
    }
  }
}

const trackPageView = function(path : string) : void {
  if (isAvailable() && path) {
    try {
      // @ts-ignore
      window.ga('send', 'pageview', path);
    }
    catch(error) {
      
    }
  }
}

const Analytics = {
  isAvailable,
  trackClick,
  trackEvent,
  trackPageView
}
export default Analytics;