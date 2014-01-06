using System.Collections.Generic;
using System.Web.Http;
using Reporting.Models;

namespace Reporting.Controllers.API {
    public class CountryController : ApiController {

        [AcceptVerbs("GET")]
        public IEnumerable<Helper.DisplayableCountry> GetAll(){
            return new Helper().GetCountries();
        }

    }
}
