import {SadDataObject, SadTableRow} from './Models/sadTrustTablesModels';
import {
  getKs2ProgressBandingObject,
  getProgress8BandingObject,
  ofstedLookup,
  OfstedRating
} from '../Helpers/OutcomesHelpers';
import {fullMonths} from '../Helpers/DateTimeHelpers';

declare var modalMap: any;


export const prepareTableRows = (sadData: SadDataObject[], targetCategory: string): SadTableRow[] => {
  const modalLookUp = modalMap;
  return sadData.filter((item) => { 
    // remove primary schools from p8 and secondary from ks2 views
    return !((item.ProgressScoreType === 'Progress 8 score' && targetCategory === 'Ks2Score') ||
      (item.ProgressScoreType === 'KS2 score' && targetCategory === 'Progress8Score') || item.ProgressScoreType === null);
    
  }).map(({
      Ks2Score,
      Name,
      NumberOfPupilsLatestTerm,
      OfstedInspectionDate,
      OfstedRating,
      Progress8Banding,
      P8Score,
      ProgressScoreType,
      SadAssesmentAreas,
      TeachersLeaderLastTerm,
      TeachersTotalLastTerm: teachersTotal,
      TotalExpenditureLatestTerm,
      TotalIncomeLatestTerm,
      Urn,
      WorkforceTotalLastTerm: ratingFigure,
      IsReturnsComplete,
      OverallPhase
    }) => {
    const out: SadTableRow = {
      urn: Urn,
      schoolName: Name,
      hasFullData: IsReturnsComplete,
      phase: OverallPhase
    };

    const data = SadAssesmentAreas.filter((area) => {
      return area.AssessmentAreaName.replaceAll(' ', '').replaceAll('-', '').toLowerCase() === targetCategory.toLowerCase();
    })[0];
    
    if (typeof data !== 'undefined') {
      out.helpText = modalLookUp.filter((item: any) => {
        return item.AssessmentArea === data.AssessmentAreaName;
      }).map((item: any) => {
        return {
          id: item.Id,
          title: item.Title, 
          assessmentArea: item.AssessmentArea,
          content: item.Content
        }
      })[0];
      
      out.thresholds = data.AllTresholds;
      
      if (data.AssessmentAreaType === 'Reserve and balance') {
        const ratingFigure = typeof data.SchoolData !== 'undefined' ?
          data.SchoolData : data.SchoolDataLatestTerm;

        if (typeof ratingFigure !== 'undefined') {
          const ratingValue: number = +(ratingFigure / TotalIncomeLatestTerm).toFixed(3);

          out.thresholdRating = data.AllTresholds.find(t => (ratingValue >= t.ScoreLow || t.ScoreLow == null)
            && (ratingValue <= t.ScoreHigh || t.ScoreHigh == null));
          out.schoolData = ratingFigure;
          out.percentageOfIncome = ratingValue * 100;
        }


      } else if (data.AssessmentAreaType === 'Spending') {
        const ratingFigure = typeof data.SchoolData !== 'undefined' ?
          data.SchoolData : data.SchoolDataLatestTerm;

        if (typeof ratingFigure !== 'undefined') {
          const ratingValue = +(ratingFigure / TotalExpenditureLatestTerm).toFixed(3);
          out.thresholdRating = data.AllTresholds.find(t => (ratingValue >= t.ScoreLow || t.ScoreLow == null)
            && (ratingValue <= t.ScoreHigh || t.ScoreHigh == null));
          
          out.thresholdSortFigure = ratingValue;
          out.schoolData = ratingFigure;
          out.percentageOfExpenditure = ratingValue * 100;
        }

      } else if (data.AssessmentAreaType === 'School characteristics') {
        if (data.AssessmentAreaName === 'Average teacher cost') {
          const ratingFigure = SadAssesmentAreas.filter(a => a.AssessmentAreaName === "Teaching staff")[0]?.SchoolDataLatestTerm;

          if (typeof ratingFigure !== 'undefined') {
            const ratingValue = +(ratingFigure / teachersTotal).toFixed(3);

            out.schoolData = ratingValue;
            out.thresholdRating = data.AllTresholds.find(t => (ratingValue >= t.ScoreLow || t.ScoreLow == null)
              && (ratingValue <= t.ScoreHigh || t.ScoreHigh == null));
          }
        }

        if (data.AssessmentAreaName === 'Senior leaders as a percentage of workforce') {
          out.schoolData = (TeachersLeaderLastTerm / ratingFigure) * 100;

          if (typeof ratingFigure !== 'undefined') {
            const ratingValue = +(ratingFigure / ratingFigure).toFixed(3);
            out.thresholdRating = data.AllTresholds.find(t => (ratingValue >= t.ScoreLow || t.ScoreLow == null)
              && (ratingValue <= t.ScoreHigh || t.ScoreHigh == null));
          }
        }

        if (data.AssessmentAreaName === 'Pupil to teacher ratio') {
          const ratingFigure = NumberOfPupilsLatestTerm / teachersTotal;
          out.schoolData = ratingFigure;

          if (typeof ratingFigure !== 'undefined') {
            out.thresholdRating = data.AllTresholds.find(t => (ratingFigure >= t.ScoreLow || t.ScoreLow == null)
              && (ratingFigure <= t.ScoreHigh || t.ScoreHigh == null));
          }
        }

        if (data.AssessmentAreaName === 'Pupil to adult ratio') {
          const ratingValue = +(NumberOfPupilsLatestTerm / ratingFigure).toFixed(3);

          out.schoolData = ratingValue;

          out.thresholdRating = data.AllTresholds.find(t => (ratingValue >= t.ScoreLow || t.ScoreLow == null)
            && (ratingValue <= t.ScoreHigh || t.ScoreHigh == null));

        }
      }

    } else {
      // outcomes sections 
      if (targetCategory === 'OfstedRating') {
        const rating: OfstedRating = {
          score: OfstedRating as number,
          ratingText: ofstedLookup.filter((r) => {
            return r.score.toString() === OfstedRating;
          })[0]?.ratingText
        };
        
        out.thresholdSortFigure = OfstedRating? OfstedRating as number : undefined

        out.helpText = modalLookUp.filter((item: any) => {
          return item.AssessmentArea === 'Ofsted'
        }).map((item: any) => {
          return {
            id: item.Id,
            title: item.Title,
            assessmentArea: item.AssessmentArea,
            content: item.Content
          }
        })[0];
        
        if (OfstedInspectionDate) {
          const inspectionDate = new Date(OfstedInspectionDate);
          rating.reportDate = `${inspectionDate.getDay()} ${fullMonths[inspectionDate.getMonth()]} ${inspectionDate.getFullYear()}`;
        }

        out.ofstedRating = rating;
      }

      if (targetCategory === 'Ks2Score' && (ProgressScoreType === 'KS2 score' || ProgressScoreType === 'All-through')) {
        out.progressScore = {...getKs2ProgressBandingObject(Ks2Score as number)};
        
        out.thresholdSortFigure = Ks2Score as number;
        out.helpText = modalLookUp.filter((item: any) => {
          return item.AssessmentArea === 'KS2'
        }).map((item: any) => {
          return {
            id: item.Id,
            title: item.Title,
            assessmentArea: item.AssessmentArea,
            content: item.Content
          }
        })[0];
      }

      if (targetCategory === 'Progress8Score' && (ProgressScoreType === 'Progress 8 score' || ProgressScoreType === 'All-through')) {
        out.progressScore = {...getProgress8BandingObject(Progress8Banding)};
        out.progressScore.score = P8Score;
        out.thresholdSortFigure = P8Score as number;
        out.helpText = modalLookUp.filter((item: any) => {
          return item.AssessmentArea === 'P8'
        }).map((item: any) => {
          return {
            id: item.Id,
            title: item.Title,
            assessmentArea: item.AssessmentArea,
            content: item.Content
          }
        })[0];
      }

    }

    return out;
  });
} 