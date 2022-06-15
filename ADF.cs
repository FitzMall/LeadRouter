using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Xml;
using RestSharp.Extensions;

namespace FMLeadRouter
{
    public class ADF
    {
        #region props

        public Prospect Adf = new Prospect();

        #endregion props

        public class Prospect
        {
            public Identity Id { get; set; }
            public string Requestdate { get; set; }
            public Vehicle Vehicles { get; set; }
            public Customer Customers { get; set; }
            public Vendor Vendors { get; set; }



            public class Vendor
            {

            }

            public class Customer
            {
                public Contact Contacts { get; set; }
                public string TimeframeDescription { get; set; }
                public string Comments { get; set; }

                public class Contact
                {
                    public string FirstName { get; set; }
                    public string LastName { get; set; }
                    public string Email { get; set; }
                    public string Phone { get; set; }
                    public string Street { get; set; }
                    public string Apartment { get; set; }
                    public string City { get; set; }
                    public string RegionCode { get; set; }
                    public string PostalCode { get; set; }
                    public string Country { get; set; }
                }
            }

            public class Vehicle
            {
                public string InterestAttr { get; set; }
                public string StatusAttr { get; set; }
                public Identity Id { get; set; }
                public string IdSource { get; set; }
                public int Year { get; set; }
                public string Make { get; set; }
                public string Model { get; set; }
                public string Vin { get; set; }
                public string Stock { get; set; }
                public string Trim { get; set; }
                public string Doors { get; set; }
                public string BodyStyle { get; set; }
                public string Transmission { get; set; }
                public string Odometer { get; set; }
                public string OdometerStatus { get; set; }
                public string OdometerUnits { get; set; }
                public string Condition { get; set; }
                public ColorCombination ColorCombinations { get; set; }
                public string ImageTag { get; set; }
                public string PriceQuote { get; set; }
                public string PriceComments { get; set; }
                public Option Options { get; set; }
                public Finance Finances { get; set; }
                public string Comments { get; set; }

                public class Finance
                {
                    public string Method { get; set; }
                    public string Amount { get; set; }//limit="maximum" type="total"
                    public string Balance { get; set; }
                }

                public class Option
                {
                    public string OptionName { get; set; }
                    public string ManufacturerCode { get; set; }
                    public string Stock { get; set; }
                    public string Weighting { get; set; }
                    public string PriceMsrp { get; set; }
                }

                public class ColorCombination
                {
                    public string InteriorColor { get; set; }
                    public string ExteriorColor { get; set; }
                    public string Preference { get; set; }
                }
            }

            public class Identity
            {
                public string Source { get; set; }
                public string Id { get; set; }
            }
        }



