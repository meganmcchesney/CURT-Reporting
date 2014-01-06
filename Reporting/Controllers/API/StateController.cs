using Reporting.Models;

namespace Reporting.Controllers.API {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;


    public class StatesController : ApiController {

        [AcceptVerbs("GET")]
        public IEnumerable<Helper.DisplayableState> GetAll(){
            return new Helper().GetStates();
        }

        [AcceptVerbs("GET")]
        public IEnumerable<Helper.DisplayableState> GetCountryStates(string country){
            return new Helper().GetCountryStates(country);
        } 
    }
}
