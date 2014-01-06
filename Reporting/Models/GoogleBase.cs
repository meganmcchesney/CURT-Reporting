using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace Reporting.Models {
    public class GoogleBase {
        readonly XmlQualifiedName _googleNs = new XmlQualifiedName("g", "http://www.w3.org/2000/xmlns/");

        public void Generate(HttpResponseBase resp) {
            // create and instantiate the writer object.
            var xw = new XmlTextWriter(resp.OutputStream, null) { Formatting = Formatting.Indented };

            // write the start of the document.
            xw.WriteStartDocument();

            xw.WriteStartElement("rss");
            xw.WriteAttributeString("version","2.0");
            xw.WriteAttributeString("xmlns", "g", null, "http://base.google.com/ns/1.0");

            xw.WriteStartElement("channel");

            xw.WriteElementString("title", FeedTitle);
            xw.WriteElementString("link", FeedLink.ToString());
            xw.WriteElementString("description", FeedDescription);

            this.WriteItems(xw);

            xw.WriteEndElement();
            xw.WriteEndElement();
            // write the end of the document.
            xw.WriteEndDocument();


            // close the writer.
            xw.Close();
        }

        private void WriteItems(XmlTextWriter xw) {
            var db = new CurtDevDataContext();
            var items = new List<SyndicationItem>();

            var parts = db.Parts.Where(x => x.status < 990 && x.status >= 800).ToList();


            foreach (var part in parts){
                var price = (from p in db.CustomerPricings
                             where p.partID.Equals(part.partID) && p.cust_id.Equals(this.CustId)
                             select p.price).FirstOrDefault<decimal>();

                
            if (price > 0){

                    string content = GoogleBase.GetDescription(part.partID);
                    var image = (db.PartImages.Where(
                        i => i.partID.Equals(part.partID) &&
                             i.height.Equals(300) &&
                             i.sort.Equals("a")
                        ).Select(i => i.path)).FirstOrDefault<string>();
                    if (string.IsNullOrEmpty(image)) {
                        image = "http://curtmfg.com/Content/img/noimage.png";
                    }
                    xw.WriteStartElement("item");

                    xw.WriteElementString("title", "CURT " + part.shortDesc + " #" + part.partID.ToString(CultureInfo.InvariantCulture));
                    xw.WriteElementString("link", this.GeneratePartLink(part.partID));
                    xw.WriteElementString("description", content);
                    xw.WriteElementString("g:image_link", image);
                    xw.WriteElementString("g:price", value: price.ToString(CultureInfo.InvariantCulture));
                    xw.WriteElementString("g:condition", "new");
                    xw.WriteElementString("g:availability", "available for order");
                    xw.WriteElementString("g:id", value: part.partID.ToString(CultureInfo.InvariantCulture));

                    xw.WriteStartElement("g:shipping");
                    xw.WriteElementString("g:country", "US");
                    xw.WriteElementString("g:service", "Ground");
                    xw.WriteElementString("g:price", "0.00 USD");
                    xw.WriteEndElement();

                    if (this.Taxes.Count == 0){
                        xw.WriteStartElement("g:tax");
                        xw.WriteElementString("g:country", "US");
                        xw.WriteElementString("g:rate", "0.00");
                        xw.WriteEndElement();
                    }
                    else{
                        foreach (var tax in this.Taxes){
                            xw.WriteStartElement("g:tax");
                            xw.WriteElementString("g:country", tax.Country);
                            xw.WriteElementString("g:region", tax.State);
                            xw.WriteElementString("g:rate", tax.Percent.ToString(CultureInfo.InvariantCulture));
                            xw.WriteEndElement();
                        }
                    }


                    xw.WriteElementString("g:gtin",
                                          "00" + part.PartAttributes.Where(x => x.field.ToUpper().Equals("UPC")).Select(
                                              x => x.value).FirstOrDefault<string>());
                    xw.WriteElementString("g:brand", "CURT");
                    xw.WriteElementString("g:mpn", part.partID.ToString(CultureInfo.InvariantCulture));
                    xw.WriteElementString("g:product_type", "Vehicles &amp; Parts &gt; Vehicle Parts &amp; Accessories");
                    xw.WriteElementString("g:google_product_category", "Vehicles &amp; Parts &gt; Vehicle Parts &amp; Accessories");
                    xw.WriteEndElement();
                }
            }
        }

        public static string GetDescription(int id){
            var db = new CurtDevDataContext();
            var content = (from c in db.Contents
                           join cb in db.ContentBridges on c.contentID equals cb.contentID
                           where cb.partID.Equals(id) && c.cTypeID.Equals(3)
                           select c.text).FirstOrDefault<string>();
            if (string.IsNullOrEmpty(content)){
                var bullets = (from c in db.Contents
                               join cb in db.ContentBridges on c.contentID equals cb.contentID
                               where cb.partID.Equals(id) && c.cTypeID.Equals(2)
                               select c.text).ToList<string>();
                content = bullets.Aggregate("", (current, bull) => current + (bull + " "));
            }
            return content;
        }

        public string GeneratePartLink(int id){
            if (string.IsNullOrEmpty(PartLink)){
                return "";
            }
            return this.PartLink.Contains("[partID]") ? PartLink.Replace("[partID]", id.ToString(CultureInfo.InvariantCulture)) : this.PartLink;
        }

        public string FeedDescription { get; set; }
        public string FeedTitle { get; set; }
        public Uri FeedLink { get; set; }
        public string PartLink { get; set; }
        public int CustId { get; set; }
        public List<GoogleTax> Taxes { get; set; } 

    }

    public class GoogleTax{
        public string Country { get; set; }
        public string State { get; set; }
        public decimal Percent { get; set; }
    }
}