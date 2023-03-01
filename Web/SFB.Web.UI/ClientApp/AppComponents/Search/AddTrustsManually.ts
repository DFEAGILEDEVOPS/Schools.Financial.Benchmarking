import agent from '../../api/agent';
import {setup, manualAddTrust as suggestionConfig} from './autosuggest';

export default class AddTrustsManually {
  app: HTMLDivElement;
  constructor(app: HTMLDivElement) {
    this.app = app;
    this.bindEvents();
  }
  
  initSuggestionComponent():void {
    setup(suggestionConfig);
  }
  
  bindEvents():void {
    const trustsToAdd = document.getElementById('TrustsToCompare');
    
    suggestionConfig.selectItemCallBack = async (item) => {
      const res = await agent.ManualComparison.addTrust(item.text.value, item.text.label);
      if (trustsToAdd instanceof HTMLDivElement) {
        trustsToAdd.innerHTML = res.toString();
        this.initSuggestionComponent();
      }
    }
    this.initSuggestionComponent();
    
    
    this.app.addEventListener('click', async (e) => {
      const target = e.target as HTMLElement;
      
      if (target.classList.contains('remove-trust')) {
        e.preventDefault();
        const companyNo = target.dataset.companyno!;
        if (companyNo) {
          const result = await agent.ManualComparison.removeTrust(companyNo);

          trustsToAdd!.innerHTML = result.toString();
          this.initSuggestionComponent();
        }
      }
      
      if (target.classList.contains('remove-new-trust')) {
        e.preventDefault();
        const newTrustPanel = document.getElementById('NewTrust');
        const addButton = document.getElementById('AddButton');
        const errorElements = document.querySelectorAll('.error-summary, .govuk-error-message');
        
        if (newTrustPanel instanceof HTMLElement) {
          newTrustPanel.style.display = 'none';
        }
        
        errorElements.forEach((element) => {
          if (element instanceof HTMLElement) {
            element.style.display = 'none';
          }
        });
        
        if (addButton instanceof HTMLElement) {
          addButton.style.display = 'block';
        }
      }
      
      if (target.id === 'displayNew') {
        e.preventDefault();
        document.getElementById('NewTrust')?.removeAttribute('style');
        const addNew = document.getElementById('AddButton');
        if (addNew instanceof HTMLElement) {
          addNew.style.display = 'none';
        }
        
        this.initSuggestionComponent();
      }
      
    });
  }
}