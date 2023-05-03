export const numberWithCommas = (x?: string): string => {
  return x ? x.replace(/\B(?=(\d{3})+(?!\d))/g, ",") : '';
}