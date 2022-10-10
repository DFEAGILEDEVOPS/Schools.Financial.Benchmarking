import React, {useEffect} from 'react';

import SadTrustTable from './SadTrustTable';
import {SadDataObject} from "./Models/sadTrustTablesModels";
import {prepareTableRows} from './prepareTableRows';

declare var sadData: any;

export interface TableProps {
  phaseFilter: string
  hasKs2Progress?: boolean
  hasProgress8?: boolean
  isLoading: boolean
  isDownload?: boolean
  trustName?: string
}

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
export const tableData = prepData(sadData.filter((x: SadDataObject) => x.LatestTerm !== null));

const InYearBalance = function ({phaseFilter, isLoading, isDownload}: TableProps) {
  return (
    <SadTrustTable
      tableData={tableData.InYearBalance}
      mode="income"
      captionText="In year balance"
      phaseFilter={phaseFilter}
      isLoading={isLoading}
      isDownload={isDownload}/>
  );
}

const RevenueReserve = function({phaseFilter, isLoading, isDownload}: TableProps) {
  return (
    <SadTrustTable
      tableData={tableData.RevenueReserve}
      mode="income"
      captionText="Revenue reserve"
      phaseFilter={phaseFilter}
      isLoading={isLoading}
      isDownload={isDownload} />
  );
}

const TeachingStaff = function({phaseFilter, isLoading, isDownload}: TableProps) {
  return (
    <SadTrustTable
      tableData={tableData.TeachingStaff}
      mode="expenditure"
      captionText="Teaching staff"
      phaseFilter={phaseFilter}
      isLoading={isLoading}
      isDownload={isDownload} />
  );
}

const SupplyStaff = function({phaseFilter, isLoading, isDownload}: TableProps) {
  return (
    <SadTrustTable
      tableData={tableData.SupplyStaff}
      mode="expenditure"
      captionText="Supply staff"
      phaseFilter={phaseFilter}
      isLoading={isLoading}
      isDownload={isDownload}  />
  );
}

const EducationSupportStaff = function({phaseFilter, isLoading, isDownload}: TableProps) {
  return (
    <SadTrustTable
      tableData={tableData.EducationSupportStaff}
      mode="expenditure"
      captionText="Education support staff"
      phaseFilter={phaseFilter}
      isLoading={isLoading}
      isDownload={isDownload} />
  );
}

const AdministrativeAndClericalStaff = function({phaseFilter, isLoading, isDownload}: TableProps) {
  return (
    <SadTrustTable
      tableData={tableData.AdministrativeAndClericalStaff}
      mode="expenditure"
      captionText="Administrative and clerical staff"
      phaseFilter={phaseFilter}
      isLoading={isLoading}
      isDownload={isDownload} />
  );
}

const OtherStaffCosts = function({phaseFilter, isLoading, isDownload}: TableProps) {
  return (
    <SadTrustTable
      tableData={tableData.OtherStaffCosts}
      mode="expenditure"
      captionText="Other staff costs"
      phaseFilter={phaseFilter}
      isLoading={isLoading}
      isDownload={isDownload} />
  );
}

const PremisesCosts = function({phaseFilter, isLoading, isDownload}: TableProps) {
  return (
    <SadTrustTable
      tableData={tableData.PremisesCosts}
      mode="expenditure"
      captionText="Premises costs"
      phaseFilter={phaseFilter}
      isLoading={isLoading}
      isDownload={isDownload} />
  );
}

const EducationalSupplies = function({phaseFilter, isLoading, isDownload}: TableProps) {
  return (
    <SadTrustTable
      tableData={tableData.EducationalSupplies}
      mode="expenditure"
      captionText="Educational supplies"
      phaseFilter={phaseFilter}
      isLoading={isLoading}
      isDownload={isDownload} />
  );
}

const Energy = function({phaseFilter, isLoading, isDownload}: TableProps) {
  return (
    <SadTrustTable
      tableData={tableData.Energy}
      mode="expenditure"
      captionText="Energy"
      phaseFilter={phaseFilter}
      isLoading={isLoading}
      isDownload={isDownload} />
  );
}

const AverageTeacherCost = function({phaseFilter, isLoading, isDownload}: TableProps) {
  return (
    <SadTrustTable
      tableData={tableData.AverageTeacherCost}
      mode="characteristics"
      dataFormat="currency"
      captionText="Average teacher cost"
      phaseFilter={phaseFilter}
      isLoading={isLoading}
      isDownload={isDownload} />
  );
}

