import React, {useEffect, useState} from 'react';
import agent from '../../api/agent';
import CharacteristicsTable, {CharacteristicsRow} from "./CharacteristicsTable";
import {SadDataObject} from '../SadTrustApp/Models/sadTrustTablesModels';
import TabPanel from "./TabPanel";

interface Props {
  uid: number,
}

// interface ISadBanding {
//  
// }
//
// interface ISadSummaryViewModel {
//   category: string
//   bandings: sad
// }

interface IEstablishmentSummary {
  name: string
  schoolData: number
  expenditure: number
}

export interface CategoryRating {
  category: string
  thresholds: IThreshold[]
}

export interface IThreshold {
  category: string
  percentageExpenditure?: number
  schoolData?: number
  thresholdIndex?: number
}


const prepCharacteristicsData = (sadData: SadDataObject[]): CharacteristicsRow[] => {

  const formatPupilsComparator = (estab: SadDataObject) => {
    const pupilsMin = estab.SadSizeLookup.NoPupilsMin;
    const pupilsMax = estab.SadSizeLookup.NoPupilsMax;
    if (pupilsMax) {
      return `Schools with ${pupilsMin} - ${pupilsMax} pupils`;
    }
    return `Schools with ${pupilsMin} or more pupils`;
  }

  return sadData.map((estab) => {
    const pc = formatPupilsComparator(estab);
    const out: CharacteristicsRow = {
      name: estab.Name,
      phase: estab.OverallPhase,
      phaseComparator: estab.OverallPhase,
      londonWeighting: estab.LondonWeighting === 'Neither' ? 'Not London' : 'London',
      weightingComparator: estab.LondonWeighting === 'Neither' ? 'Not London' : 'London',
      numberOfPupils: estab.NumberOfPupilsLatestTerm,
      pupilsComparator: pc,
      fsm: +estab.FSMLatestTerm.toFixed(1),
      fsmComparator: `Schools with ${estab.SadFSMLookup.FSMMin.toFixed(1)} - ${estab.SadFSMLookup.FSMMax.toFixed(1)} FSM`
    };
    return out;
  });
}

const categories = [
  'Teaching staff',
  'Supply staff',
  'Education support staff',
  'Administrative and clerical staff',
  'Other staff costs',
  'Premises costs',
  'Educational supplies',
  'Energy'
];
const prepCategoryData = (sadData: SadDataObject[]) => {
  const bandings = sadData[0].SadAssesmentAreas
    .filter(category => categories.indexOf(category.AssessmentAreaName) > -1)
    .map((category) => {
      return {
        category: category.AssessmentAreaName,
        thresholds: category.AllTresholds,
      }
    });
  
  const ratings = sadData.map(({Name, SadAssesmentAreas: SadAssessmentAreas, TotalExpenditureLatestTerm}) => {
    const out: { name: string, categoryRatings: IThreshold[] } = {
      name: Name,
      categoryRatings: []
    }
    SadAssessmentAreas
      .filter(category => categories.indexOf(category.AssessmentAreaName) > -1)
      .forEach((area) => {
        const ratingFigure = typeof area.SchoolData !== 'undefined' ?
          area.SchoolData : area.SchoolDataLatestTerm;

        if (typeof ratingFigure !== 'undefined') {
          const ratingValue = +(ratingFigure / TotalExpenditureLatestTerm).toFixed(3);
          const threshold = area.AllTresholds.find(t => (ratingValue >= t.ScoreLow || t.ScoreLow == null)
            && (ratingValue <= t.ScoreHigh || t.ScoreHigh == null));

          if (typeof threshold !== 'undefined') {
            const thresholdIndex = area.AllTresholds.indexOf(threshold);

            out.categoryRatings.push({
              category: area.AssessmentAreaName,
              thresholdIndex: thresholdIndex,
              schoolData: ratingFigure,
              percentageExpenditure: ratingValue * 100
            });
          }
        }
      });
    return out;
  });
  
  return {
    bandings,
    ratings,
    categories
  }
};

export default function Dashboard({uid}: Props) {
  let preppedData: CharacteristicsRow[] = [];
  const [loading, setIsLoading] = useState<boolean>(true);
  const [sadData, setSadData] = useState<SadDataObject[]>();
  const [bandingMap, setBandingMap] = useState<any[any]>();
  const [categoryData, setCategoryData] = useState<string[]>([]);
  const [characteristics, setCharacteristics] = useState<CharacteristicsRow[]>([]);
  const [ratingData, setRatingData] = useState<{ name: string, categoryRatings: IThreshold[] }[]>([])
  const [ratingsCounts, setRatingsCounts]= useState<{ category:string, counts: {}; }[]>([]);
  
  useEffect(() => {
    (async function() {
      const initialData = await agent.SelfAssessmentDashboard.TrustData(uid);

      preppedData = prepCharacteristicsData(initialData.Academies);

      setSadData(initialData?.Academies);
      const d = prepCategoryData(initialData.Academies);
      console.log(d);
      setCategoryData(d.categories);
      setBandingMap(d.bandings);
      setRatingData(d.ratings);
      categories.forEach((category) => {
        const ratingCounts = d.ratings
          .map((estab) => {
            return estab.categoryRatings.filter(rating => rating.category === category)[0]
          })
          .map(category => category.thresholdIndex)
          .reduce((acc, curr) => {
              // @ts-ignore
            if (acc[curr]) {
                // @ts-ignore
              return ++acc[curr], acc
              } else {
                // @ts-ignore
              return acc[curr] = 1, acc
              }
            
            
          }, {});
        
        ratingsCounts.push({
          category,
          counts: ratingCounts
        });
        setRatingsCounts(ratingsCounts);
      });
     
      setCharacteristics(preppedData);
      setIsLoading(false)
    })();
  }, []);

  return (
    <>
      <h2 className="govuk-heading-m">
        This trust's schools spending rating against similar schools
      </h2>
      <details className="govuk-details sfb-details" data-module="govuk-details">
        <summary className="govuk-details__summary">
          <span className="govuk-details__summary-text">View characteristics used </span>
        </summary>
        <div className="govuk-details__text">
          <CharacteristicsTable rowData={characteristics}/>
        </div>
      </details>
      <TabPanel 
        categories={categories} 
        bandingMap={bandingMap}
        ratingCounts={ratingsCounts}
        ratingData={ratingData}
        loading={loading}
      />
      <div className="govuk-!-margin-top-6">
        <p>
          See more at this trust's &nbsp;
          <a href={`/TrustSelfAssessment/${uid}/InYearBalance`}>self-assessment dashboard</a>
        </p>
       
      </div>
    </>
  )
}