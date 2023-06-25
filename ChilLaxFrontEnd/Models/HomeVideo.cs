using System;
using System.Collections.Generic;

namespace ChilLaxFrontEnd.Models;

public partial class HomeVideo
{
    public int VideoId { get; set; }

    public string VideoName { get; set; } = null!;

    public string VideoPath { get; set; } = null!;
}
