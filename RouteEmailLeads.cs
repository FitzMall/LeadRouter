using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ActiveUp.Net.WhoIs;
using Newtonsoft.Json;
using System.Globalization;
using System.Net;

namespace FMLeadRouter
{
    public class RouteEmailLeads
    {
        //private readonly EmailAccount _emailAccount;

        public RouteEmailLeads()
        {
            //EmailCredentials credentials = new EmailCredentials().GetImapCredentials();
            //_emailAccount = new EmailAccount(credentials);
        }

        //Start Monitor
        public string StartMailBoxMonitor()
        {
            //_emailAccount.StartIdleImap("INBOX");
            return "Started";
        }

        //Route Email Account
        public void RouteEmail(LeadVendor vendor)
        {
            string sql = @"";
            SqlMapperUtil.SqlWithParams<LeadRoute>(sql, new {});
        }

        //Deserialize ADF XML
        public ADF DeserializeAdf(string xml)
        {
            var adf = new ADF();

            return adf;
        }

        //Insert Email into Log
        public int InsertIntoLog(LeadRouteLog log)
        {
            const string sql = @"
                    INSERT INTO [VinSolution].[dbo].[LeadRouteLog]
                               ([MailId]
                               ,[FromAddress]
                               ,[ToAddress]
                               ,[Subject]
                               ,[Body]
                               ,[Status]
                               ,[LeadVendorId]
                               ,[LeadRouteId])
                         VALUES
                               (@MailId
                               ,@FromAddress
                               ,@ToAddress
                               ,@Subject
                               ,@Body
                               ,@Status
                               ,@LeadVendorId
                               ,@LeadRouteId)";

            var result = SqlMapperUtil.InsertUpdateOrDeleteSql(sql, log);
            return result;
        }

        public int InsertIntoMailErrorLog(MailError log)
        {
            const string sql = @"
                    INSERT INTO [VinSolution].[dbo].[MailError]
                               ([MailId]
                               ,[FromAddress]
                               ,[ToAddress]
                               ,[Subject]
                               ,[Body]
                               ,[Status]
                               ,[ErrorTitle]
                               ,[ErrorMessage]
                               ,[LeadVendorId]
                               ,[LeadRouteId])
                         VALUES
                               (@MailId
                               ,@FromAddress
                               ,@ToAddress
                               ,@Subject
                               ,@Body
                               ,@Status
                               ,@ErrorTitle
                               ,@ErrorMessage
                               ,@LeadVendorId
                               ,@LeadRouteId)";

            var result = SqlMapperUtil.InsertUpdateOrDeleteSql(sql, log);
            return result;
        }

        public int InsertIntoRoute(LeadRoute route)
        {
            const string sql = @"
                INSERT INTO [VinSolution].[dbo].[LeadRoute]
                            ([Name]
                            ,[LeadVendorId]
                            ,[VendorCode]
                            ,[ForwardEmail]
                            ,[ForwardSms]
                            ,[IsActive]
                            ,[Loc]
                            ,[Mall]
                            ,[Make]
                            ,[XmlLeadSource]
                            ,[XmlRule]
                            ,[LeadSourceAddOn]
                            ,[CreateDate])
                        VALUES
                            (@Name
                            ,@LeadVendorId
                            ,@VendorCode
                            ,@ForwardEmail
                            ,@ForwardSms
                            ,@IsActive
                            ,@Loc
                            ,@Mall
                            ,@Make
                            ,@XmlLeadSource
                            ,@XmlRule
                            ,@LeadSourceAddOn
                            ,@CreateDate)
                ";
            var result = SqlMapperUtil.InsertUpdateOrDeleteSql(sql, route);
            return result;
        }

        public LeadCrmEmail GetLeadCrmEmail(string loc)
        {
            string sql = @"SELECT * FROM [VinSolution].[dbo].[LeadCrmEmail] WHERE [LocCode] = @loc";
            var result = SqlMapperUtil.SqlWithParams<LeadCrmEmail>(sql, new{ loc }).First();
            return result;
        }


