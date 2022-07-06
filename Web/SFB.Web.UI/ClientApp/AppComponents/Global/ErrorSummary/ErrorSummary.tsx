import React from "react";
declare var errorMessages: errorMessage[];
export type errorMessage = {
  text: string;
  target: string;
}

interface Props {
  errorTitle?: string;
  errors: errorMessage[];
}


export default function ErrorSummary ({errorTitle: title, errors}: Props) {
  if (!title) {
    title = 'There is a problem';
  }
  
  
  return (
    <div className="error-summary" role="alert" aria-labelledby="ErrorSummaryHeading">
      <h1 id="ErrorSummaryHeading" className="heading-medium error-summary-heading">
        {title}
      </h1>
      <ul className="error-summary-list">
        {
          errors.map((error, i) => {
            return (
              <li key={i}>
                <a href={error.target}>{error.text}</a>
              </li>
            )
          })
        }
      </ul>
    </div>
  )
}