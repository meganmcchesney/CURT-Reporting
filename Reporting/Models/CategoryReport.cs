using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.Xml;

namespace Reporting.Models
{
    public class CategoryReport
    {
        CurtDevDataContext db = new CurtDevDataContext();

        public XElement GetParentCategories()
        {
            XElement root = new XElement("CategoyHierarchy");
            List<Category> catlst = db.Categories.Where(x => x.parentID == 0).Where(x => x.isLifestyle == 0).Select(x => x).ToList();

                foreach (Category catl in catlst)
                {
                    XElement rootCat = new XElement("Name", catl.catTitle,
                                       new XAttribute("CategoryID", catl.catID),
                                       new XAttribute("ParentID", catl.parentID));

                    root.Add(rootCat);
                    GetChildCategories(catl, rootCat);
                }
        
            return root;
        }

        public void GetChildCategories(Category catl, XElement rootCat)
        {
            if (catl != null)
            {
                List<Category> subCatList = db.Categories.Where(x => x.parentID == catl.catID).ToList();

                    foreach (Category ct in subCatList)
                    {
                        XElement subCat = new XElement("Name", ct.catTitle,
                                          new XAttribute("CategoryID", ct.catID),
                                          new XAttribute("ParentID", ct.parentID));
                        XElement catParts = new XElement("PartNumber", (from cz in db.Categories
                                                                        join z in db.CatParts on cz.catID equals z.catID
                                                                        join p in db.Parts on z.partID equals p.partID
                                                                        where cz.catID == ct.catID && p.status != 999
                                                                        select new XElement("PartID", p.partID)).ToList());
                        subCat.Add(catParts);
                        rootCat.Add(subCat);
                        GetChildCategories(ct, subCat);
                    }
            }
        }
       
       public XDocument generate(XElement root) 
       {
           XDocument xdoc = new XDocument();
       
           XElement header = new XElement("Header",
           new XElement("BlanketEffectiveDate", String.Format("{0:yyyy-MM-dd}", DateTime.Now)),
           new XElement("Brand", "CURT Manufacturing"),
           new XElement("CurrencyCode", "USD"),
           new XElement("LanguageCode", "EN"),
           new XElement("TechnicalContact", "Megan McChesney"),
           new XElement("ContactEmail", "websupport@curtmfg.com"));
       
           XElement root1 = new XElement("CategoryHierarchy");
       
           root1.Add(header);

           root1.Add(root);
      
           XElement trailer = new XElement("Trailer",
                              new XElement("TransactionDate", String.Format("{0:yyyy-MM-dd}", DateTime.Now)));
           
           root1.Add(trailer);
           xdoc.Add(root1);          
           return xdoc;
       }
    }
}