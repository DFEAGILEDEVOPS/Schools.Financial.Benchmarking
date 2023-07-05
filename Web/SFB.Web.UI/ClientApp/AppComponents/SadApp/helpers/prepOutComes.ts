import {SadDataObject} from "../Models/SadEditForm";
import {EstablishmentSadTableRow} from "../PageComponents/SadTable"



import {
    getKs2ProgressBandingObject,
    getProgress8BandingObject,
    ofstedLookup,
    OfstedRating,
    fullMonths
} from "../Models/OutcomesHelpers";
declare var modalMap: any[];
export const prepOutComes = (data: SadDataObject): EstablishmentSadTableRow[] => {
    const outcomesData: EstablishmentSadTableRow[] = [];

    const ofstedRating: OfstedRating = {
        score: data.OfstedRating as number,
        ratingText: ofstedLookup.filter((r) => {
            return r.score.toString() === data.OfstedRating
        })[0]?.ratingText
    };

    if (data.OfstedInspectionDate) {
        const inspectionDate = new Date(data.OfstedInspectionDate);
        ofstedRating.reportDate = `${inspectionDate.getDay()} ${fullMonths[inspectionDate.getMonth()]} ${inspectionDate.getFullYear()}`;
    }
    ofstedRating.urn = data.Urn;

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

    if (data.ProgressScoreType === "All-through"
        || data.ProgressScoreType ==="KS2 score") {
        const progressScore= getKs2ProgressBandingObject(data.Ks2Score as  number);
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

    if (data.ProgressScoreType === "All-through" ||
        data.ProgressScoreType === 'Progress 8 score') {
        const progressScore = getProgress8BandingObject(data.Progress8Banding as number);
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
    return outcomesData;
}
