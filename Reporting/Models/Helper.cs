using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Reporting.Models {
    public class Helper {

        public class DisplayableCountry {
            public string Abbr { get; set; }
            public string Name { get; set; }
        }

        public class DisplayableState {
            public int CountryId { get; set; }
            public string Abbr { get; set; }
            public string State { get; set; }
        }

        public IEnumerable<DisplayableCountry> GetCountries() {
            try {
                var db = new CurtDevDataContext();
                return (from c in db.Countries
                        select new DisplayableCountry() {
                            Name = c.name,
                            Abbr = c.abbr
                        }).ToList();
            } catch (Exception) {
                return new List<DisplayableCountry>();
            }
        }

        public IEnumerable<DisplayableState> GetStates() {
            try {
                var db = new CurtDevDataContext();
                return (from s in db.States
                        select new DisplayableState() {
                            CountryId = s.countryID,
                            Abbr = s.abbr,
                            State = s.state1
                        }).ToList();
            } catch (Exception) {
                return new List<DisplayableState>();
            }
        }

        public IEnumerable<DisplayableState> GetCountryStates(string country) {
            try {
                var db = new CurtDevDataContext();
                var c =
                    db.Countries.Where(x => x.name.Equals(country) || x.abbr.Equals(country)).Select(x => x.countryID).FirstOrDefault<int>();

                return (from s in db.States
                        where s.countryID.Equals(c)
                        select new Helper.DisplayableState() {
                            CountryId = c,
                            Abbr = s.abbr,
                            State = s.state1
                        }).ToList();
            } catch (Exception) {
                return new List<Helper.DisplayableState>();
            }
        }

    }
}