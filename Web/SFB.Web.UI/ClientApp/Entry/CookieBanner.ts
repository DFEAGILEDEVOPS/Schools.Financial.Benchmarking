import CookieModal from "../AppComponents/CookieBanner/CookieModal";

const el = document.querySelector('.govuk-cookie-banner');

console.log(el);
if (el instanceof HTMLDivElement) {
  console.log('in entry')
  new CookieModal(el);
}