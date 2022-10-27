using System;
using System.Collections.Generic;

namespace AzureCloudCalendarTestWebAPI.Models
{
    public partial class Calendar
    {
        public Guid CalendarId { get; set; }
        public string CalendarName { get; set; }
        public string Timezone { get; set; }
        public string CalendarDescription { get; set; }
        public string CalendarCategory { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public string CreatedBy { get; set; }
        public int? ReminderMinutes { get; set; }
        public string ReminderAction { get; set; }
        public bool? Status { get; set; }
        public DateTime? CalendarEventStartDate { get; set; }
        public DateTime? CalendarEventEndDate { get; set; }
    }
}
