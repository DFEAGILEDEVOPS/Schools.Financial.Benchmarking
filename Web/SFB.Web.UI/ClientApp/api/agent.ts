import axios, {AxiosError, AxiosResponse} from 'axios';
import {SearchSuggestion} from '../Models/Suggestions';
import {SadDataObject, SadHelpText, TrustSadResponse} from "../AppComponents/SadTrustApp/Models/sadTrustTablesModels";

axios.interceptors.response.use(
  async (response) => {
    return response;
  }
)

const responseBody = <T>(response: AxiosResponse<T>) => response.data;

const requests = {
  get:<T>(url: string) => axios.get<T>(url).then(responseBody),
  post:<T>(url: string, body: {}) => axios.post<T>(url, body).then(responseBody) 
};

const BenchmarkBasket = {
  clear: () =>
    requests.get<HTMLElement>('/school/UpdateBenchmarkBasket?withAction=RemoveAll'),
};

const SelfAssessmentHelpText = {
  get: (id: number) => requests.get<SadHelpText>(`/TrustSelfAssessment/Help/${id}`),
  getAll: () => requests.get<SadHelpText[]>('TrustSelfAssessment/Help/All'),
};

const ManualComparison = { 
  addSchool: (urn: string) => requests.get<HTMLElement>(`/manualcomparison/AddSchool?urn=${urn}`),
  removeSchool: (urn: string) => requests.get<HTMLElement>(`/manualcomparison/RemoveSchool?urn=${urn}`),
  removeAll: () => requests.get<void>('/manualcomparison/RemoveAllSchools'),
  addTrust: (companyNo: string, matName: string) => requests.get<HTMLElement>(`AddTrust?companyNo=${companyNo}&matName={matName}`),
  removeTrust: (companyNo: string) => requests.get<HTMLElement>(`RemoveTrust?companyNo=${companyNo}`)
};

const Suggestions = {
  Schools: (userInput: string, openOnly: boolean) => requests.get<SearchSuggestion[]>(`/schoolsearch/suggest?nameId=${userInput}&openOnly=${openOnly}`),
  Trusts: (userInput: string) => requests.get<SearchSuggestion[]>(`/trustsearch/suggest?name=${userInput}`),
  Location: (userInput: string) => requests.get<SearchSuggestion[]>(`/schoolsearch/suggestplace?text=${userInput}`)
};

const SelfAssessmentDashboard = {
  TrustData: (uid: number) => requests.get<TrustSadResponse>(`/TrustSelfAssessment/summary/${uid}`),
}

const agent = {
  BenchmarkBasket,
  Suggestions,
  ManualComparison,
  SelfAssessmentHelpText,
  SelfAssessmentDashboard,
};

export default agent;