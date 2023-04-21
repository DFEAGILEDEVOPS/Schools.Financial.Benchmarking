import SadTable, {EstablishmentSadTableRow} from "./SadTable";
import {SadDataObject} from "../SadTrustApp/Models/sadTrustTablesModels";
import {
  getKs2ProgressBandingObject,
  getProgress8BandingObject,
  ofstedLookup,
  OfstedRating
} from "../Helpers/OutcomesHelpers";
import {fullMonths} from "../Helpers/DateTimeHelpers";
import SfbSadHelpModal from "../Global/ModalComponents/SfbSadHelpModal";

declare var initialData: SadDataObject;
declare var modalMap: any[];

const prepData = (initialData: SadDataObject): EstablishmentSadTableRow[] => {
  const data = initialData.SadAssesmentAreas.map((item) => {
    const out:EstablishmentSadTableRow = {
      AssessmentAreaType: item.AssessmentAreaType,
      AssessmentArea: item.AssessmentAreaName,
      thresholds: item.AllTresholds,
    }
    out.helpText = modalMap.filter((helpText: any) => {
      return helpText.AssessmentArea === item.AssessmentAreaName;
    }).map((res: any) => {
      return {
        id: res.Id,
        title: res.Title,
        assessmentArea: res.AssessmentArea,
        content: res.Content
      }
    })[0];
    
    if (item.AssessmentAreaType === "Reserve and balance") {
      let percentageOfIncome = "-";
      let banding;
      if (item.SchoolDataLatestTerm && item.TotalForAreaTypeLatestTerm) {
        const schoolValue = (item.SchoolDataLatestTerm / item.TotalForAreaTypeLatestTerm);
        percentageOfIncome = (schoolValue * 100).toFixed(1) + '%';
        banding = item.AllTresholds.find(
            t => (schoolValue >= t.ScoreLow || t.ScoreLow === null) &&
                (schoolValue <= t.ScoreHigh || t.ScoreHigh === null)
        );
      }
      out.SchoolData = item.SchoolDataLatestTerm;
      out.TotalForAreaTypeLatestTerm = item.TotalForAreaTypeLatestTerm;
      out.percentageOfIncome = percentageOfIncome;
      out.rating = banding;
      
      return out;
    } 
    
    else if(item.AssessmentAreaType === "Spending") {
      let percentageOfExpenditure = '-';
      let banding;
      if (item.SchoolDataLatestTerm && item.TotalForAreaTypeLatestTerm) {
        const schoolValue = parseFloat((item.SchoolDataLatestTerm / item.TotalForAreaTypeLatestTerm).toFixed(3));
        const percentageOfExpenditure = (schoolValue* 100).toFixed(1) + '%';
        banding = item.AllTresholds.find(
            t => (schoolValue >= t.ScoreLow || t.ScoreLow === null) &&
                (schoolValue <= t.ScoreHigh || t.ScoreHigh === null)
        );
        
        out.SchoolData = item.SchoolDataLatestTerm;
        out.TotalForAreaTypeLatestTerm = item.TotalForAreaTypeLatestTerm;
        out.percentageOfExpenditure = percentageOfExpenditure;
        out.rating = banding;
        
        return out;
      }
      return {
        AssessmentAreaType: item.AssessmentAreaType,
        AssessmentArea: item.AssessmentAreaName,
      };
    } else if (item.AssessmentAreaType === "School characteristics") {
      if (item.AssessmentAreaName === 'Average teacher cost') {
        const teacherTotalCount = initialData.TeachersTotalLastTerm;
        const teacherTotalCost = initialData.SadAssesmentAreas.find(aa => aa.AssessmentAreaName === "Teaching staff")?.SchoolDataLatestTerm;
        
        if (teacherTotalCost && teacherTotalCount) {
          const schoolValue = parseFloat((teacherTotalCost / teacherTotalCount).toFixed(3));
          const rating = item.AllTresholds.find(
              t => (schoolValue >= t.ScoreLow || t.ScoreLow === null) &&
                  (schoolValue <= t.ScoreHigh || t.ScoreHigh === null)
          );
          out.TotalForAreaTypeLatestTerm = schoolValue;
          out.SchoolData = schoolValue;
          out.rating = rating;
          out.dataFormat = 'currency';
        }
        return out;
      }
      
      if (item.AssessmentAreaName === 'Senior leaders as a percentage of workforce') {
        const ratingFigure = initialData.WorkforceTotalLastTerm;
        out.SchoolData = (initialData.TeachersLeaderLastTerm / ratingFigure) * 100;
        if (typeof ratingFigure !== 'undefined') {
          const ratingValue = +(initialData.TeachersLeaderLastTerm / ratingFigure).toFixed(3);
          out.rating = item.AllTresholds.find(t => (ratingValue >= t.ScoreLow || t.ScoreLow == null)
              && (ratingValue <= t.ScoreHigh || t.ScoreHigh == null));
        }
        out.dataFormat = 'percent';
        return out;
      }
      if (item.AssessmentAreaName === 'Pupil to teacher ratio') {
        if (typeof initialData.WorkforceTotalLastTerm !== 'undefined' && initialData.TeachersTotalLastTerm > 0) {
          const ratingFigure = initialData.NumberOfPupilsLatestTerm / initialData.TeachersTotalLastTerm;
          out.SchoolData = ratingFigure;
          out.rating = item.AllTresholds.find(t => (ratingFigure >= t.ScoreLow || t.ScoreLow == null)
              && (ratingFigure <= t.ScoreHigh || t.ScoreHigh == null));
        }
        return out;
      }
      
      if (item.AssessmentAreaName === 'Pupil to adult ratio') {
        if (initialData.WorkforceTotalLastTerm > 0) {
          const ratingValue = +(initialData.NumberOfPupilsLatestTerm / initialData.WorkforceTotalLastTerm).toFixed(3);
          out.SchoolData = ratingValue;
          out.rating = item.AllTresholds.find(t => (ratingValue >= t.ScoreLow || t.ScoreLow == null)
              && (ratingValue <= t.ScoreHigh || t.ScoreHigh == null));
        }
        out.dataFormat = 'number';
        return out;
      }
      
      if (item.AssessmentAreaName === 'Teacher contact ratio (less than 1)') {
        if (item.SchoolDataLatestTerm) {
          const ratingValue = parseFloat(item.SchoolDataLatestTerm.toFixed(2));
          out.SchoolData = ratingValue;
          out.rating = item.AllTresholds.find(t => (ratingValue >= t.ScoreLow || t.ScoreLow == null)
              && (ratingValue <= t.ScoreHigh || t.ScoreHigh == null));
        }
        out.dataFormat = 'number';
        return out;
      }
      
      if (item.AssessmentAreaName === 'Predicted percentage pupil number change in 3-5 years') {
        if (item.SchoolDataLatestTerm) {
          const ratingValue = parseFloat(item.SchoolDataLatestTerm.toFixed(2));
          out.SchoolData = ratingValue;
          out.rating = item.AllTresholds.find(t => (ratingValue >= t.ScoreLow || t.ScoreLow == null)
              && (ratingValue <= t.ScoreHigh || t.ScoreHigh == null));
        }
        out.dataFormat = 'percent';
        return out;
      }
      
      if (item.AssessmentAreaName === 'Average Class size') {
        if (item.SchoolDataLatestTerm) {
          const ratingValue = parseFloat(item.SchoolDataLatestTerm.toFixed(2));
          out.SchoolData = ratingValue;
          out.rating = item.AllTresholds.find(t => (ratingValue >= t.ScoreLow || t.ScoreLow == null)
              && (ratingValue <= t.ScoreHigh || t.ScoreHigh == null));
        }
        out.dataFormat = 'number';
        return out;
      }
      return {
        AssessmentAreaType: item.AssessmentAreaType,
        AssessmentArea: item.AssessmentAreaName,
      };
      
    } else {
      return {
        AssessmentArea: item.AssessmentAreaName,
        AssessmentAreaType: item.AssessmentAreaType,
        SchoolData: item.SchoolDataLatestTerm,
      }
    }
  });
  return data;
}
export default function SadApp() {
  const d = prepData(initialData);
  const rrData: EstablishmentSadTableRow[] = d.filter((area) => {
    return area?.AssessmentAreaType === "Reserve and balance";
  });
  const exData: EstablishmentSadTableRow[] = d.filter((area) => {
    return area?.AssessmentAreaType === "Spending";
  });
  const characteristicsData: EstablishmentSadTableRow[] = d.filter((area) => {
    return area?.AssessmentAreaType === "School characteristics";
  });
  const outcomesData: EstablishmentSadTableRow[] = [];
  
  const ofstedRating: OfstedRating = {
    score: initialData.OfstedRating as number,
    ratingText: ofstedLookup.filter((r) => {
      return r.score.toString() === initialData.OfstedRating
    })[0]?.ratingText
  };
  
  if (initialData.OfstedInspectionDate) {
    const inspectionDate = new Date(initialData.OfstedInspectionDate);
    ofstedRating.reportDate = `${inspectionDate.getDay()} ${fullMonths[inspectionDate.getMonth()]} ${inspectionDate.getFullYear()}`;
  }
  ofstedRating.urn = initialData.Urn;
  
  const ofstedRow: EstablishmentSadTableRow = {
    AssessmentArea: 'Ofsted rating',
    AssessmentAreaType: 'Outcomes',
    SchoolData: ofstedRating,
  }
  ofstedRow.helpText = modalMap.filter((item: any) => {
    return item.AssessmentArea === 'Ofsted'
  }).map((item: any) => {
    return {
      id: item.Id,
      title: item.Title,
      assessmentArea: item.AssessmentArea,
      content: item.Content
    }
  })[0];
  
  outcomesData.push(ofstedRow);
  
  if (initialData.ProgressScoreType === "All-through" 
      || initialData.ProgressScoreType ==="KS2 score") {
    const progressScore= getKs2ProgressBandingObject(initialData.Ks2Score as  number);
    const ks2Progress: EstablishmentSadTableRow = {
      AssessmentAreaType: 'Outcomes',
      AssessmentArea: 'KS2 Progress',
      SchoolData: progressScore,
    } 
    
    ks2Progress.helpText = modalMap.filter((item: any) => {
      return item.AssessmentArea === 'KS2'
    }).map((item: any) => {
      return {
        id: item.Id,
        title: item.Title,
        assessmentArea: item.AssessmentArea,
        content: item.Content
      }
    })[0];
    
    outcomesData.push(ks2Progress);
  }
  
  if (initialData.ProgressScoreType === "All-through" || 
      initialData.ProgressScoreType === 'Progress 8 score') {
    const progressScore = getProgress8BandingObject(initialData.Progress8Banding as number);
    const p8: EstablishmentSadTableRow = {
      AssessmentAreaType: 'Outcomes',
      AssessmentArea: 'Progress 8',
      SchoolData: progressScore,
    }
    
    p8.helpText = modalMap.filter((item: any) => {
      return item.AssessmentArea === 'P8'
    }).map((item: any) => {
      return {
        id: item.Id,
        title: item.Title,
        assessmentArea: item.AssessmentArea,
        content: item.Content
      }
    })[0];

    outcomesData.push(p8);
  }
  
  return (
      <>
        <div className="govuk-grid-row">
          <div className="govuk-grid-column-one-half">
            <button className="sfb-button--download">Download page</button>
            <button className="sfb-button--print">Print page</button>
          </div>
          <div className="govuk-grid-column-one-half">
            <a href="#" className="govuk-button">Add a custom dashboard</a>
            <span className="inline-help-container">
              <SfbSadHelpModal
                modalTitle="Add a custom dashboard"
                modalContent='<p>The custom dashboard allows schools to plan for hypothetical or projected changes to their financial situation and see a red, amber or green (RAG) rating against it.</p><p>Custom dashboards are for personal use and <span className="govuk-!-font-weight-bold">only visible to you. Any changes you make will be viewable on subsequent visits to this school’s dashboard unless you choose to reset them.</p>' />
            </span>
          </div>
        </div>
        <div className="govuk-grid-row">
          <div className="govuk-grid-column-full">
            <h2 className="govuk-heading-m">
              {initialData.LatestTerm} submitted data
            </h2>
            <div className="govuk-caption-m">
              Dashboard year {initialData.LatestTerm}
              <span className="inline-help-container">
                <SfbSadHelpModal
                  modalContent="By choosing a different year banding figures are aligned to that year for published finance, Future years use the most recent bands and can have uplifts applied to specific expenditure areas where there is an expectation of significant expenditure changes such pending salary awards."
                  modalTitle="Dashboard year"
                />
              </span>
            </div>
          </div>
          <div className="govuk-grid-column-full">
            <SadTable data={rrData} mode="income" captionText="Reserve and balance"/>
          
            <SadTable data={exData} mode="expenditure" captionText="Spending"/>
          
            <SadTable data={characteristicsData} mode="characteristics" captionText="School characteristics"/>
          
            <SadTable data={outcomesData} mode="outcomes" captionText="Outcomes"/>
          </div>
        </div>
      </>
  )
}