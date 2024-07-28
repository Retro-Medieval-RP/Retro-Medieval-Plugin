using RetroMedieval.Modules.Configuration;

namespace PunchLimiter;

internal class LimiterConfig : IConfig
{
    public int DamageValue { get; set; }
    
    public void LoadDefaults()
    {
        DamageValue = 5;
    }
}