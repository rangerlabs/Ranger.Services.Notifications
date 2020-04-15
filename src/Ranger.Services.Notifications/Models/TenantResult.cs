using Ranger.Common;

namespace Ranger.Services.Notifications
{
    public class TenantResult : ContextTenant
    {
        public TenantResult(string tenantId, string domain, string databasePassword, bool enabled) : base(tenantId, databasePassword, enabled)
        { }

        public string Domain { get; set; }
        public string OrganizationName { get; set; }
    }
}