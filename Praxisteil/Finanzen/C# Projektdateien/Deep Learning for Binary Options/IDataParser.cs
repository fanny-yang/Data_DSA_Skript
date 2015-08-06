using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deep_Learning_for_Binary_Options
{
    interface IDataParser
    {
        double[][] GetSamples();
        double[][] GetLabels();
        int[] GetLayerSizes();
    }
}
