﻿using System;
using System.Collections.Generic;

namespace ChilLaxFrontEnd.Models
{
    public partial class FocusSlide
    {
        public int FocusId { get; set; }
        public string Category { get; set; } = null!;
        public string ImagePath { get; set; } = null!;
        public string AudioPath { get; set; } = null!;
    }
}
