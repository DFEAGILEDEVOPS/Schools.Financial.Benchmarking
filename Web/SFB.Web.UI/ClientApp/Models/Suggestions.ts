export interface SearchSuggestion {
  Id: string;
  Text: string;
}

export interface LocationSuggestion {
  Name: string;
  Coords: {
    Latitude: number;
    Longitude: number;
  }
}