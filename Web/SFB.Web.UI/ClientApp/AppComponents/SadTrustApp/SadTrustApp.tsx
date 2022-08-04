import Navigation from './Navigation';
import {BrowserRouter, Outlet, Route, Routes} from "react-router-dom";
import {prepareTableRows} from "./prepareTableRows";
import TableWrapper from "./TableWrapper";
import {SadDataObject} from "./Models/sadTrustTablesModels";
import SfbSelect from '../Global/FormControls/SfbSelect';
import React, {ChangeEvent, useState, StrictMode} from 'react';
import SfbSadMobileNav from "./SfbSadMobileNav";
declare var navigation: any;
declare var sadData: any;
declare var baseName: string;


const prepData = (data: SadDataObject[]) => {
  return {
    InYearBalance: prepareTableRows(data, 'InYearBalance'),
    RevenueReserve: prepareTableRows(data, 'RevenueReserve'),
    TeachingStaff: prepareTableRows(data, 'TeachingStaff'),
    SupplyStaff: prepareTableRows(data, 'SupplyStaff'),
    EducationSupportStaff: prepareTableRows(data, 'EducationSupportStaff'),
    AdministrativeAndClericalStaff: prepareTableRows(data, 'AdministrativeAndClericalStaff'),
    OtherStaffCosts: prepareTableRows(data, 'OtherStaffCosts'),
    PremisesCosts: prepareTableRows(data, 'PremisesCosts'),
    EducationalSupplies: prepareTableRows(data, 'EducationalSupplies'),
    Energy: prepareTableRows(data, 'Energy'),
    AverageTeacherCost: prepareTableRows(data, 'AverageTeacherCost'),
    SeniorLeadersAsAPercentageOfWorkforce: prepareTableRows(data, 'SeniorLeadersAsAPercentageOfWorkforce'),
    PupilToTeacherRatio: prepareTableRows(data, 'PupilToTeacherRatio'),
    PupilToAdultRatio: prepareTableRows(data, 'PupilToAdultRatio'),
    OfstedRating: prepareTableRows(data, 'OfstedRating'),
    Ks2Score: prepareTableRows(data, 'Ks2Score'),
    Progress8Score: prepareTableRows(data, 'Progress8Score'),
  };
}
const tableData = prepData([...sadData]);

interface LayoutProps {
  establishmentCount: number;
  phaseFilter: string;
  availablePhases: (string | undefined)[];
  filterValue: string;
  HandlePhaseChange: (e: ChangeEvent<HTMLSelectElement>) => void;
  hasKs2Progress: boolean;
  hasProgress8: boolean;
}

interface TableWrapperProps {
  phaseFilter: string;
  resetPhaseFilter: () => void;
}

function Layout({
    establishmentCount,
    availablePhases,
    HandlePhaseChange,
    filterValue,
    hasKs2Progress,
    hasProgress8
  }: LayoutProps) {
  return (
    <>
      {(establishmentCount >= 15 && availablePhases.length > 2) &&
        <>
          <hr className="govuk-section-break govuk-section-break--m govuk-section-break--visible"/>
          <SfbSelect
            label="School phase"
            id="school-phase"
            options={availablePhases}
            onChange={HandlePhaseChange}
            skipLinkTarget="sad-trust-data-table"
            value={filterValue} />
          
          <hr className="govuk-section-break govuk-section-break--m govuk-section-break--visible"/>
        </>
      }
      <div className="govuk-grid-row">
        <div className="govuk-grid-column-one-quarter sfb-panel__subnav-mobile">
          <SfbSadMobileNav navigationItems={navigation} hasProgress8={hasProgress8} hasKs2Progress={hasKs2Progress} />
        </div>
        <div className="govuk-grid-column-one-quarter sfb-panel__subnav-desktop">
          <Navigation navigationItems={navigation} hasProgress8={hasProgress8} hasKs2Progress={hasKs2Progress}/>
        </div>
        <div className="govuk-grid-column-three-quarters" id="sad-trust-data-table">
          <Outlet/>
        </div>
      </div>
    </>

  );
}

function InYearBalance({phaseFilter, resetPhaseFilter}: TableWrapperProps) {
  return (
    <TableWrapper
      tableData={tableData.InYearBalance}
      mode="income"
      captionText="In year balance"
      phaseFilter={phaseFilter}
      resetPhaseFilter={resetPhaseFilter} />
  );
}

function RevenueReserve({phaseFilter, resetPhaseFilter}: TableWrapperProps) {
  return (
    <TableWrapper
      tableData={tableData.RevenueReserve}
      mode="income"
      captionText="Revenue reserve"
      phaseFilter={phaseFilter}
      resetPhaseFilter={resetPhaseFilter} />
  );
}

