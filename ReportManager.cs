using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Phoenix.Infrastructure.ViewModel.Classifier;
using Phoenix.Infrastructure.Data.Native;
using Phoenix.Infrastructure.ViewModel.Report;
using System.Text;

namespace Phoenix.Web.Logic
{
    public class ReportManager
    {
        public List<ActualReportView> GetActualReport(FilterReport fr)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($@"WHERE AD.[Month]={fr.month}");
            if(fr.spend !=0)
            {
                sb.Append($"AND Spend_1={fr.spend}");
            } 
            List<ActualReportView> ListReport = null;
            var sql = $@"SELECT
                            AD.Id,
                            AD.[Year],
                            AD.[Month],
                            AD.Client_Id,
                            AD.Brand_Id,
                            AD.Spend_Id,
                            GroupClients.Name As GroupClient,
                            Clients.Name As Client,
                            Product1.Name As Product_1,
                            Product2.Name As Product_2,
                            Product3.Name As Product_3,
                            Spend1.Name As Spend_1,
                            Spend2.Name As Spend_2,
                            Spend3.Name As Spend_3,
                            AD.EUser,
                            AD.EDo,
                            AD.EDocumentHeaderText,
                            AD.EPurchaseOrderText,
                            AD.EOperationName,
                            AD.EMaterial,
                            AD.EMaterialDescription,
                            AD.EPostingDate,
                            AD.CreatedOn,
                            AD.ETotalQuantity,
                            AD.EDocumentDate,
                            AD.EVendorName,
                            AD.ETransactionCurrency,
                            AD.ETransactionCurrencySum,
                            AD.EIOName,
                            AD.EIOCode,
                            AD.EGLName,
                            AD.EGLCode

                            FROM
                            dbo.ActualData AS AD
                            LEFT JOIN dbo.Classifier AS Clients
                            ON Clients.Id = AD.Client_Id
                            LEFT JOIN dbo.Classifier AS GroupClients
                            ON Clients.Parent_Id = GroupClients.Id
                            LEFT JOIN dbo.Classifier AS Product3
                            ON Product3.Id = AD.Brand_Id
                            LEFT JOIN dbo.Classifier AS Product2
                            ON Product2.Id = Product3.Parent_Id
                            LEFT JOIN dbo.Classifier AS Product1
                            ON Product1.Id = Product2.Parent_Id
                            LEFT JOIN dbo.Classifier AS Spend3
                            ON Spend3.Id = AD.Spend_Id
                            LEFT JOIN dbo.Classifier AS Spend2
                            ON Spend3.Parent_Id = Spend2.Id
                            LEFT JOIN dbo.Classifier As Spend1
                            ON Spend1.Id = Spend2.Parent_Id";
            var ddm = new DirectDataManager(sql);
            ListReport = ddm.ToList<ActualReportView>().ToList();
            for(int i = 0; i < ListReport.Count; i++)
            {
                ListReport[i].Period = GetDatePerion(ListReport[i].Year, ListReport[i].Month);
   
            }
            return ListReport;
        }

        public string GetDatePerion(int year, int month)
        {
            string year_str = "";
            string month_str = "";
            if (month < 10)
            {
                month_str = "0" + month.ToString();
            }
            else
            {
                month_str = month.ToString();
            }
            year_str = year.ToString();
            return year_str + month_str;
        }
        
    }
}