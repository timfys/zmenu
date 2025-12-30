namespace Menu4Tech.Models.Interfaces
{
    public interface IMenu
    {
        public Task<MenuResponse> GenerateMenuJsonAsync(int entityId, int businessId, string folderName, string regionInfo);
        public Task<MenuResponse> GenerateMenuAsync(int entityId, int businessId, string folderName, string regionInfo);
        public MenuResponse GenerateMenuHTML(RestorantMenu restaurantMenu, string folderName, string culture);
        public MenuResponse ReadHTML(string folderName, string regionInfo);
    }
}
