using Menu4Tech.Helper;
using Menu4Tech.IBusinessAPIservice;
using Menu4Tech.Models;
using Menu4Tech.Models.Interfaces;
using Newtonsoft.Json;

namespace Menu4Tech.Services
{
    public class ViewHelper : IViewHelper
    {
        private readonly ICredential _credential;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _httpContext;

        private GeneralBusiness GeneralBusiness
        {
            get
            {
                try
                {
                    GeneralBusiness business; 
                    if (_httpContext.HttpContext.Request.Cookies.ContainsKey("Business"))
                    {
                        string businessString = CryptString.Decrypt(_httpContext.HttpContext.Request.Cookies["Business"]);
                        business = JsonConvert.DeserializeObject<GeneralBusiness>(businessString);
                    }
                    else
                    {
                        business = new GeneralBusiness() { ResultCode = -1 };
                    }
                    return business;
                }
                catch
                {
                    return new GeneralBusiness() { ResultCode = -1 };
                }
            }
            set
            {
                _httpContext.HttpContext.Response.Cookies.Append("Business",
                    CryptString.Encrypt(JsonConvert.SerializeObject(value)),
                    new CookieOptions { Expires = DateTime.Now.AddDays(7) });
            }
        }

        public string Mobile
        {
            get
            {
                try
                {
                    string mobile;
                    if (_httpContext.HttpContext.Request.Cookies.ContainsKey("Mobile"))
                    {
                        string mobileString = CryptString.Decrypt(_httpContext.HttpContext.Request.Cookies["Mobile"]);
                        mobile = JsonConvert.DeserializeObject<string>(mobileString);
                    }
                    else
                    {
                        mobile = string.Empty;
                    }
                    return mobile;
                }
                catch
                {
                    return string.Empty;
                }
            }
            set
            {
                _httpContext.HttpContext.Response.Cookies.Append("Mobile",
                    CryptString.Encrypt(JsonConvert.SerializeObject(value)),
                    new CookieOptions { Expires = DateTime.Now.AddDays(7) });
            }
        }

        public int BusinessId { get; private set; }

        public int EntityId { get; private set; }

        public string Culture { get; private set; }

        public ViewHelper(ICredential credential, IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContext)
        {
            _credential = credential;
            _webHostEnvironment = webHostEnvironment;
            _httpContext = httpContext;
        }

        public async Task<GeneralBusiness> GetGeneralBusinessByEntityId(int entityId)
        {
            if (GeneralBusiness.ResultCode != -1)
                return GeneralBusiness;

            var config = EnvironmentHelper.BusinessApiConfiguration;
            var _businessAPIClient = config.InitClient();
            var entity = UserManager.Get(new Dictionary<string, string> { { "EntityId", $"{entityId}" } });
            var result = await _businessAPIClient.General_Business_GetAsync(new General_Business_GetRequest(entityId, entity.Username, entity.Password, new string[] { "e.EntityID" }, new string[] { entityId.ToString() }, 0, 0));
            try
            {
                var response = JsonConvert.DeserializeObject<IList<GeneralBusiness>>(result.@return).First();
                if (response != null)
                {
                    GeneralBusiness = response;
                    return response;
                }
                return new GeneralBusiness()
                {
                    ResultCode = -1,
                    ResultMessage = "Business not found"
                };
            }
            catch
            {
                if (result.@return == "[]")
                {
                    return new GeneralBusiness()
                    {
                        ResultMessage = "Business not found"
                    };
                }
                var response = JsonConvert.DeserializeObject<EntityResponse>(result.@return);
                return new GeneralBusiness()
                {
                    ResultMessage = response.ResultMessage
                };
            }
        }

        public async Task<List<Language>> GetLanguages(int entityId)
        {
            var data = ParseLanguagesFile();
            var generalBusiness = await GetGeneralBusinessByEntityId(entityId);
            var languages = new List<Language>();


            if (generalBusiness.languagesISO == null)
                return new List<Language> {
                    data.FirstOrDefault(x => x.Iso == "heb"),
                    data.FirstOrDefault(x => x.Iso == "eng"),
                    data.FirstOrDefault(x => x.Iso == "rus")
                };

            string[] lngs = generalBusiness.languagesISO.Split(',');

            for (int i = 0; i < lngs.Length; i++)
            {
                Language language = data.FirstOrDefault(x => x.Iso == lngs[i].ToLower());
                if (language == null) continue;
                languages.Add(language);
            }
            return languages;
        }

        public List<Language> ParseLanguagesFile()
        {
            List<Language> languages = new List<Language>();
            string contentRoot = _webHostEnvironment.ContentRootPath;
            string langFilePath = Path.Combine(contentRoot, "Files", "languages-iso.csv");
            if (!File.Exists(langFilePath))
            {
                return new List<Language>();
            }

            string[] fileLines = File.ReadAllLines(langFilePath);

            foreach (string line in fileLines)
            {
                Language language = new Language
                {
                    Iso = line.Split(',')[0].ToLower(),
                    Name = line.Split(',')[1].Replace(';', ','),
                    CountryIso = line.Split(',')[2]
                };
                languages.Add(language);
            }

            return languages;
        }

        public void SetBusinessValues(int entityId, int businessId, string culture)
        {
            BusinessId = businessId;
            EntityId = entityId;
            Culture = culture;
        }
    }
}
