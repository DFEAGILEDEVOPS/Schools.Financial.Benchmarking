import {Cell, ColumnDef, createColumnHelper, flexRender, getCoreRowModel, useReactTable} from '@tanstack/react-table';
import {useMemo, useState} from "react";
import {isOfstedRating, isProgressScore, OfstedRating, ProgressScore} from "../Models/OutcomesHelpers";
import {SadHelpText, SadThreshold} from "../Models/SadEditForm"
import SfbSadHelpModal from "../../Global/ModalComponents/SfbSadHelpModal";
import {numberWithCommas} from "../Models/SadEditForm";
import {Link} from "react-router-dom";
declare var initialData: any;
export interface EstablishmentSadTableRow {
    AssessmentAreaType: string
    AssessmentArea: string
    SchoolData?: string | number | OfstedRating | ProgressScore
    TotalForAreaTypeLatestTerm?: string | number
    percentageOfIncome?: number | string
    percentageOfExpenditure?: number | string
    rating?: SadThreshold
    helpText?: SadHelpText
    thresholds?: SadThreshold[]
    dataFormat?: "currency" | "percent" | "number"
}

const getEditLinkHash = (label: string):string => {
    switch (label) {
        case 'Teacher contact ratio (less than 1)':
            return 'teacher-contact-ratio-less-than-1';
        case 'Predicted percentage pupil number change in 3-5 years':
            return 'predicted-percentage-pupil-number-change-in-3-5-years';
        case 'Average Class size':
            return 'average-class-size';
        default:
            return '';
    }
}

const columnHelper = createColumnHelper<EstablishmentSadTableRow>();

const reserveBalanceColumns = [
    columnHelper.accessor('AssessmentArea', {
        cell: info => info.getValue(),
        header: () => <>Assessment area</>
    }),
    columnHelper.accessor('SchoolData', {
        cell: info => {
            const cellValue = info.getValue();
            if (typeof cellValue === 'string') {
                return `£${numberWithCommas(parseFloat(cellValue).toFixed(2))}`;
            } else if (typeof cellValue === 'number') {
                return `£${numberWithCommas(cellValue.toFixed(2))}`;
            }
        },
        header: () => <>School data</>
    }),
    columnHelper.accessor('percentageOfIncome', {
        cell: info => {
            const rawValue = info.getValue();
            if (typeof rawValue === 'number') {
                return `${rawValue.toFixed(1)}%`;
            }
            if (typeof rawValue === 'string') {
                return `${parseFloat(rawValue).toFixed(1)}%`;
            }
        },
        header: () => <>% of <abbr title="income">inc.</abbr></>
    }),
    columnHelper.accessor('rating', {
        cell: info => {
            const ratingText = info.getValue()?.RatingText ?
                info.getValue()?.RatingText : <>Not available <br/><Link to={`/edit`}>Add data</Link></>;
            return (
                <div className="rating-container">
                    <div className={`rating-box ${info.getValue()?.RatingColour}`}>
                        {ratingText}
                    </div>
                    <SfbSadHelpModal
                        modalTitle={info.row.original.helpText?.title}
                        modalContent={info.row.original.helpText?.content}
                        establishmentThreshold={info.row.original.rating}
                        thresholds={info.row.original.thresholds}
                        columnHeading="% of income"
                        unitFormat="percentage"/>
                </div>
            )},
        header: () => <>Rating against thresholds</>
    }),
];

