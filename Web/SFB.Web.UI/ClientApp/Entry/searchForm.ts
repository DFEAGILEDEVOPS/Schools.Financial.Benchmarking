import autosuggest from "../AppComponents/Search/autosuggest";
import LocateUser from '../AppComponents/Search/LocateUser';
import searchPanelInputManager from '../AppComponents/Search/searchPanelInputManager';
new LocateUser();
searchPanelInputManager();
const setup = autosuggest.actions.setUp;

 autosuggest.suggestionComponents.map((component) => {
  setup(component);
});


 
 