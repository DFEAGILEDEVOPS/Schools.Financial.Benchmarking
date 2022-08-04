import autosuggest from '../AppComponents/Search/autosuggest';
import LocateUser from '../AppComponents/Search/LocateUser';
import searchPanelInputManager from '../AppComponents/Search/searchPanelInputManager';
searchPanelInputManager();

new LocateUser();

const setup = autosuggest.actions.setUp;

autosuggest.suggestionComponents.map((component) => {
  if (component.textInput === 'FindSchoolByLaCodeName') {
    component.textInput = 'FindSchoolManuallyByLaCodeName';
    setup(component);
  }
  if (component.textInput === 'FindSchoolByTown') {
    component.textInput = 'FindSchoolManuallyByTown';
    setup(component);
  }
  
});