        public LeadVendor GetLeadVendorByEmail(string email)
        {
            if (email.Contains("@anon.cargurus.com"))
            {
                string sqlGuru = @"SELECT * FROM [VinSolution].[dbo].[LeadVendor] WHERE [Email] LIKE '%@anon.cargurus.com'";
                var vendorGuru = SqlMapperUtil.SqlWithParams<LeadVendor>(sqlGuru, new { email }).FirstOrDefault();
                return vendorGuru;
            }
            if (email.Contains(".truecarmail.com"))
            {
                string sqlGuru = @"SELECT * FROM [VinSolution].[dbo].[LeadVendor] WHERE [Email] = '@leads.truecarmail.com'";
                var vendorGuru = SqlMapperUtil.SqlWithParams<LeadVendor>(sqlGuru, new { email }).FirstOrDefault();
                return vendorGuru;
            }

            string sql = @"SELECT * FROM [VinSolution].[dbo].[LeadVendor] WHERE [Email] = @email";
            var vendor = SqlMapperUtil.SqlWithParams<LeadVendor>(sql, new { email }).FirstOrDefault();
            return vendor;
        }

        public bool CheckForPrimeMarket(string zipCode)
        {
            var bFound = false;
            
            string sql = @"SELECT count(*) FROM [VinSolution].[dbo].[PrimeMarketZipCodes] WHERE [ZipCode] = @zipCode";
            var zipCount = SqlMapperUtil.SqlWithParams<int>(sql, new { zipCode }).FirstOrDefault();

            if(zipCount > 0)
            {
                bFound = true;
            }

            return bFound;
        }

        public List<LeadVendorCodeXPath> GetLeadVendorCodeXPath()
        {
            var result = new List<LeadVendorCodeXPath>();
            string sql = @"SELECT [Xml] FROM [VinSolution].[dbo].[LeadVendorCodeXPath] ORDER BY [CreateDate] asc";
            result = SqlMapperUtil.SqlWithParams<LeadVendorCodeXPath>(sql, null);

            return result;
        }

        public List<LeadRoute> GetLeadRouteByVendorCode(string make, string vendorCode)
        {
            string sql = @"SELECT * FROM [VinSolution].[dbo].[LeadRoute] WHERE IsActive = 1 AND [Make] = @make AND [VendorCode] = @vendorCode ORDER BY [XmlRule] desc";
            var vendor = SqlMapperUtil.SqlWithParams<LeadRoute>(sql, new { make, vendorCode });
            return vendor;
        }


        public List<LeadRoute> GetLeadRouteStk(int id, string vendorCode, string locCode, string mall)
        {
            Console.WriteLine("Get Lead With Stk");
            string sql = @"SELECT * FROM [VinSolution].[dbo].[LeadRoute] WHERE IsActive = 1 AND [LeadVendorId] = @id AND [VendorCode] = @vendorCode AND [Loc] = @locCode AND [Mall] = @mall ORDER BY [XmlRule] desc";
            var route = SqlMapperUtil.SqlWithParams<LeadRoute>(sql, new { id, vendorCode, locCode, mall });
            return route;
        }
        public List<LeadRoute> GetLeadRouteByLoc(int id, string locCode)
        {
            Console.WriteLine("Get Lead With Stk");
            string sql = @"SELECT * FROM [VinSolution].[dbo].[LeadRoute] WHERE IsActive = 1 AND [LeadVendorId] = @id AND [Loc] = @locCode ORDER BY [XmlRule] desc";
            var route = SqlMapperUtil.SqlWithParams<LeadRoute>(sql, new { id, locCode });
            return route;
        }

        public List<LeadRoute> GetLeadRouteNoStk(int id, string vendorCode, string make)
        {
            Console.WriteLine("Get Lead No Stk");
            var route = new List<LeadRoute>();
            string sql = @"SELECT * FROM [VinSolution].[dbo].[LeadRoute] WHERE IsActive = 1 AND [LeadVendorId] = @id AND [VendorCode] = @vendorCode AND LOWER([Make]) = @make ORDER BY [XmlRule] desc";
            route = SqlMapperUtil.SqlWithParams<LeadRoute>(sql, new { id, vendorCode, make });
            //if route is null try without make
            if (route.Count == 0)
            {
                sql = @"SELECT TOP 1 * FROM [VinSolution].[dbo].[LeadRoute] WHERE IsActive = 1 AND [LeadVendorId] = @id AND [VendorCode] = @vendorCode ORDER BY [XmlRule] desc";
                route = SqlMapperUtil.SqlWithParams<LeadRoute>(sql, new { id, vendorCode });
            }
            return route ;
        }

