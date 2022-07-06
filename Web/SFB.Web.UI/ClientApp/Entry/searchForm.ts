import autosuggest from "../AppComponents/Search/autosuggest";
import LocateUser from '../AppComponents/Search/LocateUser';
import openOnlyCheckboxManager from '../AppComponents/Search/openOnlyCheckboxManager';
new LocateUser();
openOnlyCheckboxManager();
const setup = autosuggest.actions.setUp;

 autosuggest.suggestionComponents.map((component) => {
  setup(component);
});

 
 