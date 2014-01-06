using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using Reporting.Models;
using System.Web.Script.Serialization;
using System.Xml.Linq;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;
using System.Web.Mvc.Html;
using System.Reflection;


namespace Reporting.Controllers {
    public class ReportsController : BaseController {

        public ActionResult Index() {

            return View();
        }

        public ActionResult GoogleBase(){
            var helper = new Helper();

            ViewBag.Countries = helper.GetCountries();
            return View();
        }

        public ActionResult PIES()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public void Generate(){
            try{
                string title = Request.Form["feedTitle"];
                string desc = Request.Form["feedDesc"];
                string link = Request.Form["feedLink"];
                string partLink = Request.Form["partLink"];
                int custId = Convert.ToInt32(ViewBag.Customer.customerID);
                string taxJson = Request.Form["tax"];
                
                var taxes = new List<GoogleTax>();
                try{
                    if (!string.IsNullOrEmpty(taxJson)){
                        taxes = new JavaScriptSerializer().Deserialize<List<GoogleTax>>(taxJson);
                    }
                }catch(Exception){
                    taxes = new List<GoogleTax>();
                }

                Response.ContentType = "application/rss+xml";
                Response.AddHeader("Content-Disposition", "attachment;filename=base.xml");
                new GoogleBase(){
                                    FeedLink = new Uri(link),
                                    FeedTitle = title,
                                    FeedDescription = desc,
                                    CustId = custId,
                                    Taxes = taxes,
                                    PartLink = partLink
                                }.Generate(Response);

            }catch(Exception e){
                Response.Redirect("/Reports/GoogleBase?error=There was an error while processing your request.");
            }
        }

        public void GenerateACES()
        {
           //DateTime? start = null;
           //try
           //{
           //    start = Convert.ToDateTime(startdate);
           //}
           //catch { }
            XDocument report = new ACES().GenerateReport();

            StringWriter sw = new StringWriter();

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            XmlWriter w = XmlWriter.Create(sw, settings);

            report.WriteTo(w);
            w.Flush();
            sw.GetStringBuilder().ToString();

            w.Close();

            string attachment = "attachment; filename=CURT_Reports_ACESreport-" + String.Format("{0:yyyyMMddhhmmss}", DateTime.Now) + ".xml";
            HttpContext.Response.Clear();
            HttpContext.Response.ClearHeaders();
            HttpContext.Response.ClearContent();
            HttpContext.Response.AddHeader("content-disposition", attachment);
            HttpContext.Response.ContentType = "text/xml";
            HttpContext.Response.Write(sw);
            HttpContext.Response.End();
        }

        public void GeneratePIES(string startdate = "")
        {
            DateTime? start = null;
            try
            {
                start = Convert.ToDateTime(startdate);
            }
            catch { }
            XDocument xdoc = new XDocument();
            xdoc = new Report().generate(start);
            StringWriter wr = new StringWriter();
            xdoc.Save(wr);

            string attachment = "attachment; filename=CURT_Reports_PIESreport-" + String.Format("{0:yyyyMMddhhmmss}", DateTime.Now) + ".xml";
            HttpContext.Response.Clear();
            HttpContext.Response.ClearHeaders();
            HttpContext.Response.ClearContent();
            HttpContext.Response.AddHeader("content-disposition", attachment);
            HttpContext.Response.ContentType = "text/xml";
            HttpContext.Response.ContentEncoding = System.Text.Encoding.Unicode;
            HttpContext.Response.AddHeader("Pragma", "public");
            HttpContext.Response.Write(wr.GetStringBuilder().ToString());
            HttpContext.Response.End();
        }

        public void GenerateDataReport(string startdate = "")
        {
            DateTime? start = null;
            try
            {
                start = Convert.ToDateTime(startdate);
            }
            catch { }
            XDocument xdoc = new DataReport().generate(start);
            StringWriter wr = new StringWriter();
            xdoc.Save(wr);
            string attachment = "attachment; filename=CURT_Reports_DataReport" + String.Format("{0:yyyyMMddhhmm}", DateTime.Now) + ".xml";
                HttpContext.Response.Clear();
                HttpContext.Response.ClearHeaders();
                HttpContext.Response.ClearContent();
                HttpContext.Response.AddHeader("content-disposition", attachment);
                HttpContext.Response.ContentType = "text/xml";
                HttpContext.Response.ContentEncoding = System.Text.Encoding.Unicode;
                HttpContext.Response.AddHeader("Pragma", "public");
                HttpContext.Response.Write(wr.GetStringBuilder().ToString());
                //HttpContext.Response.SetCookie(new HttpCookie("fileDownload", "true") { Path = "/" });
                HttpContext.Response.End();
           
        }

