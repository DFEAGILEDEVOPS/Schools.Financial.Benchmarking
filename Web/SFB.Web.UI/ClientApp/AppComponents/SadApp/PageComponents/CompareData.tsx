import SadTable, {EstablishmentSadTableRow} from "./SadTable";
import {prepDataRows} from "../helpers/SadDataPrep";
import {SadDataObject} from "../Models/SadEditForm";
import {Link} from "react-router-dom";
import SadCompareTable from "./SadCompareTable";


interface ICompareProps {
    scenario0: SadDataObject
    scenario1: SadDataObject
    scenario0Label?: string
    scenario1Label?: string
    removeScenarioHandler: (idx: 0 | 1) => void
    outcomes: EstablishmentSadTableRow[]
}
export default function CompareData(
    {
        scenario0,
        scenario1,
        scenario0Label,
        scenario1Label,
        removeScenarioHandler,
        outcomes
    }: ICompareProps) {
    console.log(JSON.stringify(scenario0.SadAssesmentAreas, null, 2));
    console.log(JSON.stringify(scenario1, null, 2));
    const s0Data = prepDataRows(scenario0);
    const s1Data = prepDataRows(scenario1);

    const rr0 = s0Data.filter((area) => {
        return area?.AssessmentAreaType === "Reserve and balance";
    });
    const rr1 = s1Data.filter((area) => {
        return area?.AssessmentAreaType === "Reserve and balance"
    });
    // console.log(JSON.stringify(rr0, null, 2))
    // console.log(JSON.stringify(rr1, null, 2))

    const rrData = rr0.map((item, i) => {
        return {
            scenario0: item,
            scenario1: rr1[i]
        }
    });
    const ex0 = s0Data.filter((area) => {
        return area?.AssessmentAreaType === "Spending";
    });
    const ex1 = s1Data.filter((area) => {
        return area?.AssessmentAreaType === "Spending";
    });

    const exData = ex0.map((item, i) => {
        return {
            scenario0: item,
            scenario1: ex1[i]
        }
    });

    const ch0 = s0Data.filter((area) => {
        return area?.AssessmentAreaType === "School characteristics";
    });
    const ch1 = s1Data.filter((area) => {
        return area?.AssessmentAreaType === "School characteristics";
    });
    const characteristicsData = ch0.map((item, i) => {
        return {
            scenario0: item,
            scenario1: ch1[i]
        }
    });
    return (
        <div className="govuk-grid-row">
            <div className="govuk-grid-column-one-quarter-from-desktop">&nbsp;</div>
            <div className="govuk-grid-column-one-third">
                <h2 className="govuk-heading-m">
                    {scenario0.userLabel}
                </h2>
                <div className="govuk-caption-m">
                    Scenario year {scenario0.LatestTerm}
                </div>
                <div className="sfb-sad-compare--actions">
                    <Link to='/edit0'>Edit</Link> &#124;
                    <button className="link-button" onClick={()=> removeScenarioHandler(0) }>
                        Remove
                    </button>
                </div>
            </div>
            <div className="govuk-grid-column-one-third">
                <h2 className="govuk-heading-m">
                    {scenario1.userLabel}
                </h2>
                <div className="govuk-caption-m">
                    Scenario year {scenario1.LatestTerm}
                </div>
                <div className="sfb-sad-compare--actions">
                    <Link to='/edit1'>Edit</Link> &#124;
                    <button className="link-button" onClick={()=> removeScenarioHandler(1) }>
                        Remove
                    </button>
                </div>
            </div>
            <div className="govuk-grid-column-full sfb-sad-compare">
                {rrData &&
                  <SadCompareTable data={rrData} mode="income" captionText="Reserve and balance" />
                }
                {exData &&
                  <SadCompareTable data={exData} mode="expenditure" captionText="Spending" />
                }
                {characteristicsData &&
                  <SadCompareTable data={characteristicsData} mode="characteristics" captionText="School characteristics" />
                }
                {outcomes &&
                  <SadTable data={outcomes} mode="outcomes" captionText="Outcomes"/>
                }

            </div>
        </div>
    )
}
