class CookieModal {
  private el: HTMLDivElement;
  constructor(el: HTMLDivElement) {
    this.el = el;
    this.init();
  }
  
  init() {
    const isCookiePage = window.location.href.toLowerCase().indexOf("/help/cookies") > 0;
    if (!isCookiePage) {
      document.body.classList.add('no-scroll');
    }
  }
}

export default CookieModal;