import CookieManager from './CookieManager';
import {ICookiePolicy} from "../Interfaces";


function toggleCookieBanner(accepted: boolean): void {
  const messagePanel = document.getElementById('govuk-cookie-banner-message');
  const acceptedPanel = document.getElementById('govuk-cookie-accept-confirmation');
  const rejectedPanel = document.getElementById('govuk-cookie-reject-confirmation');
  if (accepted &&
    messagePanel instanceof HTMLElement &&
    acceptedPanel instanceof HTMLElement) {

    messagePanel.classList.add('hidden');
    messagePanel.setAttribute('aria-hidden', 'true');

    acceptedPanel.classList.remove('hidden');
    acceptedPanel.removeAttribute('aria-hidden');
    acceptedPanel.removeAttribute('hidden');
  }

  if (!accepted &&
    messagePanel instanceof HTMLElement &&
    rejectedPanel instanceof HTMLElement) {

    messagePanel.classList.add('hidden');
    messagePanel.setAttribute('aria-hidden', 'true');

    rejectedPanel.classList.remove('hidden');
    rejectedPanel.removeAttribute('aria-hidden');
    rejectedPanel.removeAttribute('hidden');
  }
}

const bannerActions = function(): void {
  let policyCookie = CookieManager.cookie('cookies_policy');
  let domain ='';
  const domainElement: HTMLElement | null = document.getElementById('cookieDomain');
  if (domainElement instanceof HTMLInputElement) {
    domain = domainElement.value;
  }
  
  if (!policyCookie) {
    const cookieSettings: ICookiePolicy = { "essential": true, "settings": false, "usage": false };
    CookieManager.setCookie(
      'cookies_policy_', 
      JSON.stringify(cookieSettings), 
      {days: 365}, 
      domain);
  }
  
  const acceptAll = document.getElementById('acceptAllCookies');
  const rejectAll = document.getElementById('rejectAllCookies');
  
  if (acceptAll instanceof HTMLElement) {
    acceptAll.addEventListener('click', (e) => {
      e.preventDefault();
      const cookieSettings: ICookiePolicy = { "essential": true, "settings": true, "usage": true };
      
      CookieManager.setCookie(
        'cookies_policy_',
        JSON.stringify(cookieSettings), 
        {days: 365},
        domain);
      
      CookieManager.setCookie(
        'cookies_preferences_set_',
        'true', 
        {days: 365},
        domain);
      
      toggleCookieBanner(true);
    });
  }
  
  if (rejectAll instanceof HTMLElement) {
    rejectAll.addEventListener('click', (e) => {
      e.preventDefault();
      const cookieSettings: ICookiePolicy = { "essential": true, "settings": false, "usage": false };
      
      CookieManager.setCookie(
        'cookies_policy_',
        JSON.stringify(cookieSettings),
        {days: 365},
        domain);
      
      CookieManager.setCookie(
        'cookies_preferences_set_',
        'true',
        {days: 365},
        domain);
      
      toggleCookieBanner(false);
    });
  }
  
  const cookieBannerHideBtns = document.querySelectorAll('.cookie-banner-hide-button');
  
  if (cookieBannerHideBtns instanceof NodeList) {
    cookieBannerHideBtns.forEach((button) => {
      button.addEventListener('click', function(e) {
        e.preventDefault();
          document.querySelectorAll('.cookie-prefs-modal').forEach((banner) => {
            banner.remove();
          });
          document.querySelectorAll('.modal-overlay').forEach((overlay) =>{
            overlay.remove();
          });
          document.body.classList.remove('no-scroll');
          document.getElementById('ts-modal-page')?.removeAttribute('aria-hidden');
      });
    });
  }
}

const managePreferencesUi = function(): void {
  if (CookieManager.getCookie("cookies_preferences_set_")) {
    return;
  }
  const cookieBanner: HTMLElement | null = document.querySelector('.govuk-cookie-banner');
  const cookieMessage: HTMLElement | null = document.getElementById('govuk-cookie-banner-message');
  
  bannerActions();
  
  if (cookieBanner instanceof HTMLElement) {
    if (!(window.location.href.toLowerCase().indexOf("/help/cookies") > 0)) {

      cookieBanner.classList.remove('hidden');
      cookieBanner.removeAttribute('aria-hidden');
      cookieBanner.removeAttribute('hidden');
      
      if (cookieMessage instanceof HTMLElement) {
        cookieMessage.classList.remove('hidden');
        cookieMessage.removeAttribute('aria-hidden');
        cookieMessage.removeAttribute('hidden');
      }
    } else {
      cookieBanner.classList.add('hidden');
      cookieBanner.setAttribute('aria-hidden', 'true');
      cookieBanner.setAttribute('hidden', 'hidden');
    }
  }
}

const manageRecruitmentNotification = function(): void {
  const location: string = window.location.toString().toLowerCase();
  const isOnRecruitmentView = location.indexOf('help/get-involved') > -1 ||
    location.indexOf('/help/getinvolvedsubmission') > -1;
  const recruitmentBanner: HTMLElement | null = document.querySelector('.banner-content__recruitment-banner');
  const suppressRecruitmentBannerCookie: string | null = CookieManager.getCookie('suppress-recruitment-banner');
  if (suppressRecruitmentBannerCookie && suppressRecruitmentBannerCookie === 'yes' && recruitmentBanner instanceof HTMLElement) {
      recruitmentBanner.classList.add('hidden');
      recruitmentBanner.setAttribute('aria-hidden', 'true');
  } else {
    if (!isOnRecruitmentView && recruitmentBanner instanceof HTMLElement) {
      recruitmentBanner.classList.remove('hidden');
      recruitmentBanner.removeAttribute('aria-hidden');
      recruitmentBanner.removeAttribute('style');

      const closeButton = recruitmentBanner.querySelector('.js-dismiss-recruitment-banner');
      if (closeButton) {
        closeButton.addEventListener('click', (e) => {
          e.preventDefault();
          recruitmentBanner.classList.add('hidden');
          recruitmentBanner.setAttribute('aria-hidden', 'true');

          CookieManager.setCookie('suppress-recruitment-banner', 'yes', {days: 30})
        });
      }
    }
  }
}

const manageGACookies = function(policyCookie: ICookiePolicy) {
  if (!policyCookie.usage)  {
    CookieManager.cookie("_ga", null);
    CookieManager.cookie("_gat", null);
    CookieManager.cookie("_gid", null);
  }
}


const cookiePolicy = {
  managePreferencesUi,
  manageRecruitmentNotification,
};

export default cookiePolicy;