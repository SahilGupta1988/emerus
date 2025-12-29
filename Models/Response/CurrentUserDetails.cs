namespace Emerus.ETM.Admin.Models.Response
{
    public class CurrentUserDetails
    {
        public bool IsAuthenticated { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
    }
}
