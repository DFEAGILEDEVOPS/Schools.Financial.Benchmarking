import {Link} from "react-router-dom";
import SfbSadHelpModal from "../../Global/ModalComponents/SfbSadHelpModal";
import React from "react";

export default function ViewHeader() {
    return (
        <div className="govuk-grid-row">
            <div className="govuk-grid-column-one-half">
                <button className="sfb-button--download">Download page</button>
                <button className="sfb-button--print">Print page</button>
            </div>
            <div className="govuk-grid-column-one-half">
                <Link to="customise" className="govuk-button">Add a custom dashboard</Link>
                <span className="inline-help-container">
                <SfbSadHelpModal
                    modalTitle="Add a custom dashboard"
                    modalContent='<p>The custom dashboard allows schools to plan for hypothetical or projected changes to their financial situation and see a red, amber or green (RAG) rating against it.</p><p>Custom dashboards are for personal use and <span className="govuk-!-font-weight-bold">only visible to you. Any changes you make will be viewable on subsequent visits to this school’s dashboard unless you choose to reset them.</p>' />
              </span>
            </div>
        </div>
    )
}