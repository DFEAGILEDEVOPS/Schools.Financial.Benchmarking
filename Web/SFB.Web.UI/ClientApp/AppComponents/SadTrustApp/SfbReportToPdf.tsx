import React from 'react';
import jsPDF from 'jspdf';
import 'jspdf-autotable';
import {UserOptions} from 'jspdf-autotable';


interface jsPDFCustom extends jsPDF {
  autoTable: (options: UserOptions) => void;
}

export default function SfbReportToPdf() {
  
  const GeneratePdf = () => {
    const unit = "pt";
    const size = "A4";
    const orientation = "portrait";

    const doc = new jsPDF(orientation, unit, size) as jsPDFCustom;
    doc.text('Self-assessment dashboard for ', 14, 20)
    doc.autoTable({
      html: '.sfb-sadtrust-table',
      theme: 'grid',
      headStyles: {
        fillColor: [29, 112, 184],
        textColor: [255,255,255]
      },
    });
    
    
    doc.save(document.title);
  }

  return (
    <div>
      <button onClick={() => GeneratePdf()}>Generate Report</button>
    </div>
  );
}