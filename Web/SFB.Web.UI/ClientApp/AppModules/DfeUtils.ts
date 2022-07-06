import {IformData} from "../Interfaces";

const randomNumber = function (): number {
  return Math.floor((Math.random() * 10000000) + 1);
}

const focusTo = function (elementId: string): void {
  location.href = `#${elementId}`;
}

const getFormData = function (form: HTMLFormElement): IformData {
  return new FormData(form);
}

const clearDropDowns = function (e: Event): void {
  const target = e.target as Element;
  if (target.id !== 'DownloadLink') {
    const selectControls = document.querySelectorAll<HTMLSelectElement>('select');
    selectControls.forEach((select: HTMLSelectElement): void => {
      select.selectedIndex = -1;
    });
  }
}

const queryString = {
  get: function (name: string, url: string | null): string | null {
    if (!url) {
      url = window.location.href;
    }
    name = name.replace(/[\[\]]/g, "\\$&");

    const regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)", 'i');
    const results: string[] | null = regex.exec(url);
    if (!results) {
      return null
    }
    if (!results[2]) {
      return ' ';
    }

    return decodeURIComponent(results[2].replace(/\+/g, " "));
  },
  getHashParameter: function (url: string): string | null {
    if (!url) {
      url = window.location.href;
    }
    if (url.indexOf('#') === -1) {
      return null;
    }
    return window.location.hash;
  }
}

const getQueryString = function () {
  return encodeURIComponent(window.location.pathname + window.location.search);
}

const DfeUtils = {
  randomNumber,
  focusTo,
  getFormData,
  clearDropDowns,
  queryString,
  getQueryString
};

export default DfeUtils;