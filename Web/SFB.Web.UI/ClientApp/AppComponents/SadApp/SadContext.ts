import {createContext} from "react";
import {SadDataObject} from "../SadTrustApp/Models/sadTrustTablesModels";

interface ISadContext {
    initialData: SadDataObject;
    editedData?: SadDataObject;
    customData?: SadDataObject;
}
