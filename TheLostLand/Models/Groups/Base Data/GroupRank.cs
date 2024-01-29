namespace TheLostLand.Models.Groups.Base_Data;

public class GroupRank
{
    public int RankID { get; set; }
    public string RankName { get; set; }
    
    public bool AcceptUsers { get; set; }
    public bool KickUsers { get; set; }
    public bool BanUsers { get; set; }
    public bool UnbanUsers { get; set; }
}