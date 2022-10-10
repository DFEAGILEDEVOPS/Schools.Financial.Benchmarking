import React from 'react';

interface Props {
    message: string;
    isLoading: boolean;
}
export default function SfbLoadingMessage({ message, isLoading }: Props) {
    return (
        <div className="sfb-loading--container" aria-hidden={!isLoading} hidden={!isLoading}>
            <div className="lds-spinner">
                <div></div>
                <div></div>
                <div></div>
                <div></div>
                <div></div>
                <div></div>
                <div></div>
                <div></div>
                <div></div>
                <div></div>
                <div></div>
                <div></div>
            </div>
            <span role="alert" aria-live="assertive" aria-label={message}></span>
            <div className="govuk-body govuk-!-margin-left-7 govuk-!-margin-top-1">Loading...</div>
        </div>
    );
}