        public List<LeadRoute> GetLeadRouteNoStk(int id, string vendorCode)
        {
            Console.WriteLine("Get Lead No Stk");
            var route = new List<LeadRoute>();
            string sql = @"SELECT * FROM [VinSolution].[dbo].[LeadRoute] WHERE IsActive = 1 AND [LeadVendorId] = @id AND [VendorCode] = @vendorCode ORDER BY [XmlRule] desc";
            route = SqlMapperUtil.SqlWithParams<LeadRoute>(sql, new { id, vendorCode });
            //if route is null try without make
            if (route.Count == 0)
            {
                sql = @"SELECT TOP 1 * FROM [VinSolution].[dbo].[LeadRoute] WHERE IsActive = 1 AND [LeadVendorId] = @id AND [VendorCode] = @vendorCode ORDER BY [XmlRule] desc";
                route = SqlMapperUtil.SqlWithParams<LeadRoute>(sql, new { id, vendorCode });
            }
            return route;
        }

        public LeadRoute GetLeadRoute(int id)
        {
            string sql = @"SELECT * FROM [VinSolution].[dbo].[LeadRoute] WHERE [LeadVendorId] = @id ORDER BY [XmlRule] desc";
            var route = SqlMapperUtil.SqlWithParams<LeadRoute>(sql, new {id}).FirstOrDefault();
            return route;
        }

        public Car GetVehicleInfo(string stockNumber)
        {
            string sql = @"SELECT [StockNumber] AS 'Stock', [LocationCode] AS 'Loc', [MallCode] AS 'Mall', [VehicleCondition] AS 'Cond', [IsHandy] AS 'Wholesale' FROM [FITZWAY].[dbo].[FM_VehicleResults] WHERE [StockNumber] = @stockNumber";
            var car = SqlMapperUtil.SqlWithParams<Car>(sql, new { stockNumber }, "Fitzway").FirstOrDefault();
            return car;
        }

        public string GetVehicleChromeImagePath(string styleId, string genericExteriorColor)
        {
            var imagePath = "";

            string sql = @"SELECT DISTINCT StyleId, [ShotCode], Height, Width, UrlShort as Url, UrlShort, PrimaryColorCode,SecondaryColorCode, DateDownloaded, DateUploaded FROM [ChromeFM].[dbo].[ChromeMediaImages] WHERE [StyleId] = @StyleId AND ([PrimaryColorCode] = @GenExtColor OR [PrimaryColorCode] IS NULL) AND PATINDEX('%.jpg%',[UrlShort]) > 0 AND [DateDownloaded] IS NOT NULL AND [DateUploaded] IS NOT NULL AND [Height] <= 240";
            var images = SqlMapperUtil.SqlWithParams<ChromeImage>(sql, new { StyleId = styleId, GenExtColor = genericExteriorColor }, "Fitzway");

            var colorImage = images.Find(x => x.PrimaryColorCode == genericExteriorColor.Trim());
            if (colorImage != null && (colorImage.UrlShort != null || colorImage.UrlShort != ""))
            {
                imagePath = colorImage.UrlShort;
            }
            else
            {
                try
                {
                    colorImage = images[0];
                    imagePath = colorImage.UrlShort;
                }
                catch (Exception ex)
                {
                    imagePath = "";
                }
            }

            return imagePath;
        }

