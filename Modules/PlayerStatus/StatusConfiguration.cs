using PlayerStatus.Models;
using RetroMedieval.Modules.Configuration;

namespace PlayerStatus;

internal class StatusConfiguration : IConfig
{
    public ushort Uiid { get; set; }
    public UIPart Health { get; set; }
    public UIPart Hunger { get; set; }
    public UIPart Water { get; set; }
    public UIPart Stamina { get; set; }
    
    public void LoadDefaults()
    {
        Uiid = 17102;
        Health = new UIPart
        {
            ChildName = "HealthIcon",
            Ranges =
            [
                new ImageChangeRange
                {
                    MaxValue = 100,
                    MinValue = 75,
                    ImageURL = ""
                },
                new ImageChangeRange
                {
                    MaxValue = 74,
                    MinValue = 50,
                    ImageURL = ""
                },
                new ImageChangeRange
                {
                    MaxValue = 49,
                    MinValue = 25,
                    ImageURL = ""
                },
                new ImageChangeRange
                {
                    MaxValue = 24,
                    MinValue = 0,
                    ImageURL = ""
                }
            ]
        };
        Hunger = new UIPart
        {
            ChildName = "HungerIcon",
            Ranges =
            [
                new ImageChangeRange
                {
                    MaxValue = 100,
                    MinValue = 75,
                    ImageURL = ""
                },
                new ImageChangeRange
                {
                    MaxValue = 74,
                    MinValue = 50,
                    ImageURL = ""
                },
                new ImageChangeRange
                {
                    MaxValue = 49,
                    MinValue = 25,
                    ImageURL = ""
                },
                new ImageChangeRange
                {
                    MaxValue = 24,
                    MinValue = 0,
                    ImageURL = ""
                }
            ]
        };
        Water = new UIPart
        {
            ChildName = "WaterIcon",
            Ranges =
            [
                new ImageChangeRange
                {
                    MaxValue = 100,
                    MinValue = 75,
                    ImageURL = ""
                },
                new ImageChangeRange
                {
                    MaxValue = 74,
                    MinValue = 50,
                    ImageURL = ""
                },
                new ImageChangeRange
                {
                    MaxValue = 49,
                    MinValue = 25,
                    ImageURL = ""
                },
                new ImageChangeRange
                {
                    MaxValue = 24,
                    MinValue = 0,
                    ImageURL = ""
                }
            ]
        };
        Stamina = new UIPart
        {
            ChildName = "StaminaIcon",
            Ranges =
            [
                new ImageChangeRange
                {
                    MaxValue = 100,
                    MinValue = 75,
                    ImageURL = ""
                },
                new ImageChangeRange
                {
                    MaxValue = 74,
                    MinValue = 50,
                    ImageURL = ""
                },
                new ImageChangeRange
                {
                    MaxValue = 49,
                    MinValue = 25,
                    ImageURL = ""
                },
                new ImageChangeRange
                {
                    MaxValue = 24,
                    MinValue = 0,
                    ImageURL = ""
                }
            ]
        };
    }
}