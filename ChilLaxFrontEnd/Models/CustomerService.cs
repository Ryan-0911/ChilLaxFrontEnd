﻿using System;
using System.Collections.Generic;

namespace ChilLaxFrontEnd.Models
{
    public partial class CustomerService
    {
        public int CustomerServiceId { get; set; }
        public int MemberId { get; set; }
        public string? Message { get; set; }
        public string? Reply { get; set; }
        public string? MessageDatetime { get; set; }
        public string? ReplyDatetime { get; set; }

        public virtual Member Member { get; set; } = null!;
    }
}
