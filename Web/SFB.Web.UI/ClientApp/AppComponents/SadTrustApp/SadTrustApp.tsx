import React, {ChangeEvent, useState, StrictMode, useEffect} from 'react';
import {Outlet, Route, Routes, useLocation} from 'react-router-dom';
import Frame from 'react-frame-component';
import {SadDataObject} from './Models/sadTrustTablesModels';
import CookieManager from '../../AppModules/CookieManager';

import SfbSelect from '../Global/FormControls/SfbSelect';
import Navigation from './Navigation';
import SfbSadMobileNav from './SfbSadMobileNav';
import Views, {tableData} from './Views';
import BenchmarkCriteriaTable from './BenchmarkCriteriaTable';
import DownloadPdf from './DownloadPdf';

declare var navigation: any;
declare var sadData: any;
declare var trustName: string;

const mainStyles = document.querySelector('head')?.querySelectorAll('link[rel="stylesheet"]')[0] as HTMLLinkElement;
const styleSrc = mainStyles.href;

declare global {
  interface Window {
    ga: any;
  }
}
let acceptedTrackingCookies = false;
const policyCookie = CookieManager.getCookie('cookies_policy');
if (policyCookie) {
  const cookieSettings = JSON.parse(policyCookie);
  acceptedTrackingCookies = cookieSettings.usage;
}

interface LayoutProps {
  establishmentCount: number
  availablePhases: (string | undefined)[]
  filterValue: string
  HandlePhaseChange: (e: ChangeEvent<HTMLSelectElement>) => void
  hasKs2Progress: boolean
  hasProgress8: boolean
  isLoading?: boolean
  isPrinting: boolean
  HandlePrintClick: (e: React.MouseEvent<HTMLButtonElement>) => void
  isDownload: boolean
  HandleDownloadClick: (e: React.MouseEvent<HTMLButtonElement>) => void
}

function Layout({
    establishmentCount,
    availablePhases,
    HandlePhaseChange,
    filterValue,
    hasKs2Progress,
    hasProgress8,
    isPrinting,
    HandlePrintClick,
    isDownload,
    HandleDownloadClick
  }: LayoutProps) {
  return (
    <>
      <div className="govuk-grid-row">
        <div className="govuk-grid-column-full">
          <details className="govuk-details sfb-details" data-module="govuk-details">
            <summary className="govuk-details__summary">
                <span className="govuk-details__summary-text">
                    View characteristics used
                </span>
            </summary>
            <div className="govuk-details__text">
              <BenchmarkCriteriaTable academies={sadData}/>
            </div>
          </details>

          <div>
            <button className="sfb-button--download" onClick={(e) => HandleDownloadClick(e)} disabled={isDownload || isPrinting}>
              Download page
            </button>
            <button className="sfb-button--print" onClick={(e) => HandlePrintClick(e)} disabled={isDownload || isPrinting}>
              Print page
            </button>
            {(isDownload || isPrinting) &&
              <p className="govuk-body-s">Preparing {isPrinting && "print preview"} {isDownload && "your download"}</p>
            }
          </div>
          {establishmentCount < 15 &&
            <hr className="govuk-section-break govuk-section-break--m"/>
          }
        </div>
      </div>
      
      {(establishmentCount >= 15 && availablePhases.length > 2) &&
        <>
          <div className="govuk-grid-row">
            <div className="govuk-grid-column-full">
              <hr className="govuk-section-break govuk-section-break--m govuk-section-break--visible"/>
              <SfbSelect
                label="School phase"
                id="school-phase"
                options={availablePhases}
                onChange={HandlePhaseChange}
                skipLinkTarget="sad-trust-data-table"
                value={filterValue}/>
              <hr className="govuk-section-break govuk-section-break--m govuk-section-break--visible"/>
            </div>
          </div>
        </>
      }
      <div className="govuk-grid-row">
        <div className="govuk-grid-column-one-quarter sfb-panel__subnav-mobile">
          <SfbSadMobileNav navigationItems={navigation} hasProgress8={hasProgress8} hasKs2Progress={hasKs2Progress}/>
        </div>
        <div className="govuk-grid-column-one-quarter sfb-panel__subnav-desktop">
          <Navigation navigationItems={navigation} hasProgress8={hasProgress8} hasKs2Progress={hasKs2Progress}/>
        </div>
        <div className="govuk-grid-column-three-quarters" id="sad-trust-data-table">
          <Outlet/>
        </div>
      </div>

      {isPrinting &&
        <Frame 
          id="sfb-dashboard-print-frame"
          width="1070"
          className="sfb-dashboard-print-frame"
          initialContent={
            `<!DOCTYPE html><html lang="en">
              <head><title>${document.title}</title>
              <link rel="stylesheet" href="${styleSrc}"/>
                </head>
                <body><div></div></body>
                </html>`}>
          <div className="govuk-grid-row">
            <div className="govuk-grid-column-full sfb-print-container">
              <BenchmarkCriteriaTable academies={sadData}/>
              <Views.PrintAll
                hasKs2Progress={hasKs2Progress}
                hasProgress8={hasProgress8}
                phaseFilter={'All'}
                isLoading={false}/>
            </div>
          </div>
        </Frame>
      }
      {isDownload &&
        <Frame
          id="sfb-dashboard-download-frame"
          width="1070"
          className="sfb-dashboard-print-frame"
          initialContent={
            `<!DOCTYPE html><html lang="en">
                    <head><title>${document.title}</title>
                    <link rel="stylesheet" href="${styleSrc}"/>
                      </head>
                      <body><div></div></body>
                      </html>`}>
          <div className="govuk-grid-row">
            <div className="govuk-grid-column-full sfb-print-container">
              <BenchmarkCriteriaTable academies={sadData} isDownload={true}/>
              <DownloadPdf
                hasKs2Progress={hasKs2Progress}
                hasProgress8={hasProgress8}
                phaseFilter={'All'}
                isLoading={false}
                trustName={trustName}/>
            </div>
          </div>
        </Frame>
      }
    </>
  );
}