        public void GenerateVehicleReport(string startdate = "", string method = "")
        {
            //HttpContext.Response.SetCookie(new HttpCookie("fileDownload", "true") { Path = "/" });
            DateTime? start = null;
            try
            {
                start = Convert.ToDateTime(startdate);
            }
            catch { }
            if (method == "csv") {

                var xdoc = new VehicleReport().generate_csv(start);

                string attachment = "attachment; filename=CURT_Reports_VehicleReport" + String.Format("{0:yyyyMMddhhmm}", DateTime.Now) + ".csv";
                HttpContext.Response.Clear();
                HttpContext.Response.ClearHeaders();
                HttpContext.Response.ClearContent();
                HttpContext.Response.AddHeader("content-disposition", attachment);
                HttpContext.Response.ContentType = "text/csv";
                HttpContext.Response.ContentEncoding = System.Text.Encoding.Unicode;
                HttpContext.Response.AddHeader("Pragma", "public");
                HttpContext.Response.Write(xdoc);
                HttpContext.Response.End();
            } else {

                XDocument xdoc = new VehicleReport().generate(start);
                StringWriter wr = new StringWriter();
                xdoc.Save(wr);

                string attachment = "attachment; filename=CURT_Reports_VehicleReport" + String.Format("{0:yyyyMMddhhmm}", DateTime.Now) + ".xml";
                HttpContext.Response.Clear();
                HttpContext.Response.ClearHeaders();
                HttpContext.Response.ClearContent();
                HttpContext.Response.AddHeader("content-disposition", attachment);
                HttpContext.Response.ContentType = "text/xml";
                HttpContext.Response.ContentEncoding = System.Text.Encoding.Unicode;
                HttpContext.Response.AddHeader("Pragma", "public");
                HttpContext.Response.Write(xdoc);
                HttpContext.Response.End();
            }
        }

        public ActionResult CURTReports()
        {
            var namesDateModCSV = new List<string>();
            namesDateModCSV.Add("Full Media Report.csv");
            namesDateModCSV.Add("Image.csv");
            namesDateModCSV.Add("Installation Sheet.csv");
            ViewBag.reportNames2 = namesDateModCSV;

            var namesFullCSV = new List<string>();
            namesFullCSV.Add("Video.csv");
            ViewBag.reportNames1 = namesFullCSV;

            return View();
        }

        public ActionResult ChooseReports(string reportname, string startdate = "")
        {
            DateTime? start = null;
            try
            {
                start = Convert.ToDateTime(startdate);
            }
            catch { }
      
           switch (reportname)
            {
                case "Image.csv":
                    buildImageCSV(start);
                    break;
                case "Video.csv":
                    buildVideoCSV();
                    break;
                case "Installation Sheet.csv":
                    PartandInstallSheet(start);
                    break;
                case "Full Media Report.csv":
                    BuildMediaReport(start);
                    break;
                default:
                    Response.Redirect("~/reports/CURTReports");
                    break;
            }
            return View();
        }

