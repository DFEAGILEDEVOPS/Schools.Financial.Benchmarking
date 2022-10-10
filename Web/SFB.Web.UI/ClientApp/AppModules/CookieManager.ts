import {ICookieOptions} from "../Interfaces";

const cookie = function(name: string, 
                        value: string | null | boolean = '', 
                        options?: ICookieOptions) : void | null | string {
  if (typeof value !== 'undefined') {
    if (value === false || value === null) {
      value = '';
      return setCookie(name, '', {days: -1})
    }
  } else  {
    return getCookie(name);
  }
}


const setCookie = function(name: string,
                           value: string | null | boolean = '',
                           options: ICookieOptions = {},
                           domain?: string) {
  let cookieString = name + "=" + value + "; path=/";
  if (options.days) {
    const date = new Date();
    date.setTime(date.getTime() + (options.days * 24 * 60 * 60 * 1000));
    cookieString = cookieString + "; expires=" + date.toUTCString();
  }
  if (document.location.protocol == 'https:') {
    cookieString = cookieString + "; Secure";
  }
  cookieString = cookieString + "; samesite=lax";
  if (domain) {
    cookieString = cookieString + "; domain=" + domain;
  }
  
  document.cookie = cookieString;
}

const getCookie = function(name: string): string | null {
  const nameEQ = name + "=";
  const cookies = document.cookie.split(';');
  for (let i = 0, len = cookies.length; i < len; i++) {
    let cookie = cookies[i];
    while (cookie.charAt(0) == ' ') {
      cookie = cookie.substring(1, cookie.length);
    }
    if (cookie.indexOf(nameEQ) === 0) {
      return decodeURIComponent(cookie.substring(nameEQ.length));
    }
  }
  return null;
}

const CookieManger = {
  cookie,
  setCookie,
  getCookie
};

export default CookieManger;