
interface ISfbBreadcrumbsProps {
    urn: number,
    schoolName: string,
}
export default function SfbBreadcrumbs({urn, schoolName}: ISfbBreadcrumbsProps) {

    return (
        <div className="govuk-breadcrumbs">
            <ol className="govuk-breadcrumbs__list">
                <li className="govuk-breadcrumbs__list-item">
                    <a href="/" className="govuk-breadcrumbs__link">Home</a>
                </li>
                <li className="govuk-breadcrumbs__list-item">
                    <a href={`/school?urn=${urn.toString()}`}>{schoolName}</a>
                </li>
            </ol>
        </div>
    )
}
