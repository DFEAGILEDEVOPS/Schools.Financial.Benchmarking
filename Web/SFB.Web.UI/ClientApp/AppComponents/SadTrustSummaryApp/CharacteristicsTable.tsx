import React from 'react';

export interface CharacteristicsRow {
  name: string
  phase: string
  phaseComparator: string
  londonWeighting: string
  weightingComparator: string
  numberOfPupils: number
  pupilsComparator: string
  fsm: number | string
  fsmComparator: string
}

interface Props {
  rowData: CharacteristicsRow[]
}

export default function CharacteristicsTable({rowData}: Props) {
  return (
    <table className="govuk-table sfb-comparison-characteristics-table">
      <caption className="govuk-table__caption govuk-!-font-weight-regular">
        This benchmark was created using the following criteria
      </caption>
      <thead className="govuk-table__head">
      <tr className="govuk-table__row">
        <th className="govuk-table__header sfb-comparison-characteristics-table-cell__name">School</th>
        <th className="govuk-table__header">Characteristic</th>
        <th className="govuk-table__header">Your school</th>
        <th className="govuk-table__header">Comparison criteria</th>
      </tr>
      </thead>
      {rowData.map((row, i) => (
        <tbody className="govuk-table__body" key={`body-${i}`}>
          <tr key={`school-${i}`} className="govuk-table__row">
            <td className="govuk-table__cell" rowSpan={4}>{row.name}</td>
            <td className="govuk-table__cell">School phase</td>
            <td className="govuk-table__cell">{row.phase}</td>
            <td className="govuk-table__cell">{row.phaseComparator}</td>
          </tr>
          <tr key={`school-weighting-${i}`} className="govuk-table__row">
            <td className="govuk-table__cell">London weighting</td>
            <td className="govuk-table__cell">{row.londonWeighting}</td>
            <td className="govuk-table__cell">{row.weightingComparator}</td>
          </tr>
          <tr key={`pupils-${i}`} className="govuk-table__row">
            <td className="govuk-table__cell">Number of pupils</td>
            <td className="govuk-table__cell">{row.numberOfPupils}</td>
            <td className="govuk-table__cell">{row.pupilsComparator}</td>
          </tr>
          <tr key={`fsm-${i}`} className="govuk-table__row">
            <td className="govuk-table__cell">FSM</td>
            <td className="govuk-table__cell">{row.fsm}%</td>
            <td className="govuk-table__cell">{row.fsmComparator}</td>
          </tr>
        </tbody>
      ))}
    </table>
  )
}

