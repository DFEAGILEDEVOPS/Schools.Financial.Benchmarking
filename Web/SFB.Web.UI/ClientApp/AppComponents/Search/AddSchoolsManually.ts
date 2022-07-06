import agent from '../../api/agent';
import  {setup, manualAdd as suggestionConfig} from './autosuggest';

export default class AddSchoolsManually {
  app: HTMLDivElement;
  constructor(app: HTMLDivElement) {
    this.app = app;
    this.bindEvents();
  }
  
   initSuggestionComponent(): void {
     suggestionConfig.textInput = 'NewSchoolName';
     setup(suggestionConfig);
  }
  
  bindEvents(): void {
    const schoolsToAdd = document.getElementById('SchoolsToAdd');

    suggestionConfig.selectItemCallBack = async (item) => {
      const res = await agent.ManualComparison.addSchool(item.text.value.toString());
      if (schoolsToAdd instanceof HTMLElement) {
        schoolsToAdd.innerHTML = res.toString();
      }
    }
    
    this.initSuggestionComponent();    

    if (this.app instanceof HTMLDivElement) {
      // replacing HTML here so listener set on app parent
      this.app.addEventListener('click', async (e) => {
          const target = e.target as HTMLElement;
          
          if (target.classList.contains('remove-school')) {
            e.preventDefault();
            const urn = target.dataset.urn!;
            const result = await agent.ManualComparison.removeSchool(urn);
            schoolsToAdd!.innerHTML = result.toString();
            this.initSuggestionComponent();
          }
          
          if (target.id === 'displayNew') {
            e.preventDefault();
            document.getElementById('NewSchool')?.removeAttribute('style');
            this.initSuggestionComponent();
          }
        });
    }
  }
  
}