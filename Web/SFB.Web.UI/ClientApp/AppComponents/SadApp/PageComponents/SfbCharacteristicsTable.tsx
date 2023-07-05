import {numberToTwoDp, SadFSMLookup, SadSizeLookUp} from "../Models/SadEditForm";

interface ISfbCharacteristicsTable {
    financeType?: "Maintained" | "Academies" | "Federation";
    overallPhase: string;
    londonWeighting: "Neither" | "London";
    pupilsLatestTerm?: number;
    fsmLatestTerm?: number;
    sadSizeLookup?: SadSizeLookUp;
    sadFsmLookup?: SadFSMLookup;

}

export default function SfbCharacteristicsTable(
    {
        financeType,
        overallPhase,
        londonWeighting,
        sadSizeLookup,
        sadFsmLookup,
        fsmLatestTerm,
        pupilsLatestTerm,

    }: ISfbCharacteristicsTable
) {
    return (
        <table className="govuk-table govuk-!-font-size-16" aria-describedby="criteriaText">
            <caption id="criteriaText"  className="govuk-table__caption">
                This benchmark was created using the following criteria.
            </caption>
            <thead className="govuk-table__head">
            <tr className="govuk-table__row">
                <th scope="col" className="govuk-table__header">Characteristic</th>
                <th scope="col" className="govuk-table__header">
                    {financeType === "Federation" && <>Your federation</> }
                    {financeType !== "Federation" && <>Your school</> }
                </th>
                <th scope="col" className="govuk-table__header">Comparison criteria</th>
            </tr>
            </thead>
            <tbody className="govuk-table__body">
            <tr className="govuk-table__row">
                <td className="govuk-table__cell">
                    {financeType === "Federation" && <>Phase</> }
                    {financeType !== "Federation" && <>School phase</> }
                </td>
                <td className="govuk-table__cell">{overallPhase}</td>
                <td className="govuk-table__cell">{overallPhase}</td>
            </tr>
            <tr className="govuk-table__row">
                <td className="govuk-table__cell">London weighting</td>
                <td className="govuk-table__cell" id="lw1">
                    {londonWeighting === "Neither" ?
                        <>Not London</>:
                        <>London</>
                    }
                </td>
                <td className="govuk-table__cell" id="lw2">
                    {londonWeighting === "Neither" ?
                        <>Not London</>:
                        <>London</>
                    }
                </td>
            </tr>
            {sadSizeLookup !== null &&
                <tr className="govuk-table__row">
                  <td className="govuk-table__cell">Number of pupils</td>
                {pupilsLatestTerm &&
                  <td className="govuk-table__cell">{numberToTwoDp(pupilsLatestTerm)}</td>
                }
                {sadSizeLookup?.NoPupilsMax !== null &&
                    <td className="govuk-table__cell">
                      Schools with {numberToTwoDp(sadSizeLookup?.NoPupilsMin!)} -
                        {numberToTwoDp(sadSizeLookup?.NoPupilsMax!)}
                    </td>
                }
                {sadSizeLookup?.NoPupilsMax === null &&
                  <td className="govuk-table__cell">
                    Schools with {numberToTwoDp(sadSizeLookup?.NoPupilsMin!)} or more pupils
                  </td>
                }
                </tr>
            }
            {sadFsmLookup !== null &&
                <tr className="govuk-table__row">
                  <td className="govuk-table__cell">FSM</td>
                  <td className="govuk-table__cell">{fsmLatestTerm}</td>
                  <td className="govuk-table__cell">
                      Schools with {sadFsmLookup?.FSMMin}% - {sadFsmLookup?.FSMMax}% FSM
                  </td>
                </tr>
            }
            </tbody>
        </table>
    )
}
