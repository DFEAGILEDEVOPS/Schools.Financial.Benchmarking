import {SadDataObject} from "../Models/SadEditForm"
import {EstablishmentSadTableRow} from "../PageComponents/SadTable";
declare var initialData: SadDataObject;
declare var modalMap: any[];
export const prepDataRows = (initialData: SadDataObject): EstablishmentSadTableRow[] => {
    const data = initialData.SadAssesmentAreas.map((item) => {
        //console.log(item);
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
                    console.log(item.SchoolDataLatestTerm, ' ', typeof item.SchoolDataLatestTerm);
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
