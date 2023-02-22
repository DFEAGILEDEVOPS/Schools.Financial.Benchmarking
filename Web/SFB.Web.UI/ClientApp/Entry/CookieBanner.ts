import CookieModal from "../AppComponents/CookieBanner/CookieModal";

const el = document.querySelector('.govuk-cookie-banner');

if (el instanceof HTMLDivElement) {
  new CookieModal(el);
}