let isInitial = true;

export default function SadTrustApp() {
  const phases: (string | undefined)[] = sadData.map((item: SadDataObject) => item.OverallPhase)
    .filter((v: string, i: number, a: string) => a.indexOf(v) === i);
  const availablePhases = ['All', ...phases];
  const hasKs2Progress = tableData.Ks2Score.length > 0;
  const hasProgress8 = tableData.Progress8Score.length > 0;
  const establishmentCount = sadData.length;
  const [isPrint, setIsPrint] = useState<boolean>(false);
  const [isDownload, setIsDownload] = useState<boolean>(false);
  const [phaseFilter, setPhaseFilter] = useState<string>('All');
  const [isLoading, setIsLoading] = useState<boolean>(true);

  const HandlePhaseChange = (e: ChangeEvent<HTMLSelectElement>): void => {
    setPhaseFilter(e.target.value);
  }

  const HandlePrintClick = (e: React.MouseEvent<HTMLButtonElement>): void => {
    setIsPrint(true);
    window.setTimeout(() => {
      setIsPrint(false);
    }, 4000);
  }

  const HandleDownloadClick = (e: React.MouseEvent<HTMLButtonElement>): void => {
    setIsDownload(true);
    window.setTimeout(() => {
      setIsDownload(false);
    }, 6000);
  }

  const location = useLocation();

  useEffect(() => {
    if (acceptedTrackingCookies) {
      window.ga('set', 'page', window.location.toString());
      window.ga('send', 'pageview');
    }
    if (!isInitial) {
      setIsLoading(true);
      setPhaseFilter('All');
      window.scrollTo(0, 0);
      const table = document.querySelector('.sfb-sadtrust-table');
      if (table instanceof HTMLTableElement) {
        table.focus();
      }
      const timer = setTimeout(() => {
        setIsLoading(false);
      }, 1000);

      return () => {
        clearTimeout(timer);
      }
    }
    isInitial = false;
    setIsLoading(false);
  }, [location]);

  return (
    <StrictMode>
      <Routes>
        <Route path={"/"}
               element={
                 <Layout
                   availablePhases={availablePhases}
                   establishmentCount={establishmentCount}
                   HandlePhaseChange={HandlePhaseChange}
                   filterValue={phaseFilter}
                   hasKs2Progress={hasKs2Progress}
                   hasProgress8={hasProgress8}
                   isPrinting={isPrint}
                   HandlePrintClick={HandlePrintClick}
                   isDownload={isDownload}
                   HandleDownloadClick={HandleDownloadClick}
                 />
               }>

          <Route path="/InYearBalance"
                 element={<Views.InYearBalance phaseFilter={phaseFilter} isLoading={isLoading}/>}/>
          <Route path="/RevenueReserve"
                 element={<Views.RevenueReserve phaseFilter={phaseFilter} isLoading={isLoading}/>}/>
          <Route path="/TeachingStaff"
                 element={<Views.TeachingStaff phaseFilter={phaseFilter} isLoading={isLoading}/>}/>
          <Route path="/SupplyStaff" element={<Views.SupplyStaff phaseFilter={phaseFilter} isLoading={isLoading}/>}/>
          <Route path="/EducationSupportStaff"
                 element={<Views.EducationSupportStaff phaseFilter={phaseFilter} isLoading={isLoading}/>}/>
          <Route path="/AdministrativeAndClericalStaff"
                 element={<Views.AdministrativeAndClericalStaff phaseFilter={phaseFilter} isLoading={isLoading}/>}/>
          <Route path="/OtherStaffCosts"
                 element={<Views.OtherStaffCosts phaseFilter={phaseFilter} isLoading={isLoading}/>}/>
          <Route path="/PremisesCosts"
                 element={<Views.PremisesCosts phaseFilter={phaseFilter} isLoading={isLoading}/>}/>
          <Route path="/EducationalSupplies"
                 element={<Views.EducationalSupplies phaseFilter={phaseFilter} isLoading={isLoading}/>}/>
          <Route path="/Energy" element={<Views.Energy phaseFilter={phaseFilter} isLoading={isLoading}/>}/>
          <Route path="/AverageTeacherCost"
                 element={<Views.AverageTeacherCost phaseFilter={phaseFilter} isLoading={isLoading}/>}/>
          <Route path="/SeniorLeadersAsAPercentageOfWorkforce"
                 element={<Views.SeniorLeadersAsAPercentageOfWorkforce phaseFilter={phaseFilter}
                                                                       isLoading={isLoading}/>}/>
          <Route path="/PupilToTeacherRatio"
                 element={<Views.PupilToTeacherRatio phaseFilter={phaseFilter} isLoading={isLoading}/>}/>
          <Route path="/PupilToAdultRatio"
                 element={<Views.PupilToAdultRatio phaseFilter={phaseFilter} isLoading={isLoading}/>}/>
          <Route path="/OfstedRating" element={<Views.OfstedRating phaseFilter={phaseFilter} isLoading={isLoading}/>}/>
          <Route path="/Ks2Score" element={<Views.Ks2Score phaseFilter={phaseFilter} isLoading={isLoading}/>}/>
          <Route path="/Progress8Score"
                 element={<Views.Progress8Score phaseFilter={phaseFilter} isLoading={isLoading}/>}/>
        </Route>
      </Routes>
    </StrictMode>
  )
}