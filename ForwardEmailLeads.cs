using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading;
using RestSharp;
using S22.Imap;


namespace FMLeadRouter
{
    public class EmailCredentials
    {
        public string MailServer { get; set; }
        public int Port { get; set; }
        public bool Ssl { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }

        public EmailCredentials GetImapCredentials()
        {
            Console.WriteLine("Getting Email Credentials");
            var credentials = new EmailCredentials
            {
                MailServer = ConfigurationManager.AppSettings["imapHost"],
                Port = Convert.ToInt32(ConfigurationManager.AppSettings["imapPort"]),
                Ssl = Convert.ToBoolean(ConfigurationManager.AppSettings["ssl"]),
                Login = ConfigurationManager.AppSettings["imapUser"],
                Password = ConfigurationManager.AppSettings["imapPass"]
            };

            return credentials;
        }
    }

    public class LeadVendor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Email { get; set; }
        public int Sms { get; set; }
        public DateTime CreateDate { get; set; }
    }


    public class LocationByDealerName
    {
        public int ID { get; set; }
        public string FullName { get; set; }
        public string LocCode { get; set; }
        public string Mall { get; set; }
        public int State { get; set; }

    }


    public class LeadRoute
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int LeadVendorId { get; set; }
        public string VendorCode { get; set; }
        public string ForwardEmail { get; set; }
        public int ForwardSms { get; set; }
        public bool IsActive { get; set; }
        public string Loc { get; set; }
        public string Mall { get; set; }
        public string Make { get; set; }
        public string XmlLeadSource { get; set; }
        public string XmlVendorCode { get; set; }
        public string XmlRule { get; set; }
        public string LeadSourceAddOn { get; set; }
        public DateTime CreateDate { get; set; }
    }

    public class MailError
    {
        public int Id { get; set; }
        public string MailId { get; set; }
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Status { get; set; }//not used for anything yet.
        public int LeadVendorId { get; set; }
        public int LeadRouteId { get; set; }
        public DateTime CreateDate { get; set; }
    }


    public class LeadRouteLog
    {
        public int Id { get; set; }
        public string MailId { get; set; }
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Status { get; set; }//not used for anything yet.
        public int LeadVendorId { get; set; }
        public int LeadRouteId { get; set; }
        public DateTime CreateDate { get; set; }


    }

    public class LeadVendorCodeXPath
    {
        public string Xml { get; set; }
        public DateTime CreateDate { get; set; }
    }

    public class VendorToRoute
    {
        public int Id { get; set; }
        public int VendorId { get; set; }
        public int LeadRouteId { get; set; }
        public string VendorCode { get; set; }
        public bool IsActive { get; set; }
    }

    public class ChromeImage
    {
        public string StyleId { get; set; }
        public string ShotCode { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public string Url { get; set; }
        public string UrlShort { get; set; }
        public string PrimaryColorCode { get; set; }
        public string SecondaryColorCode { get; set; }
        public DateTime DateDownloaded { get; set; }
        public DateTime DateUploaded { get; set; }
    }
    public class Car
    {
        public string Stock { get; set; }
        public string Loc { get; set; }
        public string Mall { get; set; }
        public string Cond { get; set; }
        public string Wholesale { get; set; }
    }

    public class DealershipSearchResult
    {
        public string LocCode { get; set; }
        public string Mall { get; set; }
    }

    public class CarDetails
    {
        public string StockNumber { get; set; }
        public string StyleId { get; set; }
        public string XrefID { get; set; }
        public string Loc { get; set; }
        public string StoreName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Hours { get; set; }
        public string Mall { get; set; }
        public string VehicleCondition { get; set; }
        public string ModelYear { get; set; }
        public string MakeName { get; set; }
        public string ModelName { get; set; }
        public string VIN { get; set; }
        public decimal Freight { get; set; }
        public decimal InternetPrice { get; set; }
        public decimal MSRP { get; set; }
        public string Wholesale { get; set; }
        public string StyleName { get; set; }
        public string ModelCode { get; set; }
        public string ExteriorColor { get; set; }
        public string GenExteriorColor { get; set; }
        public string Fuel { get; set; }
        public string Transmission { get; set; }
        public string Passengers { get; set; }

    }

    public class Incentive
    {
        public string IncentiveTitle { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Store { get; set; }
        public string Description { get; set; }
    }

    public class LeadCrmEmail
    {
        public int Id { get; set; }
        public string LocCode { get; set; }
        public string Email { get; set; }
    }

    public class TrafficSource
    {
        public List<Sources> Sources { get; set; }
    }

    public class Sources
    {
        public string key { get; set; }
        public string value { get; set; }
    }

    public class S22Wrapper
    {
        private static ImapClient _client = null;
        private static EmailCredentials _creds;
        private static RouteEmailLeads _routeEmail;
        static AutoResetEvent reconnectEvent = new AutoResetEvent(false);
        private static Thread _thread;


        public BackgroundWorker bw = new BackgroundWorker();
        private bool _restart = false;


        private static ADF _adf;
        private static HtmlParser _html;

        public S22Wrapper()
        {
            _routeEmail = new RouteEmailLeads();
            _creds = new EmailCredentials().GetImapCredentials();
            _adf = new ADF();
            _html = new HtmlParser();

        }

        public void Start(string[] args)
        {
            _thread = new Thread(Connect);
            _thread.Start();
        }

        public void Stop()
        {

        }

        public void Connect()
        {

            try
            {
                bw.RunWorkerCompleted += bw_RunWorkerCompleted;
                bw.DoWork += bw_DoWork;
                bw.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                SendAlert("An error occurred:", String.Format("An error occurred: {0}", ex.Message));
            }
            finally
            {
                if (_client != null)
                    _client.Dispose();
            }
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            Console.Write("Connecting...");
            InitializeClient();
            Console.WriteLine("OK");

            reconnectEvent.WaitOne();
        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Console.WriteLine("BW Completed");
            bw.RunWorkerCompleted -= bw_RunWorkerCompleted;
            bw.DoWork -= bw_DoWork;
            bw.Dispose();
            //Cleanup the connection
            Disconnect();
            //Start the worker again
            Connect();

        }

        public void Disconnect()
        {
            _client.NewMessage -= OnNewMessage;
            _client.IdleError -= client_IdleError;
            _client.Logout();
            Console.WriteLine("Client Disconnected...");
        }

        static void InitializeClient()
        {
            // Dispose of existing instance, if any.
            if (_client != null)
                _client.Dispose();
            _client = new ImapClient(_creds.MailServer, _creds.Port, _creds.Login, _creds.Password, AuthMethod.Login, _creds.Ssl);

            //Get all unread messages
            ProcessUnreadMessages(_client.Search(SearchCondition.Unseen()).ToList());
            //Check if Inbox has more than 5 emails read, send email alert if true
            CheckInboxStatus();
            //Setup event handlers.
            _client.NewMessage += OnNewMessage;
            _client.IdleError += client_IdleError;
            SendAlert("Lead Router Connected", String.Format("The Lead Router reconnected at: {0}", DateTime.Now));
        }

        private static void ProcessUnreadMessages(List<uint> uids)
        {
            Console.WriteLine("Process Messages...");
            IEnumerable<MailMessage> messages = _client.GetMessages(uids, false);
            int thisMsg = 0;
            foreach (var mailMessage in messages)
            {
                //Check if it needs to be routed
                if (CallRouteProcessAdf2(mailMessage) == true)
                {
                    MoveProcessedMessages(uids[thisMsg]);
                };
                thisMsg += 1;
                
            }
            //Check if Inbox has more than 5 emails unread, send email alert if true
            CheckInboxStatus();
        }

        private static void ProcessMultipleMessages(List<uint> uids)
        {
            Console.WriteLine("Process Messages...");
            foreach (var id in uids)
            {
                ProcessMessage(id);
            }
        }

        private static void ProcessMessage(uint uid)
        {
            Console.WriteLine("Process Messages...");
            MailMessage mailMessage = _client.GetMessage(uid);

            //TODO ADF vs STAR check
            string xmlStandard = string.Empty;
            //xmlStandard = GetXmlStandard(mailMessage);

            //switch (xmlStandard)
            //{
            //    case "adf":
            //        CallRouteProcessAdf(mailMessage);
            //        break;
            //    case "star":
            //        CallRouteProcessStar(mailMessage);
            //        break;
            //}
            //Check if it needs to be routed

            //Send Html to HtmlArchive Folder
            if (mailMessage.IsBodyHtml && mailMessage.From.Address == "phoneleads@cars.com")
            {
                //Move to HtmlArchive folder
                MoveProcessedMessagesHtml(uid);
                //Exit function
                //return;
            }
            else
            {

                //Check if it needs to be routed
                CallRouteProcessAdf2(mailMessage);


                //Move Processed Messages
                MoveProcessedMessages(uid);
            }



            //Kludge: Issue if multiple messages come in at the same time only 1st gets read
            if (_client.Search(SearchCondition.Unseen()).Any())
            {
                ProcessUnreadMessages(_client.Search(SearchCondition.Unseen()).ToList());
            }
            //Check if Inbox has more than 2 emails read, but lingering, send email alert if true
            CheckInboxStatus();


        }

        private static bool CallRouteProcessAdf2(MailMessage mailMessage, string createRoute = null)
        {
            //This is the main body of code to determine the route...
            string errorMailShort;
            string errorMailLong;
            bool retval = true;

            Console.WriteLine("Email From: {0}", mailMessage.From.Address);
            MailMessage orgMailMessage = mailMessage;

            //Send Html to become ADF
            if (mailMessage.IsBodyHtml && mailMessage.From.Address == "phoneleads@cars.com")
            {
                //Parse HTML
                //var adfInfo = _html.CarsdotComParse(mailMessage.Body);
                //string adfPhoneLead = _adf.AdfPhoneLead(adfInfo);
                //mailMessage.Body = adfPhoneLead;
            }

            //For certain Leads, remove any content after the </adf> tag...

            if (mailMessage.Body != null && mailMessage.Body.Length > 6)
            {
                mailMessage.Body = mailMessage.Body.Substring(0, mailMessage.Body.IndexOf("</adf>") + 6);
            }

            //Remove Spaces Between Tags
            mailMessage.Body = CorrectLeadingWhiteSpaceIssues(mailMessage.Body);
            //Correct Bad Formatting
            mailMessage.Body = CorrectFormattingIssues(mailMessage.Body);
            //Note: Clean XML string for hidden characters
            mailMessage.Body = CorrectHiddenCharacters(mailMessage.Body);
            //Note: Add CDATA to comments, to prevent XML formatting break
            mailMessage.Body = WrapCommentElementInCdata(mailMessage.Body);

            //Read
            //Get Vendor
            LeadVendor vendor = _routeEmail.GetLeadVendorByEmail(mailMessage.From.Address) ?? new LeadVendor { Id = 0 };

            //Stk# Check
            var stockNumber = "";
            if (vendor.Id != 34 && vendor.Id != 45)
            {
                stockNumber = GetStockNumber(mailMessage.Body, "/adf/prospect/vehicle/stock");
            }


            var vehicleStockNumberForLookup = "";
            if (vendor.Id != 1070 && vendor.Id != 1075)
            {
                vehicleStockNumberForLookup = GetStockNumber(mailMessage.Body, "/adf/prospect/vehicle/stock");
            }

            var make = GetMake(mailMessage.Body, "/adf/prospect/vehicle/make");
            //check for used

            if (String.IsNullOrEmpty(make))
            {
                make = GetMake(mailMessage.Body, "/adf/prospect/vehicle/@status");
            }

            var status = GetMake(mailMessage.Body, "/adf/prospect/vehicle/@status");
            //check for used
            if (String.IsNullOrEmpty(status))
            {
                status = "";
            }

            var interest = GetInterest(mailMessage.Body, "/adf/prospect/vehicle/@interest");
            //check for used
            if (String.IsNullOrEmpty(interest))
            {
                interest = "";
            }

            //Spam Filter
            bool isSpam = false;
            if (vendor.Id == 24)
            {
                isSpam = SpamFilter(mailMessage.Body, "/adf/prospect/customer/contact/name[@part='last']", @"\d+");
                if (!isSpam)
                {
                    isSpam = SpamFilter(mailMessage.Body, "/adf/prospect/customer/contact/email", @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", true);
                }
            }

            //TrueCar ZipCode Prime Markets
            var primeMarketIdentifier = "";

            if (vendor.Id == 67)
            {
                var zipCode = GetZipCode(mailMessage.Body, "/adf/prospect/customer/contact/address/postalcode");

                var bPrimeMarket = false;

                if (zipCode != null && zipCode != "")
                {
                    bPrimeMarket = _routeEmail.CheckForPrimeMarket(zipCode);
                }

                if (bPrimeMarket)
                {
                    primeMarketIdentifier = " - PM";
                }
            }


            //Vendor Code
            var codeXpaths = _routeEmail.GetLeadVendorCodeXPath();
            string vendorCode = string.Empty;
            if (vendor != null && vendor.Code != null)
            {
                vendorCode = GetVendorCode(mailMessage.Body, vendor.Code);
            }
            else
            {
                foreach (var xpath in codeXpaths)
                {
                    vendorCode = GetVendorCode(mailMessage.Body, xpath.Xml);
                    if (!String.IsNullOrEmpty(vendorCode)) // || vendorCode != "0"
                    {
                        //farging Kluge - Edmunds TYV try
                        if (vendorCode == "0" || vendorCode == "callsource")
                        {
                            vendorCode = GetVendorCode(mailMessage.Body, "/adf/prospect/vendor/vendorname");
                        }
                        break;
                    }

                }
            }

            // THIS IS FOR KBB PURCHASE A VEHICLE
            if (vendorCode.IndexOf('[') > 0)
            {
                vendorCode = vendorCode.Substring(0, vendorCode.IndexOf('[')).Trim();
            }

            var detailLabel = "";

            //Is this a phone lead
            //adf/prospect/id[@sequence='2']
            var prospectChannel = "";
            prospectChannel = GetVendorCode(mailMessage.Body, "/adf/prospect/id[@sequence='2']");

            if (String.IsNullOrEmpty(prospectChannel))
            {
                prospectChannel = "";
            }
            else if (prospectChannel.ToLower() == "phone")
            {
                detailLabel = " Phone";
            }

            var bEPricerLead = false;
            var bEPriceEmail = false;

            var bDontForwardLead = false;


            //DO THIS IF IT IS DEALER.COM ONLY -- FOR NOW
            if (vendor.Id == 34)
            {
                var proService = GetProviderService(mailMessage.Body, "/adf/prospect/provider/service");
                if (!String.IsNullOrEmpty(proService))
                {
                    detailLabel = " " + proService;
                }

                // MORE FUN STUFF WITH DEALER.COM, PHONE LEADS AHOY!

                if (proService == "Website - Sales")
                {
                    detailLabel = " Phone";
                    bDontForwardLead = true;
                }
                else if (proService == "Website - Service")
                {
                    detailLabel = " Service Phone";
                    bDontForwardLead = true;
                }
                else if (proService == "Website - Parts")
                {
                    detailLabel = " Parts Phone";
                    bDontForwardLead = true;
                }

                if (proService == "Mobile Site - Sales")
                {
                    detailLabel = " Phone";
                    bDontForwardLead = true;
                }
                else if (proService == "Mobile Site - Service")
                {
                    detailLabel = " Service Phone";
                    bDontForwardLead = true;
                }
                else if (proService == "Mobile Site - Parts")
                {
                    detailLabel = " Parts Phone";
                    bDontForwardLead = true;
                }

            }


            if ((make.ToLower() == "none"))
            {
                make = "Used";
            }

            // || make.ToLower() == "subaru"
                if ((make.ToLower() == "toyota" || make.ToLower() == "subaru" || make.ToLower() == "hyundai" || make.ToLower() == "mazda" || make.ToLower() == "nissan" || make.ToLower() == "genesis" || make.ToLower() == "volkswagen" || make.ToLower() == "cadillac") && status.ToLower() == "new")
            {
                // Provider Service, check to see if the source contains E Pricer, if it does, append to Provider Name...
                var providerService = GetProviderService(mailMessage.Body, "/adf/prospect/provider/service");
                if (!String.IsNullOrEmpty(providerService))
                {
                    if (providerService.Contains("E Pricer"))
                    {
                        detailLabel = " E Pricer";
                        bEPricerLead = true;
                    }
                }
                else
                {
                    // This is for MAZDA
                    providerService = GetProviderService(mailMessage.Body, "/adf/prospect/provider/name");

                    if (!String.IsNullOrEmpty(providerService))
                    {
                        if (providerService.Contains("ePrice"))
                        {
                            detailLabel = " E Pricer";
                            bEPricerLead = true;
                        }
                    }
                    else
                    {
                        providerService = "";
                        detailLabel = "";
                        bEPricerLead = false;
                    }
                }
            }

            //if ((make.ToLower() == "toyota" || make.ToLower() == "nissan" || make.ToLower() == "cadillac" || make.ToLower() == "subaru" || make.ToLower() == "genesis" || make.ToLower() == "mazda") && status.ToLower() == "new" && vendor.Id == 4)

            //
            if ((make.ToLower() == "toyota" || make.ToLower() == "subaru" || make.ToLower() == "hyundai" || make.ToLower() == "mazda" || make.ToLower() == "nissan" || make.ToLower() == "genesis" || make.ToLower() == "volkswagen" || make.ToLower() == "cadillac") && status.ToLower() == "new" && vendor.Id == 4)
            {
                bEPricerLead = false; // SET THIS TO TRUE IF WE EVER WANT TO SEND EMAILS AGAIN FROM FITZMALL
            }

            //if ((make.ToLower() == "toyota" || make.ToLower() == "nissan" || make.ToLower() == "mazda" || make.ToLower() == "volkswagen" || make.ToLower() == "cadillac"))
            //{
            //    bEPriceEmail = true;
            //}

            //Lead Route to Forward Email
            var leadRoute = new List<LeadRoute>();


            if (vendor.Id == 1076) // all hispanic leads go to FBN only after 8/18/22 re Harold Redden/Eyal Toueg
            {

                vehicleStockNumberForLookup = ""; // make is stock # for 1076 Hispanic Media
                stockNumber = "";

            }

            //Lookup Vehicle
            CarDetails car = new CarDetails();
            if (!String.IsNullOrEmpty(vehicleStockNumberForLookup))
            {
                car = _routeEmail.GetVehicleDetails(vehicleStockNumberForLookup);
            }

            // CHECK TO SEE IF E-PRICER, IF SO, SEND ADDITIONAL EMAIL TO CUSTOMER...
            if (bEPricerLead)
            {
                if (car != null && car.Loc != null)
                {
                    //Now create new email and send...
                    var customerEmail = "statlerc@fitzmall.com";
                    var customerName = "";

                    customerEmail = GetCustomerEmail(mailMessage.Body, "/adf/prospect/customer/contact/email");
                    customerName = GetCustomerFirstName(mailMessage.Body, "/adf/prospect/customer/contact/name[@part='first']");

                    MailMessage autoResponderMessage = new MailMessage();

                    autoResponderMessage.Subject = "Right Dealership, Right Price!";
                    autoResponderMessage.IsBodyHtml = true;
                    autoResponderMessage.Body = _routeEmail.FormatAutoResponderEmail(car, customerName, vendor.Id, bEPriceEmail);

                    //SendResponderEmail(autoResponderMessage, car.Loc, customerEmail);

                    var customerComments = "";
                    if (bEPriceEmail)
                    {
                        customerComments = "   (E Pricer Lead - Customer Emailed - Stock Number: {0} - ePrice: {1} - Date/Time: {2}   )";
                    }
                    else
                    {
                        customerComments = "   (E Pricer Lead - Customer Emailed - Stock Number: {0} - FitzWay Low Price: {1} - Date/Time: {2}   )";
                    }

                    System.Globalization.NumberFormatInfo nfi = new System.Globalization.CultureInfo("en-US", false).NumberFormat;
                    nfi.CurrencyDecimalDigits = 0;
                    nfi.CurrencySymbol = "$";

                    if (bEPriceEmail)
                    {
                        var ePrice = car.InternetPrice - car.Freight;
                        customerComments = String.Format(customerComments, car.StockNumber, ePrice.ToString("C", nfi), DateTime.Now);
                    }
                    else
                    {
                        customerComments = String.Format(customerComments, car.StockNumber, car.InternetPrice.ToString("C", nfi), DateTime.Now);
                    }

                    try
                    {
                        SendResponderEmail(autoResponderMessage, car.Loc, customerEmail, car.MakeName);
                    }
                    catch (Exception ex)
                    {
                        customerComments = "   (Email Failed to send to customer, reason: " + ex.Message + "   )";
                        retval = false;
                    }

                    mailMessage.Body = ChangeCustomerComments(mailMessage.Body, "/adf/prospect/customer/comments", customerComments);

                }

            }

            //Run the rules & select one
            var route = new LeadRoute { ForwardEmail = "No Route" };
            if (bDontForwardLead == false)
            {

                if (!String.IsNullOrEmpty(stockNumber))
                {

                    if (car != null && car.Loc != null && car.Mall != "CL")
                    {
                        leadRoute = _routeEmail.GetLeadRouteStk(vendor.Id, vendorCode, car.Loc, car.Mall);


                        foreach (var rt in leadRoute)
                        {
                            bool rule = CheckingRule(mailMessage.Body, rt.XmlRule);
                            if (!rule) continue;
                            route = rt;
                            break;
                        }
                        if (!String.IsNullOrEmpty(car.Wholesale) && car.Wholesale == "Y")
                        {
                            route.Loc = String.Format("{0}~", car.Loc); //wholesale vehicles add * to end of loc code
                        }
                        else
                        {
                            route.Loc = car.Loc; //override route with car loc                        
                        }
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(vendorCode) && !String.IsNullOrEmpty(make))
                        {


                            leadRoute = _routeEmail.GetLeadRouteNoStk(vendor.Id, vendorCode, make.ToLower());
                            foreach (var rt in leadRoute)
                            {
                                bool rule = CheckingRule(mailMessage.Body, rt.XmlRule);
                                if (!rule) continue;
                                route = rt;
                                break;
                            }
                        }
                        if (!String.IsNullOrEmpty(vendorCode) && String.IsNullOrEmpty(make))
                        {

                            leadRoute = _routeEmail.GetLeadRouteNoStk(vendor.Id, vendorCode);
                            foreach (var rt in leadRoute)
                            {
                                bool rule = CheckingRule(mailMessage.Body, rt.XmlRule);
                                if (!rule) continue;
                                route = rt;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    leadRoute = _routeEmail.GetLeadRouteNoStk(vendor.Id, vendorCode, make);

                    foreach (var rt in leadRoute)
                    {
                        bool rule = CheckingRule(mailMessage.Body, rt.XmlRule);
                        if (!rule) continue;
                        route = rt;
                        break;
                    }
                }

                string vendorName = GetVendorCode(mailMessage.Body, "/adf/prospect/vendor/vendorname");



                if (vendorName == "Fitzgerald Clearwater Used Cars")
                {
                    route.Loc = "COC";
                    route.Mall = "CL";
                    route.ForwardEmail = _routeEmail.GetLeadCrmEmail(route.Loc).Email;
                }

                if (vendorName == "Fitzgerald Clearwater Outlet Center")
                {
                    route.Loc = "COC";
                    route.Mall = "CL";
                    route.ForwardEmail = _routeEmail.GetLeadCrmEmail(route.Loc).Email;
                }


                if (vendorName == "Fitzgerald Auto Mall Clearwater Used Car Outlet")
                {
                    route.Loc = "COC";
                    route.Mall = "CL";
                    route.ForwardEmail = _routeEmail.GetLeadCrmEmail(route.Loc).Email;
                }

                // Addison re Upstart: 'Except for Florida, the lead is sent to the dealer site used to locate the vehicle, not where the vehicle resides '

                    if (vendorName.Contains("Hyundai") && vendorName.Contains("Clearwater"))
                    {
                        route.Loc = "CHY";
                        route.Mall = "CL";
                        route.ForwardEmail = _routeEmail.GetLeadCrmEmail(route.Loc).Email;
                    }

                if (vendorName.Contains("Mazda") && vendorName.Contains("Annapolis"))
                {
                    route.Loc = "FMM";
                    route.Mall = "AW";
                    route.ForwardEmail = _routeEmail.GetLeadCrmEmail(route.Loc).Email;
                }

                if (vendorName.Contains("Mazda") && vendorName.Contains("Frederick"))
                {
                    route.Loc = "FAM";
                    route.Mall = "FD";
                    route.ForwardEmail = _routeEmail.GetLeadCrmEmail(route.Loc).Email;
                }

                if (vendorName.Contains("Mitsubishi") && vendorName.Contains("Annapolis"))
                    {
                        route.Loc = "FMM";
                        route.Mall = "AW";
                        route.ForwardEmail = _routeEmail.GetLeadCrmEmail(route.Loc).Email;
                    }

                    if (vendorName.Contains("Volkswagen") && vendorName.Contains("Annapolis"))
                    {
                        route.Loc = "FOC";
                        route.Mall = "AW";
                        route.ForwardEmail = _routeEmail.GetLeadCrmEmail(route.Loc).Email;
                    }

                    if (vendorName.Contains("Chevrolet") && vendorName.Contains("Frederick"))
                    {
                        route.Loc = "FCG";
                        route.Mall = "FD";
                        route.ForwardEmail = _routeEmail.GetLeadCrmEmail(route.Loc).Email;
                    }


                    if (vendorName.Contains("Cadillac") && vendorName.Contains("Frederick"))
                    {
                        route.Loc = "FCG";
                        route.Mall = "FD";
                        route.ForwardEmail = _routeEmail.GetLeadCrmEmail(route.Loc).Email;
                    }

                    if (vendorName.Contains("Cadillac") && vendorName.Contains("Annapolis"))
                    {
                        route.Loc = "FOC";
                        route.Mall = "AW";
                        route.ForwardEmail = _routeEmail.GetLeadCrmEmail(route.Loc).Email;
                    }

                    if (vendorName.Contains("Subaru") && vendorName.Contains("Clearwater"))
                    {
                        route.Loc = "CSS";
                        route.Mall = "CL";
                        route.ForwardEmail = _routeEmail.GetLeadCrmEmail(route.Loc).Email;
                    }

                    if (vendorName.Contains("Subaru") && vendorName.Contains("Rockville"))
                    {
                        route.Loc = "FBS";
                        route.Mall = "WF";
                        route.ForwardEmail = _routeEmail.GetLeadCrmEmail(route.Loc).Email;
                    }

                    if (vendorName.Contains("Subaru") && vendorName.Contains("Gaithersburg"))
                    {
                        route.Loc = "LFO";
                        route.Mall = "GA";
                        route.ForwardEmail = _routeEmail.GetLeadCrmEmail(route.Loc).Email;
                    }

                    if (vendorName.Contains("Toyota") && vendorName.Contains("Gaithersburg"))
                    {
                        route.Loc = "LFT";
                        route.Mall = "GA";
                        route.ForwardEmail = _routeEmail.GetLeadCrmEmail(route.Loc).Email;
                    }

                // Cars.com error fix
                if (vendor.Id == 1 && vendorName.Contains("Wheaton"))
                {
                    route.Loc = "WDC";
                    route.Mall = "WH";
                    route.ForwardEmail = _routeEmail.GetLeadCrmEmail(route.Loc).Email;
                }

                if (route.Name != null)
                {
                    string ls = String.Format("{0}", route.LeadSourceAddOn ?? route.Loc);

                    var editedXml = ChangeLeadSource(mailMessage.Body, route.XmlLeadSource ?? "/adf/prospect/provider/name", detailLabel + String.Format(" - {0}", ls) + primeMarketIdentifier);

                    //If their is no stock # then use default loc code in Route
                    if (!String.IsNullOrEmpty(editedXml))
                    {
                        mailMessage.Body = editedXml;
                    }
                    //Route Email
                    if (isSpam)
                    {
                        //notify me
                        mailMessage.Subject = String.Format("Lead Router SPAM ALERT:{0}", mailMessage.Subject);
                        RouteEmail(mailMessage, "statlerc@fitzmall.com,morrisonk@fitzmall.com");
                    }
                    else
                    {

                        //DO THIS IF IT IS KBB ONLY (70) KBB ICO
                        // "All KBB leads with no vehicle listed (to purchase) and trade-in info only should be directed to the Toyota store"

                        if (vendor.Id == 1080)
                        {
                            if (interest == "trade-in")
                            {
                                route.Loc = "LFT";
                                route.Mall = "GA";
                                route.ForwardEmail = _routeEmail.GetLeadCrmEmail(route.Loc).Email;
                            }
                        }


                        if (vendor.Id == 5)
                        {
                            var providerService = GetProviderService(mailMessage.Body, "/adf/prospect/provider/service");
                            if (!String.IsNullOrEmpty(providerService))
                            {
                                if (providerService.Contains("Edmunds Trade-In"))
                                {
                                    if (vendorName.Contains("Mazda") && vendorName.Contains("Annapolis"))
                                    {
                                        route.Loc = "FMM";
                                        route.Mall = "AW";
                                        route.ForwardEmail = _routeEmail.GetLeadCrmEmail(route.Loc).Email;
                                    }

                                    if (vendorName.Contains("Volkswagen"))
                                    {
                                        route.Loc = "FOC";
                                        route.Mall = "AW";
                                        route.ForwardEmail = _routeEmail.GetLeadCrmEmail(route.Loc).Email;
                                    }


                                }
                            }
                        }

                        if (vendor.Id == 2)  // AUTOTRADER leads to where the car is- re Harold Redden explicit instructions
                        {
                            if (stockNumber == "" | stockNumber == null)
                            {
                                if (route.Loc == "LFO")
                                {
                                    route.Loc = "LFT";
                                    route.Mall = "GA";
                                    route.ForwardEmail = _routeEmail.GetLeadCrmEmail(route.Loc).Email;
                                }
                            }
                            else
                            {
                                vehicleStockNumberForLookup = stockNumber; // find the location of car
                                CarDetails car2 = new CarDetails();
                                car2 = _routeEmail.GetVehicleDetails(vehicleStockNumberForLookup);

                                if (car2 != null)
                                {
                                    if (car2.Loc == "" | car2.Loc == null)
                                    {
                                        route.Loc = "LFT";
                                        route.Mall = "GA";
                                        route.ForwardEmail = _routeEmail.GetLeadCrmEmail(route.Loc).Email;
                                    }
                                    else
                                    {
                                        // LFT and LFM are the troublesome ones- must go where car is!
                                        if (route.Loc == "LFT" | route.Loc == "LFM")
                                        {
                                            route.Loc = car2.Loc;
                                            route.ForwardEmail = _routeEmail.GetLeadCrmEmail(route.Loc).Email;
                                        }
                                    }
                                }
                            }


                        }
                        RouteEmail(mailMessage, route);

                        if (make.ToLower() == "hyundai" && status.ToLower() != "used")
                        {
                            var autoHookAddress = "";

                            if (route.Loc == "LFO")
                            {
                                autoHookAddress = "fitzgeraldslakeforersthyundaileads@driveauthook.com";
                            }
                            else if (route.Loc == "CHY")
                            {
                                autoHookAddress = "fitzgeraldscountrysidehyundaileads@driveautohook.com";
                            }
                            else if (route.Loc == "CDO")
                            {
                                autoHookAddress = "fitzgeraldhyundaileads@driveautohook.com";
                            }

                            RouteEmail(mailMessage, autoHookAddress);

                        }

                    }
                }
                else
                {
                    //Forward Emails with no route
                    //Ignore these email addresses
                    if (GetIgnoredEmailList().All(s => s.ToLower() != mailMessage.To.ToString().ToLower()))
                    {
                        //If no route but ADF checks out then forward to IDD
                        if (!String.IsNullOrEmpty(stockNumber) || !String.IsNullOrEmpty(make))
                        {
                            //Add Loc Code to end of subject line
                            if (car != null && car.Loc != null) 
                            {
                                //Let's create a route and send it through again, must have a vendor
                                if (createRoute == null && vendor.Id > 0 && !String.IsNullOrEmpty(vendorCode))
                                {
                                    //get crm forwarding email
                                    LeadCrmEmail crmEmail = null;
                                    try
                                    {
                                        crmEmail = _routeEmail.GetLeadCrmEmail(car.Loc);
                                    }
                                    catch (Exception ex)
                                    {
                                        errorMailShort = "Lead Router Error Occured: Route not inserted";
                                        errorMailLong = String.Format("Message: FROM:{0} TO:{1} SUBJECT:{2}, DATETIME:{3}\n Error:{4}",
                                                orgMailMessage.From, orgMailMessage.To[0], orgMailMessage.Subject,
                                                orgMailMessage.Date(), ex.Message);
                                        retval = false;

                                        SendAlert(errorMailShort, errorMailLong
                                        );
                                        var logEmail = LogMailError(mailMessage, route, errorMailShort, errorMailLong);

                                    }
                                    finally
                                    {
                                        if (crmEmail != null)
                                        {
                                            //insert route
                                            var lr = new LeadRoute();
                                            lr.Name = String.Format("{0}-{1}-{2}", vendor.Name, car.Loc, car.Mall);
                                            lr.LeadVendorId = vendor.Id;
                                            lr.VendorCode = vendorCode;
                                            lr.ForwardEmail = crmEmail.Email;
                                            lr.IsActive = true;
                                            lr.Loc = car.Loc;
                                            lr.Mall = car.Mall;
                                            lr.Make = "Used";
                                            lr.CreateDate = DateTime.Now;
                                            try
                                            {
                                                _routeEmail.InsertIntoRoute(lr);
                                                SendAlert("New Route Added",
                                                    String.Format("Route: {0} forwards to email ->{1}", lr.Name,
                                                        lr.ForwardEmail));
                                            }
                                            catch (Exception ex)
                                            {
                                                retval = false;

                                                errorMailShort = "Lead Router Error Occured: Route not inserted";
                                                errorMailLong = String.Format("Message: FROM:{0} TO:{1} SUBJECT:{2}, DATETIME:{3}\n Error:{4}",
                                                        orgMailMessage.From, orgMailMessage.To[0], orgMailMessage.Subject,
                                                        orgMailMessage.Date(), ex.Message);

                                                SendAlert(errorMailShort, errorMailLong
                                                    );
                                                var logEmail = LogMailError(mailMessage, route, errorMailShort, errorMailLong);
                                            }
                                            finally
                                            {
                                                //route again                                        
                                                if (CallRouteProcessAdf2(orgMailMessage, "create") == false)
                                                {
                                                    retval = false;
                                                };
                                            }
                                        }
                                    }

                                }
                                if (route.Loc == null || route.Loc == "")
                                {
                                    mailMessage.Subject = String.Format("Lead Router FW:{0}-{1}", mailMessage.Subject, car.Loc);
                                    RouteEmail(mailMessage, "statlerc@fitzmall.com, morrisonk@fitzmall.com, burroughsd@fitzmall.com");
                                }
                            }
                            else
                            {
                                if (route.Loc == null || route.Loc == "")
                                {
                                    mailMessage.Subject = String.Format("Lead Router Car or CarLoc not found FW:{0}", mailMessage.Subject);
                                    RouteEmail(mailMessage, "statlerc@fitzmall.com, morrisonk@fitzmall.com, burroughsd@fitzmall.com");
                                }
                            }

                        }
                        else
                        {
                            mailMessage.Subject = String.Format("Lead Router Stock or Make Issue FW2:{0}", mailMessage.Subject);
                            RouteEmail(mailMessage, "statlerc@fitzmall.com, morrisonk@fitzmall.com, burroughsd@fitzmall.com");
                        }


                    }

                }

            }
            else
            {

            }
            var log = LogEmail(mailMessage, route);
            return (retval);
        }

        private static void CheckInboxStatus()
        {
            var inboxCount = _client.Search(SearchCondition.Unseen(), _client.DefaultMailbox).Count();
            var info = _client.GetMailboxInfo();
            if (info.Unread > 5)
            {
                SendAlert("Leader Router Inbox Count", String.Format("ALERT: {0} emails unprocessed in Router", inboxCount));
            }
        }

        // filters control characters but allows only properly-formed surrogate sequences
        private static readonly Regex _invalidXMLChars = new Regex(@"(?<![\uD800-\uDBFF])[\uDC00-\uDFFF]|[\uD800-\uDBFF](?![\uDC00-\uDFFF])|[\x00-\x08\x0B\x0C\x0E-\x1F\x7F-\x9F\uFEFF\uFFFE\uFFFF]", RegexOptions.Compiled);

        private static string CorrectHiddenCharacters(string text)
        {
            if (string.IsNullOrEmpty(text)) return "";
            return _invalidXMLChars.Replace(text, "");
        }


        //Log Email
        private static LeadRouteLog LogEmailDuplicateRoute(MailMessage mailMessage)
        {
            var log = new LeadRouteLog();
            log.MailId = String.Empty;
            log.FromAddress = mailMessage.From.Address;
            log.ToAddress = "Duplicate";
            log.Subject = mailMessage.Subject;
            log.Body = mailMessage.Body;
            log.Status = string.Empty;
            log.LeadVendorId = 0;
            log.LeadRouteId = 0;

            _routeEmail.InsertIntoLog(log);
            return log;
        }


        //Log Email
        private static LeadRouteLog LogEmail(MailMessage mailMessage, LeadRoute leadRoute)
        {
            var log = new LeadRouteLog();
            log.MailId = String.Empty;
            log.FromAddress = mailMessage.From.Address;
            log.ToAddress = leadRoute != null ? leadRoute.ForwardEmail : "No Route";
            log.Subject = mailMessage.Subject;
            log.Body = mailMessage.Body;
            log.Status = string.Empty;
            log.LeadVendorId = leadRoute != null ? leadRoute.LeadVendorId : 0;
            log.LeadRouteId = leadRoute != null ? leadRoute.Id : 0;

            _routeEmail.InsertIntoLog(log);
            return log;
        }

        private static MailError LogMailError(MailMessage mailMessage, LeadRoute leadRoute, string ErrTitle, string ErrMsg)
        {
            var log = new MailError();
            log.MailId = String.Empty;
            log.FromAddress = mailMessage.From.Address;
            log.ToAddress = leadRoute != null ? leadRoute.ForwardEmail : "No Route";
            log.Subject = mailMessage.Subject;
            log.Body = mailMessage.Body;
            log.Status = string.Empty;
            log.LeadVendorId = leadRoute != null ? leadRoute.LeadVendorId : 0;
            log.LeadRouteId = leadRoute != null ? leadRoute.Id : 0;

            _routeEmail.InsertIntoMailErrorLog(log);
            return log;
        }


        private static string CorrectFormattingIssues(string body)
        {
            //find bad character & correct bad character
            try
            {
                string result = ReplaceString(body, "&a", "p;", "&amp;");
                result = result.Replace(" & ", " &amp; ");
                return result;
            }
            catch (Exception ex)
            {
                SendAlert("Lead Router Error", String.Format("Message: {0}", ex.Message));
                Console.WriteLine(ex.Message);
            }
            return body;
        }

        private static string CorrectLeadingWhiteSpaceIssues(string body)
        {
            try
            {
                string xml = body.Replace("\n", "");
                xml = xml.Replace("\r", "");
                var reg = new Regex(@">\s*<");
                var result = reg.Replace(xml, "><");
                return result.TrimStart();
            }
            catch (Exception ex)
            {
                SendAlert("Lead Router Error", String.Format("Message: {0}", ex.Message));
                Console.WriteLine(ex.Message);

            }
            return body;
        }

        private static string WrapCommentElementInCdata(string body)
        {
            try
            {
                //already there ignore
                if (body.Contains("<comments><![CDATA["))
                {
                    return body;
                }
                string result = string.Empty;
                string xml = body.Replace("<comments>", "<comments><![CDATA[");
                xml = xml.Replace("</comments>", "]]></comments>");
                result = xml;

                return result;
            }
            catch (Exception ex)
            {
                SendAlert("Lead Router Error", String.Format("Message: {0}", ex.Message));
                Console.WriteLine(ex.Message);
            }
            return body;

        }

        public static string ReplaceString(string strSrc, string strStart, string strEnd, string replace)
        {
            string newStr = strSrc;
            if (strSrc.Contains(strStart) && strSrc.Contains(strEnd))
            {
                int Start = strSrc.IndexOf(strStart, 0);
                int End = strSrc.IndexOf(strEnd, Start) + strStart.Length;
                var theStr = strSrc.Substring(Start, End - Start);
                if ((End - Start) <= 6)
                {
                    newStr = ReplaceAt(strSrc, Start, End - Start, replace);
                }
                return newStr;
            }
            return strSrc;
        }

        public static string ReplaceAt(string str, int index, int length, string replace)
        {
            return str.Remove(index, Math.Min(length, str.Length - index))
                    .Insert(index, replace);
        }

        private static List<string> GetIgnoredEmailList()
        {
            var list = new List<string>();
            var configList = ConfigurationManager.AppSettings["ignoreEmails"];
            list = configList.Split(',').ToList();
            return list;
        }

        private static string GetStockNumber(string body, string xpath)
        {
            Console.WriteLine("Getting Stock Number...");
            string stk = string.Empty;
            //find stock
            stk = _adf.ReadXmlNode(body, xpath);
            if (!String.IsNullOrEmpty(stk))
            {
                Console.WriteLine("Found Stk#: {0}", stk);
            }
            return stk;
        }

        private static string GetCustomerEmail(string body, string xpath)
        {
            Console.WriteLine("Getting Customer Email...");
            string email = string.Empty;
            //find stock
            email = _adf.ReadXmlNode(body, xpath);
            if (!String.IsNullOrEmpty(email))
            {
                Console.WriteLine("Found Email: {0}", email);
            }
            return email;
        }

        private static string GetCustomerFirstName(string body, string xpath)
        {
            Console.WriteLine("Getting Customer First Name...");
            string name = string.Empty;
            //find stock
            name = _adf.ReadXmlNode(body, xpath);
            if (!String.IsNullOrEmpty(name))
            {
                Console.WriteLine("Found Name: {0}", name);
            }
            return name;
        }

        private static bool SpamFilter(string body, string xpath, string regex, bool inverse = false)
        {
            Console.WriteLine("Spam Filter...");
            bool spam = false;
            //find firstname
            var node = _adf.ReadXmlNode(body, xpath);
            if (!String.IsNullOrEmpty(node))
            {
                if (inverse)
                {
                    spam = !Regex.Match(node, regex).Success;
                }
                else
                {
                    spam = Regex.Match(node, regex).Success;
                }
            }
            return spam;
        }

        private static bool CheckingRule(string body, string xpath)
        {
            Console.WriteLine("Checking Rule...");
            bool node = false;
            if (String.IsNullOrEmpty(xpath))
            {
                return true;
            }
            //find stock
            node = _adf.Contains(body, xpath);
            Console.WriteLine("Found Rule Condition: {0}", node);

            return node;
        }

        private static string GetVendorCode(string body, string xpath)
        {
            Console.WriteLine("Getting Vendor Code...");
            string node = string.Empty;
            //find stock
            node = _adf.ReadXmlNode(body, xpath);
            if (!String.IsNullOrEmpty(node))
            {
                Console.WriteLine("Found Vendor Code: {0}", node);
            }
            return node;
        }

        private static string GetTrafficSource(string body, string xpath, TrafficSource trafficSources)
        {
            Console.WriteLine("Getting Traffic Source...");
            string node = string.Empty;
            //find stock
            node = _adf.ReadXmlNode(body, xpath);
            if (!String.IsNullOrEmpty(node))
            {
                Console.WriteLine("Found Traffic Source: {0}", node);
            }
            else
            {
                return node;
            }

            string match = "other";
            foreach (var src in trafficSources.Sources)
            {
                bool contains = node.Contains(src.key);
                if (contains)
                {
                    match = src.value;
                    break;
                }
            }

            return match;
        }

        private static string GetMake(string body, string xpath)
        {
            Console.WriteLine("Getting Make...");
            string node = string.Empty;
            //find stock
            node = _adf.ReadXmlNode(body, xpath);
            if (!String.IsNullOrEmpty(node))
            {
                Console.WriteLine("Found Make: {0}", node);
            }
            return node;
        }

        private static string GetZipCode(string body, string xpath)
        {
            Console.WriteLine("Getting ZipCode...");
            string node = string.Empty;
            //find stock
            node = _adf.ReadXmlNode(body, xpath);
            if (!String.IsNullOrEmpty(node))
            {
                Console.WriteLine("Found ZipCode: {0}", node);
            }
            return node;
        }

        private static string GetProviderService(string body, string xpath)
        {
            Console.WriteLine("Getting Provider Service...");
            string node = string.Empty;
            //find stock
            node = _adf.ReadXmlNode(body, xpath);
            if (!String.IsNullOrEmpty(node))
            {
                Console.WriteLine("Found Provider Service: {0}", node);
            }
            return node;
        }

        private static string GetInterest(string body, string xpath)
        {
            Console.WriteLine("Getting interest (trade-in?)...");
            string node = string.Empty;
            //find stock
            node = _adf.ReadXmlNode(body, xpath);
            if (!String.IsNullOrEmpty(node))
            {
                Console.WriteLine("Found interest (trade-in?): {0}", node);
            }
            return node;
        }

        private static string ChangeLeadSource(string body, string xpath, string attrVal)
        {
            Console.WriteLine("Changing Lead Source...");
            string xml = string.Empty;
            //find stock
            xml = _adf.AppendAttribute(body, xpath, attrVal);

            return xml;
        }
        public static string ChangeCustomerComments(string body, string xpath, string attrVal)
        {
            Console.WriteLine("Changing Customer Comments...");
            string xml = string.Empty;
            //find stock
            xml = _adf.AppendAttribute(body, xpath, attrVal);

            return xml;
        }
        private static void RouteEmail(MailMessage mailMessage, string forwardEmailAddr, string forwardEmailAddr2 = null)
        {
            Console.WriteLine("Forwarding Email with NO Route...");
            LeadRoute route = new LeadRoute();
            route.ForwardEmail = forwardEmailAddr;
            RouteEmail(mailMessage, route);
        }

        public static void RouteEmail(MailMessage mail, LeadRoute route)
        {
            Console.WriteLine("Forwarding Email...");
            using (var client = new SmtpClient())
            {
                var forward = new MailMessage();
                try
                {

                    forward.From = new MailAddress("leads@fitzmall.com");
                    mail.To.Clear();
                    forward.To.Add(route.ForwardEmail);
                    mail.CC.Clear();
                    forward.Subject = mail.Subject;
                    forward.Body = mail.Body;
                    forward.IsBodyHtml = false;

                    client.Send(forward);
                    Console.WriteLine("Email was sent...");
                }
                catch (Exception ex)
                {
                    string ForwardTo = "";
                    if (forward.To.Count > 0)
                    {
                        ForwardTo = forward.To[0].Address;
                    }
                    SendAlert("Email Not Forwarded", String.Format("Message: FROM:{0} TO:{1} SUBJECT:{2}, DATETIME:{3}{5} Error:{4}", mail.From.Address, ForwardTo, mail.Subject, mail.Date(), ex.Message, Environment.NewLine));
                    var log = LogEmail(mail, route);
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public static void SendResponderEmail(MailMessage mail, string location, string customerEmail, string make)
        {
            Console.WriteLine("Sending Auto Responder Email...");
            using (var client = new SmtpClient())
            {
                var forward = new MailMessage();
                try
                {
                    var fromAddress = new MailAddress("idd04@fitzmall.com", "Fitzgerald Auto Malls");
                    switch (location.ToUpper())
                    {
                        case "LFO":
                            if (make.ToUpper() == "GENESIS")
                            {
                                fromAddress = new MailAddress("fitzgeraldlakeforesthysubused@fitzmall.com", "Fitzgerald Genesis of Gaithersburg");
                            }
                            else if (make.ToUpper() == "HYUNDAI")
                            {
                                fromAddress = new MailAddress("fitzgeraldlakeforesthysubused@fitzmall.com", "Fitzgerald Hyundai Gaithersburg");
                            }
                            else
                            {
                                fromAddress = new MailAddress("fitzgeraldlakeforesthysubused@fitzmall.com", "Fitzgerald Subaru Gaithersburg");
                            }
                            break;
                        case "LFT":
                            fromAddress = new MailAddress("fitzgeraldlakeforesttoyota@fitzmall.com", "Fitzgerald Toyota Gaithersburg");
                            break;
                        case "FMM":
                            if (make.ToUpper() == "MAZDA")
                            {
                                fromAddress = new MailAddress("fitzgeraldannapolis@fitzmall.com", "Fitzgerald Mazda Annapolis");
                            }
                            else
                            {
                                fromAddress = new MailAddress("fitzgeraldannapolis@fitzmall.com", "Fitzgerald Mitsubishi Annapolis");
                            }
                            break;
                        case "FOC":
                            if (make.ToUpper() == "CADILLAC")
                            {
                                fromAddress = new MailAddress("fitzgeraldannapolis@fitzmall.com", "Fitzgerald Cadillac Annapolis");
                            }
                            else
                            {
                                fromAddress = new MailAddress("fitzgeraldannapolis@fitzmall.com", "Fitzgerald Volkswagen Annapolis");
                            }

                            break;
                        case "FHG":
                            if (make.ToUpper() == "CADILLAC")
                            {
                                fromAddress = new MailAddress("fhgsales@fitzmall.com", "Fitzgerald Cadillac Hagerstown");
                            }
                            break;
                        case "FTO":
                            if (make.ToUpper() == "TOYOTA")
                            {
                                fromAddress = new MailAddress("fitzgeraldchambersburgtoyotanissan@fitzmall.com", "Fitzgerald Toyota Chambersburg ");
                            }
                            else
                            {
                                fromAddress = new MailAddress("fitzgeraldchambersburgtoyotanissan@fitzmall.com", "Fitzgerald Nissan Chambersburg ");
                            }
                            break;
                        case "CHY":
                            if (make.ToUpper() == "GENESIS")
                            {
                                fromAddress = new MailAddress("fitzmallclearwater@fitzmall.com", "Fitzgerald Genesis of Clearwater");
                            }
                            else
                            {
                                fromAddress = new MailAddress("fitzmallclearwater@fitzmall.com", "Fitzgerald Hyundai Clearwater");
                            }
                            break;
                        case "CSS":
                            fromAddress = new MailAddress("fitzmallclearwater@fitzmall.com", "Fitzgerald Subaru Clearwater");
                            break;
                        case "FCG":
                            fromAddress = new MailAddress("fitzgeraldfrederick@fitzmall.com", "Fitzgerald AutoMall Frederick");
                            break;
                        case "CDO":
                            if (make.ToUpper() == "GENESIS")
                            {
                                fromAddress = new MailAddress("fitzgeraldwhiteflinthyundai@fitzmall.com", "Fitzgerald Genesis of Rockville");
                            }
                            else
                            {
                                fromAddress = new MailAddress("fitzgeraldwhiteflinthyundai@fitzmall.com", "Fitzgerald Hyundai Rockville");
                            }
                            break;
                        case "FBS":
                            fromAddress = new MailAddress("fitzgeraldwhiteflint@fitzmall.com", "Fitzgerald Subaru Rockville");
                            break;
                        default:
                            fromAddress = new MailAddress("idd04@fitzmall.com", "Fitzgerald Auto Malls");
                            break;

                    }

                    forward.From = fromAddress;
                    mail.To.Clear();
                    forward.To.Add(customerEmail);
                    mail.CC.Clear();
                    forward.Subject = mail.Subject;
                    forward.Body = mail.Body;
                    forward.IsBodyHtml = true;
                    client.Send(forward);
                    Console.WriteLine("Email was sent...");
                }
                catch (Exception ex)
                {
                    SendAlert("Email Not Forwarded", String.Format("Message: FROM:{0} TO:{1} SUBJECT:{2}, DATETIME:{3}{5} Error:{4}", mail.From.Address, forward.To[0].Address, mail.Subject, mail.Date(), ex.Message, Environment.NewLine));
                    Console.WriteLine(ex.Message);
                    throw ex;
                }
            }
        }

        private static void OnNewMessage(object sender, IdleMessageEventArgs e)
        {
            Console.WriteLine("A new message has been received. Message has UID: {0}", e.MessageUID);

            // Fetch the new message's headers and print the subject line
            MailMessage m = e.Client.GetMessage(e.MessageUID, FetchOptions.HeadersOnly);
            Console.WriteLine("New message's subject: " + m.Subject);

            ProcessMessage(e.MessageUID);
        }

        private static void client_IdleError(object sender, IdleErrorEventArgs e)
        {
            Console.Write("An error occurred while idling: ");
            Console.WriteLine(e.Exception.Message);

            //Send Error Email
            SendAlert("An error occurred while idling", String.Format("An error occurred while idling: {0}", e.Exception.Message));
            reconnectEvent.Set();

        }

        private static void MovePhoneLeadMessages(uint uid)
        {
            try
            {
                _client.MoveMessage(uid, "PhoneLeads", _client.DefaultMailbox);
            }
            catch (Exception ex)
            {
                SendAlert("An error occurred", String.Format("An error occurred: {0}", ex.Message));
            }
        }

        private static void MoveProcessedMessages(uint uid)
        {
            try
            {
                _client.SetMessageFlags(uid, _client.DefaultMailbox, MessageFlag.Seen);
                _client.MoveMessage(uid, "Processed", _client.DefaultMailbox);
            }
            catch (Exception ex)
            {
                SendAlert("An error occurred", String.Format("An error occurred: {0}", ex.Message));
            }
        }
        private static void MoveProcessedMessages(List<uint> uids)
        {
            if (uids.Any())
            {
                _client.MoveMessages(uids, "Processed");
            }
        }

        private static void MoveProcessedMessagesHtml(uint uid)
        {
            try
            {
                _client.MoveMessage(uid, "HtmlArchive", _client.DefaultMailbox);
            }
            catch (Exception ex)
            {
                SendAlert("An error occurred", String.Format("An error occurred: {0}", ex.Message));
            }
        }

        private static void SendAlert(string alert, string ex)
        {
            var client = new RestClient("https://api.fitzmall.com/Alert/");
            var request = new RestRequest("SendAlert?alert={alert}&ex={exp}", Method.POST);
            request.AddUrlSegment("alert", alert);
            request.AddUrlSegment("exp", ex);
            request.AddHeader("key", "2D73E29CE7F39AB8785D68FC5CE16");
            var response = client.Execute(request);
            Console.WriteLine("Alert Request Send ===>");
            Console.WriteLine("Response: {0}", response.Content);
        }
    }




}
