﻿using OxyPlot.Series;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace lab4._1 {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private PlotModel _plotModel;
        private int polinomes = 1;

        public MainWindow() {
            InitializeComponent();
            _plotModel = new PlotModel();
            _plotModel.InvalidatePlot(true);

            var col1 = new DataGridTextColumn();
            col1.Binding = new Binding("x");
            col1.Header = "X";

            var col2 = new DataGridTextColumn();
            col2.Binding = new Binding("y");
            col2.Header = "Y";

            DataGrid.Columns.Add(col1);
            DataGrid.Columns.Add(col2);
            RadioButton1.IsChecked = true;
        }

        bool isFirstData = true;
        private void SetProperty_OnClick(object sender, RoutedEventArgs e) {
            var x = double.Parse(XBlock.Text);
            var y = double.Parse(YBlock.Text);
            bool isEqual = false;

            if (isFirstData) {
                DataGrid.Items.Add(new XY { x = x, y = y });

                var point = new LineSeries {
                    Color = OxyColors.Black,
                    MarkerStroke = OxyColors.Black,
                    MarkerSize = 2,
                    MarkerType = MarkerType.Circle
                };
                point.Points.Add(new DataPoint(x, y));
                _plotModel.Series.Add(point);
                _plotModel.InvalidatePlot(true);

                PlotView.Model = _plotModel;
                isFirstData = false;
            }

            foreach (XY xy in DataGrid.Items) {
                if(x == xy.x && y == xy.y) { isEqual = true; }
            }
            if (!isEqual) {
                DataGrid.Items.Add(new XY { x = x, y = y });

                var point = new LineSeries {
                    Color = OxyColors.Black,
                    MarkerStroke = OxyColors.Black,
                    MarkerSize = 2,
                    MarkerType = MarkerType.Circle
                };
                point.Points.Add(new DataPoint(x, y));
                _plotModel.Series.Add(point);
                _plotModel.InvalidatePlot(true);

                PlotView.Model = _plotModel;
            }
            

            
            
        }

        private void Calculate_OnClick(object sender, RoutedEventArgs e) {
            if (DataGrid.Items.Count <= polinomes) {
                MessageBox.Show("Количество точек не должно быть меньше полинома");
                return;
            }
            

            var x = new List<double>();
            var y = new List<double>();
            foreach (XY xy in DataGrid.Items) {
                x.Add(xy.x);
                y.Add(xy.y);
            }


            var myReg = new LSM(x.ToArray(), y.ToArray());
            myReg.Polynomial(polinomes);
            foreach (var coeff in myReg.Coeff) {
                Console.WriteLine(coeff);
            }

            Console.WriteLine(myReg.Delta);
            Func<double, double> func;

            double sumY = 0;
            foreach (XY xy in DataGrid.Items) {
                sumY += xy.y;
            }

            double notY = sumY / DataGrid.Items.Count;
            double SStot = 0;
            double SSres = 0;
            if (polinomes == 1) {
                func = (x1) => {
                    ABlock.Text = $"a = {myReg.Coeff[1].ToString()}";
                    BBlock.Text = $"b = {myReg.Coeff[0].ToString()}";
                    CBlock.Text = String.Empty;

                    return myReg.Coeff.Last() * x1 + myReg.Coeff.First();
                };
            } else {
                func = (x1) => {
                    ABlock.Text = $"a = {myReg.Coeff[2].ToString()}";
                    BBlock.Text = $"b = {myReg.Coeff[1].ToString()}";
                    CBlock.Text = $"c = {myReg.Coeff[0].ToString()}";

                    return myReg.Coeff[2] * x1 * x1 + myReg.Coeff[1] * x1 + myReg.Coeff[0];
                };
            }

            double xMin = Double.MaxValue;
            double xMax = Double.MinValue
                ;
            foreach (XY xy in DataGrid.Items) {
                SStot += ( xy.y - notY ) * ( xy.y - notY );

                xMin = Math.Min(xMin,xy.x );
                xMax = Math.Max(xMax, xy.x);

                SSres += ( xy.y - func(xy.x) ) *
                         ( xy.y - func(xy.x) );
            }

            var r = Math.Sqrt(1 - ( SSres / SStot ));
            if (double.IsNaN(r)) {
                r = 1;
            }
            CoeffBlock.Text = $"determination = {r}";
            if (_plotModel.Series.Any(series => series is FunctionSeries)) {
                _plotModel.Series.Remove(_plotModel.Series.First(series => series is FunctionSeries));
            }

            //Инзменить 0 и 100
            
            var functionSeries = new FunctionSeries(func, xMin - 10, xMax + 10, 0.001);
            _plotModel.Series.Add(functionSeries);
            PlotView.Model = _plotModel;
        }

        private void RadioButton1_OnChecked(object sender, RoutedEventArgs e) {
            polinomes = 1;
            RadioButton2.IsChecked = false;
        }

        private void RadioButton2_OnChecked(object sender, RoutedEventArgs e) {
            polinomes = 2;
            RadioButton1.IsChecked = false;
        }
    }

    public struct XY {
        public double x { get; set; }
        public double y { get; set; }
    }
}
