using System;

namespace TheBrokeClub.API.Models;

public class QuoteDailyUsage
{
    public DateTime Day { get; set; }
    public int Used { get; set; } = 0;
}