        #region helpers
        //TODO finish
        public string WriteADF(ADF email)
        {
            using (TextWriter writer = new Utf8StringWriter())
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(writer, new XmlWriterSettings { Indent = true }))
                {
                    //root
                    xmlWriter.WriteStartDocument();
                }
                return writer.ToString();
            }
        }

        public ADF ReadAdfXml(string xml)
        {
            var adf = new ADF();

            return adf;
        }

        public string ReadXmlNode(string xmlString, string xpath)
        {
            string result = string.Empty;
            try
            {
                var xml = new XmlDocument();
                //xmlString = xmlString.Replace("[0x00]", "");
                xml.LoadXml(xmlString);
                if (xml.DocumentElement != null)
                {
                    var selectSingleNode = xml.DocumentElement.SelectSingleNode(xpath);
                    if (selectSingleNode != null)
                    {
                        result = selectSingleNode.InnerText;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result;
        }

        public string ReadAttribute(string xmlString, string xpath, string attributeName)
        {
            string result = string.Empty;
            try
            {
                var xml = new XmlDocument();
                xml.LoadXml(xmlString);
                var element = (XmlElement)xml.SelectSingleNode(xpath);
                if (element != null)
                {
                    result = element.GetAttribute(attributeName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result;
        }

        public bool Contains(string xmlString, string xpath)
        {
            bool result = false;
            string[] splitStrings = xpath.Split('~');
            string path = splitStrings[0];
            string search = splitStrings[1];
            try
            {
                var xml = new XmlDocument();
                xml.LoadXml(xmlString);
                ///adf/prospect/customer/comments[contains(comments,'TRILOGY')]/Type

                if (xml.DocumentElement != null)
                {
                    XmlNode nodes = xml.DocumentElement.SelectSingleNode(path);
                    if (nodes != null)
                    {
                        result = nodes.InnerText.ToLower().Contains(search.ToLower());
                        //result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result;
        }

        public string EditAttribute(string xmlString, string xpath, string attributeName, string attributeValue)
        {
            string result = string.Empty;
            try
            {
                var xml = new XmlDocument();
                xml.LoadXml(xmlString);
                var element = (XmlElement)xml.SelectSingleNode(xpath);
                if (element != null)
                {
                    element.SetAttribute(attributeName, attributeValue);
                }
                result = xml.OuterXml;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result;
        }

        public string AppendAttribute(string xmlString, string xpath, string attributeName, string attributeValue)
        {
            string result = string.Empty;
            try
            {
                var xml = new XmlDocument();
                xml.LoadXml(xmlString);
                var element = (XmlElement)xml.SelectSingleNode(xpath);
                if (element != null)
                {
                    element.SetAttribute(attributeName, String.Format("{0}{1}", element.GetAttribute(attributeName), attributeValue));
                }
                result = xml.OuterXml;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result;
        }

        public string AppendAttribute(string xmlString, string xpath, string attributeValue)
        {
            string result = string.Empty;
            try
            {
                var xml = new XmlDocument();
                xml.LoadXml(xmlString);
                var element = (XmlElement)xml.SelectSingleNode(xpath);
                if (element != null)
                {
                    element.InnerText = String.Format("{0}{1}", element.InnerText, attributeValue);
                }
                result = xml.OuterXml;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result;
        }

        public string GetXmlFile(string filename)
        {
            string result = string.Empty;
            using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("assembly.folder." + filename))
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                    result = sr.ReadToEnd();
                }
            }

            return result;
        }

        public string AdfPhoneLead(HtmlParser dataParser)
        {
            StringBuilder sb = new StringBuilder();
            //sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            //sb.AppendLine("<?adf version=\"1.0\" ?>");
            //sb.AppendLine("<adf>");
            //sb.AppendFormat("<prospect status=\"{0}\">", dataParser.VehicleCondition);
            //sb.AppendFormat("<id sequence=\"1\" source=\"CallSource ADF Leads\">{0}</id>", dataParser.ProspectId);
            //sb.AppendFormat("<requestdate>{0}</requestdate>", Convert.ToDateTime(dataParser.ProspectRequestDate).ToString());
            //sb.AppendLine("<vehicle>");
            //sb.AppendFormat("<year><![CDATA[{0}]]></year>", dataParser.VehicleYear);
            //sb.AppendFormat("<make><![CDATA[{0}]]></make>", dataParser.VehicleMake);
            //sb.AppendFormat("<model><![CDATA[{0}]]></model>", dataParser.VehicleModel);
            //sb.AppendFormat("<vin><![CDATA[{0}]]></vin>", dataParser.VehicleVin);
            //sb.AppendFormat("<stock><![CDATA[{0}]]></stock>", dataParser.VehicleStock);
            //sb.AppendLine("</vehicle>");
            //sb.AppendLine("<customer>");
            //sb.AppendLine("<contact>");
            //sb.AppendFormat("<name part=\"first\"><![CDATA[{0}]]></name>", dataParser.CustomerFirstName);
            //sb.AppendFormat("<name part=\"last\"><![CDATA[{0}]]></name>", dataParser.CustomerLastName);
            //sb.AppendFormat("<email>{0}</email>", dataParser.CustomerEmail);
            //sb.AppendFormat("<phone type=\"voice\">{0}</phone>", dataParser.CustomerPhone);
            //sb.AppendFormat("<address>");
            //sb.AppendFormat("<street line=\"1\"><![CDATA[{0}]]></street>", dataParser.CustomerAddressInfo.Street);
            //sb.AppendFormat("<city><![CDATA[{0}]]></city>", dataParser.CustomerAddressInfo.City);
            //sb.AppendFormat("<regioncode>{0}</regioncode>", dataParser.CustomerAddressInfo.State);
            //sb.AppendFormat("<postalcode>{0}</postalcode>", dataParser.CustomerAddressInfo.Zip);
            //sb.AppendFormat("<country>{0}</country>", dataParser.CustomerAddressInfo.Country);
            //sb.AppendLine("</address>");
            //sb.AppendLine("</contact>");
            //sb.AppendFormat("<id sequence=\"1\" source=\"CallSource ADF Leads\"></id>");
            //sb.AppendFormat("<comments><![CDATA[Call Duration(s): {0}; Call Result: Connected; Audio location: {1};  Phone type: Other; Toll free number: {2}]]>", dataParser.CallDuration, dataParser.AudioUrl, dataParser.VendorContactInfo.Phone);
            //sb.AppendLine("</comments>");
            //sb.AppendFormat("<AudioURL><![CDATA[{0};]]></AudioURL>", dataParser.AudioUrl.HtmlEncode());
            //sb.AppendFormat("<AudioURLEncoded>{0}</AudioURLEncoded>", dataParser.AudioUrl.HtmlEncode());
            //sb.AppendLine("</customer>");
            //sb.AppendLine("<vendor>");
            //sb.AppendFormat("<id sequence=\"1\" source=\"CallSource ADF Leads\"></id>");
            //sb.AppendFormat("<vendorname><![CDATA[{0}]]></vendorname>", dataParser.VendorContactInfo.Name);
            //sb.AppendFormat("<url>{0}</url>", dataParser.VendorUrl);
            //sb.AppendLine("<contact primarycontact=\"1\">");
            //sb.AppendFormat("<email>{0}</email>", dataParser.VendorContactInfo.Email);
            //sb.AppendFormat("<phone type=\"voice\">{0}</phone>", dataParser.VendorContactInfo.Phone);
            //sb.AppendLine("<address>");
            //sb.AppendFormat("<street line=\"1\"><![CDATA[]]></street>");
            //sb.AppendFormat("<street line=\"2\"><![CDATA[]]></street>");
            //sb.AppendFormat("<city><![CDATA[]]></city>");
            //sb.AppendFormat("<regioncode></regioncode>");
            //sb.AppendFormat("<postalcode></postalcode>");
            //sb.AppendFormat("<country></country>");
            //sb.AppendLine("</address>");
            //sb.AppendLine("</contact>");
            //sb.AppendLine("</vendor>");
            //sb.AppendLine("<provider>");
            //sb.AppendFormat("<id sequence=\"1\" source=\"CallSource ADF Leads\"></id>");
            //sb.AppendFormat("<name part=\"full\"><![CDATA[{0}]]></name>", dataParser.ProviderName);
            //sb.AppendFormat("<service><![CDATA[{0}]]></service>", dataParser.ProviderService);
            //sb.AppendFormat("<url>{0}</url>", dataParser.ProviderUrl);
            //sb.AppendFormat("<email>{0}</email>", dataParser.ProviderEmail);
            //sb.AppendFormat("<phone>{0}</phone>", dataParser.ProviderPhone);
            ////sb.AppendLine("<contact primarycontact=\"1\">");
            ////sb.AppendFormat("<name part=\"full\">CallSource Customer Care</name>");
            ////sb.AppendFormat("<email></email>");
            ////sb.AppendFormat("<phone type=\"voice\" time=\"day\">800 500-4433</phone>");
            ////sb.AppendFormat("<phone type=\"fax\" time=\"day\"></phone>");
            ////sb.AppendFormat("<address>");
            ////sb.AppendFormat("<street line=\"1\">5601 Lindero Canyon Rd., Suite 200</street>");
            ////sb.AppendFormat("<city>Westlake Village</city>");
            ////sb.AppendFormat("<regioncode>CA</regioncode>");
            ////sb.AppendFormat("<postalcode>91362</postalcode>");
            ////sb.AppendFormat("<country>US</country>");
            ////sb.AppendLine("</address>");
            ////sb.AppendLine("</contact>");
            //sb.AppendLine("</provider>");
            //sb.AppendLine("</prospect>");
            //sb.AppendLine("</adf>");

            //Correct Version
            sb.AppendLine("<?xml version=\"1.0\"?>");
            sb.AppendLine("<?adf version=\"1.0\" ?>");
            sb.AppendLine("<adf>");
            sb.AppendLine("<prospect>");
            sb.AppendFormat("<id source=\"CallSource ADF Leads\" sequence=\"1\"/>");
            sb.AppendFormat("<requestdate>{0}</requestdate>", Convert.ToDateTime(dataParser.ProspectRequestDate).ToString());
            sb.AppendFormat("<vehicle interest=\"buy\" status=\"{0}\">", dataParser.VehicleCondition);
            sb.AppendFormat("<id source=\"CallSource ADF Leads\" sequence=\"1\"/>");
            sb.AppendFormat("<year>{0}</year>", dataParser.VehicleYear);
            sb.AppendFormat("<make>{0}</make>", dataParser.VehicleMake);
            sb.AppendFormat("<model>{0}</model>", dataParser.VehicleModel);
            sb.AppendFormat("<vin>{0}</vin>", dataParser.VehicleVin);
            sb.AppendFormat("<stock>{0}</stock>", dataParser.VehicleStock);
            sb.AppendFormat("<price currency=\"USD\" type=\"asking\"/>");
            sb.AppendLine("</vehicle>");
            sb.AppendLine("<customer>");
            sb.AppendLine("<contact>");
            sb.AppendFormat("<name part=\"first\">{0}</name>", dataParser.CustomerFirstName);
            sb.AppendFormat("<name part=\"last\">{0}</name>", dataParser.CustomerLastName);
            sb.AppendFormat("<email>{0}</email>", dataParser.CustomerEmail);
            sb.AppendFormat("<phone type=\"voice\">{0}</phone>", dataParser.CustomerPhone);
            sb.AppendLine("</contact>");
            sb.AppendFormat("<comments><![CDATA[Call Duration(s): {0}; Call Result: Connected; Audio location: {1};  Phone type: Other; Toll free number: {2}]]>", dataParser.CallDuration, dataParser.AudioUrl, dataParser.VendorContactInfo.Phone);
            sb.AppendLine("</comments>");
            sb.AppendLine("</customer>");
            sb.AppendLine("<vendor>");
            sb.AppendFormat("<id>{0}</id>", dataParser.VendorId);
            sb.AppendFormat("<vendorname>{0}</vendorname>", dataParser.VendorContactInfo.Name);
            sb.AppendLine("<contact>");
            sb.AppendFormat("<name>Pre-Owned Sales</name>");
            sb.AppendFormat("<phone type=\"voice\" time=\"nopreference\">{0}</phone>", dataParser.VendorContactInfo.Phone);
            sb.AppendLine("</contact>");
            sb.AppendLine("</vendor>");
            sb.AppendLine("<provider>");
            sb.AppendFormat("<name>{0}</name>", dataParser.ProviderName);
            sb.AppendFormat("<service>{0}</service>", dataParser.ProviderService);
            sb.AppendFormat("<url>{0}</url>", dataParser.VendorUrl);
            sb.AppendFormat("<email>{0}</email>", dataParser.VendorContactInfo.Email);
            sb.AppendFormat("<phone type=\"voice\">{0}</phone>", dataParser.VendorContactInfo.Phone);
            sb.AppendLine("</provider>");
            sb.AppendFormat("<leadtype>Sales</leadtype>");
            sb.AppendLine("</prospect>");
            sb.AppendLine("</adf>");
            return sb.ToString();
        }

        #endregion helpers
    }

    public class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding
        {
            get
            {
                return Encoding.UTF8;
            }
        }
    }
}