        public void buildImageCSV(DateTime? start = null)
        {
            CurtDevDataContext db = new CurtDevDataContext();
            string strComma = ",";
            string attachment = "attachment; filename=CURT_Reports__ImageReport" + String.Format("{0:yyyyMMddhhmm}", DateTime.Now) + ".csv";
            HttpContext.Response.Clear();
            HttpContext.Response.ClearHeaders();
            HttpContext.Response.ClearContent();
            HttpContext.Response.AddHeader("content-disposition", attachment);
            HttpContext.Response.ContentType = "text/csv";
            HttpContext.Response.ContentEncoding = System.Text.Encoding.Unicode;
            StreamWriter swOutputFile = new StreamWriter(Response.OutputStream);

            List<MyPart> partAndImage = new List<MyPart>();
            List<int> statuses = new List<int> { 800, 900 };
            if (start != null)
            {
                partAndImage = db.PartImages.Where(p => (p.path != null)).Where(p=>statuses.Contains(p.Part.status)).Where(p => p.Part.dateModified >= start)
                        .OrderByDescending(p => p.Part.dateModified).Select(p => new MyPart
                        {
                            PartID = p.partID,
                            dateModified = p.Part.dateModified.ToShortDateString(),
                            path = p.path,
                        }).ToList();
            }else{
                partAndImage = db.PartImages.Where(p => (p.path != null)).OrderByDescending(p => p.Part.dateModified).Select(p => new MyPart
                        {
                            PartID = p.partID,
                            dateModified = p.Part.dateModified.ToShortDateString(),
                            path = p.path,
                        }).ToList();
            }

            try
            {
                StringBuilder sbConcatJob = new StringBuilder();
                sbConcatJob.Append("Date Modified").Append(strComma).Append("Part Number").Append(strComma).Append("File Path").Append(Environment.NewLine);
                foreach (var x in partAndImage)
                    {
                        sbConcatJob.Append(x.dateModified).Append(strComma).Append(x.PartID).Append(strComma).Append(x.path).Append(Environment.NewLine);
                    }
                swOutputFile.WriteLine(sbConcatJob.ToString());
                swOutputFile.Close();
                HttpContext.Response.End();
            }  catch (Exception e) {
                Response.Redirect("/reports/CURTReports");
            }
        }

        public List<string> getPartVideos(int partID)
        {
            CurtDevDataContext db = new CurtDevDataContext();
            List<string> ListofVideos = db.PartVideos.Where(x => x.partID == partID).Select(x => x.video).ToList();
            return ListofVideos;
        }

        public List<MyPart> getPartAndVideo()
        {
            CurtDevDataContext db = new CurtDevDataContext();
            List<int> partIDs = db.Parts.Where(x => x.status == 800).Select(x => x.partID).ToList();
            List<MyPart> partAndVideo = new List<MyPart>();
            foreach (int x in partIDs)
            {
                MyPart myPart = new MyPart();
                myPart.PartID = x;
                myPart.Videos = getPartVideos(x);
                partAndVideo.Add(myPart);
            }
            return partAndVideo;
        }

        public void buildVideoCSV()
        {
            string attachment = "attachment; filename=CURT_Reports_VideoReport" + String.Format("{0:yyyyMMddhhmm}", DateTime.Now) + ".csv";
            HttpContext.Response.Clear();
            HttpContext.Response.ClearHeaders();
            HttpContext.Response.ClearContent();
            HttpContext.Response.AddHeader("content-disposition", attachment);
            HttpContext.Response.ContentType = "text/csv";
            HttpContext.Response.ContentEncoding = System.Text.Encoding.Unicode;
            StreamWriter swOutputFile = new StreamWriter(Response.OutputStream);

            string strComma = ",";
            List<MyPart> partsObj = new List<MyPart>();
            partsObj = getPartAndVideo();

            foreach (MyPart part in partsObj)
            {
                foreach (string imagePath in part.Videos)
                {
                    StringBuilder sbConcatJob = new StringBuilder();
                    sbConcatJob.Append(part.PartID).Append(strComma).Append("www.youtube.com/watch?v=").Append(imagePath).Append(strComma);
                    swOutputFile.WriteLine(sbConcatJob.ToString());
                }
            }

            HttpContext.Response.End();
        }

