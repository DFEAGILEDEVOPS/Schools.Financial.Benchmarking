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
      const res = await agent.ManualComparison.addTrust(item.value, item.text.value.toString());
      if (trustsToAdd instanceof HTMLDivElement) {
        trustsToAdd.innerHTML = res.toString();
      }
    }
    this.initSuggestionComponent();
    
    
    this.app.addEventListener('click', async (e) => {
      const target = e.target as HTMLElement;
      
      if (target.classList.contains('remove-trust')) {
        e.preventDefault();
        const companyNo = target.dataset.companyno!;
        const result = await agent.ManualComparison.removeTrust(companyNo);
        
        trustsToAdd!.innerHTML = result.toString();
        this.initSuggestionComponent();
      }
      
      if (target.id === 'displayNew') {
        e.preventDefault();
        document.getElementById('NewTrust')?.removeAttribute('style');
        this.initSuggestionComponent();
      }
      
    });
  }
}