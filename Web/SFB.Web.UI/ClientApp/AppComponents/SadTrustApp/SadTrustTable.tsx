import {SadTableRow} from './Models/sadTrustTablesModels';
import {
  Cell,
  ColumnDef,
  flexRender,
  getCoreRowModel,
  getSortedRowModel, 
  Header,
  SortingFn,
  SortingState,
  useReactTable,
} from '@tanstack/react-table'

import {useLocation} from 'react-router-dom';
import {CSSProperties, useEffect, useMemo, useState} from 'react';
import SfbLoadingMessage from '../Global/SfbLoadingMessage';
import SfbSadHelpModal from '../Global/ModalComponents/SfbSadHelpModal';
import {numberWithCommas} from '../Helpers/formatHelpers';
import CookieManager from '../../AppModules/CookieManager';

declare var saBaseUrl: string;
declare global {
  interface Window { ga: any; }
}
let acceptedTrackingCookies = false;
const policyCookie = CookieManager.getCookie('cookies_policy');
if (policyCookie) {
  const cookieSettings = JSON.parse(policyCookie);
  acceptedTrackingCookies = cookieSettings.usage;
}
declare module '@tanstack/table-core' {
  interface SortingFns {
    incomeThresholdSorting?: SortingFn<unknown>,
    expenditureThresholdSorting?: SortingFn<unknown>,
    outcomesSorting?: SortingFn<unknown>,
  }
}

interface Props {
  tableData: SadTableRow[];
  mode: string;
  dataFormat?: string;
  captionText: string;
  phaseFilter: string;
  resetPhaseFilter: () => void;
}

interface IIncomeThresholdSortingLookUp {
  text: string;
  value: number;
}

const lookUp: IIncomeThresholdSortingLookUp[] = [
  {text: 'lowrisk', value: 0},
  {text: 'mediumrisk', value: 1},
  {text: 'highrisk', value: 2}
];

function IncompleteDataModal(props: { schoolName: string;}) {
  return (
    <SfbSadHelpModal
      establishmentName={props.schoolName}
      useExclaimIcon={true}
      modalTitle="Incomplete financial data"
      modalContent="<p>This school doesn't have a complete set in financial data for this period</p>" />
  );
}

