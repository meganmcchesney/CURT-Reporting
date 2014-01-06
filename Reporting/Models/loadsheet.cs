using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Reporting.Models {
    public class Loadsheet {
    
        public static string GenerateCellValue(int field, Part p, CurtDevDataContext db){
            string temp = "";
            // lookup field by ID
            LoadsheetField fieldObj = db.LoadsheetFields.Where(x => x.id == field).FirstOrDefault<LoadsheetField>();
            if (fieldObj.defaultValue != null) {
                temp = fieldObj.defaultValue;
            } else {
                #region PossibleFields
                switch (field) {
                    case 7: // CURT partID
                        temp = p.partID.ToString();
                        break;
                    case 9: // CURT shortDesc
                        temp = p.shortDesc;
                        break;
                    case 10: // CURT UPC
                                {
                            PartAttribute pa = p.PartAttributes.Where(x => x.field == "UPC" && x.partID == p.partID).FirstOrDefault<PartAttribute>();
                            temp = (pa != null) ? pa.value : "";
                            break;
                        }
                    case 11: // CURT GTIN
                                {
                                    PartAttribute pa = p.PartAttributes.Where(x => x.field == "UPC" && x.partID == p.partID).FirstOrDefault<PartAttribute>();
                                    temp = (pa != null) ? ("00" + pa.value) : "";
                                    break;
                                }
                    case 12: // CURT Package Weight
                                {
                                    PartPackage package = p.PartPackages.Where(x => x.partID == p.partID).FirstOrDefault<PartPackage>();
                                    temp = (package != null) ? package.weight.ToString() : "";
                                    break;
                                }
                    case 13: // CURT Package Length
                                {
                                    PartPackage package = p.PartPackages.Where(x => x.partID == p.partID).FirstOrDefault<PartPackage>();
                                    temp = (package != null) ? package.length.ToString() : "";
                                    break;
                                }
                    case 14: // CURT Package Width
                                {
                                    PartPackage package = p.PartPackages.Where(x => x.partID == p.partID).FirstOrDefault<PartPackage>();
                                    temp = (package != null) ? package.width.ToString() : "";
                                    break;
                                }
                    case 15: // CURT Package Height
                                {
                                    PartPackage package = p.PartPackages.Where(x => x.partID == p.partID).FirstOrDefault<PartPackage>();
                                    temp = (package != null) ? package.height.ToString() : "";
                                    break;
                                }
                    case 19: // CURT Long Description
                                {
                                    string listDescription = "";
                                    Content c = p.ContentBridges.Where(x => x.Content.cTypeID == 11 && x.partID == p.partID).Select(x => x.Content).FirstOrDefault<Content>();
                                    listDescription = (c != null) ? (" " + c.text) : "";
                                    temp = "CURT " + p.shortDesc + listDescription;
                                    break;
                                }
                    case 24: // CURT Jobber Price
                                {
                                    Price jobber = p.Prices.Where(x => x.priceType == "Jobber" && x.partID == p.partID).FirstOrDefault<Price>();
                                    temp = (jobber != null) ? jobber.price1.ToString() : "0.00";
                                    break;
                                }
                    case 25: // CURT List (Retail) Price
                                {
                                    Price listPrice = p.Prices.Where(x => x.priceType == "List" && x.partID == p.partID).FirstOrDefault<Price>();
                                    temp = (listPrice != null) ? listPrice.price1.ToString() : "0.00";
                                    break;
                                }
                    case 26: // CURT MAP (Min Advertised) Price
                                {
                                    Price mapPrice = p.Prices.Where(x => x.priceType == "Map" && x.partID == p.partID).FirstOrDefault<Price>();
                                    temp = (mapPrice != null) ? mapPrice.price1.ToString() : "0.00";
                                    break;
                                }
                    case 27: // CURT eMAP (Internet Min Advertised) Price
                                {
                                    Price emapPrice = p.Prices.Where(x => x.priceType == "eMap" && x.partID == p.partID).FirstOrDefault<Price>();
                                    temp = (emapPrice != null) ? emapPrice.price1.ToString() : "0.00";
                                    break;
                                }
                    case 29: // AAIA Brand Code such as BKDK
                                {
                                    temp = "BKDK";
                                    break;
                                }
                    case 37: // Class of the Hitch: Example: 2 - for Class 2 hitch.
                                {
                                    int classID = p.classID;
                                    switch (classID) {
                                        case 1: {
                                                temp = "1";
                                                break;
                                            }
                                        case 2: {
                                                temp = "2";
                                                break;
                                            }
                                        case 3: {
                                                temp = "3";
                                                break;
                                            }
                                        case 4: {
                                                temp = "4";
                                                break;
                                            }
                                        case 5: {
                                                temp = "Front";
                                                break;
                                            }
                                        case 7: {
                                                temp = "5";
                                                break;
                                            }
                                        case 8: {
                                                temp = "5 Xtra";
                                                break;
                                            }
                                        default: {
                                                temp = "N/A";
                                                break;
                                            }
                                    }
                                    break;
                                }
                    case 41: // CURT Receiver Tube Size
                                {
                                    PartAttribute pa = p.PartAttributes.Where(x => x.field == "Receiver Tube Size" && x.partID == p.partID).FirstOrDefault<PartAttribute>();
                                    temp = (pa != null) ? pa.value : "";
                                    break;
                                }
                    case 45: // CURT Mount Style - Front or Rear Mount
                                {
                                    temp = "";
                                    if (p.classID <= 10) {
                                        temp = (p.classID == 5) ? "Front Mount" : "Rear Mount";
                                    }
                                    break;
                                }
                    case 53: // CURT GTW - Gross Trailer Weight
                                {
                                    temp = "Varies by application";
                                    break;
                                }
                    case 54: // CURT TW - Tongue Weight
                                {
                                    PartAttribute pa = p.PartAttributes.Where(x => x.field == "Tongue Weight(TW)" && x.partID == p.partID).FirstOrDefault<PartAttribute>();
                                    temp = (pa != null) ? pa.value : "";
                                    break;
                                }
                    case 55: // CURT Drilling Required
                                {
                                    temp = "Yes/No";
                                    List<VehiclePart> vps = p.VehicleParts.Where(x => x.partID == p.partID && x.drilling != "").ToList<VehiclePart>();

                                    int drillYes = 0;
                                    int drillNo = 0;
                                    foreach (VehiclePart vp in vps) {
                                        if (vp.drilling == "No drilling required") {
                                            drillNo += 1;
                                        } else if (vp.drilling == "Drilling required") {
                                            drillYes += 1;
                                        } else {

                                        }
                                    }
                                    if (drillYes == vps.Count) {
                                        temp = "Yes";
                                    }
                                    if (drillNo == vps.Count) {
                                        temp = "No";
                                    }

                                    break;
                                }
                    case 60: // CURT Tube Shape - round or square
                                {
                                    temp = "Square";
                                    Content c = p.ContentBridges.Where(x => x.Content.cTypeID == 2 && x.partID == p.partID && x.Content.text.Contains("Round tube")).Select(x => x.Content).FirstOrDefault<Content>();
                                    if (c != null) {
                                        temp = "Round";
                                    }
                                    break;
                                }

                    case 61: // Weight Carrying Capacity - AKA MAX GTW.
                                {
                                    PartAttribute pa = p.PartAttributes.Where(x => x.field == "Weight Carrying Capacity(WC)" && x.partID == p.partID).FirstOrDefault<PartAttribute>();
                                    temp = (pa != null) ? pa.value : "";
                                    break;
                                }

                    case 62: // JEGS Part Number 
                                {
                                    temp = "229-" + p.partID;
                                    break;
                                }
                    case 63: // Makes
                                {
                                    var makes = (from v in db.Vehicles
                                                 join vp in db.VehicleParts on v.vehicleID equals vp.vehicleID
                                                 join s in db.Styles on v.styleID equals s.styleID
                                                 join mo in db.Models on v.modelID equals mo.modelID
                                                 join ma in db.Makes on v.makeID equals ma.makeID
                                                 join y in db.Years on v.yearID equals y.yearID
                                                 where vp.partID.Equals(p.partID)
                                                 select ma.make1).Distinct().ToList<string>();
                                    temp = string.Join("/", makes);
                                    break;
                                }
                    case 64: // JEGS Description
                                #region jegsDesc
 {
                                    List<YearRangeResult> listItems = new List<YearRangeResult>();
                                    var allVehicles = (from v in db.Vehicles
                                                       join vp in db.VehicleParts on v.vehicleID equals vp.vehicleID
                                                       join s in db.Styles on v.styleID equals s.styleID
                                                       join mo in db.Models on v.modelID equals mo.modelID
                                                       join ma in db.Makes on v.makeID equals ma.makeID
                                                       join y in db.Years on v.yearID equals y.yearID
                                                       where vp.partID.Equals(p.partID)
                                                       select new FullVehicle {
                                                           year = y.year1,
                                                           model = mo.model1,
                                                           make = ma.make1,
                                                           style = s.style1
                                                       }
                                                 ).Distinct().ToList<FullVehicle>();
                                    foreach (FullVehicle vehicle in allVehicles) {
                                        YearRangeResult yrr = listItems.Where(x => x.make == vehicle.make && x.model == vehicle.model).FirstOrDefault();
                                        if (yrr == null) {
                                            YearRangeResult yrrNew = new YearRangeResult {
                                                make = vehicle.make,
                                                model = vehicle.model,
                                                style = vehicle.style,
                                                years = new List<double>(),
                                                count = 1
                                            };
                                            yrrNew.years.Add(vehicle.year);
                                            listItems.Add(yrrNew);
                                        } else {
                                            yrr.years.Add(vehicle.year);
                                        }
                                    }
                                    //  distinct the years, sort the order, grab the first year and find un interupted order.
                                    string item = "";
                                    if (p.Classes != null) {
                                        Class pClass = p.Classes.FirstOrDefault<Class>();
                                        if (pClass != null) {
                                            item = pClass.class1;
                                        }
                                    }
                                    string tempItem = "";
                                    foreach (YearRangeResult yrr in listItems) {
                                        List<double> years = yrr.years.Distinct().ToList<double>();
                                        // example output: Class 3 Receiver Hitch<li>2014 Forester<li>
                                        tempItem += "<li>" + years.Min().ToString() + "-" + years.Max().ToString() + " " + yrr.model + " " + yrr.style;

                                    }
                                    temp = item + tempItem;
                                    break;
                                }
                                #endregion

                    case 65: // CQNNA Short Description
                                {
     PartAttribute pa = p.PartAttributes.Where(x => x.field == "Receiver Tube Size" && x.partID == p.partID).FirstOrDefault<PartAttribute>();
     //temp = "CURT " + (pa != null?pa.value:"") + p.shortDesc;
     temp = "CURT " + p.shortDesc;
     break;
 }
                    case 66: // CQNNA Long Description
                                {
                                    PartAttribute tubeSize = p.PartAttributes.Where(x => x.field == "Receiver Tube Size" && x.partID == p.partID).FirstOrDefault<PartAttribute>();
                                    Content c = p.ContentBridges.Where(x => x.Content.cTypeID == 11 && x.partID == p.partID).Select(x => x.Content).FirstOrDefault<Content>();
                                    temp = "CURT" + (tubeSize != null ? " " + tubeSize.value : "") + " " + p.shortDesc + (c != null ? " - " + c.text : "");
                                    break;
                                }
                    case 67: // Available Date
                                {
                                    temp = DateTime.Now.ToShortDateString();
                                    break;
                                }
                    case 69: // Year,Make,Model Applications 
                                {
                                    List<YearRangeResult> listItems = new List<YearRangeResult>();
                                    var allVehicles = (from v in db.Vehicles
                                                       join vp in db.VehicleParts on v.vehicleID equals vp.vehicleID
                                                       join s in db.Styles on v.styleID equals s.styleID
                                                       join mo in db.Models on v.modelID equals mo.modelID
                                                       join ma in db.Makes on v.makeID equals ma.makeID
                                                       join y in db.Years on v.yearID equals y.yearID
                                                       where vp.partID.Equals(p.partID)
                                                       select new FullVehicle {
                                                           year = y.year1,
                                                           model = mo.model1,
                                                           make = ma.make1,
                                                           style = s.style1
                                                       }
                                                 ).Distinct().ToList<FullVehicle>();
                                    foreach (FullVehicle vehicle in allVehicles) {
                                        YearRangeResult yrr = listItems.Where(x => x.make == vehicle.make && x.model == vehicle.model).FirstOrDefault();
                                        if (yrr == null) {
                                            YearRangeResult yrrNew = new YearRangeResult {
                                                make = vehicle.make,
                                                model = vehicle.model,
                                                style = vehicle.style,
                                                years = new List<double>(),
                                                count = 1
                                            };
                                            yrrNew.years.Add(vehicle.year);
                                            listItems.Add(yrrNew);
                                        } else {
                                            yrr.years.Add(vehicle.year);
                                        }
                                    }
                                    //  distinct the years, sort the order, grab the first year and find un interupted order.
                                    List<string> vehicleStrings = new List<string>();
                                    foreach (YearRangeResult yrr in listItems) {
                                        List<double> years = yrr.years.Distinct().ToList<double>();
                                        // example output: Class 3 Receiver Hitch<li>2014 Forester<li>
                                        vehicleStrings.Add(" " + years.Min().ToString() + "-" + years.Max().ToString() + " " + yrr.make + " " + yrr.model + " ");
                                    }
                                    temp = string.Join("/", vehicleStrings);
                                    break;
                                }
                    case 76: // CURT Category
                                {
                                    List<CatPart> catParts = p.CatParts.Where(x => x.partID == p.partID).ToList<CatPart>();
                                    List<string> categories = new List<string>();
                                    if (catParts != null) {
                                        foreach (CatPart cp in catParts) {
                                            if (cp.Category != null) {
                                                categories.Add(cp.Category.catTitle);
                                            }
                                        }
                                    } else {
                                        temp = "";
                                    }
                                    temp = string.Join("/", categories);
                                    break;
                                }
                    case 82: // Amazon Product Title
                                { // CURT Mfg 11000 Class 1 Trailer Hitch. Hitch, pin &clip. ball mount not included.
                                    string partClass = "";
                                    if (p.Classes != null) {
                                        Class pClass = p.Classes.FirstOrDefault<Class>();
                                        if (pClass != null) {
                                            partClass = pClass.class1;
                                        }
                                    }
                                    Content c = p.ContentBridges.Where(x => x.Content.cTypeID == 11 && x.partID == p.partID).Select(x => x.Content).FirstOrDefault<Content>();
                                    temp = "CURT Mfg " + p.partID + " " + partClass + " " + p.shortDesc + (c != null ? " - " + c.text : "");
                                    break;
                                }
                    case 84: // Amazon Browse Hidden Keywords
                                {

                                    temp = "";
                                    break;
                                }
                    case 85: // Amazon Describe the Product - Very long description
                                {
                                    temp = "";
                                    break;
                                }
                    case 86: // Amazon Product Bullet 1
                                {
                                    List<Content> bullets = p.ContentBridges.Where(x => x.Content.cTypeID == 2 && x.partID == p.partID).Select(x => x.Content).OrderBy(x=>x.text).ToList<Content>();
                                    if (bullets.Count > 0) {
                                        temp = bullets[0].text;
                                    }
                                    break;
                                }
                    case 87: // Amazon Product Bullet 2
                                {
                                    List<Content> bullets = p.ContentBridges.Where(x => x.Content.cTypeID == 2 && x.partID == p.partID).Select(x => x.Content).OrderBy(x => x.text).ToList<Content>();
                                    if (bullets.Count > 1) {
                                        temp = bullets[1].text;
                                    }
                                    break;
                                }
                    case 88: // Amazon Product Bullet 3
                                {
                                    List<Content> bullets = p.ContentBridges.Where(x => x.Content.cTypeID == 2 && x.partID == p.partID).Select(x => x.Content).OrderBy(x => x.text).ToList<Content>();
                                    if (bullets.Count > 2) {
                                        temp = bullets[2].text;
                                    }
                                    break;
                                }
                    case 89: // Amazon Product Bullet 4
                                {
                                    List<Content> bullets = p.ContentBridges.Where(x => x.Content.cTypeID == 2 && x.partID == p.partID).Select(x => x.Content).OrderBy(x => x.text).ToList<Content>();
                                    if (bullets.Count > 3) {
                                        temp = bullets[3].text;
                                    }
                                    break;
                                }
                    default:

                                temp = "";
                                break;
                }
                #endregion
            }
            return temp;
        }
    
    }

    public class YearRangeResult {
        public List<double> years { get; set; }
        public string make { get; set; }
        public string model { get; set; }
        public string style { get; set; }
        public int count { get; set; }
    }
    public class FullVehicle {
        public double year { get; set; }
        public string make { get; set; }
        public string model { get; set; }
        public string style { get; set; }
    }
}