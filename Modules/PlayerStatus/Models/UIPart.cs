using System.Collections.Generic;

namespace PlayerStatus.Models;

internal class UIPart
{
    public string ChildName { get; set; }
    public List<ImageChangeRange> Ranges { get; set; }
}