using System.Collections.Generic;
using Ranger.Services.Notifications.Data;

namespace Ranger.Services.Notifications
{
    public static class FrontendNotificationsConfig
    {
        public static IEnumerable<FrontendNotification> GetFrontendNotifications()
        {
            return new List<FrontendNotification>()
            {
                TenantOnboardingCompleteNotification,
                TenantOnboardingRejectedNotification,
                TokenRefreshNotification,
                PermissionsUpdatedNotification,
                ForceSignoutNotification
            };
        }

        private static FrontendNotification TenantOnboardingCompleteNotification = new FrontendNotification()
        {
            BackendEventKey = "TenantOnboarding",
            OperationsState = OperationsStateEnum.Completed,
            PusherEventName = "tenant-onboard",
            Text = "Your domain has been successfully created. Check your email to complete your registration"
        };
        private static FrontendNotification TenantOnboardingRejectedNotification = new FrontendNotification()
        {
            BackendEventKey = "TenantOnboarding",
            OperationsState = OperationsStateEnum.Rejected,
            PusherEventName = "tenant-onboard",
            Text = "An issue arose creating your domain. We'll investigate and be in touch shortly"
        };
        private static FrontendNotification TokenRefreshNotification = new FrontendNotification()
        {
            BackendEventKey = "TokenRefresh",
            PusherEventName = "token-refresh",
            Text = ""
        };
        private static FrontendNotification PermissionsUpdatedNotification = new FrontendNotification()
        {
            BackendEventKey = "PermissionsUpdated",
            PusherEventName = "permissions-updated",
            Text = "Your permissions were just updated and will be available to you momentarily. Check your email for more detailed information"
        };
        private static FrontendNotification ForceSignoutNotification = new FrontendNotification()
        {
            BackendEventKey = "ForceSignoutNotification",
            PusherEventName = "force-signout",
            Text = "",
        };
    }
}