using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ranger.Services.Notifications.Data
{
    public class FrontendNotification
    {
        [Required]
        public string BackendEventKey { get; set; }

        [Required]
        public string PusherEventName { get; set; }

        public OperationsStateEnum OperationsState { get; set; }

        [Required]
        [StringLength(160)]
        public string Text { get; set; }
    }
}