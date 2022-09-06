import React, {SyntheticEvent, useState} from 'react';
import {IThreshold} from "./Dashboard";
import CallOutModal from "./CallOutModal";
import TrustSummaryMobileNav from "./TrustSummaryMobileNav";
import SfbLoadingMessage from "../Global/SfbLoadingMessage";

interface Props {
  categories: string[]
  bandingMap: {category: string, thresholds: any[]}[]
  ratingCounts: { category:string, counts: {}; }[]
  ratingData: { name: string, categoryRatings: IThreshold[] }[]
  loading: boolean
}

export default function TabPanel(
  {
    categories,
    bandingMap,
    ratingCounts,
    ratingData,
    loading
  }: Props) {
  const [current, setCurrent] = useState<string>('Teaching staff')

  function handleCategoryChange(e: SyntheticEvent , category: string): void {
    e.preventDefault();
    setCurrent(category)
  }
  
  function renderCallout(count: number, index: number, category: string) {
    const subText = count === 1 ? 'school' : 'schools';
    // get a list of schools to populate the 'help' modal
    const categorySchools = ratingData.filter((school) => {
      return school.categoryRatings.find(r => r.thresholdIndex === index && r.category === category);
    });
    const bandingDescription = bandingMap.find(p => p.category === category)?.thresholds[index].RatingText

    return (
      <>
        {count > 0 &&
          <>
            <div className="sfb-call-out-box">
              <CallOutModal
                schools={categorySchools}
                modalTitle={bandingDescription}
                category={category}/>

              <div className="sfb-call-out-box__content">
                <h2 className="sfb-call-out-box__header">{count.toString()}</h2>
                <p className="sfb-call-out-box__subtext">{subText}</p>
              </div>
            </div>
          </>
        }
      </>
    )
  }
  return (
    <>
      <nav className="sfb-sad-dashboard-mobile-nav">
      <TrustSummaryMobileNav 
        handleCategoryChange={handleCategoryChange} 
        navItems={categories} 
        currentCategory={current} />
      </nav>
      <nav className="sfb-panel-tabs">
        <ul>
          {categories.map((category, i) =>  (
              <li key={`${category}-link-${i}`}>
                <a href="#" role="button"
                    onClick={(e) => handleCategoryChange(e, category)}
                   className={category === current? "govuk-link govuk-link--no-visited-state govuk-link--no-underline sfb-panel-tab sfb-panel-tab__selected": " govuk-link govuk-link--no-visited-state govuk-link--no-underline sfb-panel-tab"}>
                {category !== 'Administrative and clerical staff' ? category: 'Admin. and clerical staff'}</a>
              </li>
            )
          )}
        </ul>
      </nav>
      <section className="sfb-panel-tabs__content_wrapper">
        {loading && <SfbLoadingMessage message="Loading..." isLoading={loading}/>}
        <div className="sfb-panel-tabs__tab_content" key={`panel-${current}`}>
          {bandingMap && bandingMap.filter(m => m.category === current)[0]?.thresholds.map((threshold, j) => {
            const panelCounts: { [key: number]: number } = ratingCounts.filter(catType => current === catType.category)[0].counts;
            const count = panelCounts ? panelCounts[j] : 0;
            const thresholdCount = bandingMap.find(b => b.category === current)?.thresholds.length
            return (
              <div key={`${current}-card-${j}`}
                   className={`sfb-rating-panel-card sfb-rating-panel-card-${thresholdCount}`}>
                {renderCallout(count, j, current)}
                <div
                  className={`sfb-rating-panel-banding sfb-rating-panel-banding__${threshold.RatingColour.replaceAll(' ', '').toLowerCase()}`}>
                  <p>{threshold.RatingText}</p>
                </div>
              </div>
            )
          })
          }
        </div>
      </section>
    </>
  )
}