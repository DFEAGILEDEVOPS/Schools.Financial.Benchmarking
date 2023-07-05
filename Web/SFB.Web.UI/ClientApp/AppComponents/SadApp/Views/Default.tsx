import SfbSadHeader, {typesOfFinance} from "../PageComponents/SfbSadHeader";
import {SadDataObject} from "../Models/SadEditForm";
import SfbCharacteristicsTable from "../PageComponents/SfbCharacteristicsTable";
import SfbSadViewActions from "../PageComponents/SfbSadViewActions";
import ViewDefaultData from "../PageComponents/ViewDefaultData";
import {ITableRowData} from "../helpers/prepRows";
import {EstablishmentSadTableRow} from "../PageComponents/SadTable";
import SfbWarningMessage from "../PageComponents/SfbWarningMessage";

declare var initialData: SadDataObject;

interface IDefaultSadProps {
    estabCustomData: SadDataObject | null;
    removeScenarioHandler: (idx: 0 | 1) => void;
    scenarioRows: ITableRowData | null;
    outcomesData?: EstablishmentSadTableRow[];
}

export default function Default(
    {
        estabCustomData,
        removeScenarioHandler,
        scenarioRows,
        outcomesData,
    }: IDefaultSadProps
) {
    console.log(JSON.stringify(estabCustomData, null, 2));
    console.log(estabCustomData?.lastEdit);
    return (
        <>
            <SfbSadHeader
                dashboardYear={initialData.LatestTerm}
                heading="Self-assessment dashboard"
                urn={initialData.Urn}
                schoolName={initialData.Name}
                financeType={initialData.FinanceType}
                editDate={estabCustomData?.lastEdit}
            />

            {!initialData.IsReturnsComplete &&
              <div className="govuk-grid-row">
                <div className="govuk-grid-column-full">
                  <SfbWarningMessage
                    message={`This school doesn't have a complete
                     set of financial data for this period.
                     You should edit the values with more up to date information.`} />
                </div>
              </div>
            }

            <div className="govuk-grid-row">
                <div className="govuk-grid-column-full">
                    <details className="govuk-details" data-module="govuk-details">
                        <summary className="govuk-details__summary hide-in-print">
                            <span className="govuk-details__summary-text govuk-!-font-size-16">
                              View characteristics used
                            </span>
                        </summary>
                        <div className="govuk-details__text govuk-!-font-size-16 show-in-print" id="criteriaTables">
                            <SfbCharacteristicsTable
                                overallPhase={initialData.OverallPhase}
                                // @ts-ignore
                                londonWeighting={initialData.LondonWeighting}
                                sadSizeLookup={initialData.SadSizeLookup}
                                sadFsmLookup={initialData.SadFSMLookup}
                                fsmLatestTerm={initialData.FSMLatestTerm}
                                pupilsLatestTerm={initialData.NumberOfPupilsLatestTerm}/>
                        </div>
                    </details>
                </div>
            </div>

            <SfbSadViewActions
                estabCustomData={estabCustomData}
                removeScenarioHandler={removeScenarioHandler}
                scenarioIndex={0}
            />

            <ViewDefaultData
                LatestTerm={initialData.LatestTerm}
                rrData={scenarioRows?.rrData}
                exData={scenarioRows?.exData}
                characteristicsData={scenarioRows?.characteristicsData}
                outcomesData={outcomesData}/>

        </>
    )
}
