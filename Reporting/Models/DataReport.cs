using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Linq;

namespace Reporting.Models
{
    public class DataReport
    {
        public XDocument generate(DateTime? start = null)
        {
            CurtDevDataContext db = new CurtDevDataContext();
            Dictionary<string, string> pricecodes = new Dictionary<string, string>();
            pricecodes.Add("Jobber", "Jobber");
            pricecodes.Add("List", "ListRetail");
            pricecodes.Add("Map", "MAP");
            pricecodes.Add("eMap", "eMap");


            XDocument xdoc = new XDocument(new XDeclaration("1.0", "utf-16", "yes"));
            XElement root = new XElement("CURTProductInformation");

            XElement header = new XElement("Header",
                new XElement("BlanketEffectiveDate", String.Format("{0:yyyy-MM-dd}", DateTime.Now)),
                new XElement("Brand", "CURT Manufacturing"),
                new XElement("CurrencyCode", "USD"),
                new XElement("LanguageCode", "EN"),
                new XElement("TechnicalContact", "Megan McChesney"),
                new XElement("ContactEmail", "websupport@curtmfg.com"));

            XElement items = new XElement("Items");

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
                XElement item = new XElement("Item",
                    new XElement("PartNumber", part.partID),
                    new XElement("DateModified", part.dateModified),
                    new XElement("BrandLabel", "CURT"),

                    new XElement("Description", part.shortDesc), 
                    new XElement("PartDescription", (from cb in part.ContentBridges
                                                     where cb.Content.cTypeID.Equals(11)
                                                     select cb.Content.text).FirstOrDefault() ?? ""),
                    new XElement("UPC", (from pa in part.PartAttributes
                                         where pa.field.Equals("UPC")
                                         select pa.value).FirstOrDefault() ?? ""),

                    new XElement("Class",  part.Classes
                        .Where(x => x.classID.Equals(part.classID))
                        .Select(x => x.class1).FirstOrDefault<string>()),

                    new XElement("HitchType", Reporting.Controllers.ReportsController.HitchType(part)),

                    new XElement("MinimumOrderQuantity", 1,
                        new XAttribute("UOM", "EA")),

                    new XElement("Attributes", (from pa in part.PartAttributes 
                                                where pa.field != "UPC" && pa.field != "Weight"
                                                select new XElement(pa.field.Replace(" ","").Replace("(","").Replace(")","").Replace("/","").ToString(), pa.value))),
                    new XElement("WebContent", (from pc in part.ContentBridges
                                                join cc in db.Contents on pc.contentID equals cc.contentID
                                                join ct in db.ContentTypes on cc.cTypeID equals ct.cTypeID
                                                where cc.cTypeID != '5'
                                                orderby ct.type
                                                select new XElement("Content", cc.text,
                                                       new XAttribute("ContentType", ct.type)))),
                                                                                          
                     new XElement("Categories", (from p in part.CatParts 
                                                join c in db.Categories on p.catID equals c.catID
                                                select new XElement("CategoryName", c.catTitle.ToString().Trim(),
                                                       new XAttribute("CategoryID", c.catID),
                                                       new XAttribute("ParentCategoryID", c.parentID)))),
                                                   //new XElement("CategoryContent", (from pc in part.ContentBridges
                                                   //                                 join cc in db.Contents on pc.contentID equals cc.contentID
                                                   //                                 join ct in db.ContentTypes on cc.cTypeID equals ct.cTypeID
                                                   //                                 where cc.cTypeID.Equals(7)
                                                   //                                 select new XElement("Content", cc.text,
                                                   //                                        new XAttribute("ContentType", ct.type))))),

                    //new XElement("ParentCategoryName", Reporting.Controllers.ReportsController.ParentCategory(part)),
                    //new XAttribute("ParentCategoryID", c.parentID))))),

                    new XElement("RelatedParts", (from p in part.RelatedParts
                                                  select new XElement("PartNumber", p.relatedID))),

                    new XElement("VehicleIDs", (from p in part.VehicleParts 
                                                select new XElement("VehicleID", p.vehicleID,
                                                       new XAttribute("Drilling", p.drilling),
                                                       new XAttribute("InstallTime", p.installTime),
                                                       new XAttribute("Visibility", p.exposed)))),
                    new XElement("Cost", "Please reference your CURT price sheet"),
                    new XElement("PriceCode", part.priceCode),
 
                    new XElement("Prices", (from p in part.Prices
                                            select new XElement("Pricing",
                                                   new XAttribute("PriceType", pricecodes[p.priceType]),
                                                   new XAttribute("EffectiveDate", String.Format("{0:yyyy-MM-dd}", DateTime.Now)),
                                                   new XElement("Price", "$" + p.price1.ToString("0.00"))
                                            )).ToList<XElement>()),
                    new XElement("Packages", (from pp in part.PartPackages
                                                select new XElement("Package",
                                                    new XElement("PackageUOM", pp.PackageUnits.code),
                                                    new XElement("QuantityofEaches", pp.quantity),
                                                    new XElement("Dimensions",
                                                    new XAttribute("UOM", pp.DimensionUnits.code),
                                                    new XElement("Height", pp.height),
                                                    new XElement("Width", pp.width),
                                                    new XElement("Length", pp.length)),
                                                    new XElement("Weights",
                                                    new XAttribute("UOM", pp.weightUnits.code),
                                                    new XElement("Weight", pp.weight)),
                                                    new XElement("FreightClass", "50"),
                                                    new XElement("LTL", (pp.weight >= 150 ? "Yes" : "No"))))),
                    new XElement("DigitalAssets", (from x in part.PartVideos
                                                     select new XElement("DigitalFileInformation",
                                                            new XElement("AssetType", "Video Path"),
                                                            new XElement("FilePath", "www.youtube.com/watch?v=" + x.video)
                                                     )).ToList<XElement>(),
                                                    (from pi in part.PartImages
                                                    select new XElement("DigitalFileInformation",
                                                           new XElement("AssetType", "Image Path"),
                                                           new XElement("FilePath", pi.path),
                                                           new XElement("FileName", part.partID + "_" + pi.height + "x" + pi.width + "_" + pi.sort + ".jpg"),
                                                           new XElement("FileType", "JPG"),
                                                           new XElement("AssetDimensions",
                                                           new XAttribute("UOM", "PX"),
                                                           new XElement("AssetHeight", pi.height),
                                                           new XElement("AssetWidth", pi.width))))
                                                    .ToList<XElement>()));
                items.Add(item);
            }
            XElement trailer = new XElement("Trailer",
                               new XElement("ItemCount", parts.Count),
                               new XElement("TransactionDate", String.Format("{0:yyyy-MM-dd}", DateTime.Now)));