        public string FormatAutoResponderEmail(CarDetails carDetails, string customerName, int vendorId, bool bEPriceEmail)
        {
            
            var emailBody = @"
<html xmlns='http://www.w3.org/1999/xhtml'>
	<head>
		<meta http-equiv='Content-Type' content='text/html; charset=UTF-8'>
		<meta name='viewport' content='width=device-width, initial-scale=1.0'>
		<style>
			body { font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif;font-size: 14px;line-height: 1.42857143;color: #333;}
	   </style>
	</head>
	<body style='margin: 0; padding: 0 15px; width='100%'>
		<table align='center' border='0' cellpadding='0' cellspacing='0' width='800px'>
			<tbody>
				<tr style='text-align:center;'>
					<td><img alt='' src='https://www.fitzmall.com/assets/images/logos/{make}_100x100.png'/></td>
					<td><img alt='' src='https://www.fitzmall.com/content/imgs/FitzwayClearShort.png'/></td>
					<td><h1 style='margin-top:25px;'>{mall}</h1></td>
				</tr>
			</tbody>
		</table>
		<table align='center' border='0' cellpadding='0' cellspacing='0' width='800px'>
			<tbody>
			<tr><td><hr/></td></tr>
			<tr style='text-align:left;'><td style='padding-top:0px; padding-bottom:5px;'><p>Hi {customer},</p></td></tr>
			<tr style='text-align:center;'><td style='padding-top:0px; padding-bottom:10px;'><p >Thank you for choosing {mall}, you have picked the right dealership for the best price.</p></td></tr>
			<tr style='text-align:center;'>
				<td>
					<h2 style='margin-bottom:5px;'>{vehicle}</h2>
					<h2 style='margin-bottom:5px;'>FitzWay Low Price: <span style='font-size: 1.8em;vertical-align: middle;'>{price}</style></h2>
                    <p>The FitzWay Low Price always includes freight that many dealers add back later in the deal.</p><br/>
					<img src='{photo}'/>
				</td>
			</tr>
			<tr style='text-align:center'>
				<td style='padding-top:10px;'>
					<h2 style='margin-bottom:5px;'></h2>
		    		<p>
		    			<strong>Stock Number:</strong> {stock}
		    		</p>
		    		<p>
						<strong>Body Style:</strong> {style} (Model#: {model})
					</p>
					<p>
						<strong>VIN:</strong> {vin}
					</p>
					<p>
						<strong>Fuel / Engine:</strong> {fuel}
					</p>
					<p>
						<strong>Transmission:</strong> {transmission}
					</p>
					<p>
						<strong>Ext. Color:</strong> {color}
		    		</p>
					<p>
						<strong>MSRP: </strong> {msrp}
					</p>
					<p style='text-align:center;'>
						<a href='{website-results}' target='_blank'><img src='https://www.fitzmall.com/assets/images/viewdetails.png'/></a>
                    </p>
				</td>
			</tr>
			<tr><td><hr/></td></tr>
			<tr>
				<td style='padding-top:20px;padding-bottom:20px;'>
				<p>One of my professional consultants will be contacting you shortly to answer your questions, gather more information, and see if there are any additional incentives you might be eligible for that could reduce the price even further.</p>
				<p>We pride ourselves in providing you the best car buying experience and it starts with the test drive. </p>
				<p>It is worth the conversation with your consultant because there may be other ways to save when you're shopping for a vehicle that many people don't even know about.</p>
				{incentives}
				<p>Looking forward to meeting you in person.</p>
				<p>
					{salesmanager}<br />
		            {title}<br/>
		            {address}<br/>
		            {citystatezip}<br/>
		            {phone}<br/>
		            <a href='mailto:{email}'>{email}</a>
				</p>
				<p>
					<a href='{website}' target='_blank'>{website}</a>
				</p>
				</td>
			</tr>
			</tbody>
		</table>
	</body>
</html>

            ";

            var ePriceEmailBody = @"
<html xmlns='http://www.w3.org/1999/xhtml'>
	<head>
		<meta http-equiv='Content-Type' content='text/html; charset=UTF-8'>
		<meta name='viewport' content='width=device-width, initial-scale=1.0'>
		<style>
			body { font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif;font-size: 14px;line-height: 1.42857143;color: #333;}
	   </style>
	</head>
	<body style='margin: 0; padding: 0 15px; width='100%'>
		<table align='center' border='0' cellpadding='0' cellspacing='0' width='800px'>
			<tbody>
				<tr style='text-align:center;'>
					<td><img alt='' src='https://www.fitzmall.com/assets/images/logos/{make}_100x100.png'/></td>
					<td><img alt='' src='https://www.fitzmall.com/content/imgs/FitzwayClearShort.png'/></td>
					<td><h1 style='margin-top:25px;'>{mall}</h1></td>
				</tr>
			</tbody>
		</table>
		<table align='center' border='0' cellpadding='0' cellspacing='0' width='800px'>
			<tbody>
			<tr><td><hr/></td></tr>
			<tr style='text-align:left;'><td style='padding-top:0px; padding-bottom:5px;'><p>Hi {customer},</p></td></tr>
			<tr style='text-align:center;'><td style='padding-top:0px; padding-bottom:10px;'><p >Thank you for choosing {mall}, you have picked the right dealership for the best price.</p></td></tr>
			<tr style='text-align:center;'>
				<td>
					<h2 style='margin-bottom:5px;'>{vehicle}</h2>
					<h2 style='margin-bottom:5px;'>ePrice: <span style='font-size: 1.8em;vertical-align: middle;'>{price}*</style></h2>
					<img src='{photo}'/>
				</td>
			</tr>
			<tr style='text-align:center'>
				<td style='padding-top:10px;'>
					<h2 style='margin-bottom:5px;'></h2>
		    		<p>
		    			<strong>Stock Number:</strong> {stock}
		    		</p>
		    		<p>
						<strong>Body Style:</strong> {style} (Model#: {model})
					</p>
					<p>
						<strong>VIN:</strong> {vin}
					</p>
					<p>
						<strong>Fuel / Engine:</strong> {fuel}
					</p>
					<p>
						<strong>Transmission:</strong> {transmission}
					</p>
					<p>
						<strong>Ext. Color:</strong> {color}
		    		</p>
					<p>
						<strong>MSRP: </strong> {msrp}
					</p>
					<p style='text-align:center;'>
						<a href='{website-results}' target='_blank'><img src='https://www.fitzmall.com/assets/images/viewdetails.png'/></a>
                    </p>
				</td>
			</tr>
			<tr><td><hr/></td></tr>
			<tr>
				<td style='padding-top:20px;padding-bottom:20px;'>				
				<p>One of our professional consultants will be contacting you shortly to answer your questions, gather more information, and see if <strong><u>there are any additional incentives you might be eligible for that could reduce the price even further.</u></strong></p>
				<p>We pride ourselves in providing you a transparent car buying experience and it starts with the test drive.</p>
				<p>It is worth the conversation with your consultant because <strong><u>there may be other ways to save when you're shopping for a vehicle that many people don't even know about.</u></strong></p>
                <p>* To be consistent with our competition, the  ePrice does not include freight, taxes, title, license and the {processingfee} {processinglabel} (Not required by law). This allows you to compare our low prices to other dealerships who do not include freight in their offer. It's simply a matter of delivering the most competitive price to you so you can make the best decision for yourself.</p>
				{incentives}
				<p>Looking forward to meeting you in person.</p>
				<p>
					{salesmanager}<br />
		            {title}<br/>
		            {address}<br/>
		            {citystatezip}<br/>
		            {phone}<br/>
		            <a href='mailto:{email}'>{email}</a>
				</p>
				<p>
					<a href='{website}' target='_blank'>{website}</a>
				</p>
				</td>
			</tr>
			</tbody>
		</table>
	</body>
</html>

            ";

            var processingFee = "$499";
            var processingLabel = "Processing Charge";

            if (carDetails.State.Trim().ToUpper() == "FL")
            {
                processingFee = "$599";
                processingLabel = "Processing Charge";
            }
            else if (carDetails.State.Trim().ToUpper() == "PA")
            {
                processingFee = "$389";
                processingLabel = "Documentary Fee";
            }

            
            if (bEPriceEmail)
            {
                emailBody = ePriceEmailBody;
                emailBody = emailBody.Replace("{processingfee}", processingFee);
                emailBody = emailBody.Replace("{processinglabel}", processingLabel);
            }

            var salesManager = "";
            var website = "";
            var websiteResults = "";
            var title = "";

            if (carDetails.Loc == "LFT")
            {
                salesManager = "Phil Formichelli";
                title = "Sales Manager";
                website = "https://www.fitzgeraldtoyotagaithersburg.com/";
                websiteResults = "https://www.fitzgeraldtoyotagaithersburg.com/new-inventory/index.htm?search=" + carDetails.StockNumber;
            }
            else if (carDetails.Loc == "FTO")
            {
                if (carDetails.MakeName.ToLower() == "toyota")
                {
                    salesManager = "Emily Moore";
                    title = "General Manager";
                    website = "https://www.fitzgeraldtoyotachambersburg.com";
                    websiteResults = "https://www.fitzgeraldtoyotachambersburg.com/new-inventory/index.htm?search=" + carDetails.StockNumber;
                }
                else if (carDetails.MakeName.ToLower() == "nissan")
                {
                    salesManager = "Emily Moore";
                    title = "General Manager";
                    website = "https://www.fitzgeraldnissan.com/";
                    websiteResults = "https://www.fitzgeraldnissan.com/new-inventory/index.htm?search=" + carDetails.StockNumber;
                }

            }
            else if (carDetails.Loc == "FMM")
            {
                salesManager = "Jeff Shaver";
                title = "General Manager";
                website = "https://www.fitzmallmazdaannapolis.com/";
                websiteResults = "https://www.fitzmallmazdaannapolis.com/new-inventory/index.htm?search=" + carDetails.StockNumber;
            }
            else if (carDetails.Loc == "FOC")
            {
                if (carDetails.MakeName.ToLower() == "cadillac")
                {
                    salesManager = "Rod Ritter";
                    title = "General Sales Manager";
                    website = "https://www.fitzgeraldcadillacannapolis.com/";
                    websiteResults = "https://www.fitzgeraldcadillacannapolis.com/new-inventory/index.htm?search=" + carDetails.StockNumber;
                }
                else if (carDetails.MakeName.ToLower() == "volkswagen")
                {
                    salesManager = "Rod Ritter";
                    title = "General Sales Manager";
                    website = "https://www.fitzgeraldvolkswagen.com/";
                    websiteResults = "https://www.fitzgeraldvolkswagen.com/new-inventory/index.htm?search=" + carDetails.StockNumber;
                }


            }
            else if (carDetails.Loc == "LFO")
            {
                if (carDetails.MakeName.ToLower() == "subaru")
                {
                    salesManager = "Felipe Teixeira";
                    title = "General Sales Manager";
                    website = "https://www.fitzgeraldsubarugaithersburg.com/";
                    websiteResults = "https://www.fitzgeraldsubarugaithersburg.com/new-inventory/index.htm?search=" + carDetails.StockNumber;
                }
                else
                {
                    salesManager = "Jenna Keller";
                    title = "Sales Manager";
                    website = "https://www.fitzgeraldgenesisgaithersburg.com/";
                    websiteResults = "https://www.fitzgeraldgenesisgaithersburg.com/new-inventory/index.htm?search=" + carDetails.StockNumber;
                }
            }
            else if (carDetails.Loc == "CDO")
            {
                salesManager = "David Ascher";
                title = "Sales Manager";
                website = "https://www.fitzgeraldgenesisrockville.com/";
                websiteResults = "https://www.fitzgeraldgenesisrockville.com/new-inventory/index.htm?search=" + carDetails.StockNumber;
            }
            else if (carDetails.Loc == "FBS")
            {
                salesManager = "Ken McAlpine";
                title = "Sales Manager";
                website = "https://www.fitzmallsubaru.com/";
                websiteResults = "https://www.fitzmallsubaru.com/new-inventory/index.htm?search=" + carDetails.StockNumber;
            }
            else if (carDetails.Loc == "FCG")
            {
                if (carDetails.MakeName.ToLower() == "cadillac")
                {
                    salesManager = "Roy Daves";
                    title = "General Manager";
                    website = "https://www.fitzgeraldcadillacfrederick.com/";
                    websiteResults = "https://www.fitzgeraldcadillacfrederick.com/new-inventory/index.htm?search=" + carDetails.StockNumber;
                }
                else if (carDetails.MakeName.ToLower() == "mazda")
                {
                    salesManager = "Roy Daves";
                    title = "General Manager";
                    website = "https://www.frederickmazda.com/";
                    websiteResults = "https://www.frederickmazda.com/new-inventory/index.htm?search=" + carDetails.StockNumber;
                }
            }
            else if (carDetails.Loc == "CHY")
            {
                salesManager = "Lane Campbell";
                title = "General Sales Manager";
                website = "https://www.fitzgeraldgenesisclearwater.com/";
                websiteResults = "https://www.fitzgeraldgenesisclearwater.com/new-inventory/index.htm?search=" + carDetails.StockNumber;
            }
            else if (carDetails.Loc == "CSS")
            {
                salesManager = "Corinna Baker";
                title = "Sales Manager";
                website = "https://www.fitzgeraldsubaru.com/";
                websiteResults = "https://www.fitzgeraldsubaru.com/new-inventory/index.htm?search=" + carDetails.StockNumber;
            }
            else if (carDetails.Loc == "FHG")
            {
                if (carDetails.MakeName.ToLower() == "cadillac")
                {
                    salesManager = "Wendy Coy";
                    title = "General Manager";
                    website = "https://www.fitzgeraldcadillachagerstown.com/";
                    websiteResults = "https://www.fitzgeraldcadillachagerstown.com/new-inventory/index.htm?search=" + carDetails.StockNumber;
                }
            }
                if (vendorId == 4)
            {
                website = "https://www.fitzmall.com";
                websiteResults = "https://www.fitzmall.com/Inventory/Detail/" + carDetails.XrefID;
            }

            // Creates a TextInfo based on the "en-US" culture.
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

            NumberFormatInfo nfi = new System.Globalization.CultureInfo("en-US", false).NumberFormat;
            nfi.CurrencyDecimalDigits = 0;
            nfi.CurrencySymbol = "$";

            var autoMallName = "FITZGERALD" + " " + carDetails.MakeName + ' ' + carDetails.City;
            var yearMakeModel = carDetails.ModelYear + " " + carDetails.MakeName + " " + carDetails.ModelName;

            var vehiclePrice = "";
            if (bEPriceEmail)
            {
                vehiclePrice = (carDetails.InternetPrice - carDetails.Freight).ToString("C", nfi);
            }
            else
            {
                vehiclePrice = carDetails.InternetPrice.ToString("C", nfi);
            }

            var MSRPPrice = carDetails.MSRP.ToString("C", nfi);
            var cityStateZip = textInfo.ToTitleCase(textInfo.ToLower(carDetails.City)).Trim() + ", " + carDetails.State.Trim() + " " + carDetails.Zip.Trim();

            var carImage = "";
            var fitzmallImage = "https://www.fitzmall.com/picturedata/UCimages/" + carDetails.VIN + "_A.jpg";
            var vinImageExists = CheckIfImageExists(fitzmallImage);

            if (!vinImageExists)
            {
                //Get Chrome Image...
                var imagePath = GetVehicleChromeImagePath(carDetails.StyleId, carDetails.GenExteriorColor);
                if (imagePath != "")
                {
                    carImage = "https://www.fitzmall.com/picturedata/MediaGallery/" + imagePath;
                }
                else
                {
                    carImage = "https://www.fitzmall.com/content/imgs/details/large-nophotos.png";
                }
            }
            else
            {
                carImage = fitzmallImage;
            }

            var incentiveString = "";
            var incentives = GetVehicleIncentives(carDetails.ModelCode, carDetails.Mall);

            if(incentives != null && incentives.Count > 0)
            {
                foreach(var incentive in incentives)
                {
                    if (incentive.EndDate > DateTime.Now)
                    {
                        incentiveString += "<p>* " + incentive.Description + "</p>";
                    }
                }
            }

            try
            {
                emailBody = emailBody.Replace("{make}", carDetails.MakeName);
                emailBody = emailBody.Replace("{mall}", textInfo.ToTitleCase(textInfo.ToLower(autoMallName)).Trim());
                emailBody = emailBody.Replace("{customer}", customerName);
                emailBody = emailBody.Replace("{vehicle}", yearMakeModel);
                emailBody = emailBody.Replace("{price}", vehiclePrice);
                emailBody = emailBody.Replace("{photo}", carImage);
                emailBody = emailBody.Replace("{vin}", carDetails.VIN);
                emailBody = emailBody.Replace("{stock}", carDetails.StockNumber);
                emailBody = emailBody.Replace("{style}", carDetails.StyleName);
                emailBody = emailBody.Replace("{model}", carDetails.ModelCode);
                emailBody = emailBody.Replace("{fuel}", carDetails.Fuel);
                emailBody = emailBody.Replace("{transmission}", carDetails.Transmission);
                emailBody = emailBody.Replace("{color}", textInfo.ToTitleCase(textInfo.ToLower(carDetails.ExteriorColor)));
                emailBody = emailBody.Replace("{msrp}", MSRPPrice);
                emailBody = emailBody.Replace("{incentives}", incentiveString);
                emailBody = emailBody.Replace("{website-results}", websiteResults);
                emailBody = emailBody.Replace("{website}", website);
                emailBody = emailBody.Replace("{salesmanager}", salesManager);
                emailBody = emailBody.Replace("{title}", title);
                emailBody = emailBody.Replace("{address}", textInfo.ToTitleCase(textInfo.ToLower(carDetails.Address)).Trim());
                emailBody = emailBody.Replace("{citystatezip}", cityStateZip);
                emailBody = emailBody.Replace("{phone}", String.Format("{0:###-###-####}", Convert.ToInt64(carDetails.Phone)));
                emailBody = emailBody.Replace("{email}", carDetails.Email);
            }
            catch (Exception ex)
            {

            }
            //var finalEmailBody = String.Format(emailBody, customerName, autoMallName, yearMakeModel, vehiclePrice, salesManagerLine, address);

            return emailBody;
        }

        public bool CheckIfImageExists(string imageUrl)
        {
            var exists = false;

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(imageUrl);
            request.Method = "HEAD";

            try
            {
                request.GetResponse();

                if (request.Address.AbsolutePath.Contains("HttpStatus404"))
                {
                    exists = false;
                }
                else
                {
                    exists = true;
                }
            }
            catch
            {
                exists = false;
            }

            return exists;
        }


        public CarDetails GetVehicleDetails(string stockNumber)
        {
            string sql = @"SELECT 
                            [StockNumber],
                            StyleId,
                            [XrefID],
                            [LocationCode] as Loc,
                            [S_StoreName] as StoreName,
                            [S_Address] as 'Address',
                            [S_City] as City,
                            [S_State] as 'State',
                            [S_Zip] as Zip,
                            [s_Phone] as Phone,
                            [s_email] as Email,
                            [s_Hours] as 'Hours',
                            [MallCode] AS 'Mall',
                            [VehicleCondition],
                            ModelYear,
                            MakeName,
                            ModelName,
                            VIN,
                            Freight,
                            InternetPrice,
                            MSRPPrice as MSRP,
                            [IsHandy] AS 'Wholesale',
                            StyleName,
                            ModelCode,
                            ExteriorColor,
                            GenExteriorColor,
                            Fuel,
                            Transmission,
                            Passengers
                             FROM [FITZWAY].[dbo].[FM_VehicleResults] WHERE [StockNumber] = @stockNumber";

            var car = SqlMapperUtil.SqlWithParams<CarDetails>(sql, new { stockNumber }, "Fitzway").FirstOrDefault();
            return car;
        }

        public List<Incentive> GetVehicleIncentives(string modelNumber, string storeLocation)
        {
            var sql = @"
                Select 
	                IN_title as IncentiveTitle, 
	                IN_startDT as StartDate, 
	                IN_endDT as EndDate, 
	                IN_store as Store, 
	                IN_Description as 'Description'
                from [FITZWAY].[dbo].[incentives] 
                where IN_Number in ( Select [IX_number] from [FITZWAY].[dbo].[incentivesXREF] where IX_ModelNo = @Model) and IN_Store = @StoreLocation

            ";

            var incentives = SqlMapperUtil.SqlWithParams<Incentive>(sql, new { Model = modelNumber, StoreLocation = storeLocation }, "Fitzway");
            return incentives;
        }

        public Car GetVehicleInfoByVin(string vinNumber)
        {
            string sql = @"SELECT [StockNumber] AS 'Stock', [LocationCode] AS 'Loc', [MallCode] AS 'Mall', [VehicleCondition] As 'Cond' FROM [FITZWAY].[dbo].[FM_VehicleResults] WHERE [Vin] = @vinNumber";
            var car = SqlMapperUtil.SqlWithParams<Car>(sql, new { vinNumber }, "Fitzway").FirstOrDefault();
            return car;
        }

        public TrafficSource TrafficSources(string filePath)
        {
            var results = new TrafficSource();
            string text = String.Empty;
            if (File.Exists(filePath))
            {
                text = File.ReadAllText(filePath);
                results = JsonConvert.DeserializeObject<TrafficSource>(text);
            }
            return results;
        } 

    }
}