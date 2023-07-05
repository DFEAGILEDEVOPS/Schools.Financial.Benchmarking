
interface ISfbWarningProps {
    message: string;
}
export default function SfbWarningMessage({message}: ISfbWarningProps) {
    return (
        <div className="govuk-warning-text" id="partialWarning">
            <span className="govuk-warning-text__icon" aria-hidden="true">!</span>
            <strong className="govuk-warning-text__text">
                <span className="govuk-warning-text__assistive">Warning</span>
                <span id="partialWarning__text">{message}</span>
            </strong>
        </div>
    )
}
