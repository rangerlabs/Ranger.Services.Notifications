using Ranger.Common;

namespace Ranger.Services.Notifications
{
    public class TenantResult
    {
        public string TenantId { get; set; }
        public bool Enabled { get; set; }
        public string Domain { get; set; }
        public string OrganizationName { get; set; }
    }
}