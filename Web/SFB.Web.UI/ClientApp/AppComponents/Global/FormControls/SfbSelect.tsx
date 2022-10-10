import React, {ChangeEvent} from 'react';

interface Props {
  label: string;
  id: string;
  options: (string | undefined)[];
  onChange: (e: ChangeEvent<HTMLSelectElement>) => void;
  skipLinkTarget?: string;
  value: string;
}

export default function SfbSelect(props: Props) {
  return (
    <div className="govuk-form-group">
      <label className="govuk-label" htmlFor={props.id}>
        {props.label}
      </label>
      <select className="govuk-select" id={props.id} name={props.id} onChange={props.onChange} value={props.value}>
        {props.options.map(option => (
          <option value={option} key={option?.toLowerCase()}>{option}</option>
          ))
        }
      </select>
      {props.skipLinkTarget &&
        <a href={`#${props.skipLinkTarget}`} className="govuk-skip-link" data-module="govuk-skip-link">Skip to table</a>
      }
      
    </div>
  )
}