function TeachingStaff({phaseFilter, resetPhaseFilter}: TableWrapperProps) {
  return (
    <TableWrapper
      tableData={tableData.TeachingStaff}
      mode="expenditure"
      captionText="Teaching staff"
      phaseFilter={phaseFilter}
      resetPhaseFilter={resetPhaseFilter} />
  );
}

function SupplyStaff({phaseFilter, resetPhaseFilter}: TableWrapperProps) {
  return (
    <TableWrapper
      tableData={tableData.SupplyStaff}
      mode="expenditure"
      captionText="Supply staff"
      phaseFilter={phaseFilter}
      resetPhaseFilter={resetPhaseFilter} />
  );
}

function EducationSupportStaff({phaseFilter, resetPhaseFilter}: TableWrapperProps) {
  return (
    <TableWrapper
      tableData={tableData.EducationSupportStaff}
      mode="expenditure"
      captionText="Education support staff"
      phaseFilter={phaseFilter}
      resetPhaseFilter={resetPhaseFilter} />
  );
}

function AdministrativeAndClericalStaff({phaseFilter, resetPhaseFilter}: TableWrapperProps) {
  return (
    <TableWrapper
      tableData={tableData.AdministrativeAndClericalStaff}
      mode="expenditure"
      captionText="Administrative and clerical staff"
      phaseFilter={phaseFilter}
      resetPhaseFilter={resetPhaseFilter} />
  );
}

function OtherStaffCosts({phaseFilter, resetPhaseFilter}: TableWrapperProps) {
  return (
    <TableWrapper
      tableData={tableData.OtherStaffCosts}
      mode="expenditure"
      captionText="Other staff costs"
      phaseFilter={phaseFilter}
      resetPhaseFilter={resetPhaseFilter} />
  );
}

function PremisesCosts({phaseFilter, resetPhaseFilter}: TableWrapperProps) {
  return (
    <TableWrapper
      tableData={tableData.PremisesCosts}
      mode="expenditure"
      captionText="Premises costs"
      phaseFilter={phaseFilter}
      resetPhaseFilter={resetPhaseFilter} />
  );
}

function EducationalSupplies({phaseFilter, resetPhaseFilter}: TableWrapperProps) {
  return (
    <TableWrapper
      tableData={tableData.EducationalSupplies}
      mode="expenditure"
      captionText="Educational supplies"
      phaseFilter={phaseFilter}
      resetPhaseFilter={resetPhaseFilter} />
  );
}

function Energy({phaseFilter, resetPhaseFilter}: TableWrapperProps) {
  return (
    <TableWrapper
      tableData={tableData.Energy}
      mode="expenditure"
      captionText="Energy"
      phaseFilter={phaseFilter}
      resetPhaseFilter={resetPhaseFilter} />
  );
}

function AverageTeacherCost({phaseFilter, resetPhaseFilter}: TableWrapperProps) {
  return (
    <TableWrapper
      tableData={tableData.AverageTeacherCost}
      mode="characteristics"
      dataFormat="currency"
      captionText="Average teacher cost"
      phaseFilter={phaseFilter}
      resetPhaseFilter={resetPhaseFilter} />
  );
}

function SeniorLeadersAsAPercentageOfWorkforce({phaseFilter, resetPhaseFilter}: TableWrapperProps) {
  return (
    <TableWrapper
      tableData={tableData.SeniorLeadersAsAPercentageOfWorkforce}
      mode="characteristics"
      dataFormat="percentage"
      captionText="Senior leaders as a percentage of workforce"
      phaseFilter={phaseFilter}
      resetPhaseFilter={resetPhaseFilter}/>
  );
}

function PupilToTeacherRatio({phaseFilter, resetPhaseFilter}: TableWrapperProps) {
  return (
    <TableWrapper
      tableData={tableData.PupilToTeacherRatio}
      mode="characteristics"
      captionText="Pupil to teacher ratio"
      phaseFilter={phaseFilter}
      resetPhaseFilter={resetPhaseFilter}/>
  );
}

function PupilToAdultRatio({phaseFilter, resetPhaseFilter}: TableWrapperProps) {
  return (
    <TableWrapper
      tableData={tableData.PupilToAdultRatio}
      mode="characteristics"
      captionText="Pupil to adult ratio"
      phaseFilter={phaseFilter}
      resetPhaseFilter={resetPhaseFilter}/>
  );
}


function OfstedRating({phaseFilter, resetPhaseFilter}: TableWrapperProps) {
  return (
    <TableWrapper
      tableData={tableData.OfstedRating}
      mode="outcomes"
      captionText="Ofsted rating"
      phaseFilter={phaseFilter}
      resetPhaseFilter={resetPhaseFilter}/>
  )
}

function Ks2Score({phaseFilter, resetPhaseFilter}: TableWrapperProps) {
  return (
    <TableWrapper
      tableData={tableData.Ks2Score}
      mode="outcomes"
      captionText="KS2 score"
      phaseFilter={phaseFilter}
      resetPhaseFilter={resetPhaseFilter}/>
  );
}

