namespace Menu4Tech.Models
{
    public class EntityResponse
    {
        public int ResultCode { get; set; }
        public string ResultMessage { get; set; }
        public int? ExecuteTime { get; set; }

    }
    public class GeneralFunctionAPIModel :EntityResponse
    {
        public string Mobile { get; set; }
    }
    public class EntityFindAPIModel : EntityResponse
    {
        public string EntityId { get; set; }
        public string Email { get; set; }
        public string mobile_verified { get; set; }
        public string ol_username { get; set; }
        public string Phone { get; set; }
    }
    public class LoginAPIModel : EntityResponse
    {
        public int EntityId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Zip { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
                 
    }
}
