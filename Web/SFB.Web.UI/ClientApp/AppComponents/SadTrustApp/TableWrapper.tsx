import {SadTableRow} from './Models/sadTrustTablesModels';
import SadTrustTable from './SadTrustTable';
interface Props {
  tableData: SadTableRow[];
  mode: string;
  dataFormat?: string;
  captionText: string;
  phaseFilter: string;
  resetPhaseFilter: () => void;
}
export default function TableWrapper(props:Props) {
  return (
    <div>
      <SadTrustTable
        tableData={props.tableData}
        mode={props.mode} 
        captionText={props.captionText} 
        dataFormat={props.dataFormat} 
        phaseFilter={props.phaseFilter}
        resetPhaseFilter={props.resetPhaseFilter}/>
    </div>
    
  )
}