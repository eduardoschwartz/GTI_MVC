using System;
using System.ComponentModel.DataAnnotations;

namespace GTI_Models.Models {
    public class Calendar_event {
        [Key]
        public int EventId { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public  DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string ThemeColor { get; set; }
        public bool IsFullDay { get; set; }
    }
}
