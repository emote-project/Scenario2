using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perception
{
    class EyebrowsController
    {
        // Maximum number of measure kept in memory. These will be use to compute an average to smoth possible outliers. 
        const int MAX_HISTORY = 10;

        // brow raiser
        List<double> lastAU2 = new List<double>();
        // brow lower
        List<double> lastAU4 = new List<double>();

        public double AU2BrowRaiser
        {
            get
            {
                if (lastAU2.Count > 0)
                    return lastAU2.Average();
                else
                    return 0;
            }
        }
        public double AU4BrowLower
        {
            get
            {
                if (lastAU4.Count > 0)
                    return lastAU4.Average();
                else
                    return 0;
            }
        }

        public void SetBrowsValues(double au2, double au4)
        {
            lastAU2.Add(au2);
            lastAU4.Add(au4);

            if (lastAU2.Count > MAX_HISTORY) lastAU2.RemoveAt(0);
            if (lastAU4.Count > MAX_HISTORY) lastAU4.RemoveAt(0);
        }
    }
}
