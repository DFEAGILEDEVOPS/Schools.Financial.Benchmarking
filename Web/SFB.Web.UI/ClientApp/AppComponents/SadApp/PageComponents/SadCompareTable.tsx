import {EstablishmentSadTableRow, getDataLabelAttribute} from "./SadTable";
import {createColumnHelper, flexRender, getCoreRowModel, useReactTable} from "@tanstack/react-table";
import {numberWithCommas} from "../Models/SadEditForm";
import SfbSadHelpModal from "../../Global/ModalComponents/SfbSadHelpModal";
import {Fragment, useState} from "react";
import {Link} from "react-router-dom";


export interface IEstablishmentSadCompareRow {
    scenario0: EstablishmentSadTableRow;
    scenario1: EstablishmentSadTableRow;
}
const columnHelper = createColumnHelper<IEstablishmentSadCompareRow>();

const reserveBalanceColumns = [
    columnHelper.accessor('scenario0.AssessmentArea', {
        cell: info => info.getValue(),
        header: () => <>Assessment area</>
    }),
    columnHelper.accessor('scenario0.SchoolData', {
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
    columnHelper.accessor('scenario0.percentageOfIncome', {
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
    columnHelper.accessor('scenario0.rating', {
        cell: info => {
            const ratingText = info.getValue()?.RatingText ?
                info.getValue()?.RatingText : <>Not available <br/><Link to={'/edit'}>Add data</Link></>;
            return (
                <div className="rating-container">
                    <div className={`rating-box ${info.getValue()?.RatingColour}`}>
                        {ratingText}
                    </div>
                    <SfbSadHelpModal
                        modalTitle={info.row.original.scenario0.helpText?.title}
                        modalContent={info.row.original.scenario0.helpText?.content}
                        establishmentThreshold={info.row.original.scenario0.rating}
                        thresholds={info.row.original.scenario0.thresholds}
                        columnHeading="% of income"
                        unitFormat="percentage"/>
                </div>
            )},
        header: () => <>Rating against thresholds</>
    }),
    columnHelper.accessor('scenario1.SchoolData', {
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
    columnHelper.accessor('scenario1.percentageOfIncome', {
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
    columnHelper.accessor('scenario1.rating', {
        cell: info => {
            const ratingText = info.getValue()?.RatingText ?
                info.getValue()?.RatingText : <>Not available <br/><Link to={'/customise'}>Add data</Link></>;
            return (
                <div className="rating-container">
                    <div className={`rating-box ${info.getValue()?.RatingColour}`}>
                        {ratingText}
                    </div>
                    <SfbSadHelpModal
                        modalTitle={info.row.original.scenario1.helpText?.title}
                        modalContent={info.row.original.scenario1.helpText?.content}
                        establishmentThreshold={info.row.original.scenario1.rating}
                        thresholds={info.row.original.scenario1.thresholds}
                        columnHeading="% of income"
                        unitFormat="percentage"/>
                </div>
            )},
        header: () => <>Rating against thresholds</>
    }),
];

const spendingColumns = [
    columnHelper.accessor('scenario0.AssessmentArea', {
        cell: info => info.getValue(),
        header: () => <>Assessment area</>
    }),
    columnHelper.accessor('scenario0.SchoolData', {
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
    columnHelper.accessor('scenario0.percentageOfExpenditure', {
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
    columnHelper.accessor('scenario0.rating', {
        cell: info => {
            const ratingText = info.getValue()?.RatingText ?
                info.getValue()?.RatingText : <>Not available <br/><Link to={'/edit'}>Add data</Link></>;
            return(
                <div className="rating-container">
                    <div className={`rating-box ${info.getValue()?.RatingColour}`}>
                        {ratingText}
                    </div>
                    <SfbSadHelpModal
                        modalTitle={info.row.original.scenario0.helpText?.title}
                        modalContent={info.row.original.scenario0.helpText?.content}
                        establishmentThreshold={info.row.original.scenario0.rating}
                        thresholds={info.row.original.scenario0.thresholds}
                        columnHeading="% of expenditure"
                        unitFormat="percentage"/>
                </div>
            )},
        header: () => <>Rating against thresholds</>
    }),
    columnHelper.accessor('scenario1.SchoolData', {
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
    columnHelper.accessor('scenario1.percentageOfExpenditure', {
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
    columnHelper.accessor('scenario1.rating', {
        cell: info => {
            const ratingText = info.getValue()?.RatingText ?
                info.getValue()?.RatingText : <>Not available <br/><Link to={'/customise'}>Add data</Link></>;
            return(
                <div className="rating-container">
                    <div className={`rating-box ${info.getValue()?.RatingColour}`}>
                        {ratingText}
                    </div>
                    <SfbSadHelpModal
                        modalTitle={info.row.original.scenario1.helpText?.title}
                        modalContent={info.row.original.scenario1.helpText?.content}
                        establishmentThreshold={info.row.original.scenario1.rating}
                        thresholds={info.row.original.scenario1.thresholds}
                        columnHeading="% of expenditure"
                        unitFormat="percentage"/>
                </div>
            )},
        header: () => <>Rating against thresholds</>
    }),
];

const characteristicsColumns  = [
    columnHelper.accessor('scenario0.AssessmentArea', {
        cell: info => info.getValue(),
        header: () => <>Assessment area</>
    }),
    columnHelper.accessor('scenario0.SchoolData', {
        cell: ({row}) => {
            const cellValue = row.original.scenario0.SchoolData as number;
            const format= row.original.scenario0.dataFormat;
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
    columnHelper.accessor('scenario0.rating', {
        cell: info => {
            const ratingText = info.getValue()?.RatingText ?
                info.getValue()?.RatingText : <>Not available <br/><Link to={'/edit'}>Add data</Link></>;
            return(
                <div className="rating-container">
                    <div className={`rating-box ${info.getValue()?.RatingColour}`}>
                        {ratingText}
                    </div>
                    <SfbSadHelpModal
                        modalTitle={info.row.original.scenario0.helpText?.title}
                        modalContent={info.row.original.scenario0.helpText?.content}
                        establishmentThreshold={info.row.original.scenario0.rating}
                        thresholds={info.row.original.scenario0.thresholds}
                        columnHeading="% of expenditure"
                        unitFormat="percentage"/>
                </div>
            )
        },
        header: () => <>Rating against thresholds</>
    }),
    columnHelper.accessor('scenario1.SchoolData', {
        cell: ({row}) => {
            const cellValue = row.original.scenario1.SchoolData as number;
            const format= row.original.scenario1.dataFormat;
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
    columnHelper.accessor('scenario1.rating', {
        cell: info => {
            const ratingText = info.getValue()?.RatingText ?
                info.getValue()?.RatingText : <>Not available <br/><Link to={'/customise'}>Add data</Link></>;
            return(
                <div className="rating-container">
                    <div className={`rating-box ${info.getValue()?.RatingColour}`}>
                        {ratingText}
                    </div>
                    <SfbSadHelpModal
                        modalTitle={info.row.original.scenario1.helpText?.title}
                        modalContent={info.row.original.scenario1.helpText?.content}
                        establishmentThreshold={info.row.original.scenario1.rating}
                        thresholds={info.row.original.scenario1.thresholds}
                        columnHeading="% of expenditure"
                        unitFormat="percentage"/>
                </div>
            )
        },
        header: () => <>Rating against thresholds</>
    }),
];

interface Props {
    data: IEstablishmentSadCompareRow[],
    captionText?: string,
    mode: 'income' | 'expenditure' | 'characteristics'
}
export default function SadCompareTable({data, captionText, mode}: Props) {
    function setColumns() {
        if (mode === 'income') {
            return reserveBalanceColumns;
        }
        else if (mode === 'expenditure') {
            return spendingColumns;
        }
        else {
            return characteristicsColumns;
        }
    }

    const [tableData, setTableData] = useState<IEstablishmentSadCompareRow[]>([...data]);

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
    });

    return (
        <div>
            <table className="govuk-table sfb-establishment-sad-table sfb-establishment-sad-compare-table">
                {captionText &&
                  <caption className="govuk-table__caption govuk-table__caption--m">
                      {captionText}
                  </caption>
                }
                <thead className="govuk-table__head">
                {table.getHeaderGroups().map(headerGroup => (
                    <tr key={headerGroup.id} className="govuk-table__row">
                        {headerGroup.headers.map((header, i) => {
                            let cellClass = header.id.indexOf('percentage') > -1 ?
                                "govuk-table__header sfb-sad-compare-percent-cell" :
                                "govuk-table__header";
                            cellClass = header.id.indexOf('rating') > -1 ?
                                `${cellClass} sfb-sad-compare-rating-cell`:
                                cellClass;
                            cellClass = i > 0 ? `${cellClass}  sfb-fill-header` : `${cellClass} sfb-sad-compare-row-header`;
                            return (
                                <Fragment key={`frag-${header.id}`}>
                                    {header.id === 'scenario1_SchoolData' &&
                                      <td className="sfb-sad-compare-spacer">&nbsp;</td>
                                    }
                                    <th key={header.id} className={cellClass}>
                                        {header.isPlaceholder
                                            ? null
                                            : flexRender(
                                                header.column.columnDef.header,
                                                header.getContext()
                                            )}
                                    </th>
                                </Fragment>
                            )
                        })}
                    </tr>
                ))}
                </thead>
                <tbody className="govuk-table__body">
                {table.getRowModel().rows.map(row => (
                    <tr key={row.id} className="govuk-table__row">
                        {row.getVisibleCells().map((cell, i) => {
                            if (i === 0) {
                                return (
                                    <th key={cell.id} className="govuk-table__header sfb-sad-compare-row-header">
                                        {flexRender(cell.column.columnDef.cell, cell.getContext())}
                                    </th>
                                )
                            }
                            const dataLabel = getDataLabelAttribute(cell);
                            let cellClass = cell.id.indexOf('percentage') > -1 ?
                                "govuk-table__cell sfb-sad-compare-percent-cell" :
                                "govuk-table__cell";

                            cellClass = cell.id.indexOf('rating') > -1 ?
                                `${cellClass} sfb-sad-compare-rating-cell`:
                                cellClass;
                            return (
                                <Fragment key={cell.id}>
                                    {cell.id.indexOf('scenario1_SchoolData') > -1 &&
                                      <td className="sfb-sad-compare-spacer">&nbsp;</td>
                                    }
                                    <td className={cellClass} data-label={dataLabel}>
                                        {flexRender(cell.column.columnDef.cell, cell.getContext())}
                                    </td>
                                </Fragment>
                            )
                        })}
                    </tr>
                ))}

                </tbody>
            </table>
        </div>
    )
}
