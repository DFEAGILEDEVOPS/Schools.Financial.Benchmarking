﻿import CookieManager from './CookieManager';
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
      'cookies_policy', 
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
        'cookies_policy',
        JSON.stringify(cookieSettings), 
        {days: 365},
        domain);
      
      CookieManager.setCookie(
        'cookies_preferences_set',
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
        'cookies_policy',
        JSON.stringify(cookieSettings),
        {days: 365},
        domain);
      
      CookieManager.setCookie(
        'cookies_preferences_set',
        'true',
        {days: 365},
        domain);
      
      toggleCookieBanner(false);
    });
  }
  
  const cookieBannerHideBtn = document.querySelector('.cookie-banner-hide-button');
  
  if (cookieBannerHideBtn instanceof HTMLElement) {
    cookieBannerHideBtn.addEventListener('click', function(e){
      e.preventDefault();
      document.querySelectorAll('.govuk-cookie-banner').forEach((banner) => {
        banner.classList.add('hidden');
        banner.setAttribute('aria-hidden', 'true');
        banner.setAttribute('hidden', 'hidden' );
      });
    });
  }
}

const managePreferencesUi = function(): void {
  const cookieBanner: HTMLElement | null = document.querySelector('.govuk-cookie-banner');
  const cookieMessage: HTMLElement | null = document.getElementById('govuk-cookie-banner-message');
  
  if (cookieBanner instanceof HTMLElement) {
    if (!CookieManager.cookie("cookies_preferences_set") &&
      !(window.location.href.toLowerCase().indexOf("/help/cookies") > 0)) {

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
  const currentPolicyCookie = CookieManager.cookie('cookies_policy');
  const recruitmentBanner: HTMLElement | null = document.querySelector('.banner-content__recruitment-banner');
  
  if (currentPolicyCookie) {
    const currentPolicy: ICookiePolicy = JSON.parse(currentPolicyCookie)


    if (currentPolicy?.settings) {
      const suppressRecruitmentBannerCookie: string | null = CookieManager.getCookie('suppress-recruitment-banner');
      if (suppressRecruitmentBannerCookie !== null && suppressRecruitmentBannerCookie === 'yes') {

        if (recruitmentBanner instanceof HTMLElement) {
          recruitmentBanner.classList.add('hidden');
          recruitmentBanner.setAttribute('aria-hidden', 'true');
        }
      } else {
        if (!isOnRecruitmentView) {
          if (recruitmentBanner instanceof HTMLElement) {
            recruitmentBanner.classList.remove('hidden');
            recruitmentBanner.removeAttribute('aria-hidden');

            const closeButton = recruitmentBanner.querySelector('.js-dismiss-recruitment-banner');
            if (closeButton) {
              closeButton.addEventListener('click', (e) => {
                e.preventDefault();
                recruitmentBanner.classList.add('hidden');
                recruitmentBanner.setAttribute('aria-hidden', 'true');

                const cookieValue = CookieManager.getCookie('cookies_policy');
                if (cookieValue) {
                  const policy = JSON.parse(cookieValue);

                  if (policy.settings) {
                    CookieManager.setCookie('suppress-recruitment-banner', 'yes', {days: 180})
                  }
                }
              });
            }
          }
        }
      }
    }
  }
}

const  manageGACookies = function(policyCookie: ICookiePolicy) {
  if (!policyCookie.usage)  {
    CookieManager.cookie("_ga", null);
    CookieManager.cookie("_gat", null);
    CookieManager.cookie("_gid", null);
  }
}


const cookiePolicy = {
  bannerActions,
  managePreferencesUi,
  manageRecruitmentNotification,
  manageGACookies
};

export default cookiePolicy;