function Progress8Score({phaseFilter, resetPhaseFilter}: TableWrapperProps) {
  return (
    <TableWrapper
      tableData={tableData.Progress8Score}
      mode="outcomes"
      captionText="Progress 8 score"
      phaseFilter={phaseFilter}
      resetPhaseFilter={resetPhaseFilter} />
  );
}

export default function SadTrustApp() {
  const phases: (string | undefined)[] = sadData.map((item: SadDataObject) => item.OverallPhase)
    .filter((v:string, i: number, a: string) => a.indexOf(v) === i);
  const allPhases = ['All', ...phases];
  
  const hasKs2Progress = tableData.Ks2Score.length > 0;
  const hasProgress8 = tableData.Progress8Score.length > 0;
  
  const [establishmentCount, setEstablishmentCount] = useState<number>(sadData.length);
  const [phaseFilter, setPhaseFilter] = useState<string>('All');
  const [availablePhases, setAvailablePhases] = useState<(string | undefined)[]>(allPhases);
  const HandlePhaseChange = (e: ChangeEvent<HTMLSelectElement>) => {
    setPhaseFilter(e.target.value);
  }
  const ResetPhaseFilter = () => {
    setPhaseFilter('All');
  }

  return (
    <StrictMode>
    <BrowserRouter basename={baseName}>
      <Routes>
        <Route path={"/"} 
               element={
                  <Layout
                    phaseFilter={phaseFilter}
                    availablePhases={availablePhases}
                    establishmentCount={establishmentCount}
                    HandlePhaseChange={HandlePhaseChange}
                    filterValue={phaseFilter}
                    hasKs2Progress={hasKs2Progress}
                    hasProgress8={hasProgress8}/>
               }>
          
          <Route path="/InYearBalance" element={<InYearBalance phaseFilter={phaseFilter} resetPhaseFilter={ResetPhaseFilter} />}/>
          <Route path="/RevenueReserve" element={<RevenueReserve phaseFilter={phaseFilter} resetPhaseFilter={ResetPhaseFilter} />}/>
          <Route path="/TeachingStaff" element={<TeachingStaff phaseFilter={phaseFilter} resetPhaseFilter={ResetPhaseFilter} />}/>
          <Route path="/SupplyStaff" element={<SupplyStaff phaseFilter={phaseFilter} resetPhaseFilter={ResetPhaseFilter}  />}/>
          <Route path="/EducationSupportStaff" element={<EducationSupportStaff phaseFilter={phaseFilter} resetPhaseFilter={ResetPhaseFilter} />}/>
          <Route path="/AdministrativeAndClericalStaff" element={<AdministrativeAndClericalStaff phaseFilter={phaseFilter} resetPhaseFilter={ResetPhaseFilter} />}/>
          <Route path="/OtherStaffCosts" element={<OtherStaffCosts phaseFilter={phaseFilter} resetPhaseFilter={ResetPhaseFilter} />}/>
          <Route path="/PremisesCosts" element={<PremisesCosts phaseFilter={phaseFilter} resetPhaseFilter={ResetPhaseFilter} />}/>
          <Route path="/EducationalSupplies" element={<EducationalSupplies  phaseFilter={phaseFilter} resetPhaseFilter={ResetPhaseFilter}  />}/>
          <Route path="/Energy" element={<Energy  phaseFilter={phaseFilter} resetPhaseFilter={ResetPhaseFilter} />}/>
          <Route path="/AverageTeacherCost" element={<AverageTeacherCost phaseFilter={phaseFilter} resetPhaseFilter={ResetPhaseFilter} />}/>
          <Route path="/SeniorLeadersAsAPercentageOfWorkforce" element={<SeniorLeadersAsAPercentageOfWorkforce phaseFilter={phaseFilter} resetPhaseFilter={ResetPhaseFilter} />}/>
          <Route path="/PupilToTeacherRatio" element={<PupilToTeacherRatio phaseFilter={phaseFilter} resetPhaseFilter={ResetPhaseFilter} />}/>
          <Route path="/PupilToAdultRatio" element={<PupilToAdultRatio phaseFilter={phaseFilter} resetPhaseFilter={ResetPhaseFilter} />}/>
          <Route path="/OfstedRating" element={<OfstedRating phaseFilter={phaseFilter} resetPhaseFilter={ResetPhaseFilter} />}/>
          <Route path="/Ks2Score" element={<Ks2Score phaseFilter={phaseFilter} resetPhaseFilter={ResetPhaseFilter} />}/>
          <Route path="/Progress8Score" element={<Progress8Score phaseFilter={phaseFilter} resetPhaseFilter={ResetPhaseFilter} />}/>
        </Route>
      </Routes>
    </BrowserRouter>
    </StrictMode>
  )
}