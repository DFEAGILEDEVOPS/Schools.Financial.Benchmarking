export interface OfstedRating {
  score: number;
  ratingText: string;
  reportDate?: string;
  urn?: number;
}

export function isOfstedRating(obj:any): boolean {
  return typeof parseInt(obj?.score, 10) === 'number' && typeof obj?.ratingText === 'string';
}

export interface ProgressScore {
  score?: number;
  text?: string;
  className?: string;
  banding?: number
}

export function isProgressScore (obj: any): boolean {
  return (typeof obj.score === 'number' || typeof obj.banding === 'number')
      && typeof obj.text === 'string';
} 

export const ofstedLookup: OfstedRating[] = [
  {score: 0, ratingText: 'No data available'},
  {score: 1, ratingText: 'Outstanding'},
  {score: 2, ratingText: 'Good'},
  {score: 3, ratingText: 'Requires Improvement'},
  {score: 4, ratingText: 'Inadequate'}
];

export const getKs2ProgressBandingObject = (score: number): ProgressScore => {
  const progressConfig: ProgressScore = {};
  progressConfig.score = score;
  if (score < -3) {
    progressConfig.text = 'Well below average';
    progressConfig.className = 'progress-well-below-average';
  } else if (score >= -3 && score < -2) {
    progressConfig.text = 'Below average';
    progressConfig.className = 'progress-below-average';
  } else if (score >= -2 && score <= 2) {
    progressConfig.text = 'Average';
    progressConfig.className = 'progress-average';
  } else if (score > 2 && score <= 3) {
    progressConfig.text = 'Above average';
    progressConfig.className = 'progress-above-average';
  } else if (score > 3) {
    progressConfig.text = 'Well above average';
    progressConfig.className = 'progress-well-above-average';
  }

  return progressConfig;
};

export const getProgress8BandingObject = (banding: number): ProgressScore => {
  const progressConfig: ProgressScore = {};
  progressConfig.banding = banding;
  if (banding === 5) {
    progressConfig.text = 'Well below average';
    progressConfig.className = 'progress-well-below-average';
  } else if (banding === 4) {
    progressConfig.text = 'Below average';
    progressConfig.className = 'progress-below-average';
  } else if (banding === 3) {
    progressConfig.text = 'Average';
    progressConfig.className = 'progress-average';
  } else if (banding === 2) {
    progressConfig.text = 'Above average';
    progressConfig.className = 'progress-above-average';
  } else if (banding === 1) {
    progressConfig.text = 'Well above average';
    progressConfig.className = 'progress-well-above-average';
  } else {
    progressConfig.text = 'Unknown';
    progressConfig.className = 'progress-unknown'; 
  }

  return progressConfig;
}