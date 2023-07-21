using System;
using System.Collections.Generic;

namespace ChilLaxFrontEnd.Models
{
    public partial class FocusDetail
    {
        public string FocusDetailId { get; set; } = null!;
        public DateTime StartDatetime { get; set; }
        public DateTime EndDatetime { get; set; }
        public int Duration { get; set; }
    }
}
