namespace Menu4Tech.Models.Interfaces
{
    public interface ILogRequest
    {
        public Task EntityLogAsync(int entityId,string ip);
    }
}
