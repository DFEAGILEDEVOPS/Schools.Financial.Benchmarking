import {Cell, ColumnDef, createColumnHelper, getCoreRowModel, useReactTable} from '@tanstack/react-table';
import {useMemo, useState} from "react";
import {OfstedRating, ProgressScore} from "../Helpers/OutcomesHelpers";
import {SadThreshold} from "../SadTrustApp/Models/sadTrustTablesModels";
import SfbSadHelpModal from "../Global/ModalComponents/SfbSadHelpModal";
declare var initialData: any;
interface SadTableRow {
  AssessmentArea: string
  SchoolData?: string | number | OfstedRating | ProgressScore
  percentageOfIncome?: number
  percentageOfExpenditure?: number
  rating: SadThreshold
}

const columnHelper = createColumnHelper<SadTableRow>();

const reserveBalanceColumns = [
  columnHelper.accessor('AssessmentArea', {
    cell: info => info.getValue(),
    header: () => <>Assessment area</>
  }),
  columnHelper.accessor('SchoolData', {
    cell: info => info.getValue(),
    header: () => <>School data</>
  }),
  columnHelper.accessor('percentageOfIncome', {
    cell: info => info.getValue(),
    header: () => <>% of<abbr title="income">inc.</abbr></>
  }),
  columnHelper.accessor('rating', {
    cell: info => (
      <div className="rating-container">
        <div className={`rating-box ${info.getValue()?.RatingColour}`}>
          {info.getValue()?.RatingText}
        </div>
        
      </div>
    ),
    header: () => <>Rating against thresholds</>
  }),
];

interface Props {
  initialData: SadTableRow[],
}
export default function SadTable() { 
  const [tableData, setTableData] = useState<SadTableRow[]>( [...initialData] );
  const table = useReactTable({
    data: tableData,
    columns: reserveBalanceColumns,
    getCoreRowModel: getCoreRowModel(),
  })
  return (
    
  )


}