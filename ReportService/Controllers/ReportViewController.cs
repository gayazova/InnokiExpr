using FastReport.Web;
using Microsoft.AspNetCore.Mvc;

namespace ReportService.Controllers
{
    public class ReportViewController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Designer()
        {
            var webReport = new WebReport();
            webReport.Width = "1000";
            webReport.Height = "1000";

            webReport.Report.Load(@"D:\Загрузки\FastReport.CoreWin_Demo\FastReport.CoreWin 2022.2.0 Demo\Reports\Simple List.frx");
            var dataSet = new System.Data.DataSet();
            dataSet.ReadXml(@"D:\Загрузки\FastReport.CoreWin_Demo\FastReport.CoreWin 2022.2.0 Demo\Reports\nwind.xml");
            webReport.Report.RegisterData(dataSet, "NorthWind");
            webReport.Mode = WebReportMode.Designer;
            FastReport.Utils.Config.WebMode = true;
            webReport.Toolbar.ShowOnDialogPage = false;
            webReport.Toolbar.ShowRefreshButton = false;
            webReport.Toolbar.Exports.ExportTypes = Exports.Pdf | Exports.Excel2007 | Exports.Word2007;
            webReport.Toolbar.PrintInHtml = false;
            webReport.DesignerPath = "/WebReportDesigner/index.html"; //Задаем URL онлайн дизайнера
            webReport.DesignerSaveMethod = (x, y, z) => "/Report/SaveDesignedReport"; //Задаем URL представления для метода сохранения отчета
            return View(nameof(Designer), webReport);
        }
    }
}