const spendingColumns = [
    columnHelper.accessor('AssessmentArea', {
        cell: info => info.getValue(),
        header: () => <>Assessment area</>
    }),
    columnHelper.accessor('SchoolData', {
        cell: info => {
            const cellValue = info.getValue();
            if (typeof cellValue === 'string') {
                return `£${numberWithCommas(parseFloat(cellValue).toFixed(2))}`;
            } else if (typeof cellValue === 'number') {
                return `£${numberWithCommas(cellValue.toFixed(2))}`;
            }
        },
        header: () => <>School data</>
    }),
    columnHelper.accessor('percentageOfExpenditure', {
        cell:  info => {
            const rawValue = info.getValue();
            if (typeof rawValue === 'number') {
                return `${rawValue.toFixed(1)}%`;
            }
            if (typeof rawValue === 'string') {
                return `${parseFloat(rawValue).toFixed(1)}%`;
            }
        },
        header: () => <>% of <abbr title="expenditure">exp.</abbr></>
    }),
    columnHelper.accessor('rating', {
        cell: info => {
            const ratingText = info.getValue()?.RatingText ?
                info.getValue()?.RatingText : <>Not available <br/><Link to={`/edit`}>Add data</Link></>;
            return(
                <div className="rating-container">
                    <div className={`rating-box ${info.getValue()?.RatingColour}`}>
                        {ratingText}
                    </div>
                    <SfbSadHelpModal
                        modalTitle={info.row.original.helpText?.title}
                        modalContent={info.row.original.helpText?.content}
                        establishmentThreshold={info.row.original.rating}
                        thresholds={info.row.original.thresholds}
                        columnHeading="% of expenditure"
                        unitFormat="percentage"/>
                </div>
            )},
        header: () => <>Rating against thresholds</>
    }),
];

const characteristicsColumns  = [
    columnHelper.accessor('AssessmentArea', {
        cell: info => info.getValue(),
        header: () => <>Assessment area</>
    }),
    columnHelper.accessor('SchoolData', {
        cell: ({row}) => {
            const cellValue = row.original.SchoolData as number;
            const format= row.original.dataFormat;
            if (cellValue) {
                if (format === 'percent') {
                    return `${cellValue.toFixed(1)}%`;
                } else if (format === 'currency') {
                    return `£${numberWithCommas(cellValue.toFixed(2))}`;
                } else {
                    return cellValue.toFixed(2);
                }
            }
        } ,
        header: () => <>School data</>
    }),
    columnHelper.accessor('rating', {
        cell: info => {
            const hash = getEditLinkHash(info.row.original.AssessmentArea);
            const ratingText = info.getValue()?.RatingText ?
                info.getValue()?.RatingText : <>Not available <br/><Link to={`/edit#${hash}`}>Add data</Link></>;
            return(
                <div className="rating-container">
                    <div className={`rating-box ${info.getValue()?.RatingColour}`}>
                        {ratingText}
                    </div>
                    <SfbSadHelpModal
                        modalTitle={info.row.original.helpText?.title}
                        modalContent={info.row.original.helpText?.content}
                        establishmentThreshold={info.row.original.rating}
                        thresholds={info.row.original.thresholds}
                        columnHeading="% of expenditure"
                        unitFormat="percentage"/>
                </div>
            )
        },
        header: () => <>Rating against thresholds</>
    }),
];

const outcomesColumns = [
    columnHelper.accessor('AssessmentArea', {
        cell: info => info.getValue(),
        header: () => <>Assessment area</>
    }),
    columnHelper.accessor('SchoolData', {
        header: () => <>School data</>,
        cell: ({row}) => {
            if (isOfstedRating(row.original.SchoolData)) {
                const ofstedRating = row.original.SchoolData as OfstedRating;
                return (
                    <div className="ofsted-rating--container">
                        <div className="ofsted-rating__detail">
                            {(ofstedRating && ofstedRating?.score > 0) &&
                              <>
                                <div className="ofsted-rating--score">
                                  <div className={`ofsted-rating ofsted-rating-${ofstedRating?.score}`}>{ofstedRating?.score}&nbsp;</div>
                                  <div className="ofsted-rating--text">{ofstedRating?.ratingText}&nbsp;</div>
                                </div>
                                  {ofstedRating.urn &&
                                    <a target="_blank" className="ofsted-rating--report-link"
                                       href={`https://reports.ofsted.gov.uk/inspection-reports/find-inspection-report/provider/ELS/${ofstedRating.urn}`}>
                                      Ofsted report
                                    </a>
                                  }
                                <span>Inspected {ofstedRating?.reportDate}</span>
                              </>
                            }
                            {(ofstedRating && ofstedRating?.score == 0) &&
                              <div className="ofsted-rating--score ofsted-rating--unavailable">
                                <div className={`ofsted-rating ofsted-rating-${ofstedRating?.score}`}>No data available</div>
                              </div>
                            }
                        </div>
                        <SfbSadHelpModal
                            modalTitle={row.original.helpText?.title}
                            modalContent={row.original.helpText?.content} />
                    </div>
                )
            } else if (isProgressScore(row.original.SchoolData)) {
                const progressScore = row.original.SchoolData as ProgressScore;
                return (
                    <div className="progress-score-wrapper">
                        <div className={`progress-score-container  ${progressScore?.className}`}>
                            {progressScore?.score?.toFixed(2)} {progressScore?.text}
                        </div>

                        <SfbSadHelpModal
                            modalTitle={row.original.helpText?.title}
                            modalContent={row.original.helpText?.content} />
                    </div>
                )
            }
        },
    }),
];

