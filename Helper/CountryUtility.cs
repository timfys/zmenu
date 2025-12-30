using Menu4Tech.IBusinessAPIservice;
using Newtonsoft.Json;
using File = System.IO.File;

namespace Menu4Tech.Helper
{
    public static class CountryUtility
    {
        public static List<Country>? _countries { get; set; } = null;

        public static Country[] GetListOfCountries()
        {
            var client = EnvironmentHelper.BusinessApiConfiguration.InitClient();

            var apiRequest = new General_DataList_GetRequest
            {
                TableName = "countries",
                FilterFields = new[] {"Order by"},
                FilterValues = new[] {"LastDateUsed desc, CountryName"}
            };

            var apiResponse = client.General_DataList_Get(apiRequest);

            var countries = JsonConvert.DeserializeObject<List<Country>>(apiResponse.@return);

            return countries.Where(x => !x.Name.Equals("country", StringComparison.OrdinalIgnoreCase)).ToArray();
        }

        public static Country GetCountryByAlpha2(string alpha2)
            => Countries.FirstOrDefault(x => x.Alpha2 == alpha2.ToUpper());

        /*public static string GetClientCountryAlpha2(HttpContext context)
        {
            var request = context.Request;

            var forwardedClient = request.Headers["X-Forwarded-For"].FirstOrDefault()?
                .Split(',')?
                .FirstOrDefault()?
                .Trim();

            /* Ensure IPv4 or exit#1#
            var userHostAddress = forwardedClient ?? context.Connection.RemoteIpAddress?.ToString();

            var isIpAddress = IPAddress.TryParse(userHostAddress, out var ipAddress);
            if (!isIpAddress || ipAddress.AddressFamily != AddressFamily.InterNetwork)
            {
                return null;
            }

            var client = new SiteToolsApi.SiteToolsClient();
            var req = new SiteToolsApi.GetIPAddressRequest(ipAddress.ToString());

            try
            {
                var resp = client.GetIPAddress(req);

                var doc = new XmlDocument();
                doc.LoadXml(resp.@return);

                var countryAlpha2 = doc.SelectSingleNode("LocationInfo/CountryCode").InnerText;
                if (!string.IsNullOrWhiteSpace(countryAlpha2))
                {
                    return countryAlpha2.ToLower();
                }
            }
            catch
            {
            }

            return null;
        }*/
        /*public static Country[] Countries = new[]
        {
            new Country {Name = "Afghanistan", Alpha2 = "AF", Code = "93"},
            new Country {Name = "Albania", Alpha2 = "AL", Code = "355"},
            new Country {Name = "Algeria", Alpha2 = "DZ", Code = "213"},
            new Country {Name = "American Samoa", Alpha2 = "AS", Code = "1-684"},
            new Country {Name = "Andorra", Alpha2 = "AD", Code = "376"},
            new Country {Name = "Angola", Alpha2 = "AO", Code = "244"},
            new Country {Name = "Anguilla", Alpha2 = "AI", Code = "1-264"},
            new Country {Name = "Antarctica", Alpha2 = "AQ", Code = "672"},
            new Country {Name = "Antigua and Barbuda", Alpha2 = "AG", Code = "1-268"},
            new Country {Name = "Argentina", Alpha2 = "AR", Code = "54"},
            new Country {Name = "Armenia", Alpha2 = "AM", Code = "374"},
            new Country {Name = "Aruba", Alpha2 = "AW", Code = "297"},
            new Country {Name = "Australia", Alpha2 = "AU", Code = "61"},
            new Country {Name = "Austria", Alpha2 = "AT", Code = "43"},
            new Country {Name = "Azerbaijan", Alpha2 = "AZ", Code = "994"},
            new Country {Name = "Bahamas", Alpha2 = "BS", Code = "1-242"},
            new Country {Name = "Bahrain", Alpha2 = "BH", Code = "973"},
            new Country {Name = "Bangladesh", Alpha2 = "BD", Code = "880"},
            new Country {Name = "Barbados", Alpha2 = "BB", Code = "1-246"},
            new Country {Name = "Belarus", Alpha2 = "BY", Code = "375"},
            new Country {Name = "Belgium", Alpha2 = "BE", Code = "32"},
            new Country {Name = "Belize", Alpha2 = "BZ", Code = "501"},
            new Country {Name = "Benin", Alpha2 = "BJ", Code = "229"},
            new Country {Name = "Bermuda", Alpha2 = "BM", Code = "1-441"},
            new Country {Name = "Bhutan", Alpha2 = "BT", Code = "975"},
            new Country {Name = "Bolivia", Alpha2 = "BO", Code = "591"},
            new Country {Name = "Bosnia and Herzegovina", Alpha2 = "BA", Code = "387"},
            new Country {Name = "Botswana", Alpha2 = "BW", Code = "267"},
            new Country {Name = "Brazil", Alpha2 = "BR", Code = "55"},
            new Country {Name = "British Indian Ocean Territory", Alpha2 = "IO", Code = "246"},
            new Country {Name = "British Virgin Islands", Alpha2 = "VG", Code = "1-284"},
            new Country {Name = "Brunei", Alpha2 = "BN", Code = "673"},
            new Country {Name = "Bulgaria", Alpha2 = "BG", Code = "359"},
            new Country {Name = "Burkina Faso", Alpha2 = "BF", Code = "226"},
            new Country {Name = "Burundi", Alpha2 = "BI", Code = "257"},
            new Country {Name = "Cambodia", Alpha2 = "KH", Code = "855"},
            new Country {Name = "Cameroon", Alpha2 = "CM", Code = "237"},
            new Country {Name = "Canada", Alpha2 = "CA", Code = "1"},
            new Country {Name = "Cape Verde", Alpha2 = "CV", Code = "238"},
            new Country {Name = "Cayman Islands", Alpha2 = "KY", Code = "1-345"},
            new Country {Name = "Central African Republic", Alpha2 = "CF", Code = "236"},
            new Country {Name = "Chad", Alpha2 = "TD", Code = "235"},
            new Country {Name = "Chile", Alpha2 = "CL", Code = "56"},
            new Country {Name = "China", Alpha2 = "CN", Code = "86"},
            new Country {Name = "Christmas Island", Alpha2 = "CX", Code = "61"},
            new Country {Name = "Cocos Islands", Alpha2 = "CC", Code = "61"},
            new Country {Name = "Colombia", Alpha2 = "CO", Code = "57"},
            new Country {Name = "Comoros", Alpha2 = "KM", Code = "269"},
            new Country {Name = "Cook Islands", Alpha2 = "CK", Code = "682"},
            new Country {Name = "Costa Rica", Alpha2 = "CR", Code = "506"},
            new Country {Name = "Croatia", Alpha2 = "HR", Code = "385"},
            new Country {Name = "Cuba", Alpha2 = "CU", Code = "53"},
            new Country {Name = "Curacao", Alpha2 = "CW", Code = "599"},
            new Country {Name = "Cyprus", Alpha2 = "CY", Code = "357"},
            new Country {Name = "Czech Republic", Alpha2 = "CZ", Code = "420"},
            new Country {Name = "Democratic Republic of the Congo", Alpha2 = "CD", Code = "243"},
            new Country {Name = "Denmark", Alpha2 = "DK", Code = "45"},
            new Country {Name = "Djibouti", Alpha2 = "DJ", Code = "253"},
            new Country {Name = "Dominica", Alpha2 = "DM", Code = "1-767"},
            new Country {Name = "Dominican Republic", Alpha2 = "DO", Code = "1-809"},
            new Country {Name = "Dominican Republic", Alpha2 = "DO", Code = "1-829"},
            new Country {Name = "Dominican Republic", Alpha2 = "DO", Code = "1-849"},
            new Country {Name = "East Timor", Alpha2 = "TL", Code = "670"},
            new Country {Name = "Ecuador", Alpha2 = "EC", Code = "593"},
            new Country {Name = "Egypt", Alpha2 = "EG", Code = "20"},
            new Country {Name = "El Salvador", Alpha2 = "SV", Code = "503"},
            new Country {Name = "Equatorial Guinea", Alpha2 = "GQ", Code = "240"},
            new Country {Name = "Eritrea", Alpha2 = "ER", Code = "291"},
            new Country {Name = "Estonia", Alpha2 = "EE", Code = "372"},
            new Country {Name = "Ethiopia", Alpha2 = "ET", Code = "251"},
            new Country {Name = "Falkland Islands", Alpha2 = "FK", Code = "500"},
            new Country {Name = "Faroe Islands", Alpha2 = "FO", Code = "298"},
            new Country {Name = "Fiji", Alpha2 = "FJ", Code = "679"},
            new Country {Name = "Finland", Alpha2 = "FI", Code = "358"},
            new Country {Name = "France", Alpha2 = "FR", Code = "33"},
            new Country {Name = "French Polynesia", Alpha2 = "PF", Code = "689"},
            new Country {Name = "Gabon", Alpha2 = "GA", Code = "241"},
            new Country {Name = "Gambia", Alpha2 = "GM", Code = "220"},
            new Country {Name = "Georgia", Alpha2 = "GE", Code = "995"},
            new Country {Name = "Germany", Alpha2 = "DE", Code = "49"},
            new Country {Name = "Ghana", Alpha2 = "GH", Code = "233"},
            new Country {Name = "Gibraltar", Alpha2 = "GI", Code = "350"},
            new Country {Name = "Greece", Alpha2 = "GR", Code = "30"},
            new Country {Name = "Greenland", Alpha2 = "GL", Code = "299"},
            new Country {Name = "Grenada", Alpha2 = "GD", Code = "1-473"},
            new Country {Name = "Guam", Alpha2 = "GU", Code = "1-671"},
            new Country {Name = "Guatemala", Alpha2 = "GT", Code = "502"},
            new Country {Name = "Guernsey", Alpha2 = "GG", Code = "44-1481"},
            new Country {Name = "Guinea", Alpha2 = "GN", Code = "224"},
            new Country {Name = "Guinea-Bissau", Alpha2 = "GW", Code = "245"},
            new Country {Name = "Guyana", Alpha2 = "GY", Code = "592"},
            new Country {Name = "Haiti", Alpha2 = "HT", Code = "509"},
            new Country {Name = "Honduras", Alpha2 = "HN", Code = "504"},
            new Country {Name = "Hong Kong", Alpha2 = "HK", Code = "852"},
            new Country {Name = "Hungary", Alpha2 = "HU", Code = "36"},
            new Country {Name = "Iceland", Alpha2 = "IS", Code = "354"},
            new Country {Name = "India", Alpha2 = "IN", Code = "91"},
            new Country {Name = "Indonesia", Alpha2 = "ID", Code = "62"},
            new Country {Name = "Iran", Alpha2 = "IR", Code = "98"},
            new Country {Name = "Iraq", Alpha2 = "IQ", Code = "964"},
            new Country {Name = "Ireland", Alpha2 = "IE", Code = "353"},
            new Country {Name = "Isle of Man", Alpha2 = "IM", Code = "44-1624"},
            new Country {Name = "Israel", Alpha2 = "IL", Code = "972"},
            new Country {Name = "Italy", Alpha2 = "IT", Code = "39"},
            new Country {Name = "Ivory Coast", Alpha2 = "CI", Code = "225"},
            new Country {Name = "Jamaica", Alpha2 = "JM", Code = "1-876"},
            new Country {Name = "Japan", Alpha2 = "JP", Code = "81"},
            new Country {Name = "Jersey", Alpha2 = "JE", Code = "44-1534"},
            new Country {Name = "Jordan", Alpha2 = "JO", Code = "962"},
            new Country {Name = "Kazakhstan", Alpha2 = "KZ", Code = "7"},
            new Country {Name = "Kenya", Alpha2 = "KE", Code = "254"},
            new Country {Name = "Kiribati", Alpha2 = "KI", Code = "686"},
            new Country {Name = "Kosovo", Alpha2 = "XK", Code = "383"},
            new Country {Name = "Kuwait", Alpha2 = "KW", Code = "965"},
            new Country {Name = "Kyrgyzstan", Alpha2 = "KG", Code = "996"},
            new Country {Name = "Laos", Alpha2 = "LA", Code = "856"},
            new Country {Name = "Latvia", Alpha2 = "LV", Code = "371"},
            new Country {Name = "Lebanon", Alpha2 = "LB", Code = "961"},
            new Country {Name = "Lesotho", Alpha2 = "LS", Code = "266"},
            new Country {Name = "Liberia", Alpha2 = "LR", Code = "231"},
            new Country {Name = "Libya", Alpha2 = "LY", Code = "218"},
            new Country {Name = "Liechtenstein", Alpha2 = "LI", Code = "423"},
            new Country {Name = "Lithuania", Alpha2 = "LT", Code = "370"},
            new Country {Name = "Luxembourg", Alpha2 = "LU", Code = "352"},
            new Country {Name = "Macau", Alpha2 = "MO", Code = "853"},
            new Country {Name = "Macedonia", Alpha2 = "MK", Code = "389"},
            new Country {Name = "Madagascar", Alpha2 = "MG", Code = "261"},
            new Country {Name = "Malawi", Alpha2 = "MW", Code = "265"},
            new Country {Name = "Malaysia", Alpha2 = "MY", Code = "60"},
            new Country {Name = "Maldives", Alpha2 = "MV", Code = "960"},
            new Country {Name = "Mali", Alpha2 = "ML", Code = "223"},
            new Country {Name = "Malta", Alpha2 = "MT", Code = "356"},
            new Country {Name = "Marshall Islands", Alpha2 = "MH", Code = "692"},
            new Country {Name = "Mauritania", Alpha2 = "MR", Code = "222"},
            new Country {Name = "Mauritius", Alpha2 = "MU", Code = "230"},
            new Country {Name = "Mayotte", Alpha2 = "YT", Code = "262"},
            new Country {Name = "Mexico", Alpha2 = "MX", Code = "52"},
            new Country {Name = "Micronesia", Alpha2 = "FM", Code = "691"},
            new Country {Name = "Moldova", Alpha2 = "MD", Code = "373"},
            new Country {Name = "Monaco", Alpha2 = "MC", Code = "377"},
            new Country {Name = "Mongolia", Alpha2 = "MN", Code = "976"},
            new Country {Name = "Montenegro", Alpha2 = "ME", Code = "382"},
            new Country {Name = "Montserrat", Alpha2 = "MS", Code = "1-664"},
            new Country {Name = "Morocco", Alpha2 = "MA", Code = "212"},
            new Country {Name = "Mozambique", Alpha2 = "MZ", Code = "258"},
            new Country {Name = "Myanmar", Alpha2 = "MM", Code = "95"},
            new Country {Name = "Namibia", Alpha2 = "NA", Code = "264"},
            new Country {Name = "Nauru", Alpha2 = "NR", Code = "674"},
            new Country {Name = "Nepal", Alpha2 = "NP", Code = "977"},
            new Country {Name = "Netherlands", Alpha2 = "NL", Code = "31"},

            /* A part of Netherland Kingdom now #1#
            new Country {Name = "Netherlands Antilles", Alpha2 = "NL", Code = "599"},
            //new Country { Name = "Netherlands Antilles", Alpha2 = "AN", Code = "599" },

            new Country {Name = "New Caledonia", Alpha2 = "NC", Code = "687"},
            new Country {Name = "New Zealand", Alpha2 = "NZ", Code = "64"},
            new Country {Name = "Nicaragua", Alpha2 = "NI", Code = "505"},
            new Country {Name = "Niger", Alpha2 = "NE", Code = "227"},
            new Country {Name = "Nigeria", Alpha2 = "NG", Code = "234"},
            new Country {Name = "Niue", Alpha2 = "NU", Code = "683"},
            new Country {Name = "North Korea", Alpha2 = "KP", Code = "850"},
            new Country {Name = "Northern Mariana Islands", Alpha2 = "MP", Code = "1-670"},
            new Country {Name = "Norway", Alpha2 = "NO", Code = "47"},
            new Country {Name = "Oman", Alpha2 = "OM", Code = "968"},
            new Country {Name = "Pakistan", Alpha2 = "PK", Code = "92"},
            new Country {Name = "Palau", Alpha2 = "PW", Code = "680"},
            new Country {Name = "Palestine", Alpha2 = "PS", Code = "970"},
            new Country {Name = "Panama", Alpha2 = "PA", Code = "507"},
            new Country {Name = "Papua New Guinea", Alpha2 = "PG", Code = "675"},
            new Country {Name = "Paraguay", Alpha2 = "PY", Code = "595"},
            new Country {Name = "Peru", Alpha2 = "PE", Code = "51"},
            new Country {Name = "Philippines", Alpha2 = "PH", Code = "63"},
            new Country {Name = "Pitcairn", Alpha2 = "PN", Code = "64"},
            new Country {Name = "Poland", Alpha2 = "PL", Code = "48"},
            new Country {Name = "Portugal", Alpha2 = "PT", Code = "351"},
            new Country {Name = "Puerto Rico", Alpha2 = "PR", Code = "1-787"},
            new Country {Name = "Puerto Rico", Alpha2 = "PR", Code = "1-939"},
            new Country {Name = "Qatar", Alpha2 = "QA", Code = "974"},
            new Country {Name = "Republic of the Congo", Alpha2 = "CG", Code = "242"},
            new Country {Name = "Reunion", Alpha2 = "RE", Code = "262"},
            new Country {Name = "Romania", Alpha2 = "RO", Code = "40"},
            new Country {Name = "Russia", Alpha2 = "RU", Code = "7"},
            new Country {Name = "Rwanda", Alpha2 = "RW", Code = "250"},
            new Country {Name = "Saint Barthelemy", Alpha2 = "BL", Code = "590"},
            new Country {Name = "Saint Helena", Alpha2 = "SH", Code = "290"},
            new Country {Name = "Saint Kitts and Nevis", Alpha2 = "KN", Code = "1-869"},
            new Country {Name = "Saint Lucia", Alpha2 = "LC", Code = "1-758"},
            new Country {Name = "Saint Martin", Alpha2 = "MF", Code = "590"},
            new Country {Name = "Saint Pierre and Miquelon", Alpha2 = "PM", Code = "508"},
            new Country {Name = "Saint Vincent and the Grenadines", Alpha2 = "VC", Code = "1-784"},
            new Country {Name = "Samoa", Alpha2 = "WS", Code = "685"},
            new Country {Name = "San Marino", Alpha2 = "SM", Code = "378"},
            new Country {Name = "Sao Tome and Principe", Alpha2 = "ST", Code = "239"},
            new Country {Name = "Saudi Arabia", Alpha2 = "SA", Code = "966"},
            new Country {Name = "Senegal", Alpha2 = "SN", Code = "221"},
            new Country {Name = "Serbia", Alpha2 = "RS", Code = "381"},
            new Country {Name = "Seychelles", Alpha2 = "SC", Code = "248"},
            new Country {Name = "Sierra Leone", Alpha2 = "SL", Code = "232"},
            new Country {Name = "Singapore", Alpha2 = "SG", Code = "65"},
            new Country {Name = "Sint Maarten", Alpha2 = "SX", Code = "1-721"},
            new Country {Name = "Slovakia", Alpha2 = "SK", Code = "421"},
            new Country {Name = "Slovenia", Alpha2 = "SI", Code = "386"},
            new Country {Name = "Solomon Islands", Alpha2 = "SB", Code = "677"},
            new Country {Name = "Somalia", Alpha2 = "SO", Code = "252"},
            new Country {Name = "South Africa", Alpha2 = "ZA", Code = "27"},
            new Country {Name = "South Korea", Alpha2 = "KR", Code = "82"},
            new Country {Name = "South Sudan", Alpha2 = "SS", Code = "211"},
            new Country {Name = "Spain", Alpha2 = "ES", Code = "34"},
            new Country {Name = "Sri Lanka", Alpha2 = "LK", Code = "94"},
            new Country {Name = "Sudan", Alpha2 = "SD", Code = "249"},
            new Country {Name = "Suriname", Alpha2 = "SR", Code = "597"},
            new Country {Name = "Svalbard and Jan Mayen", Alpha2 = "SJ", Code = "47"},
            new Country {Name = "Swaziland", Alpha2 = "SZ", Code = "268"},
            new Country {Name = "Sweden", Alpha2 = "SE", Code = "46"},
            new Country {Name = "Switzerland", Alpha2 = "CH", Code = "41"},
            new Country {Name = "Syria", Alpha2 = "SY", Code = "963"},
            new Country {Name = "Taiwan", Alpha2 = "TW", Code = "886"},
            new Country {Name = "Tajikistan", Alpha2 = "TJ", Code = "992"},
            new Country {Name = "Tanzania", Alpha2 = "TZ", Code = "255"},
            new Country {Name = "Thailand", Alpha2 = "TH", Code = "66"},
            new Country {Name = "Togo", Alpha2 = "TG", Code = "228"},
            new Country {Name = "Tokelau", Alpha2 = "TK", Code = "690"},
            new Country {Name = "Tonga", Alpha2 = "TO", Code = "676"},
            new Country {Name = "Trinidad and Tobago", Alpha2 = "TT", Code = "1-868"},
            new Country {Name = "Tunisia", Alpha2 = "TN", Code = "216"},
            new Country {Name = "Turkey", Alpha2 = "TR", Code = "90"},
            new Country {Name = "Turkmenistan", Alpha2 = "TM", Code = "993"},
            new Country {Name = "Turks and Caicos Islands", Alpha2 = "TC", Code = "1-649"},
            new Country {Name = "Tuvalu", Alpha2 = "TV", Code = "688"},
            new Country {Name = "U.S. Virgin Islands", Alpha2 = "VI", Code = "1-340"},
            new Country {Name = "Uganda", Alpha2 = "UG", Code = "256"},
            new Country {Name = "Ukraine", Alpha2 = "UA", Code = "380"},
            new Country {Name = "United Arab Emirates", Alpha2 = "AE", Code = "971"},
            new Country {Name = "United Kingdom", Alpha2 = "GB", Code = "44"},
            new Country {Name = "United States", Alpha2 = "US", Code = "1"},
            new Country {Name = "Uruguay", Alpha2 = "UY", Code = "598"},
            new Country {Name = "Uzbekistan", Alpha2 = "UZ", Code = "998"},
            new Country {Name = "Vanuatu", Alpha2 = "VU", Code = "678"},
            new Country {Name = "Vatican", Alpha2 = "VA", Code = "379"},
            new Country {Name = "Venezuela", Alpha2 = "VE", Code = "58"},
            new Country {Name = "Vietnam", Alpha2 = "VN", Code = "84"},
            new Country {Name = "Wallis and Futuna", Alpha2 = "WF", Code = "681"},
            new Country {Name = "Western Sahara", Alpha2 = "EH", Code = "212"},
            new Country {Name = "Yemen", Alpha2 = "YE", Code = "967"},
            new Country {Name = "Zambia", Alpha2 = "ZM", Code = "260"},
            new Country {Name = "Zimbabwe", Alpha2 = "ZW", Code = "263"},
        };*/

