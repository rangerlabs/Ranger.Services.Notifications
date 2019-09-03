using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ranger.Services.Notifications
{
    public class FrontendNotification
    {
        public int Id { get; set; }
        [Required]
        public string BackendEventName { get; set; }
        [Required]
        public string PusherEventName { get; set; }
        [Required]
        [StringLength(160)]
        public string Text { get; set; }
    }
}