        public void PartandInstallSheet(DateTime? start = null)
        {
            CurtDevDataContext db = new CurtDevDataContext();
            string strComma = ",";
            string attachment = "attachment; filename=CURT_Reports_InstallationSheets" + String.Format("{0:yyyyMMddhhmm}", DateTime.Now) + ".csv";
            HttpContext.Response.Clear();
            HttpContext.Response.ClearHeaders();
            HttpContext.Response.ClearContent();
            HttpContext.Response.AddHeader("content-disposition", attachment);
            HttpContext.Response.ContentType = "text/csv";
            HttpContext.Response.ContentEncoding = System.Text.Encoding.Unicode;
            StreamWriter swOutputFile = new StreamWriter(Response.OutputStream);

            List<MyPart> partAndInstall = new List<MyPart>();
            List<int> statuses = new List<int> { 800, 900 };
            if (start != null)
            {
                    partAndInstall = (from p in db.Parts
                                      where statuses.Contains(p.status) && p.dateModified >= start && p.ContentBridges.Any(x => x.Content.cTypeID.Equals(5))
                                      orderby(p.dateModified) descending
                                      select new MyPart
                                      {
                                          dateModified = p.dateModified.ToShortDateString(),
                                          PartID = p.partID,
                                          InstallSheet = p.ContentBridges.Where(x => x.Content.cTypeID.Equals(5)).Select(x => x.Content.text).FirstOrDefault()
                                      }).ToList<MyPart>();
            }else{
                    partAndInstall = (from p in db.Parts
                                      where statuses.Contains(p.status) && p.ContentBridges.Any(x => x.Content.cTypeID.Equals(5))
                                      select new MyPart
                                      {
                                          dateModified = p.dateModified.ToShortDateString(),
                                          PartID = p.partID,
                                          InstallSheet = p.ContentBridges.Where(x => x.Content.cTypeID.Equals(5)).Select(x => x.Content.text).FirstOrDefault()
                                      }).ToList<MyPart>();
            }

            try
            {
                StringBuilder sbConcatJob = new StringBuilder();
                sbConcatJob.Append("Date Modified").Append(strComma).Append("Part Number").Append(strComma).Append("File Path").Append(Environment.NewLine);
                foreach (var x in partAndInstall)
                    {
                        sbConcatJob.Append(x.dateModified).Append(strComma).Append(x.PartID).Append(strComma).Append(x.InstallSheet).Append(Environment.NewLine);
                    }
                swOutputFile.WriteLine(sbConcatJob.ToString());
                swOutputFile.Close();
                HttpContext.Response.End();
            }  catch (Exception e) {
                Response.Redirect("/reports/CURTReports");
            }
        }

        public void BuildMediaReport(DateTime? start = null)
        {
            string attachment = "attachment; filename=CURT_Reports_FullMediaReport" + String.Format("{0:yyyyMMddhhmm}", DateTime.Now) + ".csv";
            HttpContext.Response.Clear();
            HttpContext.Response.ClearHeaders();
            HttpContext.Response.ClearContent();
            HttpContext.Response.AddHeader("content-disposition", attachment);
            HttpContext.Response.ContentType = "text/csv";
            HttpContext.Response.ContentEncoding = System.Text.Encoding.Unicode;

            StreamWriter swOutputFile = new StreamWriter(Response.OutputStream);

            string strComma = ",";
            CurtDevDataContext db = new CurtDevDataContext();
            List<MyPart> mediaPart = new List<MyPart>();
            List<int> statuses = new List<int> { 800, 900 };

                    if (start != null)
                    {
                            mediaPart = (from p in db.Parts
                                         where statuses.Contains(p.status) && p.dateModified >= start && p.ContentBridges.Any(x => x.Content.cTypeID.Equals(5))
                                         orderby(p.dateModified) descending
                                         select new MyPart
                                         {
                                             dateModified = p.dateModified.ToShortDateString(),
                                             PartID = p.partID,
                                             InstallSheet = p.ContentBridges.Where(x => x.Content.cTypeID.Equals(5)).Select(x => x.Content.text).FirstOrDefault(),
                                             Images = p.PartImages.Select(x => x.path).ToList(),
                                             Videos = p.PartVideos.Select(x => x.video).ToList()
                                         }).ToList<MyPart>();

                      }else{
                            mediaPart = (from p in db.Parts
                                         where statuses.Contains(p.status) && p.ContentBridges.Any(x => x.Content.cTypeID.Equals(5))
                                         select new MyPart
                                         {
                                             dateModified = p.dateModified.ToShortDateString(),
                                             PartID = p.partID,
                                             InstallSheet = p.ContentBridges.Where(x => x.Content.cTypeID.Equals(5)).Select(x => x.Content.text).FirstOrDefault(),
                                             Images = p.PartImages.Select(x => x.path).ToList(),
                                             Videos = p.PartVideos.Select(x => x.video).ToList()
                                         }).ToList<MyPart>();
                      }

            try
            {
                StringBuilder sbConcatJob = new StringBuilder();
                
                foreach (var x in mediaPart)
                {
                    StringBuilder sbConcatJob1 = new StringBuilder();
                    sbConcatJob1.Append("Date Modified = ").Append(x.dateModified).Append(strComma);
                    swOutputFile.WriteLine(sbConcatJob1.ToString());

                    StringBuilder sbConcatJob5 = new StringBuilder();
                    sbConcatJob5.Append("Part Number #").Append(x.PartID).Append(strComma);
                    swOutputFile.WriteLine(sbConcatJob5.ToString());
                    
                    StringBuilder sbConcatJob4 = new StringBuilder();
                    sbConcatJob4.Append(x.InstallSheet).Append(strComma);
                    swOutputFile.WriteLine(sbConcatJob4.ToString());
                   
                    foreach (string videoPath in x.Videos)
                    {
                        StringBuilder sbConcatJob2 = new StringBuilder();
                        sbConcatJob2.Append("www.youtube.com/watch?v=").Append(videoPath).Append(strComma);
                        swOutputFile.WriteLine(sbConcatJob2.ToString());
                    }

                    foreach (string imagePath in x.Images)
                    {
                        StringBuilder sbConcatJob3 = new StringBuilder();
                        sbConcatJob3.Append(imagePath).Append(strComma);
                        swOutputFile.WriteLine(sbConcatJob3.ToString());
                    }

                    StringBuilder sbConcatJob6 = new StringBuilder();
                    sbConcatJob6.Append(Environment.NewLine);
                    swOutputFile.WriteLine(sbConcatJob6.ToString());
                }
                swOutputFile.WriteLine(sbConcatJob.ToString());
                swOutputFile.Close();
                HttpContext.Response.End();
            }  catch (Exception e) {
                Response.Redirect("/reports/CURTReports");
            }
        }