export const getDataLabelAttribute = (cell: Cell<any, any>) => {
    const cellId = cell.column.id;
    switch (cellId) {
        case 'SchoolData':
        case 'scenario0_SchoolData':
        case 'scenario1_SchoolData':
            return 'School data';

        case 'percentageOfExpenditure':
        case 'scenario0_percentageOfExpenditure':
        case 'scenario1_percentageOfExpenditure':
            return 'Percentage of expenditure';

        case 'percentageOfIncome':
        case 'scenario0_percentageOfIncome':
        case 'scenario1_percentageOfIncome':
            return 'Percentage of income';

        case 'rating':
        case 'scenario0_rating':
        case 'scenario1_rating':
            return 'Rating against thresholds';
    }
}
interface Props {
    data: EstablishmentSadTableRow[],
    captionText?: string
    mode: 'income' | 'expenditure' | 'characteristics' | 'outcomes'
}
export default function SadTable({data, mode, captionText}: Props) {

    function setColumns() {
        if (mode === 'income') {
            return reserveBalanceColumns;
        } else if (mode === 'expenditure') {
            return spendingColumns;
        } else if (mode === 'characteristics') {
            return characteristicsColumns;
        } else {
            return outcomesColumns;
        }
    }

    const [tableData, setTableData] = useState<EstablishmentSadTableRow[]>( [...data] );
    const table = useReactTable({
        data: tableData,
        columns: setColumns(),
        getCoreRowModel: getCoreRowModel(),
        enableSorting: false,
        sortingFns: {
            incomeThresholdSorting: () => { return 0 },
            expenditureThresholdSorting: () => { return 0 },
            outcomesSorting: () => { return 0 },
        }

    })
    return (
        <div>
            <table className="govuk-table sfb-establishment-sad-table">
                {captionText &&
                  <caption className="govuk-table__caption govuk-table__caption--m">
                      {captionText}
                  </caption>
                }
                <thead className="govuk-table__head">
                {table.getHeaderGroups().map(headerGroup => (
                    <tr key={headerGroup.id} className="govuk-table__row">
                        {headerGroup.headers.map(header => (
                            <th key={header.id} className="govuk-table__header">
                                {header.isPlaceholder
                                    ? null
                                    : flexRender(
                                        header.column.columnDef.header,
                                        header.getContext()
                                    )}
                            </th>
                        ))}
                    </tr>
                ))}
                </thead>
                <tbody className="govuk-table__body">
                {table.getRowModel().rows.map(row => (
                    <tr key={row.id} className="govuk-table__row">
                        {row.getVisibleCells().map((cell, i) => {
                            if (i === 0) {
                                return (
                                    <th key={cell.id} className="govuk-table__header">
                                        {flexRender(cell.column.columnDef.cell, cell.getContext())}
                                    </th>
                                )
                            }
                            const dataLabel = getDataLabelAttribute(cell);
                            return (
                                <td key={cell.id} className="govuk-table__cell" data-label={dataLabel}>
                                    {flexRender(cell.column.columnDef.cell, cell.getContext())}
                                </td>
                            )
                        })}
                    </tr>
                ))}
                </tbody>
            </table>
        </div>
    )
}
