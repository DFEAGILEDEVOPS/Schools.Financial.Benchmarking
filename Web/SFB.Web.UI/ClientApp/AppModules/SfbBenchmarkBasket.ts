import agent from "../api/agent";

export async function clearBenchmarkBasket(): Promise<void> {
  try {
    const response: HTMLElement = await agent.BenchmarkBasket.clear();
    const basketElement: HTMLElement | null = document.getElementById('benchmarkBasket');
    if (basketElement instanceof HTMLElement) {
      const basketParent: HTMLElement | null = basketElement.parentElement;
      if (basketParent instanceof HTMLElement) {
        basketElement.innerHTML = '';
        basketParent.appendChild(response);
      }
    }
    const isSearch: HTMLElement | null = document.querySelector('.search-results');
    const hasChartsDisplayed: HTMLElement | null = document.querySelector('.benchmarking-charts');
    if (isSearch) {
      window.location.reload();
    }

    if (hasChartsDisplayed) {
      location.replace('/BenchmarkCharts');
    }
  } catch (error) {
    console.log(error);
  }
}