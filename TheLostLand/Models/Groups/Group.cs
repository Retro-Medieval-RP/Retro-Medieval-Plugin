using System.Collections.Generic;

namespace TheLostLand.Models.Groups;

public class Group
{
    public string GroupName { get; set; }
    public List<ulong> GroupMembers { get; set; }
}