let isInitial = true;
export default function SadTrustTable({tableData, mode, dataFormat, captionText, phaseFilter, resetPhaseFilter}: Props) {
 
  const [isLoading, setIsLoading] = useState<boolean>(false);
  const location = useLocation();
  const [sorting, setSorting] = useState<SortingState>([{
    'id': "schoolName",
    'desc': false
  }]);
  const [data, setData] = useState<SadTableRow[]>(tableData);
  
  useEffect(() => {
    if (acceptedTrackingCookies) {
      window.ga('set', 'page', window.location.toString());
      window.ga('send', 'pageview');
    }
    if (!isInitial) {
      setIsLoading(true);
      resetPhaseFilter();
      setData(tableData);
      window.scrollTo(0, 0);
      const table = document.querySelector('.sfb-sadtrust-table');
      if (table instanceof HTMLTableElement) {
        table.focus();
      }
      const timer = setTimeout(() => {
        setIsLoading(false);
      }, 1000);
      return () => clearTimeout(timer);
    } else {
      setData(tableData);
    }
    isInitial = false;
  }, [location]);
  
  useEffect(() => {
    if (phaseFilter !== 'All') {
      setData(tableData.filter((row) => {
        return row.phase === phaseFilter;
      }));
    } else {
      setData(tableData);
    }
  },[phaseFilter])

  const incomeColumns = useMemo<ColumnDef<SadTableRow>[]>(
    () => [
      {
        header: 'School',
        accessorKey: 'schoolName',
        cell: ({row}) => {
          return (
            <>
              <a href={`${saBaseUrl}${row.original.urn}`} className="table-school-name-link">
                {row.original.schoolName}</a>
              {!(row.original.hasFullData) &&
                <IncompleteDataModal schoolName={row.original.schoolName} />
              }
            </>
          )},
        id: 'schoolName',
        sortingFn: 'text',
      },
      {
        header: 'School data',
        accessorKey: 'schoolData',
        cell: ({row}) => {
          if (typeof row.original.schoolData !== 'undefined' && row.original.schoolData) {
            return `£${numberWithCommas(row.original.schoolData.toFixed(2))}`;
          }
          return "-"
        },
        sortingFn: 'alphanumericCaseSensitive',
      },
      {
        header: ()=> <>% of <abbr title="income">inc</abbr></>,
        accessorKey: 'percentageOfIncome',
        cell: ({row}) => {
          if (typeof row.original.percentageOfIncome !== 'undefined' && row.original.schoolData) {
            return `${numberWithCommas(row.original.percentageOfIncome.toFixed(2))}%`;
          }
          return "-"
        },
        sortingFn: 'alphanumericCaseSensitive',
      },
      {
        header: 'Rating against thresholds',
        id: 'ratingAgainstThresholds',
        accessorFn: row => row.thresholdRating?.RatingText,
        cell: ({row}) => (
          <div className="rating-container">
            <div className={`rating-box ${row.original.thresholdRating?.RatingColour}`}>
              {row.original.thresholdRating?.RatingText}
            </div>
            <SfbSadHelpModal
              modalTitle={row.original.helpText?.title}
              modalContent={row.original.helpText?.content}
              establishmentName={row.original.schoolName}
              establishmentThreshold={row.original.thresholdRating}
              thresholds={row.original.thresholds}
              columnHeading="% of income"
              unitFormat="percentage" />
          </div>
        ),
        sortingFn: 'incomeThresholdSorting',
      }
    ],
    []
  );
  
  const expenditureColumns = useMemo<ColumnDef<SadTableRow>[]>(
    () => [
      {
        header: 'School',
        accessorKey: 'schoolName',
        cell: ({row}) => {
          return (
            <>
              <a href={`${saBaseUrl}${row.original.urn}`} className="table-school-name-link">
                {row.original.schoolName}</a>
              {!(row.original.hasFullData) &&
                <IncompleteDataModal schoolName={row.original.schoolName} />
              }
            </>
          )},
        id: 'schoolName',
        sortingFn: 'text',
      },
      {
        header: 'School data',
        accessorKey: 'schoolData',
        cell: ({row}) => {
          if (typeof row.original.schoolData !== 'undefined' && row.original.schoolData) {
            return `£${numberWithCommas(row.original.schoolData.toFixed(2))}`;
          }
          return "-"
        },
        sortingFn: 'alphanumericCaseSensitive',
      },
      {
        header: () => <>% of <abbr title="expenditure">exp.</abbr></> ,
        accessorKey: 'percentageOfExpenditure',
        cell: ({row}) => {
          if (typeof row.original.percentageOfExpenditure !== 'undefined' && row.original.schoolData) {
            return `${numberWithCommas(row.original.percentageOfExpenditure.toFixed(2))}%`;
          }
          return "-"
        },
        sortingFn: 'alphanumericCaseSensitive',
      },
      {
        header: 'Rating against thresholds',
        id: 'ratingAgainstThresholds',
        accessorFn: row => row.thresholdRating?.RatingText,
        cell: ({row}) => (
          <div className="rating-container">
            <div className={`rating-box ${row.original.thresholdRating?.RatingColour}`}>
              {row.original.thresholdRating?.RatingText}
            </div>
            <SfbSadHelpModal
              modalTitle={row.original.helpText?.title}
              modalContent={row.original.helpText?.content}
              establishmentName={row.original.schoolName}
              establishmentThreshold={row.original.thresholdRating}
              thresholds={row.original.thresholds}
              columnHeading='% of expenditure'
              unitFormat="percentage" />
          </div>
        ),
        sortingFn: 'expenditureThresholdSorting',
      }
    ],
    []
  );
  const characteristicsColumns = useMemo<ColumnDef<SadTableRow>[]>(
    () => [
      {
        header: 'School',
        accessorKey: 'schoolName',
        cell: ({row}) => {
          return (
            <>
              <a href={`${saBaseUrl}${row.original.urn}`} className="table-school-name-link">
                {row.original.schoolName}</a>
              {!(row.original.hasFullData) &&
                <IncompleteDataModal schoolName={row.original.schoolName} />
              }
            </>
          )},
        id: 'schoolName',
        sortingFn: 'text',
      },
      {
        header: 'School data',
        accessorKey: 'schoolData',
        cell: ({row}) => {
          const value = row.original.schoolData;
          if (value) {
            if (dataFormat === 'currency') {
              return `£${numberWithCommas(value.toFixed(2))}`;
            } else if (dataFormat === 'percentage') {
              return `${value.toFixed(1)}%`;
            } else {
              return `${value.toFixed(1)}`;
            }
          }
          return '-';
        },
        sortingFn: 'alphanumericCaseSensitive',
      },
      {
        header: 'Rating against thresholds',
        id: 'ratingAgainstThresholds',
        accessorFn: row => row.thresholdRating?.RatingText,
        cell: ({row}) => (
          <div className="rating-container">
            <div className={`rating-box ${row.original.thresholdRating?.RatingColour}`}>
              {row.original.thresholdRating?.RatingText}
            </div>
            <SfbSadHelpModal
              modalTitle={row.original.helpText?.title}
              modalContent={row.original.helpText?.content}
              establishmentName={row.original.schoolName}
              establishmentThreshold={row.original.thresholdRating}
              thresholds={row.original.thresholds}
              columnHeading='% of expenditure'
              unitFormat={dataFormat} />
          </div>
        )
      }
    ],
    []
  );
  
  const outcomesColumns = useMemo<ColumnDef<SadTableRow>[]>(
    () => [
      {
        header: 'School',
        accessorKey: 'schoolName',
        cell: ({row}) => {
          return (
            <>
              <a href={`${saBaseUrl}${row.original.urn}`} className="table-school-name-link">
                {row.original.schoolName}</a>
              {!(row.original.hasFullData) &&
                <IncompleteDataModal schoolName={row.original.schoolName} />
              }
            </>
          )},
        id: 'schoolName',
        sortingFn: 'text',
      },
      {
        header: 'School data',
        accessorKey: 'schoolData',
        cell: ({row}) => {
          if (location.pathname === '/OfstedRating') {
            const classNames = `ofsted-rating ofsted-rating-${row.original.ofstedRating?.score}`;
            return (
              <div className="ofsted-rating--container">
                <div className="ofsted-rating__detail">
                  {(row.original.ofstedRating && row.original.ofstedRating?.score > 0) &&
                    <>
                      <div className="ofsted-rating--score">
                        <div className={classNames}>{row.original.ofstedRating?.score}</div>
                        <div className="ofsted-rating--text">{row.original.ofstedRating?.ratingText}</div>
                      </div>
                      <a target="_blank" className="ofsted-rating--report-link"
                         href={`https://reports.ofsted.gov.uk/inspection-reports/find-inspection-report/provider/ELS/${row.original.urn}`}>
                        Ofsted report
                      </a>
                      <span>Inspected {row.original.ofstedRating?.reportDate}</span>
                    </>
                  }
                  {(row.original.ofstedRating && row.original.ofstedRating?.score == 0) &&
                    <div className="ofsted-rating--score ofsted-rating--unavailable">
                      <div className={classNames}>No data available</div>
                    </div>
                  }
                </div>
                  <SfbSadHelpModal
                    modalTitle={row.original.helpText?.title}
                    modalContent={row.original.helpText?.content}
                    establishmentName={row.original.schoolName}/>
              </div>
            )
          }
          // progress scores
          return (
            <div className="progress-score-wrapper">
              <div className={`progress-score-container  ${row.original.progressScore?.className}`}>
                {row.original.progressScore?.score?.toFixed(2)} {row.original.progressScore?.text}
              </div>
              <SfbSadHelpModal
                modalTitle={row.original.helpText?.title}
                modalContent={row.original.helpText?.content}
                establishmentName={row.original.schoolName} />
            </div>
          )
        },
        sortingFn: 'outcomesSorting',
      },
    ],
    []
  );
  
  function setColumns() {
    if (mode === 'income') {
      return incomeColumns;
    } else if (mode === 'expenditure') {
      return expenditureColumns;
    } else if (mode === 'characteristics') {
      return characteristicsColumns;
    } else {
      return outcomesColumns;
    }
  }

  const table = useReactTable({
    data,
    columns: setColumns(),
    state: {
      sorting,
    },
    sortingFns: {
      incomeThresholdSorting: (rowA: any, rowB: any, columnId: any): number => {
        const sortValueA = lookUp.filter((item) => item.text === rowA.getValue(columnId).toLowerCase().replaceAll(' ', ''))[0].value;
        const sortValueB = lookUp.filter((item) => item.text === rowB.getValue(columnId).toLowerCase().replaceAll(' ', ''))[0].value;
        return sortValueA < sortValueB ? 1 : -1;
      },
      expenditureThresholdSorting: (rowA:any, rowB: any,columnId:any) : number => {
        return rowA.original.thresholdSortFigure < rowB.original.thresholdSortFigure ? 1 : -1;
      },
      outcomesSorting: (rowA:any, rowB: any, columnId: any) : number => {
        return rowA.original.thresholdSortFigure < rowB.original.thresholdSortFigure ? 1 : -1;
      }
    },
    onSortingChange: setSorting,
    getCoreRowModel: getCoreRowModel(),
    getSortedRowModel: getSortedRowModel(),
    enableSortingRemoval: false,
  });
  
  const visibilityCss = {
    visibility: isLoading? 'hidden' : 'visible'
  } as CSSProperties;

  
  const headerCellClassNames = (header: Header<any, any>): string => {
    let out = '';
    if (header.column.getCanSort()) {
      out += 'sfb-sort-table__button ';
    }
    out += 'sfb-sort-table__button-'+ header.getContext().column.id;
    out += {
      asc: ' sorted-asc',
      desc: ' sorted-desc',
    }[header.column.getIsSorted() as string] ?? '';
    
    return out;
  }
  
  const getDataLabelAttribute = (cell: Cell<any,any>) => {
    const cellId = cell.column.id;
    switch (cellId) {
      case 'schoolName':
        return 'School name';
      case 'schoolData':
        return 'School data';
      case 'percentageOfExpenditure':
        return 'Percentage of expenditure';
      case 'percentageOfIncome':
        return 'Percentage of Income';
      case 'ratingAgainstThresholds':
        return 'Rating against thresholds';
    }
  }
  
  return (
    <>
      {isLoading &&
        (<SfbLoadingMessage message={"loading..."} isLoading={isLoading}/>)
      }
      {!isLoading && (
        <div style={visibilityCss}>
          {/*<pre>{JSON.stringify(sorting, null, 2)}</pre>*/}
        <table className="govuk-table sfb-sadtrust-table" tabIndex={-1}>
          <caption className="govuk-table__caption govuk-table__caption--m" aria-live="assertive">
            {captionText}
          </caption>
          <thead className="govuk-table__head">
          {table.getHeaderGroups().map(headerGroup => (
            <tr key={headerGroup.id} className="govuk-table__row">
              {headerGroup.headers.map(header => (
                <th key={header.id} colSpan={header.colSpan} className="govuk-table__header sfb-sort-table__header">
                  {header.isPlaceholder ? null : (
                    <div
                      {...{
                        className: headerCellClassNames(header),
                        onClick: header.column.getToggleSortingHandler(),
                      }}
                    >
                      <>{flexRender(
                        header.column.columnDef.header,
                        header.getContext()
                      )}</>
                    </div>
                  )}
                </th>
              ))}
            </tr>
          ))}
          </thead>
          <tbody className="govuk-table__body">
          {table.getRowModel().rows.map(row => (
            <tr key={row.id} className="govuk-table__row">
              {row.getVisibleCells().map((cell, i) => (
                <td key={cell.id} className="govuk-table__cell" data-label={getDataLabelAttribute(cell)}>
                  {flexRender(cell.column.columnDef.cell, cell.getContext())}
                </td>
              ))}
            </tr>
          ))}
          {(table.getRowModel().rows.length === 0) &&
            <tr className="govuk-table__row">
              <td className="govuk-table__cell" colSpan={table.getAllColumns().length}>
                <p>No results. You can try changing the phase filter. </p>
              </td>
            </tr>
          }
          </tbody>
        </table>
        </div>
      )}
    </>
  );
}
