using Flow.Data;

namespace Flow.Models
{
    public class OrganizationLayoutViewModel
    {
        public bool UserHasExistingOrganization { get; set; }
        public List<Organization> UserOrganizations { get; set; }
    }
}
