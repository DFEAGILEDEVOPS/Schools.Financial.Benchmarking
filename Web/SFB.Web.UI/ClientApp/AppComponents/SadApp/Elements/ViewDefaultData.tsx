import SfbSadHelpModal from "../../Global/ModalComponents/SfbSadHelpModal";
import SadTable, {EstablishmentSadTableRow} from "../SadTable";
import React from "react";
import {Link} from "react-router-dom";

interface IDefaultViewProps {
    LatestTerm: string;
    rrData?: EstablishmentSadTableRow[];
    exData?: EstablishmentSadTableRow[];
    characteristicsData?: EstablishmentSadTableRow[];
    outcomesData?: EstablishmentSadTableRow[];
}

export default function ViewDefaultData(
    {
        LatestTerm,
        rrData,
        exData,
        characteristicsData,
        outcomesData
    }: IDefaultViewProps) {

    return (
        <div className="govuk-grid-row">
            <div className="govuk-grid-column-full">
                <h2 className="govuk-heading-m">
                    {LatestTerm} submitted data
                </h2>
                <div className="govuk-caption-m">
                    Dashboard year {LatestTerm}
                    <span className="inline-help-container">
                      <SfbSadHelpModal
                          modalContent="By choosing a different year banding figures are aligned to that year for published finance, Future years use the most recent bands and can have uplifts applied to specific expenditure areas where there is an expectation of significant expenditure changes such pending salary awards."
                          modalTitle="Dashboard year"
                      />
                    </span>
                </div>
                <Link to="edit">Edit data</Link>
            </div>
            <div className="govuk-grid-column-full">
                {rrData &&
                    <SadTable data={rrData} mode="income" captionText="Reserve and balance"/>
                }
                {exData &&
                    <SadTable data={exData} mode="expenditure" captionText="Spending"/>
                }
                {characteristicsData &&
                    <SadTable data={characteristicsData} mode="characteristics" captionText="School characteristics"/>
                }
                {outcomesData &&
                    <SadTable data={outcomesData} mode="outcomes" captionText="Outcomes"/>
                }

            </div>
        </div>
    )
}