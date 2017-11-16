using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dianoga.Optimizers.Pipelines.DianogaWebP
{
    public class WebPOptimizer : CommandLineToolOptimizer
    {
        protected override string CreateToolArguments(string tempFilePath, string tempOutputPath)
        {
            return $"\"{tempFilePath}\" -o \"{tempOutputPath}\" ";
        }
    }
}
