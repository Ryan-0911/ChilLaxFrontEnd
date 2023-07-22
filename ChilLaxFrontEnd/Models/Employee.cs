﻿using System;
using System.Collections.Generic;

namespace ChilLaxFrontEnd.Models
{
    public partial class Employee
    {
        public int EmpId { get; set; }
        public bool EmpPermission { get; set; }
        public string EmpName { get; set; } = null!;
        public string EmpAccount { get; set; } = null!;
        public string EmpPassword { get; set; } = null!;
        public bool? Available { get; set; }
    }
}