const SeniorLeadersAsAPercentageOfWorkforce = function ({phaseFilter, isLoading, isDownload}: TableProps) {
  return (
    <SadTrustTable
      tableData={tableData.SeniorLeadersAsAPercentageOfWorkforce}
      mode="characteristics"
      dataFormat="percentage"
      captionText="Senior leaders as a percentage of workforce"
      phaseFilter={phaseFilter}
      isLoading={isLoading}
      isDownload={isDownload}  />
  );
}

const PupilToTeacherRatio = function({phaseFilter, isLoading, isDownload}: TableProps) {
  return (
    <SadTrustTable
      tableData={tableData.PupilToTeacherRatio}
      mode="characteristics"
      captionText="Pupil to teacher ratio"
      phaseFilter={phaseFilter}
      isLoading={isLoading}
      isDownload={isDownload} />
  );
}

const PupilToAdultRatio = function({phaseFilter, isLoading, isDownload}: TableProps) {
  return (
    <SadTrustTable
      tableData={tableData.PupilToAdultRatio}
      mode="characteristics"
      captionText="Pupil to adult ratio"
      phaseFilter={phaseFilter}
      isLoading={isLoading}
      isDownload={isDownload} />
  );
}

const OfstedRating = function({phaseFilter, isLoading, isDownload}: TableProps) {
  return (
    <SadTrustTable
      tableData={tableData.OfstedRating}
      mode="outcomes"
      captionText="Ofsted rating"
      phaseFilter={phaseFilter}
      isLoading={isLoading}
      isDownload={isDownload}  />
  )
}

const Ks2Score = function({phaseFilter, isLoading, isDownload}: TableProps) {
  return (
    <SadTrustTable
      tableData={tableData.Ks2Score}
      mode="outcomes"
      captionText="KS2 score"
      phaseFilter={phaseFilter}
      isLoading={isLoading}
      isDownload={isDownload} />
  );
}

const Progress8Score = function({phaseFilter, isLoading, isDownload}: TableProps) {
  return (
    <SadTrustTable
      tableData={tableData.Progress8Score}
      mode="outcomes"
      captionText="Progress 8 score"
      phaseFilter={phaseFilter}
      isLoading={isLoading}
      isDownload={isDownload}  />
  );
}

const PrintAll = function ({ hasKs2Progress, hasProgress8, phaseFilter, isLoading, isDownload}:TableProps) {
  useEffect(()=> {
    const iframe = parent.document.getElementById('sfb-dashboard-print-frame') as HTMLIFrameElement;
    const iWindow = iframe.contentWindow;
    if (iWindow) {
      window.setTimeout(()=> {
        iWindow.print();
      }, 1000);
    }
  },[]);
  return (
    <>
      {InYearBalance({phaseFilter, isLoading})}
      {RevenueReserve({phaseFilter, isLoading})}
      {TeachingStaff({phaseFilter, isLoading})}
      {SupplyStaff({phaseFilter, isLoading})}
      {EducationSupportStaff({phaseFilter, isLoading})}
      {AdministrativeAndClericalStaff({phaseFilter, isLoading})}
      {OtherStaffCosts({phaseFilter, isLoading})}
      {PremisesCosts({phaseFilter, isLoading})}
      {EducationalSupplies({phaseFilter, isLoading})}
      {Energy({phaseFilter, isLoading})}
      {AverageTeacherCost({phaseFilter, isLoading})}
      {SeniorLeadersAsAPercentageOfWorkforce({phaseFilter, isLoading})}
      {PupilToTeacherRatio({phaseFilter, isLoading})}
      {PupilToAdultRatio({phaseFilter, isLoading})}
      {OfstedRating({phaseFilter, isLoading})}
      {hasKs2Progress && Ks2Score({phaseFilter, isLoading})}
      {hasProgress8 && Progress8Score({phaseFilter, isLoading})}
    </>
  )
};

const Views = {
  InYearBalance,
  RevenueReserve,
  TeachingStaff,
  SupplyStaff,
  EducationSupportStaff,
  AdministrativeAndClericalStaff,
  OtherStaffCosts,
  PremisesCosts,
  EducationalSupplies,
  Energy,
  AverageTeacherCost,
  SeniorLeadersAsAPercentageOfWorkforce,
  PupilToTeacherRatio,
  PupilToAdultRatio,
  OfstedRating,
  Ks2Score,
  Progress8Score,
  PrintAll,
};

export default Views;