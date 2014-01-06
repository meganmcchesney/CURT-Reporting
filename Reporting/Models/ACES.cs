using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.IO;
using System.Xml;
using System.ComponentModel;
using System.Text;

namespace Reporting.Models
{
    public class ACES
    {
        public XDocument GenerateReport()  //DateTime? start = null
        {
            CurtDevDataContext db = new CurtDevDataContext();
            AAIA.pcdbDataContext pcdb = new AAIA.pcdbDataContext();
            AAIA.qdbDataContext qdb = new AAIA.qdbDataContext();
            AAIA.VCDBDataContext vcdb = new AAIA.VCDBDataContext();

            XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
            XNamespace xsd = "http://www.w3.org/2001/XMLSchema";


            XDocument report = new XDocument(new XDeclaration("1.0", "utf-16", "yes"));

            XElement xdoc = new XElement("ACES",
                new XAttribute(XNamespace.Xmlns + "xsi", xsi),
                new XAttribute(XNamespace.Xmlns + "xsd", xsd),
                new XAttribute("version", "1.0"),
                new XElement("Header",
                    new XElement("Company", "CURT Manufacturing"),
                    new XElement("SenderName", "Nichole Scott"),
                    new XElement("SenderPhone", "877-287-8634"),
                    new XElement("TransferDate", String.Format("{0:yyyy-MM-dd}", DateTime.Now)),
                    new XElement("MfrCode", "BKDK"),
                    new XElement("DocumentTitle", "Trailer Hitches"),
                    new XElement("EffectiveDate", String.Format("{0:yyyy-MM-dd}", DateTime.Now)),
                    new XElement("SubmissionType", "FULL")),
                    new XElement("VcdbVersionDate", String.Format("{0:yyyy-MM-dd}", vcdb.VCDBVersions.Select(x => x.VersionDate).FirstOrDefault())),
                    new XElement("QdbVersionDate", String.Format("{0:yyyy-MM-dd}", qdb.QDBVersions.Select(x => x.VersionDate).FirstOrDefault())),
                    new XElement("PcdbVersionDate", String.Format("{0:yyyy-MM-dd}", pcdb.PCDBVersions.Select(x => x.VersionDate).FirstOrDefault())),


                 (from vp in db.vcdb_VehicleParts
                  where vp.PartNumber < 15000 && vp.PartNumber >= 13000
                  select new XElement("App",
                    new XAttribute("action", "A"),
                    new XAttribute("id", vp.ID),
                    new XElement("BaseVehicle",
                         new XAttribute("id", vp.vcdb_Vehicle.BaseVehicle.AAIABaseVehicleID ?? 0)),


                    ((vp.vcdb_Vehicle.SubModelID != null) ? new XElement("SubModel", new XAttribute("id", vp.vcdb_Vehicle.Submodel.AAIASubmodelID)) : null),
                      ((vp.vcdb_Vehicle.ConfigID != null) ? (from ca in vp.vcdb_Vehicle.VehicleConfig.VehicleConfigAttributes
                                                             where ca.ConfigAttribute.vcdbID != null
                                                              select new XElement(
                                                                    ca.ConfigAttribute.ConfigAttributeType.AcesType.name, 
                                                                    new XAttribute("id", ca.ConfigAttribute.vcdbID.ToString()))
                                                                 ).ToList<XElement>() : null),
                    ((vp.vcdb_Vehicle.ConfigID != null) ? (from ca in vp.vcdb_Vehicle.VehicleConfig.VehicleConfigAttributes
                                                           where ca.ConfigAttribute.vcdbID == null
                                                           select new XElement("Note", ca.ConfigAttribute.value)).ToList<XElement>() : null),
                    (from n in vp.Notes select new XElement("Note", XmlConvert.EncodeName(n.note1))).ToList<XElement>(),
                    new XElement("Qty", 1),
                    ((vp.Part.ACESPartTypeID != null) ? new XElement("PartType", new XAttribute("id", vp.Part.ACESPartTypeID)) : null),
                    new XElement("MfrLabel", XmlConvert.EncodeName(vp.Part.shortDesc)),
                    new XElement("Position", (vp.Part.classID == 5 || vp.Part.classID == 11)? "22" : (vp.Part.classID > 0)? "30":"0"),
                    new XElement("Part", vp.PartNumber))),

                new XElement("Footer",
                new XElement("RecordCount", db.vcdb_VehicleParts.Count())));


            report.Add(xdoc);

            
            return report;
        }
    }
}