            root.Add(header);
            root.Add(items);
            root.Add(trailer);
            xdoc.Add(root);

            return xdoc;
        }

        public class CSVRow
        {
            public string data { get; set; }
        }

        public string generate_csv(DateTime? start = null)
        {
            var db = new CurtDevDataContext();

            string delim = ",";

            var sb = new StringBuilder();

            var rows = new List<CSVRow>();
            if (start == null)
            {
                rows = (from p in db.Parts
                        join vp in db.VehicleParts on p.partID equals vp.partID
                        join v in db.Vehicles on vp.vehicleID equals v.vehicleID
                        join y in db.Years on v.yearID equals y.yearID
                        join ma in db.Makes on v.makeID equals ma.makeID
                        join mo in db.Models on v.modelID equals mo.modelID
                        join s in db.Styles on v.styleID equals s.styleID
                        where p.status != 999




                        select new CSVRow
                        {
                            data = y.year1.ToString() + delim + ma.make1.Replace(",", "") + delim + mo.model1.Replace(",", "") + delim + s.style1.Replace(",", "") + delim + p.partID + delim + p.shortDesc.Replace(",", "")
                        }).ToList<CSVRow>();
            }
            else
            {
                rows = (from p in db.Parts
                        join vp in db.VehicleParts on p.partID equals vp.partID
                        join v in db.Vehicles on vp.vehicleID equals v.vehicleID
                        join y in db.Years on v.yearID equals y.yearID
                        join ma in db.Makes on v.makeID equals ma.makeID
                        join mo in db.Models on v.modelID equals mo.modelID
                        join s in db.Styles on v.styleID equals s.styleID
                        where p.status != 999 && p.dateModified >= start
                        select new CSVRow
                        {
                            data = y.year1.ToString() + delim + ma.make1.Replace(",", "") + delim + mo.model1.Replace(",", "") + delim + s.style1.Replace(",", "") + delim + p.partID + delim + p.shortDesc.Replace(",", "")
                        }).ToList<CSVRow>();
            }

            sb.AppendLine("Year,Make,Model,Style,Part #, Short Description");
            foreach (var row in rows)
            {
                sb.AppendLine(row.data);
            }
            return sb.ToString();
        }
    }
}