import {EstablishmentSadTableRow} from "../PageComponents/SadTable";

export interface ITableRowData {
    rrData: EstablishmentSadTableRow[]
    exData: EstablishmentSadTableRow[]
    characteristicsData: EstablishmentSadTableRow[],
}
export const prepRows = (data: EstablishmentSadTableRow[]): ITableRowData => {
    const rrData: EstablishmentSadTableRow[] = data.filter((area) => {
        return area?.AssessmentAreaType === "Reserve and balance";
    });
    const exData: EstablishmentSadTableRow[] = data.filter((area) => {
        return area?.AssessmentAreaType === "Spending";
    });
    const characteristicsData: EstablishmentSadTableRow[] = data.filter((area) => {
        return area?.AssessmentAreaType === "School characteristics";
    });
    return {
        rrData,
        exData,
        characteristicsData
    }
}
