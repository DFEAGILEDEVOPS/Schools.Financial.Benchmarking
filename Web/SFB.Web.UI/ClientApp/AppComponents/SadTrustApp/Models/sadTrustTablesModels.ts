import {OfstedRating, ProgressScore} from "../../Helpers/OutcomesHelpers";

// TODO refactor this + establishment SAD to correct typos in keys
export interface SadSizeLookUp {
  Term: string;
  OverallPhase: string;
  HasSixthForm: boolean;
  NoPupilsMin: number;
  NoPupilsMax: number;
  SizeType: string;
}

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

export interface SadTableRow {
  urn: number;
  schoolName: string;
  schoolData?: number;
  percentageOfIncome?: number;
  percentageOfExpenditure?: number;
  thresholdRating?: SadThreshold;
  thresholdSortFigure?: number;
  thresholds?: SadThreshold[];
  progressScore?: ProgressScore;
  ofstedRating?: OfstedRating;
  helpText?: SadHelpText;
  hasFullData: boolean;
  phase?: string
}

export interface SadHelpText {
  id: number;
  title: string;
  content: string;
  assessmentArea: string;
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
}
