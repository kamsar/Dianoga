using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Dianoga.Optimizers.Pipelines.DianogaWebP
{
    public class WebPOptimizer : CommandLineToolOptimizer
    {
        private bool? _browserSupportWebP;
        public virtual bool BrowserSupportWebP
        {
            get
            {
                if (_browserSupportWebP != null)
                {
                    return _browserSupportWebP.Value;
                }
                return (HttpContext.Current != null) 
                    && (HttpContext.Current.Request.AcceptTypes != null)
                    && (HttpContext.Current.Request.AcceptTypes.Contains("image/webp"));
            }
            set { _browserSupportWebP = value; }
        }

        public override void Process(OptimizerArgs args)
        {
            if (BrowserSupportWebP)
            {
                base.Process(args);
            }
        }

        protected override string CreateToolArguments(string tempFilePath, string tempOutputPath)
        {
            return $"\"{tempFilePath}\" -o \"{tempOutputPath}\" ";
        }
    }
}
