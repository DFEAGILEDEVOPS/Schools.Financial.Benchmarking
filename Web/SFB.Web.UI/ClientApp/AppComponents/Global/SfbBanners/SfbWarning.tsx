
interface Props {
  message: string
}
export default function SfbWarning({message} : Props) {
  
  return (
    <div className="govuk-inset-text app-govuk-inset-text--orange ">
        <div className="govuk-warning-text">
          <span className="govuk-warning-text__icon" aria-hidden="true">!</span>
          <strong className="govuk-warning-text__text">
            <span className="govuk-warning-text__assistive">Warning</span>{message}</strong>
      </div>
    </div>
  )
}