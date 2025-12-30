using System.Globalization;
using System.Text;
using Menu4Tech.Helper;
using Menu4Tech.IBusinessAPIservice;
using Menu4Tech.Models;
using Menu4Tech.Models.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace Menu4Tech.Services
{
    public class MenuService : IMenu
    {

        private readonly IMemoryCache _memoryCache;
        private readonly ICredential _credential;
        private readonly BusinessAPIClient _businessAPIClient;
        public MenuService(IMemoryCache memoryCache, ICredential credential)
        {
            _memoryCache = memoryCache;
            _credential = credential;
            _businessAPIClient = EnvironmentHelper.BusinessApiConfiguration.InitClient();

        }

        public async Task<MenuResponse> GenerateMenuJsonAsync(int entityId, int businessId, string folderName, string regionInfo)
        {
            Menu jsonMenu = null;
            MenuResponse menuResponse = null;
            //if (_memoryCache.TryGetValue("IsProductsUpdated", out var _) == false)
            //{
            using (StreamReader reader = new StreamReader(folderName + "Menu" + regionInfo + ".json"))
            {
                string tempMenuJson = reader.ReadToEnd();
                jsonMenu = JsonConvert.DeserializeObject<Menu>(tempMenuJson);
                string userName = string.Empty;
                string password = string.Empty;
                if (_credential.GetEnv() == "true")
                {
                    userName = _credential.GetUserName();
                    password = _credential.GetPassword();
                }

                menuResponse = await GenerateMenuAsync(entityId, businessId, folderName, regionInfo);
                jsonMenu.SyncModifiedDate = DateTime.UtcNow;

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(5));
                _memoryCache.Set("IsProductsUpdated", false, cacheEntryOptions);


            }
            if (jsonMenu != null)
            {
                var json = JsonConvert.SerializeObject(jsonMenu);

                using (StreamWriter file = File.CreateText(folderName + "Menu" + regionInfo + ".json"))
                {
                    JsonSerializer serializer = new JsonSerializer();

                    //serialize object directly into file stream
                    serializer.Serialize(file, jsonMenu);
                }
            }

            return menuResponse;



            //else
            //{
            //    return ReadHTML(folderName, regionInfo);
            //}

        }


        public async Task<MenuResponse> GenerateMenuAsync(int entityId, int businessId, string folderName, string regionInfo)
        {


            var httpClient = new HttpClient();
            string userName = "";
            string password = "";

            if (_credential.GetEnv() == "true")
            {
                userName = _credential.GetUserName();
                password = _credential.GetPassword();
            }

            // Call the web service to retrieve the data
            //using (var response = await httpClient.PostAsync(webServiceUrl, content))
            //{
            var config = EnvironmentHelper.BusinessApiConfiguration;
            var client = config.InitClient();
            var entity = UserManager.Get(new Dictionary<string, string> { { "EntityId", $"{entityId}" } });
            var response = await client.Sales_Product_GetAsync(new Sales_Product_GetRequest(entityId, entity.Username, entity.Password, businessId,
                new string[] { "ProductName", "CategoryID", "Description", "Price", "smallimage", "SellPrice_CurrencyISO", "ProductName_" + regionInfo, "Description_"+regionInfo },
                new string[] { "IsDeleted", "order by"}, new string[] { "0", "order_index" }, 0, 0));
            //var productResponse = await response.Content.ReadAsStringAsync();

            //var tempResponse = XElement.Parse(response.@return);
            //string rec = JsonConvert.DeserializeObject<Product>(response.@return);
            //if (rec == "-1")
            //{
            //    MenuResponse menuResponseError = new MenuResponse();
            //    menuResponseError.IsUpdated = false;
            //    menuResponseError.Content = response.@return;
            //    return menuResponseError;
            //}
            var products = JsonConvert.DeserializeObject<IList<Product>>(response.@return);

            // Check whether the folder exists or not, If not create new one.
            if (products.Count > 0)
            {
                if (!Directory.Exists(folderName))
                {
                    Directory.CreateDirectory(folderName);
                }
                if (!Directory.Exists(folderName + @"products"))
                {
                    Directory.CreateDirectory(folderName + @"products");
                }
                if (!Directory.Exists(folderName + @"categories"))
                {
                    Directory.CreateDirectory(folderName + @"categories");
                }
            }

            // Download Product Logo.
            foreach (Product product in products)
            {
                foreach (var translationKey in product.JsonTranslations.Keys)
                {
                    if (translationKey.StartsWith("ProductName_"))
                    {
                        var translation = product.JsonTranslations[translationKey].ToString();
                        if (!string.IsNullOrWhiteSpace(translation))
                        {
                            product.Name = translation;
                        }
                        continue;
                    }

                    if (translationKey.StartsWith("Description_"))
                    {
                        var translation = product.JsonTranslations[translationKey].ToString();
                        if (!string.IsNullOrWhiteSpace(translation))
                        {
                            product.Description = translation;
                        }
                    }
                }

                using (FileStream fs = new FileStream(folderName + @"products\" + product.Id + ".jpg", FileMode.OpenOrCreate, FileAccess.Write))
                {
                    fs.Write(product.Logo, 0, product.Logo.Length);
                    fs.Close();
                }
            }

            
            var tempCategoryResponse = await client.Sales_Products_Categories_GetAsync(new Sales_Products_Categories_GetRequest(
                entityId, entity.Username, entity.Password, businessId,
                new string[] { "Name", "smallimage", "CategoryID", "Name_"+regionInfo },
                new string[] { "IsDeleted", "order by" }, new string[] { "0", "order_index" },
                0, 0));
            var categories = JsonConvert.DeserializeObject<IList<Category>>(tempCategoryResponse.@return);
            if (categories.Count > 0)
            {
                if (!Directory.Exists(folderName))
                {
                    Directory.CreateDirectory(folderName);
                }
            }
            foreach (var category in categories)
            {
                foreach (var translationKey in category.JsonTranslations.Keys)
                {
                    if (translationKey.StartsWith("Name_"))
                    {
                        var translation = category.JsonTranslations[translationKey].ToString();
                        if (!string.IsNullOrWhiteSpace(translation))
                        {
                            category.Name = translation;
                        }
                    }
                }

                var tempProducts = products.Where(x => x.CategoryId == category.Id).ToList();
                foreach (Product product in tempProducts)
                    category.Products.Add(product);

                using (FileStream fs = new FileStream(folderName + @"categories\" + category.Id + ".jpg", FileMode.OpenOrCreate, FileAccess.Write))
                {
                    fs.Write(category.Logo, 0, category.Logo.Length);
                    fs.Close();
                }
            }

            var menu = new RestorantMenu();
            // Todo Menu Logo needs to be retrieved
            menu.Categories = categories;
            menu.Logo = folderName + "logo.jpg";

            // Create Menu.json file for first time
            if (!File.Exists(folderName + "Menu" + regionInfo + ".json"))
            {
                Menu jsonMenu = new Menu();
                jsonMenu.SyncModifiedDate = DateTime.UtcNow;
                jsonMenu.LastViewDate = DateTime.UtcNow;
                using (StreamWriter file = File.CreateText(folderName + "Menu" + regionInfo + ".json"))
                {
                    JsonSerializer serializer = new JsonSerializer();

                    //serialize object directly into file stream
                    serializer.Serialize(file, jsonMenu);
                }
            }

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromSeconds(5));

            _memoryCache.Set("IsProductsUpdated", false, cacheEntryOptions);

            #region Generate Entity

            var result = await client.Entity_FindAsync(new Entity_FindRequest(entityId, entity.Username, entity.Password, businessId, false, new string[] { "Customfield1", "Company", "Address", "City", "Zip", "Country", "Phone" },
                new string[] { "entityID" }, new string[] { entityId.ToString() }, 0, 0));
            if (!Directory.Exists(folderName))
            {
                Directory.CreateDirectory(folderName);
            }
            var entityDetailsObj = JsonConvert.DeserializeObject<IList<Entity>>(result.@return);

            var entityFileName = folderName + "Entity" + ".json";
            if (File.Exists(entityFileName)){
                File.Delete(entityFileName);
            }

            using (StreamWriter file = new StreamWriter(folderName + "Entity" + ".json", false)) //System.IO.File.CreateText(folderName + "Entity" + ".json"))
            {
                JsonSerializer serializer = new JsonSerializer();

                //serialize object directly into file stream
                serializer.Serialize(file, entityDetailsObj);
            }
            #endregion


            MenuResponse menuResponse = new MenuResponse();            
            menuResponse = GenerateMenuHTML(menu, folderName, regionInfo);
            return menuResponse;

            // }
            // }

        }
        public MenuResponse GenerateMenuHTML(RestorantMenu restaurantMenu, string folderName, string culture)
        {
            restaurantMenu.Logo = $@"Static\{restaurantMenu.Logo}";
            StringBuilder stringBuilder = new StringBuilder();
            string currencySymbol = string.Empty;
            MenuResponse menuResponse = new MenuResponse();
            
            try
            {
                if (culture.Trim().ToUpper().Equals("HEB"))
                    stringBuilder.Append($"<div class=\"form-group has-search\"><span class=\"fa fa-search form-control-feedback\"></span><input id=\"SearchContent\" type =\"text\" class=\"form-control\" placeholder=\"Search\" dir=\"rtl\"></div><div id=\"navlist\" dir=\"rtl\">");
                else
                    stringBuilder.Append($"<div class=\"form-group has-search\"><span class=\"fa fa-search form-control-feedback\"></span><input id=\"SearchContent\" type =\"text\" class=\"form-control\" placeholder=\"Search\"></div><div id=\"navlist\">");
                foreach (var categories in restaurantMenu.Categories)
                {
                    if (categories.Products.Count >= 1)
                        stringBuilder.Append($"<a href=#{categories.Id}><button class=\"btn\">{categories.Name}</button></a>");
                }

                stringBuilder.Append($"</div><div class=\"container container-sm container-md container-lg container-xl\">");
                foreach (var categories in restaurantMenu.Categories)
                {
                    if (categories.Products.Count >= 1)
                    {
                        if (culture.Trim().ToUpper().Equals("HEB"))
                        {
                            stringBuilder.Append($"<div class=\"allprod\" id={categories.Id}>");
                            stringBuilder.Append($"<div class=\"catheading\"><div class=\"row cate\" style=\" border-bottom:2px solid #000;\"></div>");
                            stringBuilder.Append($"<div class=\"catheadingnew\" id={categories.Id}><div class=\"col-12 col-sm-12 col-md-12 col-lg-12 col-xl-12 text-center\" style=\"font-size:xx-large\"><span class=\"text-center\">{categories.Name}</span></div></div>");
                            stringBuilder.Append($"<div class=\"row cate\" style=\" border-bottom:2px solid #000;margin-bottom:10px;\"></div></div>");
                            foreach (var product in categories.Products)
                            {
                                var price = product.Price.ToString();
                                string[] strSplits = price.Split(new char[] { '.' });
                                if (strSplits.Length <= 1)
                                    price = string.Concat(price, ".00");
                                if (strSplits.Length == 2 && strSplits[1].Length <= 1)
                                    price = string.Concat(price, "0");
                                string imagePath = @$"{ folderName + @"products\" + product.Id + ".jpg"}";
                                long length = new FileInfo(@$"{ folderName + @"products\" + product.Id + ".jpg"}").Length;
                                bool isFileExists = (length > 0) ? true : false;

                                if (product.IsVisible) continue;
                                TryGetCurrencySymbol(product.CurrencyISO, out currencySymbol);
                                stringBuilder.Append($"<div class=\"row no-gutter prod\" style=\"justify-content:center;margin-bottom:10px;\" dir=\"rtl\">");

                                if (isFileExists)
                                    stringBuilder.Append($"<div class=\"col-1 col-sm-1 col-md-1\"><div><img class=\"imageHeight\" src=\"\\static\\{imagePath}\"></div></div>");
                                else
                                    stringBuilder.Append($"<div class=\"col-1 col-sm-1 col-md-1\"><div class=\"noImageHeight\"></div></div>");

                                stringBuilder.Append($"<div class=\"col-10 col-sm-10 col-md-11 col-lg-11 col-xl-11\"><div class=\"container container-sm container-md container-lg container-xl\" style=\"text-align:right;\"><div class=\"row margin-lt\">");
                                stringBuilder.Append($"<div style=\"width:85%;padding:0px;\"><div class=\"classic33yXj\" ><span class=\"classic1PykL\" style=\"float:right\"><span style= \"font-size:18px;float:right\">{ product.Name}</span></span>");

                                stringBuilder.Append($"<div class=\"classic1qdUp\" aria-hidden=\"true\" role=\"presentation\">");
                                stringBuilder.Append($".........................................................................................................................................................................................................................................................");
                                stringBuilder.Append($"</div></div></div>");
                                stringBuilder.Append($"<div style=\"align-self:flex-end;width:5%;padding:0px;\" dir=\"rtl\"><div class=\"font-weight-bold\" style=\"color:#8B0000;text-align:right;\"> {currencySymbol + price }</div></div>");
                                stringBuilder.Append($"</div>");
                                stringBuilder.Append($"<div class=\"row margin-lt\"><div class=\"col-12 col-sm-11 col-md-11 col-lg-12 col-xl-12 text-right\" style=\"font-size:15px;white-space:pre-line;padding:0px;\">{product.Description.Replace("\n", "<br/>")}</div></div>");
                                stringBuilder.Append($"</div></div>");
                                stringBuilder.Append($"</div>");
                            }
                            stringBuilder.Append($"</div>");
                        }
                        else
                        {
                            stringBuilder.Append($"<div class=\"allprod\" id={categories.Id}>");
                            stringBuilder.Append($"<div class=\"catheading\"><div class=\"row cate\" style=\" border-bottom:2px solid #000;\"></div>");
                            stringBuilder.Append($"<div class=\"catheadingnew\" id={categories.Id}><div class=\"col-12 col-sm-12 col-md-12 col-lg-12 col-xl-12 text-center\" style=\"font-size:xx-large\"><span data-name={categories.Name} class=\"text-center\">{categories.Name}</span></div></div>");
                            stringBuilder.Append($"<div class=\"row cate\" style=\" border-bottom:2px solid #000;margin-bottom:10px;\"></div></div>");
                            foreach (var product in categories.Products)
                            {
                                if (product.IsVisible) continue;
                                var price = product.Price.ToString();
                                string[] strSplits = price.Split(new char[] { '.' });
                                if (strSplits.Length <= 1)
                                    price = string.Concat(price, ".00");
                                if (strSplits.Length == 2 && strSplits[1].Length <= 1)
                                    price = string.Concat(price, "0");
                                TryGetCurrencySymbol(product.CurrencyISO, out currencySymbol);
                                stringBuilder.Append($"<div class=\"row no-gutter prod\" style=\"margin-bottom:10px;\">");

                                string imagePath = @$"{ folderName + @"products\" + product.Id + ".jpg"}";
                                long length = new FileInfo(@$"{ folderName + @"products\" + product.Id + ".jpg"}").Length;
                                bool isFileExists = (length > 0) ? true : false;
                                imagePath = @$"Static\{ folderName + @"products\" + product.Id + ".jpg"}";
                                if (isFileExists)
                                    stringBuilder.Append($"<div class=\"col-1 col-sm-1 col-md-1\"><div><img class=\"imageHeight\" src=\"\\{imagePath}\"></div></div>");
                                else
                                    stringBuilder.Append($"<div class=\"col-1 col-sm-1 col-md-1\"><div class=\"noImageHeight\"></div></div>");


                                stringBuilder.Append($"<div class=\"col-10 col-sm-10 col-md-11 col-lg-11 col-xl-11\"><div class=\"container container-sm container-md container-lg container-xl\"><div class=\"row margin-lt\">");


                                stringBuilder.Append($"<div style=\"width:85%;margin-left: 13px;\"><div class=\"classic33yXj\" data-name={ product.Name}><span class=\"classic1PykL\"><span style=\"font-size:18px\">{ product.Name}</span></span>");

                                stringBuilder.Append($"<div class=\"classic1qdUp\"  aria-hidden=\"true\" role=\"presentation\">");
                                stringBuilder.Append($".........................................................................................................................................................................................................................................................");
                                stringBuilder.Append($"</div></div>");
                                stringBuilder.Append($"</div>");
                                stringBuilder.Append($"<div style=\"align-self:flex-end;width:5%;\"><div class=\"font-weight-bold \" style=\"color:#8B0000;\"> { currencySymbol + price}</div></div></div>");

                                stringBuilder.Append($"<div class=\"row margin-lt\"><div class=\"col-12 col-sm-11 col-md-11 col-lg-12 col-xl-12\" style=\"font-size:15px;white-space:pre-line;\" data-name={ product.Name}>{product.Description.Replace("\n", "<br/>")}</div></div>");
                                stringBuilder.Append($"</div></div></div>");
                            }
                            stringBuilder.Append($"</div>");
                        }
                    }
                }
                stringBuilder.Append($"</div>");
                System.IO.File.WriteAllText(folderName + "Menu" + culture + ".html", stringBuilder.ToString());
                menuResponse.IsSuccess = true;
                menuResponse.Response = "Sync Successfuly.";
                _memoryCache.Set("MenuHtml", stringBuilder.ToString());

            }
            catch (Exception es)
            {
                menuResponse.IsSuccess = false;
                menuResponse.Response = es.Message;
                throw;                
            }
            return menuResponse;
        }

        public MenuResponse ReadHTML(string folderName, string regionInfo)
        {
            string menuHtml = string.Empty;
            bool isUpdated = false;
            using (StreamReader streamReader = new StreamReader(folderName + "Menu" + regionInfo + ".html", Encoding.UTF8))
            {
                string contents = streamReader.ReadToEnd();
                _memoryCache.Set("MenuHtml", contents);
                menuHtml = contents;
            }
            if (_memoryCache.TryGetValue("MenuHtml", out var tempMenuHtml))
            {
                //if (!menuHtml.Equals(tempMenuHtml))
                isUpdated = true;
            }
            return new MenuResponse { IsUpdated = isUpdated, Content = menuHtml };
        }


        private bool TryGetCurrencySymbol(string ISOCurrencySymbol, out string symbol)
        {
            symbol = CultureInfo
                .GetCultures(CultureTypes.AllCultures)
                .Where(c => !c.IsNeutralCulture)
                .Select(culture =>
                {
                    try
                    {
                        return new RegionInfo(culture.Name);
                    }
                    catch
                    {
                        return null;
                    }
                })
                .Where(ri => ri != null && ri.ISOCurrencySymbol == ISOCurrencySymbol)
                .Select(ri => ri.CurrencySymbol)
                .FirstOrDefault();
            return symbol != null;
        }
    }
}
