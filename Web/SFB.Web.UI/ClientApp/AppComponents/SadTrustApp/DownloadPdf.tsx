import React, {useEffect} from 'react';
import Views, {TableProps} from './Views';
import jsPDF from 'jspdf';
import 'jspdf-autotable';
import {UserOptions} from 'jspdf-autotable';


interface jsPDFCustom extends jsPDF {
  autoTable: (options: UserOptions) => void;
}

export default function DownloadPdf({hasKs2Progress, hasProgress8, phaseFilter, isLoading, trustName}: TableProps) {
  const GeneratePdf = () => {
    const unit = "pt";
    const size = "A4";
    const orientation = "portrait";
    let yPos = 30;

    const iframe = parent.document.getElementById('sfb-dashboard-download-frame') as HTMLIFrameElement;
    const reportTables = iframe.contentWindow?.document.querySelectorAll('table');
    const doc = new jsPDF(orientation, unit, size) as jsPDFCustom;
    const totalPagesExp = '{total_pages_count_string}';

    reportTables?.forEach((table: HTMLTableElement, i) => {
      doc.autoTable({
        html: table,
        theme: 'grid',
        headStyles: {
          fillColor: [29, 112, 184],
          textColor: [255, 255, 255],
          lineColor: [255, 255, 255],
        },
        startY: yPos,
        pageBreak: i === 0 ? 'auto' : 'always',
        didDrawPage: function (data) {
          let str = 'Page ' + (doc as any).internal.getNumberOfPages()
          if (typeof doc.putTotalPages === 'function') {
            str = str + ' of ' + totalPagesExp + ': Self assessment dashboard for ' + trustName;
          }
          doc.setFontSize(10);

          const pageSize = doc.internal.pageSize
          const pageHeight = pageSize.height ? pageSize.height : pageSize.getHeight()
          doc.text(str, data.settings.margin.left, pageHeight - 10);
        },
      });

      yPos = (doc as any).lastAutoTable.finalY + 10;
    });
    if (typeof doc.putTotalPages === 'function') {
      doc.putTotalPages(totalPagesExp)
    }

    doc.save(`Self-assessment-dashboard-${trustName?.replace(/[^a-z0-9]/gi, '')}.pdf`);
  };

  useEffect(() => {
    const timer = setTimeout(() => {
      GeneratePdf();
    }, 3000);

    return () => {
      clearTimeout(timer);
    }
  }, []);
  return (
    <>
      {Views.InYearBalance({phaseFilter, isLoading, isDownload: true})}
      {Views.RevenueReserve({phaseFilter, isLoading, isDownload: true})}
      {Views.TeachingStaff({phaseFilter, isLoading, isDownload: true})}
      {Views.SupplyStaff({phaseFilter, isLoading, isDownload: true})}
      {Views.EducationSupportStaff({phaseFilter, isLoading, isDownload: true})}
      {Views.AdministrativeAndClericalStaff({phaseFilter, isLoading, isDownload: true})}
      {Views.OtherStaffCosts({phaseFilter, isLoading, isDownload: true})}
      {Views.PremisesCosts({phaseFilter, isLoading, isDownload: true})}
      {Views.EducationalSupplies({phaseFilter, isLoading, isDownload: true})}
      {Views.Energy({phaseFilter, isLoading, isDownload: true})}
      {Views.AverageTeacherCost({phaseFilter, isLoading, isDownload: true})}
      {Views.SeniorLeadersAsAPercentageOfWorkforce({phaseFilter, isLoading, isDownload: true})}
      {Views.PupilToTeacherRatio({phaseFilter, isLoading, isDownload: true})}
      {Views.PupilToAdultRatio({phaseFilter, isLoading, isDownload: true})}
      {Views.OfstedRating({phaseFilter, isLoading, isDownload: true})}
      {hasKs2Progress && Views.Ks2Score({phaseFilter, isLoading, isDownload: true})}
      {hasProgress8 && Views.Progress8Score({phaseFilter, isLoading, isDownload: true})}
    </>
  )
}