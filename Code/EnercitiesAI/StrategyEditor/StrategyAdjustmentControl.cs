using System;
using System.Collections.Generic;
using System.Windows.Forms;
using EmoteEvents;
using EnercitiesAI.AI;
using EnercitiesAI.AI.Planning;
using EnercitiesAI.Domain;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace StrategyEditor
{
    public enum ParamType
    {
        Power,
        Money,
        Oil,
        Score,
        Homes,
        Environment
    }

    public partial class StrategyAdjustmentControl : UserControl
    {
        private const int NUM_POINTS = 100;
        private const double STRATEGY_ADJUSTMENT = 0.2;
        private readonly OxyColor _color;
        private readonly DomainInfo _domainInfo;
        private readonly EnercitiesGameInfo _gameInfo;
        private readonly double _paramMin;
        private readonly double _paramRange;
        private readonly ParamType _paramType;
        private readonly Player _player;
        private readonly PlotModel _plotModel;
        private readonly int _xMax;
        private readonly int _xMin;
        private FunctionSeries _functionSeries;

        public StrategyAdjustmentControl(
            DomainInfo domainInfo, EnercitiesGameInfo gameInfo, Player player,
            OxyColor color, ParamType paramType, double paramMin, double paramRange, int xMin, int xMax)
        {
            this._player = player;
            this._color = color;
            this._paramType = paramType;
            this._paramMin = paramMin;
            this._paramRange = paramRange;
            this._xMin = xMin;
            this._xMax = xMax;
            this._gameInfo = gameInfo;
            this._domainInfo = domainInfo;

            InitializeComponent();

            //creates plot with given function and creates axes
            this._plotModel = new PlotModel
                              {
                                  PlotType = PlotType.Cartesian,
                                  Background = OxyColors.White
                              };
            this._plotModel.Axes.Add(new LinearAxis
                                     {
                                         Minimum = 0,
                                         AbsoluteMinimum = 0,
                                         Maximum = 1,
                                         AbsoluteMaximum = 1,
                                         Position = AxisPosition.Left
                                     });
            this._plotModel.Axes.Add(new LinearAxis
                                     {
                                         AbsoluteMinimum = xMin,
                                         AbsoluteMaximum = xMax,
                                         Position = AxisPosition.Bottom
                                     });
            this.plotView.Model = this._plotModel;

            this.Update();
        }

        private double ParamValue
        {
            get
            {
                var strategy = this._player.Strategy;
                double value;
                switch (this._paramType)
                {
                    case ParamType.Environment:
                        value = strategy.EnvironmentAdjustParam;
                        break;
                    case ParamType.Homes:
                        value = strategy.HomesAdjustParam;
                        break;
                    case ParamType.Money:
                        value = strategy.MoneyAdjustParam;
                        break;
                    case ParamType.Oil:
                        value = strategy.OilAdjustParam;
                        break;
                    case ParamType.Power:
                        value = strategy.PowerAdjustParam;
                        break;
                    default:
                        value = strategy.ScoreAdjustParam;
                        break;
                }
                var paramMax = this._paramMin + this._paramRange;
                if (value > paramMax)
                    value = paramMax;
                else if (value < this._paramMin)
                    value = _paramMin;
                return value;
            }
            set
            {
                var strategy = this._player.Strategy;
                var paramMax = this._paramMin + this._paramRange;
                if (value > paramMax)
                    value = paramMax;
                else if (value < this._paramMin)
                    value = _paramMin;

                switch (this._paramType)
                {
                    case ParamType.Environment:
                        strategy.EnvironmentAdjustParam = value;
                        break;
                    case ParamType.Homes:
                        strategy.HomesAdjustParam = value;
                        break;
                    case ParamType.Money:
                        strategy.MoneyAdjustParam = value;
                        break;
                    case ParamType.Oil:
                        strategy.OilAdjustParam = value;
                        break;
                    case ParamType.Power:
                        strategy.PowerAdjustParam = value;
                        break;
                    default:
                        strategy.ScoreAdjustParam = value;
                        break;
                }
            }
        }

        private Func<double, double> Function
        {
            get
            {
                Func<double, double> function;
                switch (this._paramType)
                {
                    case ParamType.Environment:
                        function = StrategyAdjustment.EnvironmentAdjustment(
                            this.ParamValue, this._player.Strategy.ScoreAdjustParam);
                        break;
                    case ParamType.Homes:
                        function = StrategyAdjustment.HomesAdjustment(this.ParamValue);
                        break;
                    case ParamType.Money:
                        function = StrategyAdjustment.ResourceAdjustment(this.ParamValue);
                        break;
                    case ParamType.Oil:
                        function = StrategyAdjustment.ResourceAdjustment(this.ParamValue);
                        break;
                    case ParamType.Power:
                        function = StrategyAdjustment.ResourceAdjustment(this.ParamValue);
                        break;
                    default:
                        function = StrategyAdjustment.ScoreAdjustment(STRATEGY_ADJUSTMENT, this.ParamValue);
                        break;
                }
                return function;
            }
        }

        private Dictionary<string, double> GameValues
        {
            get
            {
                var values = new Dictionary<string, double>();
                switch (this._paramType)
                {
                    case ParamType.Environment:
                        values.Add(this._paramType.ToString(), this._gameInfo.EnvironmentScore);
                        break;
                    case ParamType.Homes:
                        var level = this._gameInfo.Level;
                        var homesNeedRatio =
                            1d - ((double) this._gameInfo.Population/
                                  this._domainInfo.Scenario.WinConditions[level].Population);
                        values.Add(this._paramType.ToString(), 2*(1 - homesNeedRatio));
                        break;
                    case ParamType.Money:
                        values.Add(this._paramType.ToString(), this._gameInfo.Money);
                        break;
                    case ParamType.Oil:
                        values.Add(this._paramType.ToString(), this._gameInfo.Oil);
                        break;
                    case ParamType.Power:
                        values.Add(this._paramType.ToString(), this._gameInfo.PowerProduction);
                        break;
                    default:
                        values.Add("Environment", this._gameInfo.EnvironmentScore);
                        values.Add("Economy", this._gameInfo.EconomyScore);
                        values.Add("Wellbeing", this._gameInfo.WellbeingScore);
                        break;
                }
                return values;
            }
        }

        private Dictionary<string, int> AnnotationPoints
        {
            get
            {
                var dictionary = new Dictionary<string, int>();
                foreach (var gameValue in this.GameValues)
                {
                    var value = gameValue.Value;
                    if (value > this._xMax) value = this._xMax;
                    else if (value < this._xMin) value = this._xMin;
                    dictionary.Add(gameValue.Key, (int) (NUM_POINTS*(value - this._xMin)/(this._xMax - this._xMin)));
                }
                return dictionary;
            }
        }

        public new void Update()
        {
            this.UpdateFunction();
            this.UpdateTrackBar();
        }

        public event EventHandler<double> ParamValueChanged;

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            this.plotView.Visible = this.Enabled;
        }

        private void UpdateFunction()
        {
            this.AddSeries(this.Function);
            this.AddAnnotations();

            this.plotView.InvalidatePlot(true);
            this.txtBoxParam.Text = this.ParamValue.ToString("0.000");
        }

        private void AddAnnotations()
        {
            this._plotModel.Annotations.Clear();
            var annotationPoints = this.AnnotationPoints;
            foreach (var point in annotationPoints)
            {
                var dataPoint = this._functionSeries.Points[point.Value];
                this._plotModel.Annotations.Add(
                    new PointAnnotation
                    {
                        X = dataPoint.X,
                        Y = dataPoint.Y,
                        Text = point.Key,
                        Fill = this._color,
                        FontSize = 10
                    });
            }
        }

        private void AddSeries(Func<double, double> function)
        {
            this._plotModel.Series.Clear();
            this._functionSeries =
                new FunctionSeries(function, this._xMin, this._xMax, (this._xMax - this._xMin)/(double) NUM_POINTS)
                {
                    Color = this._color,
                    Smooth = true
                };
            this._plotModel.Series.Add(_functionSeries);
        }

        private void UpdateTrackBar()
        {
            this.trackBar.Value = (int) (((this.ParamValue - this._paramMin)/this._paramRange)*100);
        }

        private void TrackBarScroll(object sender, EventArgs e)
        {
            this.ParamValue = this._paramMin + ((this.trackBar.Value/100d)*this._paramRange);
            this.UpdateFunction();

            if (this.ParamValueChanged != null)
                this.ParamValueChanged(this, this.ParamValue);
        }
    }
}