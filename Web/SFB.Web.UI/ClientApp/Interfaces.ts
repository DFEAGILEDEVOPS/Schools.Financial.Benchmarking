import Awesomplete from "awesomplete";

export interface IformData {
    [key: string]: any;
}

// cookie setting duration
export interface ICookieOptions {
  days?: number;
}

// cookie preference
export interface ICookiePolicy {
  essential: boolean;
  settings: boolean;
  usage: boolean;
}


export interface IAnalyticsPayload {
  eventCategory: string;
  eventAction: string;
  eventLabel: string | null;
  transport?: string;
  hitCallback?: Function;
}

export interface IAutosuggestionComponent {
  textInput: HTMLInputElement;
  hiddenInput: HTMLInputElement;
  mapSuggestions?: () => Awesomplete.Suggestion;
  openOnly?: () => boolean;
  remoteSource: boolean;
  remoteSourceFunction?: Function;
  localSource?: string;
}

export interface ICoordinates {
  latitude: number;
  longitude: number;
}