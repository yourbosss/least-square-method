using OxyPlot.Series;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab4._1 {
    public class MainViewModel {
        public MainViewModel() {
            this.MyModel = new PlotModel { Title = "Example 1" };
            this.MyModel.Series.Add(new FunctionSeries(Math.Cos, 0, 10, 0.1, "cos(x)"));
            this.MyModel.InvalidatePlot(true);
        }
        //refresh oxyplot invalidate
        public PlotModel MyModel { get; private set; }
    }
}
