import CookieManager from "./CookieManager";


const getData = function(cookieName: string): string | void {
  const cookieData = CookieManager.getCookie(cookieName);
  if (cookieData) {
    return decodeURIComponent(cookieData);
  }
};

const establishmentIsInList = function(id: string, cookieName: string = 'sfb_comparison_list'): boolean {
  const cookieData = getData(cookieName);
  if (!cookieData) {
    return false;
  }
  const listJson: { BS: [] } = JSON.parse(cookieData); 
  
  return listJson.BS.filter((establishment: {U: string}) => {
    return establishment.U === id;
  }).length > 0;
};


const getCount = function(cookieName: string = 'sfb_comparison_list'): number {
  const cookieData = getData(cookieName);
  if (!cookieData) {
    return 0;
  }
  const listJson: { BS: [] } = JSON.parse(cookieData);
  
  return listJson.BS.length;
};


const ComparisonList = {
  getData,
  establishmentIsInList,
  getCount
};

export default ComparisonList;