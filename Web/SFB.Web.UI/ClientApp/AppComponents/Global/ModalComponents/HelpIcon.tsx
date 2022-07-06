import React from "react";

interface Props {
  onClick: Function;
  buttonId?: string;
}

export default function HelpIcon(props: Props) {
  const buttonId = props.buttonId || 'btn-help-icon'
  return (
    <span className="help-icon no-margin hide-in-print">
        <span className="icon dark-blue">
            <a className="govuk-link helpLink hide-in-print" href="#" id={props.buttonId}
               onClick={()=> props.onClick}>
            </a>
        </span>
    </span>
  );
} 