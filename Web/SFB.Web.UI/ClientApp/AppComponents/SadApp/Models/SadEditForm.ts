import {Dispatch, SetStateAction} from "react";

export interface IFormValues {
    "Name of dashboard": string;
    "Year of dashboard": string;
    "Number of pupils (FTE)": number;
    "School workforce (FTE)": number;
    "Number of teachers (FTE)": number;
    "Senior leadership (FTE)": number;
    "Teacher contact ratio (less than 1)"?: number;
    "Predicted percentage pupil number change in 3-5 years"?: number;
    "Average class size"?: number;
    "Total income": number;
    "Total expenditure": number;
    "Revenue reserve"?: number;
    "Spending on teaching staff"?: number;
    "Spending on supply staff"?: number;
    "Spending on education support staff"?: number;
    "Spending on administrative and clerical staff"?: number;
    "Spending on other staff costs"?: number;
    "Spending on premises costs"?: number;
    "Spending on educational supplies"?: number;
    "Spending on energy"?: number;
    "StoreScenario"?: boolean;
    "isEdited"?: boolean;
}

export interface SadDataObject {
    Urn: number;
    Name: string,
    FinanceType: string;
    LondonWeighting: string;
    NumberOfPupilsLatestTerm: number;
    OfstedRating: string | number;
    OfstedInspectionDate: string | Date;
    P8Score: number;
    Ks2Score?: number,
    ProgressScoreType: string;
    Progress8Banding: number,
    FSMLatestTerm: number;
    LatestTerm:string;
    HasSixthForm: boolean;
    IsReturnsComplete: boolean;
    DoReturnsExist: boolean;
    OverallPhase: string;
    TotalExpenditureLatestTerm: number;
    TotalIncomeLatestTerm: number;
    TeachersTotalLastTerm: number;
    TeachersLeaderLastTerm: number;
    WorkforceTotalLastTerm: number;
    AvailableScenarioTerms: string[];
    SadSizeLookup: SadSizeLookUp;
    SadFSMLookup: SadFSMLookup;
    SadAssesmentAreas: SadAssessmentArea[];
    lastEdit?: Date;
    userLabel?: string;
}
export interface SadSizeLookUp {
    Term: string;
    OverallPhase: string;
    HasSixthForm: boolean;
    NoPupilsMin: number;
    NoPupilsMax: number;
    SizeType: string;
}
export type Dispatcher<S> = Dispatch<SetStateAction<S | null>>
export interface SadFSMLookup {
    Term: string;
    OverallPhase: string;
    HasSixthForm: boolean;
    FSMMin: number;
    FSMMax: number;
    FSMScale: string;
}

export interface SadThreshold {
    Term: string;
    ScoreLow: number;
    ScoreHigh: number;
    RatingText: string;
    RatingColour: string;
}

export interface SadAssessmentArea {
    AssessmentAreaType: string;
    AssessmentAreaName: string;
    SchoolDataLatestTerm?: number;
    SchoolData?: number;
    TotalForAreaTypeLatestTerm?: number;
    AllTresholds: SadThreshold[];
}

export interface SadHelpText {
    id: number;
    title: string;
    content: string;
    assessmentArea: string;
}

export const numberWithCommas = (x?: string): string => {
    return x ? x.replace(/\B(?=(\d{3})+(?!\d))/g, ",") : '';
}

export const numberToTwoDp = (n: number, dp = 2): string => {
    return (Math.round(n * 100) / 100).toFixed(2);
}
