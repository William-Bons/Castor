using Castor.database;
using Castor.database.tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Castor.database.reports
{
    public interface ICastorHtmlReport
    {
        public string HtmlReport { get; }
        public DatePeriod datePeriod { get; set; }
        public void Calculate();
        public void RegisterInReports()
        {
            using CastorContext context = new CastorContext();
            Reports _CreatedReport = new Reports();
            //save report html into database
            _CreatedReport.Created = DateTime.Now;
            _CreatedReport.DateStart = DateOnly.FromDateTime(datePeriod.Start);
            _CreatedReport.DateEnd = DateOnly.FromDateTime(datePeriod.End);
            _CreatedReport.ReportName = this.GetType().FullName;
            _CreatedReport.ReportData = Encoding.UTF8.GetBytes(HtmlReport);
            context.Reports.Update(_CreatedReport);
            context.SaveChanges();
        }

    }
}