        public void CategoryReport()
        {
            CategoryReport cr = new CategoryReport();
            XElement x = cr.GetParentCategories();
                        
            XDocument xdoc = cr.generate(x);
            StringWriter wr = new StringWriter();
            xdoc.Save(wr);

            string attachment = "attachment; filename=CURT_Reports_CategoryReport" + String.Format("{0:yyyyMMddhhmm}", DateTime.Now) + ".xml";
            HttpContext.Response.Clear();
            HttpContext.Response.ClearHeaders();
            HttpContext.Response.ClearContent();
            HttpContext.Response.AddHeader("content-disposition", attachment);
            HttpContext.Response.ContentType = "text/xml";
            HttpContext.Response.ContentEncoding = System.Text.Encoding.Unicode;
            HttpContext.Response.AddHeader("Pragma", "public");
            HttpContext.Response.Write(wr.GetStringBuilder().ToString());
            //HttpContext.Response.SetCookie(new HttpCookie("fileDownload", "true") { Path = "/" });
            HttpContext.Response.End();
        }

        public static string HitchType(Part part)
        {
            CurtDevDataContext db = new CurtDevDataContext();
            int hitchTypeCat = (from p in db.Parts
                                     join cp in db.CatParts on p.partID equals cp.partID
                                     join c in db.Categories on cp.catID equals c.catID
                                     where p.partID.Equals(part.partID)
                                     select c.catID).FirstOrDefault();

            List<int> MountTypeList = new List<int>();
            MountTypeList = (from p in db.Categories
                             where p.catTitle.Contains("Hitches")
                             select p.catID).ToList<int>();

            string hitchType;

                if (MountTypeList.Contains(hitchTypeCat))
                {
                    switch(hitchTypeCat)
                    {
                        case 1:
                        hitchTypeCat = 11;
                        hitchType = "Front Mount";
                            break;
                  
                        default:
                        hitchType = "Rear Mount";
                            break;
                    }
                } else {
                    hitchType = "";
                }

            return hitchType;
          }





        //public static CatPart ParentCategory(Part part)
        //{
        //    CurtDevDataContext db = new CurtDevDataContext();
        //    List<CatPart> catlist = new List<CatPart>();
        //
        //    CatPart PartCat = new CatPart();
        //    PartCat = (from cp in db.CatParts 
        //               where cp.partID.Equals(part.partID) 
        //               select cp.catID);  
        //    catlist.Add(PartCat);
        //
        //    catlist.Add(db.Categories.Where(x => x.catID == catID).FirstOrDefault<CatPart>());
        //    bool more = (catlist[0].parentID == 0) ? false : true;
        //            while (more) {
        //                catlist.Add(db.Categories.Where(x => x.catID == catlist[catlist.Count() - 1].parentID).FirstOrDefault<CatPart>());
        //                if (catlist[catlist.Count() - 1].parentID == 0) {
        //                    more = false;
        //                }
        //    return catlist;
        //}

       
    }
}

