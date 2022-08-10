﻿/* 
 TODO refactor search into separate forms so that the open-only checkboxes no longer
  need to have their disabled state managed with js
*/
export default function searchPanelInputManager(){
  const searchElementsContainer = document.getElementById('SearchTypesAccordion');
  const radios = searchElementsContainer?.querySelectorAll('.govuk-radios__input') as NodeListOf<Element>;
  const submitButtons = searchElementsContainer?.querySelectorAll('.govuk-button') as NodeListOf<Element>;

  if (radios instanceof NodeList) {
    radios.forEach((radio) => {
      if (radio instanceof HTMLInputElement) {
        radio.addEventListener('change', function() {
          const allChx = searchElementsContainer?.querySelectorAll('input[name=\'openOnly\']');
          const panelChx = radio.parentElement?.nextElementSibling?.querySelector('input[name=\'openOnly\']') as HTMLInputElement;
          const panelBtn =  radio.parentElement?.nextElementSibling?.querySelector('.govuk-button') as HTMLButtonElement;
          if (submitButtons instanceof NodeList) {
            submitButtons.forEach(btn => btn.setAttribute('disabled', 'disabled'));
          }
          if (panelBtn instanceof HTMLButtonElement) {
            panelBtn.removeAttribute('disabled');
          }

          if (!panelChx) { return; }
          
          if (allChx instanceof NodeList) {
            allChx.forEach(chx => chx.setAttribute('disabled', 'disabled'));
          }
          if (panelChx instanceof HTMLInputElement) {
            panelChx.removeAttribute('disabled');
          }
        });
      }
    });
  }
}

 