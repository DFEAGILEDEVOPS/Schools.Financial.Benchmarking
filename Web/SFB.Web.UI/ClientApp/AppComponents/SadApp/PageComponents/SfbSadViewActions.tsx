import {SadDataObject} from "../Models/SadEditForm";
import {Link} from "react-router-dom";
import SfbSadRemoveWarning from "./SfbSadRemoveWarning";
import SfbSadHelpModal from "../../Global/ModalComponents/SfbSadHelpModal";

interface ISadViewActionsProps {
    estabCustomData: SadDataObject | null;
    scenarioIndex: 0 | 1;
    removeScenarioHandler: (idx: 0 | 1) => void;
}

export default function SfbSadViewActions(
    {
        estabCustomData,
        scenarioIndex,
        removeScenarioHandler

    } : ISadViewActionsProps
) {
    const HandlePrintClick = () => {
        window.print();
    }

    return (
        <div className="govuk-grid-row">
            <div className="govuk-grid-column-one-half">
                <button className="sfb-button--download">Download page</button>
                <button 
                    className="sfb-button--print" 
                    onClick={()=> HandlePrintClick()}>Print page</button>
            </div>
            <div className="govuk-grid-column-one-half sfb-align-right--desktop">
                  <Link to="edit1" className="govuk-button sfb-sad-add-custom">
                    Add a custom dashboard</Link>
                {typeof estabCustomData?.lastEdit !== 'undefined' &&
                  <SfbSadRemoveWarning
                    removeScenarioHandler={removeScenarioHandler}
                    idx={scenarioIndex} />
                }
                <span className="inline-help-container">
                <SfbSadHelpModal
                    modalTitle="Add a custom dashboard"
                    modalContent='<p>The custom dashboard allows schools to plan for hypothetical or projected changes to their financial situation and see a red, amber or green (RAG) rating against it.</p><p>Custom dashboards are for personal use and <span className="govuk-!-font-weight-bold">only visible to you. Any changes you make will be viewable on subsequent visits to this school’s dashboard unless you choose to reset them.</p>' />
              </span>
            </div>
        </div>
    )
}
