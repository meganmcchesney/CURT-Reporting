using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Reporting.Models {
    public partial class Customer {

       //public bool Login(string email, int id) {
       //    try {
       //        CurtDevDataContext db = new CurtDevDataContext();
       //
       //        Customer cust = db.Customers.Where(x => x.email.Equals(email) && x.customerID.Equals(id)).FirstOrDefault<Customer>();
       //        if (cust == null || cust.customerID == 0) {
       //            return false;
       //        }
       //
       //        return true;
       //    } catch (Exception) {
       //        return false;
       //    }
       //}

        public bool Login(string email, string password)
        {
            try
            {
                string loginResponse = loginAPIAuth(email, password);
                ICustomer customer = JsonConvert.DeserializeObject<ICustomer>(loginResponse);

                // get response, check response if its null then return false;
               
                if (customer == null)
                {
                    return false;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public CustomerUser GetCustomerUser(string email)
         {
             CurtDevDataContext db = new CurtDevDataContext();
             CustomerUser user = db.CustomerUsers.Where(x => x.email.Equals(email)).FirstOrDefault<CustomerUser>();
             
             return user;
         }

        public async Task<Customer> Get(int id = 0) {
            this.customerID = (id == 0) ? this.customerID : id;

            CurtDevDataContext db = new CurtDevDataContext();
            Customer c = db.Customers.Where(x => x.customerID.Equals(this.customerID)).FirstOrDefault<Customer>();

            return c;
        }

        // Our attempt at using reflection to throw our object data into this,
        // it'll probably be a total fucking flop. ^_^
        public void Populate(Customer c) {
            foreach (PropertyInfo prop in this.GetType().GetProperties()) {
                if (!prop.CanWrite) {
                    throw new InvalidOperationException(
                        string.Format(
                            "Unable to set property {0} of type {1}", 
                            prop.Name, 
                            typeof(Customer).Name
                        )
                    );
                }
                prop.SetValue(c, c.GetType().GetProperty(prop.Name));
            }
        }

        public string loginAPIAuth(string email, string UnencryptedPassword)
        {
            string resp = HttpPost("http://api.curtmfg.com/v3/customer/auth", "email=" + email + "&password=" + UnencryptedPassword);
            //ICustomer customer = JsonConvert.DeserializeObject<ICustomer>(resp);
            return resp;
        }

        public string loginAPIAuth(string key)
        {
          
            string resp = HttpPost("http://api.curtmfg.com/v3/customer/auth", "key=" + key);
            
            return resp;
        }

        public static string HttpPost(string URI, string Parameters)
        {
            System.Net.WebRequest req = System.Net.WebRequest.Create(URI);
            //Add these, as we're doing a POST
            req.ContentType = "application/x-www-form-urlencoded";
            req.Method = "POST";
            //We need to count how many bytes we're sending. Post'ed Faked Forms should be name=value&
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(Parameters);
            req.ContentLength = bytes.Length;
            System.IO.Stream os = req.GetRequestStream();
            os.Write(bytes, 0, bytes.Length); //Push it out there
            os.Close();
            System.Net.WebResponse resp = req.GetResponse();
            if (resp == null) return null;
            System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
            return sr.ReadToEnd().Trim();
        }



    }

 
    public class ICustomer
    {

        #region Private Properties

        private int _Id = 0;

        private string _Name = "";

        private string _Email = "";

        private string _Address = "";

        private string _Address2 = "";

        private string _City = "";

        private string _StateAbbreviation = "";

        private string _State = "";

        private string _Country = "";

        private string _CountryAbbreviation = "";

        private string _PostalCode = "";

        private string _Phone = "";

        private string _Fax = "";

        private string _ContactPerson = "";

        private float _Latitude = 0;

        private float _Longitude = 0;

        private ICustomer _Parent = null;

        private string _Website = "";

        private string _SearchUrl = "";

        private string _Logo = "";

        private string _DealerType = "";

        private string _DealerTier = "";

        private string _SalesRepresentative = "";

        private int _SalesRepresentativeCode = 0;

        private string _MapixCode = "";

        private string _MapixDescription = "";

        private List<Location> _Locations = null;

        private List<ICustomerUser> _Users = null;

        #endregion

        #region Public Properties

        public int Id
        {

            get { return _Id; }

            set { _Id = value; }

        }

        public string Name
        {

            get { return _Name ?? ""; }

            set { _Name = value; }

        }

        public string Email
        {

            get { return _Email ?? ""; }

            set { _Email = value; }

        }

        public string Address
        {

            get { return _Address ?? ""; }

            set { _Address = value; }

        }

        public string Address2
        {

            get { return _Address2 ?? ""; }

            set { _Address2 = value; }

        }

        public string City
        {

            get { return _City ?? ""; }

            set { _City = value; }

        }

        public string StateAbbreviation
        {

            get { return _StateAbbreviation ?? ""; }

            set { _StateAbbreviation = value; }

        }

        public string State
        {

            get { return _State ?? ""; }

            set { _State = value; }

        }

        public string Country
        {

            get { return _Country ?? ""; }

            set { _Country = value; }

        }

        public string CountryAbbreviation
        {

            get { return _CountryAbbreviation ?? ""; }

            set { _CountryAbbreviation = value; }

        }

        public string PostalCode
        {

            get { return _PostalCode ?? ""; }

            set { _PostalCode = value; }

        }

        public string Phone
        {

            get { return _Phone ?? ""; }

            set { _Phone = value; }

        }

        public string Fax
        {

            get { return _Fax ?? ""; }

            set { _Fax = value; }

        }

        public string ContactPerson
        {

            get { return _ContactPerson ?? ""; }

            set { _ContactPerson = value; }

        }

        public float Latitude
        {

            get { return _Latitude; }

            set { _Latitude = value; }

        }

        public float Longitude
        {

            get { return _Longitude; }

            set { _Longitude = value; }

        }

        public string Website
        {

            get { return _Website ?? ""; }

            set { _Website = value; }

        }

        public ICustomer Parent
        {

            get { return _Parent; }

            set { _Parent = value; }

        }

        public string SearchUrl
        {

            get { return _SearchUrl ?? ""; }

            set { _SearchUrl = value; }

        }

        public string Logo
        {

            get { return _Logo ?? ""; }

            set { _Logo = value; }

        }

        public string DealerType
        {

            get { return _DealerType ?? ""; }

            set { _DealerType = value; }

        }

        public string DealerTier
        {

            get { return _DealerTier ?? ""; }

            set { _DealerTier = value; }

        }

        public string SalesRepresentative
        {

            get { return _SalesRepresentative ?? ""; }

            set { _SalesRepresentative = value; }

        }

        public int SalesRepresentativeCode
        {

            get { return _SalesRepresentativeCode; }

            set { _SalesRepresentativeCode = value; }

        }

        public string MapixCode
        {

            get { return _MapixCode ?? ""; }

            set { _MapixCode = value; }

        }

        public string MapixDescription
        {

            get { return _MapixDescription ?? ""; }

            set { _MapixDescription = value; }

        }

        public List<Location> Locations
        {

            get { return _Locations; }

            set { _Locations = value; }

        }

        public List<ICustomerUser> Users
        {

            get { return _Users; }

            set { _Users = value; }

        }

        #endregion

        public partial class ICustomerUser
        {

            private string _Email = "";

            private string _Name = "";

            private string _DateAdded = "";

            private bool _Active = false;

            private bool _Sudo = false;

            private Location _Location = null;

            private List<ApiCredentials> _Keys = null;

            private bool _Current = false;

            public string Email
            {

                get
                {

                    return this._Email ?? "";

                }

                set
                {

                    if (value != null && value != this._Email)
                    {

                        this._Email = value;

                    }

                }

            }

            public string Name
            {

                get
                {

                    return this._Name ?? "";

                }

                set
                {

                    if (value != null && value != this._Name)
                    {

                        this._Name = value;

                    }

                }

            }

            public string DateAdded
            {

                get
                {

                    return this._DateAdded ?? "";

                }

                set
                {

                    if (value != null && value != this._DateAdded)
                    {

                        this._DateAdded = value;

                    }

                }

            }

            public bool Active
            {

                get
                {

                    return this._Active;

                }

                set
                {

                    if (value != this._Active)
                    {

                        this._Active = value;

                    }

                }

            }


            public bool Current
            {

                get
                {

                    return this._Current;

                }

                set
                {

                    if (value != this._Current)
                    {

                        this._Current = value;

                    }

                }

            }

            public bool Sudo
            {

                get
                {

                    return this._Sudo;

                }

                set
                {

                    if (value != this._Sudo)
                    {

                        this._Sudo = value;

                    }

                }

            }

            public Location Location
            {

                get
                {

                    return this._Location;

                }

                set
                {

                    if (value != this._Location)
                    {

                        this._Location = value;

                    }

                }

            }

            public List<ApiCredentials> Keys
            {

                get
                {

                    return this._Keys;

                }

                set
                {

                    if (value != this._Keys)
                    {

                        this._Keys = value;

                    }

                }

            }

        }

        public class ApiCredentials
        {

            public Guid Key { get; set; }

            public string Type { get; set; }

        }

        public class Location
        {

            public int Id { get; set; }

            public string Name { get; set; }

            public string Address { get; set; }

            public string City { get; set; }

            public string State { get; set; }

            public string StateAbbreviation { get; set; }

            public string Country { get; set; }

            public string CountryAbbreviation { get; set; }

            public string PostalCode { get; set; }

            public string Email { get; set; }

            public string Phone { get; set; }

            public string Fax { get; set; }

            public double Latitude { get; set; }

            public double Longitude { get; set; }

            public int CustomerId { get; set; }

            public string ContactPerson { get; set; }

            public bool IsPrimary { get; set; }

            public bool ShippingDefault { get; set; }

        }

        public class KOrder
        {

            public int OrderId { get; set; }

            public int AcctId { get; set; }

            public DateTime OrderDate { get; set; }

            public int VehicleId { get; set; }

            public string Fname { get; set; }

            public string Lname { get; set; }

            public string Email { get; set; }

            public string Phone { get; set; }

            public int IsProcessed { get; set; }

            public int ItemCount { get; set; }

            public decimal TotalPrice { get; set; }

            public string Address { get; set; }

            public string City { get; set; }

            public string StateAbbreviation { get; set; }

            public string State { get; set; }

            public int StateId { get; set; }

            public string CountryAbbreviation { get; set; }

            public string Country { get; set; }

            public string PostalCode
            {

                get;

                set;

            }

            public int LocationId { get; set; }

        }

        public class KOrderItem
        {

            public int ItemId { get; set; }

            public int OrderId { get; set; }

            public int PartId { get; set; }

            public int Quantity { get; set; }

            public decimal Price { get; set; }

            public int IsFulfilled { get; set; }

        }

    }
}