const chartMoneyFormat = function (amount: number): string {
  if (amount === null)
    return 'Not applicable';
  else if (amount >= 1000000)
    return `£${parseFloat((amount / 1000000).toFixed(2)).toString()}m`;
  else if (amount <= -1000000)
    return `-£${parseFloat(Math.abs(amount / 1000000).toFixed(2)).toString()}m`;
  else if (amount >= 10000)
    return `£${parseFloat((amount / 1000).toFixed(1)).toString()}k`;
  else if (amount <= -10000)
    return `-£${parseFloat(Math.abs(amount / 1000).toFixed(1)).toString()}k`;
  else if (amount < 0)
    return `-£${numberWithCommas(parseFloat(Math.abs(amount).toFixed(0)))}`;
  else
    return `£${numberWithCommas(parseFloat(amount.toFixed(0)))}`;

}

const chartPercentageFormat = function (amount: number): string {
  if (amount === null)
    return 'Not applicable';
  else {
    if (amount > 0 && amount < 2) {
      return parseFloat(amount.toFixed(2)).toString() + '%';
    } else {
      return parseFloat(amount.toFixed(1)).toString() + '%';
    }
  }
}

const chartDecimalFormat = function (amount: number): string {
  if (amount === null) {
    return 'Not applicable';
  } else {
    return parseFloat(amount.toFixed(2)).toString();
  }
}

const chartIntegerFormat = function (amount: number): string {
  if (amount === null) {
    return 'Not applicable';
  } else {
    return parseFloat(amount.toFixed(0)).toString();
  }
}

const numberWithCommas = function (x: number) {
  return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
}

const ChartingUtils = {
  chartMoneyFormat,
  chartPercentageFormat,
  chartDecimalFormat,
  numberWithCommas
}

export default ChartingUtils;