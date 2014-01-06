using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Linq;
using System.Data.Entity;

namespace Reporting.Models
{
    public class VehicleReport
    {

        public class CSVRow {
            //public string data { get; set; }
            public string vehicleID { get; set; }
            public double year { get; set; }
            public string make { get; set; }
            public string model { get; set; }
            public string style { get; set; }
            public string PartID { get; set; }
            public string shortDesc { get; set; }
            public DateTime dateModified { get; set; }
            public string note { get; set; }
        }

        public string generate_csv(DateTime? start = null) {
            var db = new CurtDevDataContext();

            string delim = ",";

            var sb = new StringBuilder();

            var rows = new List<CSVRow>();
            if (start == null) {
                rows = (from p in db.Parts
                        join vp in db.VehicleParts on p.partID equals vp.partID
                        join v in db.Vehicles on vp.vehicleID equals v.vehicleID
                        join y in db.Years on v.yearID equals y.yearID
                        join ma in db.Makes on v.makeID equals ma.makeID
                        join mo in db.Models on v.modelID equals mo.modelID
                        join s in db.Styles on v.styleID equals s.styleID
                        where p.status != 999
                        orderby p.partID, ma.make1, mo.model1, s.style1, y.year1
                        select new CSVRow {
                            vehicleID = v.vehicleID.ToString(),
                            year = y.year1,
                            make = ma.make1,
                            model = mo.model1, 
                            style = s.style1,
                            PartID = p.partID.ToString(),
                            shortDesc = p.shortDesc,
                            dateModified = p.dateModified,

                        }).ToList<CSVRow>();
            } else {
                rows = (from p in db.Parts
                        join vp in db.VehicleParts on p.partID equals vp.partID
                        join v in db.Vehicles on vp.vehicleID equals v.vehicleID
                        join y in db.Years on v.yearID equals y.yearID
                        join ma in db.Makes on v.makeID equals ma.makeID
                        join mo in db.Models on v.modelID equals mo.modelID
                        join s in db.Styles on v.styleID equals s.styleID
                        where p.status != 999 && p.dateModified >= start
                        orderby p.partID, ma.make1, mo.model1, s.style1, y.year1
                        select new CSVRow {
                            vehicleID = v.vehicleID.ToString(),
                            year = y.year1,
                            make = ma.make1,
                            model = mo.model1,
                            style = s.style1,
                            PartID = p.partID.ToString(),
                            shortDesc = p.shortDesc,
                            dateModified = p.dateModified,
                        }).ToList<CSVRow>();
            }

            sb.AppendLine("VehicleID,Year,Make,Model,Style,Part Number,Short Description,Date Modified");
            foreach (var row in rows) {
                
                sb.Append(row.vehicleID).Append(delim).Append(row.year).Append(delim).Append(row.make).Append(delim).Append(row.model).Append(delim).Append(row.style.Replace(",", " ")).Append(delim).Append(row.PartID).Append(delim).Append(row.shortDesc).Append(delim).Append(row.dateModified).Append(Environment.NewLine);
            }
            return sb.ToString();
        }

        public XDocument generate(DateTime? start = null)
        {
            CurtDevDataContext db = new CurtDevDataContext();
            XDocument xml = new XDocument();
            XElement root = new XElement("CURTData");
            XElement header = new XElement("Header",
                new XElement("BlanketEffectiveDate", String.Format("{0:yyyy-MM-dd}", DateTime.Now)),
                new XElement("TechnicalContact", "Nichole Scott"),
                new XElement("ContactEmail", "websupport@curtmfg.com"));

            XElement vehicles = new XElement("ApplicationInformation");
            
           List<Part> parts = new List<Part>();
           if (start == null)
           {
               parts = db.Parts.Where(x => x.status != 999).OrderBy(x => x.partID).ToList<Part>();
           }
           else
           {
              parts = db.Parts.Where(x => x.dateModified >= start).Where(x => x.status != 999).OrderBy(x => x.partID).ToList<Part>();
           }
           
           foreach (Part part in parts)
            {
                
                XElement vehicle = new XElement("Applications",
                         (from p in db.VehicleParts
                          join v in db.Vehicles on p.vehicleID equals v.vehicleID
                          join y in db.Years on v.yearID equals y.yearID
                          join ma in db.Makes on v.makeID equals ma.makeID
                          join mo in db.Models on v.modelID equals mo.modelID
                          join s in db.Styles on v.styleID equals s.styleID
                          where p.partID.Equals(part.partID)
                          select new XElement("Vehicle",
                                  new XElement("VehicleID", v.vehicleID),
                                  new XElement("Year", y.year1),
                                  new XElement("Make", ma.make1),
                                  new XElement("Model", mo.model1),
                                  new XElement("Style", s.style1),
                                  new XElement("PartNumber", p.partID,
                                  new XAttribute("Description", part.shortDesc)),
                                  new XElement("Notes", (from cb in db.ContentBridges
                                                         join c in db.Contents on cb.contentID equals c.contentID
                                                         where cb.partID.Equals(p.partID) && c.cTypeID.Equals(1)
                                                         select new XElement("Note", c.text))))).ToList<XElement>());
              vehicles.Add(vehicle);
            }
           

            XElement trailer = new XElement("Trailer",
               new XElement("ItemCount", parts.Count),
               new XElement("TransactionDate", String.Format("{0:yyyy-MM-dd}", DateTime.Now)));

            root.Add(header);
            root.Add(vehicles);
            root.Add(trailer);
            xml.Add(root);
            return xml;
        }
    }
}