        public static List<Country> Countries
        {
            get
            {
                if (ExpireTime is null || ExpireTime < DateTime.UtcNow || _countries is null)
                {
                    _countries = JsonConvert.DeserializeObject<List<Country>>(File.ReadAllText($"{EnvironmentHelper.Environment.WebRootPath}/countries.json"));
                    ExpireTime = DateTime.UtcNow + TimeSpan.FromMinutes(EnvironmentHelper.SeoConfiguration.RamCacheTime);
                }
                
                return _countries;
            }
        }

        public static DateTime? ExpireTime { get; set; }

        public class Country
        {
            [JsonProperty("CountryName")] public string Name;
            [JsonProperty("ISO3166")] public string Alpha2;
            [JsonProperty("CallingCode")] public string Code;
            [JsonProperty("LastDateUsed")] public DateTime? LastDateUsed;
            [JsonProperty("CurrencyCode")] public string CurrencyCode;
        }

        public static string Get3LetterIsoBy2(string iso2)
        {
            switch (iso2.ToLower())
            {
                case "en": return "ENG";
                case "fr": return "FRA";
                case "he": return "HEB";
                case "ru": return "RUS";
                case "ar": return "ARA";
                case "uk": return "UKR";
                case "es": return "SPA";
                default: return "ENG";
            }
        }
    }
}