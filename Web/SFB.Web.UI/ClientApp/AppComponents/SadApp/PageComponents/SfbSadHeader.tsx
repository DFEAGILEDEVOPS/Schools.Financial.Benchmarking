interface ISadHeaderProps {
    heading: string;
    schoolName: string;
    urn: number;
    dashboardYear: string;
    editDate?: Date;
    financeType?: string;
    suppressDataDescription?: boolean;
}

export type typesOfFinance = "Maintained" | "Academies" | "Federation";
export default function SfbSadHeader(
    {
        heading,
        schoolName,
        urn,
        dashboardYear,
        editDate,
        financeType,
        suppressDataDescription,
    }: ISadHeaderProps) {

    return (
        <div className="govuk-grid-row">
            <div className="govuk-grid-column-full govuk-!-padding-top-4">
                <h1 className="govuk-heading-xl">{heading}</h1>
                {editDate &&
                  <div  id="dateCaption" className="govuk-caption-m date-caption">
                    Page last edited: {editDate.toLocaleString()}
                  </div>
                }
            </div>
            <div className="govuk-grid-column-full">
                <p className="govuk-body-s sfb-highlight-container">
                    Assessing&nbsp;
                    <strong className="sfb-highlight">
                        <a href={`/school?urn=${urn}`}
                           className="govuk-link govuk-link--inverse">{schoolName}</a>
                    </strong>
                    {!suppressDataDescription &&
                      <>
                        &nbsp;using&nbsp;
                        <span className="govuk-!-font-weight-bold">
                        {dashboardYear}
                    </span>&nbsp;finance data.
                          {financeType === "Academies" &&
                            <>&nbsp;This data includes the academy's share of <strong>MAT central finance</strong>.</>
                          }
                      </>
                    }
                </p>

            </div>
        </div>
    )
}
