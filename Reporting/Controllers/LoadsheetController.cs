using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Reporting.Models;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.HSSF.Util;
using NPOI.POIFS.FileSystem;
using NPOI.HPSF;
using System.IO;
using System.Web.Security;

namespace Reporting.Controllers
{
    public class LoadsheetController : BaseController
    {
        //
        // GET: /Loadsheet/

        public ActionResult Index(string error = "")
        {
            if (ViewBag.isAdmin != true) {
                return Redirect("/Reports");
            }
            if (error != "") {
                ViewBag.err = error;
            }
            try {
                List<CustomerLoadsheet> customerLoadsheets = new List<CustomerLoadsheet>();
                CurtDevDataContext db = new CurtDevDataContext();
                customerLoadsheets = db.CustomerLoadsheets.ToList<CustomerLoadsheet>();
                ViewBag.loadsheets = customerLoadsheets;
            } catch (Exception e) {
                ViewBag.err = "Error: " + e.Message;
            }
            return View();
        }

        [HttpGet]
        public ActionResult Customize() {
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Customize(int loadsheetID) {
            try {
                if (ViewBag.isAdmin != true) {
                    return Redirect("/Reports");
                }
                if (loadsheetID == 0) {
                    throw new Exception("You must select a loadsheet");
                }
                CurtDevDataContext db = new CurtDevDataContext();

                CustomerLoadsheet loadsheet = db.CustomerLoadsheets.Where(x => x.id == loadsheetID).FirstOrDefault<CustomerLoadsheet>();
                ViewBag.loadsheet = loadsheet;
                
                List<LoadsheetField> masterFieldList = new List<LoadsheetField>();
                masterFieldList = db.LoadsheetFields.Where(x => x.loadsheetID == loadsheet.id || x.loadsheetID == 0).OrderBy(x => x.loadsheetID).ToList<LoadsheetField>();
                ViewBag.masterFieldList = masterFieldList;

                List<LoadsheetField> customerFieldList = new List<LoadsheetField>();
                customerFieldList = db.CustomerLoadsheetFields.Where(x => x.loadsheedID == loadsheet.id).OrderBy(x => x.displayOrder).Select(x => x.LoadsheetFields).ToList<LoadsheetField>();
                ViewBag.customerFieldList = customerFieldList;
                
            } catch (Exception e) {
                return RedirectToAction("Index", new { error = e.Message });
            }
            return View();
        }


        public FileContentResult GenerateCSV(int id, string strDateAdded, int[] fieldIDs) {
            string csv = "";
            string seperator = ",";
            int loadsheetID = id;
            DateTime? dateAdded = null;
            try {
                dateAdded = Convert.ToDateTime(strDateAdded);
            } catch { }
            // get list of parts based off dateAdded
            CurtDevDataContext db = new CurtDevDataContext();
            List<Part> parts = db.Parts.Where(x => x.dateAdded >= dateAdded).ToList<Part>();
            CustomerLoadsheet loadsheet = db.CustomerLoadsheets.Where(x => x.id == loadsheetID).FirstOrDefault();
            if (loadsheet == null) {
                loadsheet = new CustomerLoadsheet();
                loadsheet.name = "Generic";
            }

            // foreach through the list of parts
            foreach (Part p in parts) {
                string row = "";
                string temp = "";
                foreach (int field in fieldIDs) {
                    temp = Loadsheet.GenerateCellValue(field, p, db);
                    temp = (temp != "") ? temp : "";
                    row += temp.Replace(",","");
                    row += seperator;

                }// end foreach fieldIDs
                csv += row;
                csv += Environment.NewLine;
            }// end foreach part

            return File(new System.Text.UTF8Encoding().GetBytes(csv), "text/csv", loadsheet.name + "_Loadsheet_" + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year + ".csv");
        }

        public void GenerateXLS(int id, string strDateAdded, int[] fieldIDs) {
            var workbook = new HSSFWorkbook();
            var sheet = workbook.CreateSheet("loadsheet");
            var rowIndex = 0;
           
            int loadsheetID = id;
            DateTime? dateAdded = null;
            try {
                dateAdded = Convert.ToDateTime(strDateAdded);
            } catch { }
            // get list of parts based off dateAdded
            CurtDevDataContext db = new CurtDevDataContext();
            List<Part> parts = db.Parts.Where(x => x.dateAdded >= dateAdded).ToList<Part>();
            CustomerLoadsheet loadsheet = db.CustomerLoadsheets.Where(x => x.id == loadsheetID).FirstOrDefault();
            if (loadsheet == null) {
                loadsheet = new CustomerLoadsheet();
                loadsheet.name = "Generic";
            }

            // foreach through the list of parts
            #region RowGeneration
            foreach (Part p in parts) {
                string temp = "";
                var xlsRow = sheet.CreateRow(rowIndex);
                var columnNum = 0;
                foreach (int field in fieldIDs) {
                    temp = Loadsheet.GenerateCellValue(field, p, db);
                    temp = (temp != "") ? temp : "";
                    xlsRow.CreateCell(columnNum).SetCellValue(temp);
                    columnNum++;
                }// end foreach fieldIDs
                rowIndex++;
            }// end foreach part
            #endregion

            using (var exportData = new MemoryStream())
                    {
                    workbook.Write(exportData);
                    string saveAsFileName = string.Format("Loadsheet-{0:d}.xls", DateTime.Now).Replace("/", "-");
                    Response.ContentType = "application/vnd.ms-excel";
                    Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", saveAsFileName));
                    Response.Clear();
                    Response.BinaryWrite(exportData.GetBuffer());
                    Response.End();
                    }
        }

    }

}
