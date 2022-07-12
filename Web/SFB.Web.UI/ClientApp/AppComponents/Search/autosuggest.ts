import agent from '../../api/agent';
import Awesomplete from 'awesomplete';

declare var localAuthorities: any;
const actionKeys: string[] = ['ArrowLeft', 'ArrowUp', 'ArrowRight', 'ArrowDown', 'Escape', 'Enter']; 
const MINCHARS = 2;


interface AutosuggestionComponent {
  textInput: string;
  hiddenInput: string;
  mapSuggestions?: () => Awesomplete.Suggestion;
  openOnly?: () => boolean;
  remoteSource: boolean;
  remoteSourceFunction?: (userInput: string, openOnly: boolean) => Promise<any[]>;
  localSource?: string;
  mapperFunction?: (suggestion: any) => AwesompleteSuggestion;
  selectItemCallBack?: (suggestion: any) => void;
}

type NameSearchResult = {
  Id: string;
  Text: string;
}

type LocationSearchResult = {
  id: string;
  name: string;
  coords: {
    longitude: number,
    latitude: number;
  }
}

type LaSearchResult = {
  LANAME: string;
  REGION: string;
  REGIONNAME: string;
  id: string;
}

interface AwesompleteSuggestion {
  label: string;
  value: string | number;
}

function mapNameResult(suggestion: NameSearchResult): AwesompleteSuggestion {
  return {
    label: suggestion.Text,
    value: suggestion.Id
  }
}
function mapLaResult(suggestion: LaSearchResult): AwesompleteSuggestion {
  return {
    label: suggestion.LANAME,
    value: suggestion.id
  }
}

function mapLocationResult(suggestion: LocationSearchResult): AwesompleteSuggestion {
  return {
    label: suggestion.name,
    value: `${suggestion.coords.latitude}, ${suggestion.coords.longitude}`
  }
}

const establishmentName: AutosuggestionComponent = {
  textInput: 'FindByNameId',
  hiddenInput :'FindByNameIdSuggestionId',
  openOnly: () => {
    const chx = document.getElementById('openOnlyName') as HTMLInputElement;
    return chx && chx.checked; 
  },
  remoteSource: true,
  remoteSourceFunction: agent.Suggestions.Schools,
  mapperFunction: mapNameResult,
};

const establishmentLocation: AutosuggestionComponent = {
  textInput: 'FindSchoolByTown',
  hiddenInput : 'LocationCoordinates',
  openOnly: () => {
    const chx = document.getElementById('openOnlyLocation') as HTMLInputElement;
    return chx && chx.checked; 
  },
  remoteSource: true,
  remoteSourceFunction: agent.Suggestions.Location,
  mapperFunction: mapLocationResult,
};

const establishmentLa: AutosuggestionComponent = {
  textInput: 'FindSchoolByLaCodeName',
  hiddenInput : 'SelectedLocalAuthorityId',
  openOnly: () => {
    const chx = document.getElementById('openOnlyLa') as HTMLInputElement;
    return chx && chx.checked; 
  },
  remoteSource: false,
  localSource: localAuthorities,
  mapperFunction: mapLaResult
};

const trustName: AutosuggestionComponent = {
  textInput: 'FindByTrustName',
  hiddenInput : 'FindByTrustNameSuggestionId',
  remoteSource: true,
  remoteSourceFunction: agent.Suggestions.Trusts,
  mapperFunction: mapNameResult,
};

const trustLa: AutosuggestionComponent = {
  textInput: 'FindTrustByLaCodeName',
  hiddenInput : 'SelectedLocalAuthorityIdTrust',
  remoteSource: false,
  localSource: localAuthorities,
  mapperFunction: mapLaResult
};

const trustLocation: AutosuggestionComponent = {
  textInput: 'FindTrustByTown',
  hiddenInput : 'LocationCoordinatesForTrust',
  remoteSource: true,
  remoteSourceFunction: agent.Suggestions.Location,
  mapperFunction: mapLocationResult,
};


const manualAddByName: AutosuggestionComponent  = {
  textInput: 'NewSchoolName',
  hiddenInput :'FindByNameIdSuggestionId',
  openOnly: () => {
    const openOnlyField = document.getElementById('openOnly') as HTMLInputElement;
    return openOnlyField?.value === 'True';
  },
  remoteSource: true,
  remoteSourceFunction: agent.Suggestions.Schools,
  mapperFunction: mapNameResult,
};

const manualAddTrustByName: AutosuggestionComponent = {
  textInput: 'NewTrustName',
  hiddenInput : 'FindByTrustNameSuggestionId',
  remoteSource: true,
  remoteSourceFunction: agent.Suggestions.Trusts,
  mapperFunction: mapNameResult,
};


const suggestionComponents = [
  establishmentName,
  establishmentLocation,
  establishmentLa,
  trustName,
  trustLocation,
  trustLa
];

const actions = {
  setUp: (
    {
      hiddenInput,
      remoteSource,
      remoteSourceFunction,
      textInput,
      openOnly,
      localSource,
      mapperFunction,
      selectItemCallBack
    }: AutosuggestionComponent) => {

    const input = document.getElementById(textInput) as HTMLInputElement;
    const _hiddenInput = document.getElementById(hiddenInput) as HTMLInputElement;
    
    if (input) {
      let interval: ReturnType<typeof setTimeout>;
      const options: Awesomplete.Options = {};
      options.minChars = MINCHARS;

      options.replace = (suggestion: string | Awesomplete.Suggestion) => {
        input.value = suggestion as string;
      };

      if (remoteSource) {
        options.filter = () => {
          return true
        };
      } else {
        options.filter = Awesomplete.FILTER_STARTSWITH;
      }
      
      options.sort = false;
      options.autoFirst = true;
      // @ts-ignore
      options.data = mapperFunction;

      if (localSource) {
        options.list = localSource;
      }

      const _awesomplete = new Awesomplete(input, options);

      if (selectItemCallBack && typeof selectItemCallBack === 'function') {
       
        input.addEventListener<any>('awesomplete-select', (e) => {
          selectItemCallBack(e);
        });
      } else {
        input.addEventListener<any>('awesomplete-select', (e) => {
          _hiddenInput.value = e.text.value;
        });
      }
      

      if (remoteSource && typeof remoteSourceFunction !== 'undefined') {
        input.addEventListener('keyup', async (e) => {
          clearTimeout(interval);
          const keyName = e.key;
          let shouldGetResults = true;
          if (actionKeys.indexOf(keyName) > -1) {
            shouldGetResults = false;
          }

          if (input.value.length > MINCHARS && shouldGetResults) {
            interval = setTimeout(async function () {
              const openEstabs = openOnly ? openOnly() : true;
              _awesomplete.list = await remoteSourceFunction(input.value, openEstabs);
            }, 200);
          }
        });
      }
      
      input.addEventListener('focus', () => {
        if (_awesomplete.ul.childNodes.length > 0 &&
            _awesomplete.ul.hasAttribute('hidden') &&
            input.value.length > MINCHARS) {
          _awesomplete.evaluate();
          _awesomplete.open();
        } 
      });
    }
  }
}


export const manualAdd = manualAddByName;

export const manualAddTrust = manualAddTrustByName;

export const setup = actions.setUp;

const autoSuggest = {
  actions,
  suggestionComponents
}

export default autoSuggest;