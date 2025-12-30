namespace Menu4Tech.Models.Interfaces
{
    public interface ICredential
    {
        public string GetUserName();
        public string GetPassword();
        public string GetWebServiceUrl();
        public string GetEnv();
        public string GetEntityId();
    }
}
