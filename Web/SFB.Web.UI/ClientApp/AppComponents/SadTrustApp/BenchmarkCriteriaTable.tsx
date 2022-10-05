import React from 'react';
import {SadDataObject} from './Models/sadTrustTablesModels';

interface Props {
  academies: SadDataObject[],
  isDownload?: boolean
}
export default function BenchmarkCriteriaTable({academies, isDownload}: Props) {
  const renderSadSizeRow = function(academy: SadDataObject){
    const pupilsMax = academy.SadSizeLookup.NoPupilsMax;
    const pupilsMin = academy.SadSizeLookup.NoPupilsMin;
    const cellText = pupilsMax !== null ?
      `Schools with ${pupilsMin} - ${pupilsMax} pupils` :
      `Schools with ${pupilsMin} or more pupils`;
    
    return (
      <tr className="govuk-table__row" key={`key-${academy.Urn}-sadsize`}>
        <td className="govuk-table__cell">Number of pupils</td>
        <td className="govuk-table__cell">{academy.NumberOfPupilsLatestTerm}</td>
        <td className="govuk-table__cell">{cellText}</td>
      </tr>
    );
  };
  
  const renderFsmRow = function (academy:SadDataObject) {
    const formattedFsm = academy.FSMLatestTerm.toFixed(1);
    const fsmMin = academy.SadFSMLookup.FSMMin.toFixed(1);
    const fsmMax = academy.SadFSMLookup.FSMMax.toFixed(1);
    
    return (
        <tr className="govuk-table__row" key={`key-${academy.Urn}-fsm`}>
          <td className="govuk-table__cell">FSM</td>
          <td className="govuk-table__cell">
            {formattedFsm}%
          </td>
          <td className="govuk-table__cell">
            Schools with {fsmMin} - {fsmMax}% FSM
          </td>
        </tr>
      );
  }
  return (
    <table className="govuk-table sfb-comparison-characteristics-table">
      <caption className="govuk-table__caption">
        This benchmark was created using the following criteria.
      </caption>
      <thead className="govuk-table__head">
      {isDownload && 
        <tr className="govuk-table__row">
          <th colSpan={4}>This benchmark was created using the following criteria.</th>
        </tr>
      }
      <tr className="govuk-table__row">
        <th scope="col" className="govuk-table__header sfb-comparison-characteristics-table-cell__name">School</th>
        <th scope="col" className="govuk-table__header">Characteristic</th>
        <th scope="col" className="govuk-table__header">Your school</th>
        <th scope="col" className="govuk-table__header">Comparison criteria</th>
      </tr>
      </thead>
      <tbody className="govuk-table__body">
      {academies && academies.map((academy) => {
        let rowSpan = 2;
        if (academy.SadSizeLookup !== null) {
          rowSpan++;
        }
        if (academy.SadFSMLookup !== null) {
          rowSpan++;
        }
        return (
          <React.Fragment key={`key-${academy.Urn}-base`}>
            <tr className="govuk-table__row sfb-table__row--top-border" key={`key-${academy.Urn}-base`}>
              <td className="govuk-table__cell" rowSpan={rowSpan}>{academy.Name}</td>
              <td className="govuk-table__cell">School phase</td>
              <td className="govuk-table__cell">{academy.OverallPhase}</td>
              <td className="govuk-table__cell">{academy.OverallPhase}</td>
            </tr>
            <tr className="govuk-table__row" key={`key-${academy.Urn}-weighting`}>
              <td className="govuk-table__cell">London weighting</td>
              <td className="govuk-table__cell">{academy.LondonWeighting == "Neither" ? "Not London" : "London"}</td>
              <td className="govuk-table__cell">{academy.LondonWeighting == "Neither" ? "Not London" : "London"}</td>
            </tr>
            {academy.SadSizeLookup && renderSadSizeRow(academy)}
            {academy.SadFSMLookup && renderFsmRow(academy)}
          </React.Fragment>
        )
      })}
      </tbody>
    </table>
  )
}