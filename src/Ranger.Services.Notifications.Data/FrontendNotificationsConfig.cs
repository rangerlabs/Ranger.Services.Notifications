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
                TenantOnboardingRejectedNotification
            };
        }

        private static FrontendNotification TenantOnboardingCompleteNotification = new FrontendNotification()
        {
            BackendEventKey = "TenantOnboarding",
            OperationsState = OperationsStateEnum.Completed,
            PusherEventName = "tenant-onboard",
            Text = "Your domain has been successfully created. Check your email to complete your registration."
        };
        private static FrontendNotification TenantOnboardingRejectedNotification = new FrontendNotification()
        {
            BackendEventKey = "TenantOnboarding",
            OperationsState = OperationsStateEnum.Rejected,
            PusherEventName = "tenant-onboard",
            Text = "An issue arose creating your domain. We'll investigate and be in touch shortly."
        };
    }
}