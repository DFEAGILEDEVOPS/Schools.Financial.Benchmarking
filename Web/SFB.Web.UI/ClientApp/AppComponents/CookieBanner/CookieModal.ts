class CookieModal {
  private el: HTMLDivElement;
  private readonly modal?: Element;
  constructor(el: HTMLDivElement) {
    this.el = el;
    const cookieModal = document.querySelector('.cookie-prefs-modal');
    if (cookieModal instanceof HTMLDialogElement) {
      this.modal = cookieModal;
    }
    
    this.init();
  }
  
  init() {
    const isCookiePage = window.location.href.toLowerCase().indexOf("/help/cookies") > 0;
    if (!isCookiePage) {
      document.body.classList.add('no-scroll');
      const page = document.getElementById('ts-modal-page');
      
      page?.setAttribute('aria-hidden', 'true');
      
      if (this.modal instanceof HTMLDialogElement) {
        this.modal.focus();
        const contentArea = document.getElementById('govuk-cookie-banner-message');
        if (contentArea instanceof HTMLDivElement) {
          const modalKeyPress = (e: KeyboardEvent) => {
            this.manageModalKeyPress(e, contentArea)
          }
          
          document.addEventListener('keydown', modalKeyPress);
          
        }
      }
    }
  }
  manageModalKeyPress(e: KeyboardEvent, container: HTMLDivElement) {
    if (this.modal instanceof HTMLDialogElement) {
      if (e.code === 'Tab') {
        const focusableItems = container.querySelectorAll('a[href], button:not([disabled])');
        const focusableItemsCount = focusableItems.length;
        const focusedItem = document.activeElement;
        
        if (focusedItem) {
          const focusedItemIndex = Array.prototype.indexOf.call(focusableItems, focusedItem);
          if (!e.shiftKey && (focusedItemIndex === focusableItemsCount - 1)) {
            (focusableItems[0] as HTMLElement).focus();
            e.preventDefault();
          }
          if (e.shiftKey && focusedItemIndex === 0) {
            (focusableItems[focusableItemsCount - 1] as HTMLElement).focus();
            e.preventDefault();
          }
          if (focusedItemIndex === -1) {
            (focusableItems[0] as HTMLElement).focus();
            e.preventDefault();
          }
        }
      }
    }
  }

}

export default CookieModal;