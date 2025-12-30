namespace Menu4Tech.Models.Interfaces
{
    public interface IViewHelper
    {
        Task<GeneralBusiness> GetGeneralBusinessByEntityId(int entityId);
        List<Language> ParseLanguagesFile();
        Task<List<Language>> GetLanguages(int entityId);
        void SetBusinessValues(int entityId, int businessId, string culture);

        int BusinessId { get; }
        int EntityId { get; }

        string Culture { get; }

        string Mobile { get; set; }

    }
}
