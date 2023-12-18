using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheLostLand.Configs
{
    public abstract class ModuleConfiguration : IConfig
    {
        public abstract void LoadDefaults();
    }
}
