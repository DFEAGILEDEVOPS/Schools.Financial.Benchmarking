import axios, {AxiosError, AxiosResponse} from 'axios';
import {SearchSuggestion} from '../Models/Suggestions';

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

const ManualComparison = { 
  addSchool: (urn: string) => requests.get<HTMLElement>(`/manualcomparison/AddSchool?urn=${urn}`),
  removeSchool: (urn: string) => requests.get<HTMLElement>(`/manualcomparison/RemoveSchool?urn=${urn}`),
  removeAll: () => requests.get<void>('/manualcomparison/RemoveAllSchools'),
}

const Suggestions = {
  Schools: (userInput: string, openOnly: boolean) => requests.get<SearchSuggestion[]>(`/schoolsearch/suggest?nameId=${userInput}&openOnly=${openOnly}`),
  Trusts: (userInput: string) => requests.get<SearchSuggestion[]>(`/trustsearch/suggest?name=${userInput}`),
  Location: (userInput: string) => requests.get<SearchSuggestion[]>(`/schoolsearch/suggestplace?text=${userInput}`)
};

const agent = {
  BenchmarkBasket,
  Suggestions,
  ManualComparison,
};

export default agent;