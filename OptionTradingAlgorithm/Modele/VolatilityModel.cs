using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptionTradingAlgorithm.Modele
{
    public abstract class VolatilityModel
    {
        public abstract double GetVolatility(double time);

    }
}
