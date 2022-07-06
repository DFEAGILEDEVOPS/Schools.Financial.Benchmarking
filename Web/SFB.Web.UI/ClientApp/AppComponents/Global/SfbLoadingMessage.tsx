import React, {useState} from 'react';
import './sfbLoadingMessage.scss';

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
            <span className="govuk-body govuk-!-margin-left-2">Loading...</span>
        </div>
    );
}