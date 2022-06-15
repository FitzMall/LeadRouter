using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

namespace FMLeadRouter
{
    public class HtmlParser
    {
        #region props

        public string ProspectId { get; set; }
        public string ProspectRequestDate { get; set; }

        public string VehicleCondition { get; set; }
        public string VehicleYear { get; set; }
        public string VehicleMake { get; set; }
        public string VehicleModel { get; set; }
        public string VehicleVin { get; set; }
        public string VehicleStock { get; set; }

        public string Comments { get; set; }

        public string CustomerName { get; set; }
        public string CustomerLastName { get; set; }
        public string CustomerFirstName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
        public CustomerAddress CustomerAddressInfo { get; set; }

        public class CustomerAddress
        {
            public string Street { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string Zip { get; set; }
            public string Country { get; set; }
        }

        public string AudioUrl { get; set; }
        public string CallDuration { get; set; }

        public string VendorId { get; set; }
        public string VendorName { get; set; }
        public string VendorUrl { get; set; }
        public VendorContact VendorContactInfo { get; set; }

        public string ProviderName { get; set; }
        public string ProviderService { get; set; }
        public string ProviderUrl { get; set; }
        public string ProviderEmail { get; set; }
        public string ProviderPhone { get; set; }

        public class VendorContact
        {
            public string Name { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string TrackPhone { get; set; }
            public string Street { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string Zip { get; set; }
            public string Country { get; set; }
        }
        #endregion props

        #region helpers

        public HtmlParser CarsdotComParse(string html)
        {
            var adfInfo = new HtmlParser();
            adfInfo.VendorContactInfo = new VendorContact();
            adfInfo.CustomerAddressInfo = new CustomerAddress();

            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(html);

            HtmlNodeCollection nodes = document.DocumentNode.SelectNodes("//tr[@bgcolor='#EAEAEA']");//.Descendants("tr").Select(x => x.Elements("td").Select(e => e.InnerText));
            //[@bgcolor='#EAEAEA']
            var tds = nodes.Descendants("td");

            adfInfo.ProspectRequestDate = tds.Select(x => x.InnerText).First();
            adfInfo.VendorContactInfo.Email = tds.Skip(1).Select(x => x.InnerText).First();
            adfInfo.VendorContactInfo.Name = tds.Skip(6).Select(x => x.InnerText).First().Trim();
            adfInfo.VendorContactInfo.TrackPhone = tds.Skip(3).Select(x => x.InnerText).First();
            adfInfo.VendorContactInfo.Phone = tds.Skip(4).Select(x => x.InnerText).First();

            adfInfo.VendorId = tds.Skip(2).Select(x => x.InnerText).First();
            adfInfo.VendorName = tds.Skip(6).Select(x => x.InnerText).First().Trim();

            adfInfo.VehicleCondition = tds.Skip(17).Select(x => x.InnerText).First();
            string zip = tds.Skip(15).Select(x => x.InnerText).First();
            adfInfo.CustomerAddressInfo.Zip = zip.Substring(zip.Length - 5);
            adfInfo.CustomerPhone = tds.Skip(11).Select(x => x.InnerText).First();

            adfInfo.AudioUrl = tds.Skip(10).Select(x => x.ChildNodes["a"]).First().Attributes["href"].Value;
            adfInfo.CallDuration = tds.Skip(12).Select(x => x.InnerText).First();

            //nodes.Descendants("a").
            adfInfo.CustomerFirstName = "Unknown";
            adfInfo.CustomerLastName = "Name";
            adfInfo.CustomerAddressInfo.Country = "USA";
            adfInfo.ProviderName = "Cars.com";
            adfInfo.ProviderService = "Cars.com";
            adfInfo.ProviderEmail = "support@cars.com";
            adfInfo.ProviderPhone = "8882527731";
            adfInfo.ProviderUrl = "Cars.com";

            return adfInfo;
        }


        #endregion helpers
    }
}
