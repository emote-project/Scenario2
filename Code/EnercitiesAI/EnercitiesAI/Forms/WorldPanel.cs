using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using EmoteEnercitiesMessages;
using EmoteEvents;
using EnercitiesAI.AI.Actions;
using EnercitiesAI.AI.Game;
using EnercitiesAI.Domain.World;

namespace EnercitiesAI.Forms
{
    public class WorldPanel : Panel
    {
        #region Constants

        public const int AXIS_SIZE = 17;
        private const int DEF_CELL_SIZE = 60;
        private const int AXIS_TEXT_OFFSET = 2;
        private const int CELL_LABEL_OFFSET = 2;
        private const string FONT_FAMILY = "Segoe UI";
        private const TextRenderingHint TEXT_RENDERING = TextRenderingHint.ClearTypeGridFit;

        #endregion

        #region Colors

        private static readonly Color BorderColor = Color.Black;
        private static readonly Color LastCellBorderColor = Color.DarkRed;
        private static readonly Color SeaColor = Color.LightSkyBlue;
        private static readonly Color RiverColor = Color.DeepSkyBlue;
        private static readonly Color HillsColor = Color.DarkGreen;
        private static readonly Color PlainsColor = Color.LawnGreen;
        private static readonly Color EnvironmentColor = Color.ForestGreen;
        private static readonly Color EconomyColor = Color.Sienna;
        private static readonly Color EnergyColor = Color.Yellow;
        private static readonly Color WellbeingColor = Color.SandyBrown;
        private static readonly Color ResidentialColor = Color.RosyBrown;
        private static readonly Color CityHallColor = Color.IndianRed;
        private static readonly Color UnusedColor = Color.LightSlateGray;
        private static readonly SolidBrush AxisBrush = new SolidBrush(Color.Black);
        private static readonly SolidBrush LabelBrush = new SolidBrush(Color.Black);

        #endregion

        #region Fields

        private readonly HashSet<Coordinate> _updatedCoords = new HashSet<Coordinate>();
        private float _cellSizeRatio = Single.NaN;
        private Game _game;

        #endregion

        #region Constructors

        public WorldPanel()
        {
            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                ControlStyles.OptimizedDoubleBuffer, true);
            this.BackColor = Color.White;
            this.ForceRepaint = true;
            this.CellSize = DEF_CELL_SIZE;
        }

        public WorldPanel(Game game) : this()
        {
            this.Game = game;
        }

        #endregion

        #region Properties

        public bool ForceRepaint { get; set; }

        public int CellSize { get; set; }

        public bool GridVisible { get; set; }

        public Game Game
        {
            get { return this._game; }
            set
            {
                if (this._game != null)
                {
                    this._game.StateUpdated -= this.OnGameStateUpdated;
                    this._game.ActionExecuted -= this.OnGameActionExecuted;
                }
                this._game = value;
                if (value == null) return;
                this._game.StateUpdated += this.OnGameStateUpdated;
                this._game.ActionExecuted += this.OnGameActionExecuted;
                this._updatedCoords.Clear();
                this.Invalidate();
            }
        }

        public Size GridSize
        {
            get
            {
                return this.Game == null
                    ? new Size(DEF_CELL_SIZE, DEF_CELL_SIZE)
                    : new Size(this.Game.DomainInfo.WorldGrid.Width, this.Game.DomainInfo.WorldGrid.Height);
            }
        }

        public Size ControlSize
        {
            get
            {
                var worldGrid = this.Game.DomainInfo.WorldGrid;
                return this.Game == null
                    ? new Size(DEF_CELL_SIZE + AXIS_SIZE, DEF_CELL_SIZE + AXIS_SIZE)
                    : new Size(AXIS_SIZE + (worldGrid.Width*this.CellSize), AXIS_SIZE + (worldGrid.Height*this.CellSize));
            }
        }

        private int Level
        {
            get { return this.Game.State.GameInfoState.Level; }
        }

        private Font LabelFont
        {
            get { return new Font(FONT_FAMILY, (6f*this.CellSize)/DEF_CELL_SIZE, FontStyle.Bold); }
        }

        private Font AxisFont
        {
            get { return new Font(FONT_FAMILY, (8f*this.CellSize)/DEF_CELL_SIZE, FontStyle.Regular); }
        }

        public new void Invalidate()
        {
            this.ForceRepaint = true;
            base.Invalidate();
        }

        #endregion

        #region Event handlers

        protected override void OnParentChanged(EventArgs e)
        {
            this.AttachParentEvents();
            base.OnParentChanged(e);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            //auto adjusts cell size (and thus all elements..)
            if (!this._cellSizeRatio.Equals(Single.NaN))
            {
                this.CellSize = (int) (this.Size.Height*this._cellSizeRatio);
                this.Invalidate();
            }
            base.OnSizeChanged(e);
        }

        private void OnGameActionExecuted(IPlayerAction action)
        {
            this._updatedCoords.Clear();
            if (action is SkipTurn) return;

            var actions = new List<IPlayerAction>();
            if (action is UpgradeStructures)
                actions.AddRange(((UpgradeStructures) action).Upgrades);
            else
                actions.Add(action);

            foreach (var playerAction in actions)
            {
                var actionInfo = playerAction.ToEnercitiesActionInfo();
                this._updatedCoords.Add(new Coordinate(actionInfo.CellX, this.GridSize.Height - 1 - actionInfo.CellY));
            }
        }

        private void OnGameStateUpdated(EnercitiesGameInfo state, EnercitiesGameInfo expectedState)
        {
            this.Invalidate();
        }

        #endregion

        #region Private methods

        private void AttachParentEvents()
        {
            if (this.Parent == null) return;
            this.Parent.Paint += (obj, e) => this.Invalidate();
            this.Parent.DragEnter += (obj, e) => this.Invalidate();
            this.Parent.DragOver += (obj, e) => this.Invalidate();
            this.Parent.Move += (obj, e) => this.Invalidate();
            this.Parent.GotFocus += (obj, e) => this.Invalidate();
            this.Parent.LostFocus += (obj, e) => this.Invalidate();
        }

        private Size ResizeParent(Size size)
        {
            if ((size.Equals(new Size()) || this.Parent == null)) return new Size();

            var oldSize = this.Parent.Size;
            this.Parent.Size = size;
            this.Parent.Invalidate();
            return oldSize;
        }

        private Size ResizeParent(int controlHeight)
        {
            if ((controlHeight.Equals(0) || this.Parent == null)) return new Size();

            var newParHeight = controlHeight + (this.Parent.Height - this.Height);
            var newParWidth = (double) newParHeight*this.Parent.Width/this.Parent.Height;

            return this.ResizeParent(new Size((int) newParWidth, newParHeight));
        }

        #endregion

        #region Painting methods

        public void SaveImage(string filePath, int height = int.MaxValue, ImageFormat format = null)
        {
            var oldParentSize = new Size();
            if (format == null) format = ImageFormat.Png;

            //checks need to change parent (and world panel) size
            var changeSize = !height.Equals(int.MaxValue) && !height.Equals(this.Height);
            if (changeSize) oldParentSize = this.ResizeParent(height);

            //changes panel properties
            var oldBoderStyle = this.BorderStyle;
            this.BorderStyle = BorderStyle.None;
            this.Invalidate();
            this.Update();
            this.ForceRepaint = true;

            //gets and saves bmp to file
            var bmp = new Bitmap(this.Width, this.Height);
            this.DrawToBitmap(bmp, new Rectangle(new Point(), this.Size));
            bmp.Save(filePath, format);

            //restores parent size and properties
            if (changeSize) this.ResizeParent(oldParentSize);
            this.BorderStyle = oldBoderStyle;
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (!this.GridVisible || !this.ForceRepaint || (this.Game == null)) return;

            //checks ratio defined
            if (this._cellSizeRatio.Equals(Single.NaN))
                this._cellSizeRatio = (float) this.CellSize/this.Height;

            e.Graphics.TextRenderingHint = TEXT_RENDERING;
            for (var x = 0; x < this.GridSize.Width; x++)
            {
                this.DrawXAxisTick(e, x);

                for (var y = 0; y < this.GridSize.Height; y++)
                {
                    var invY = this.GridSize.Height - y - 1;
                    var startLoc = this.GetCellStartLocation(x, y);
                    var gridUnit = this.Game.DomainInfo.WorldGrid[x, invY];

                    if (x == 0)
                        this.DrawYAxisTick(e, invY, y);

                    //draws all cell elements
                    this.DrawBackground(gridUnit, e, startLoc);
                    this.DrawBorder(gridUnit, e, startLoc);
                    this.DrawCellLabels(gridUnit, e, x, y);
                }
            }

            this.DrawUpdatedCellsBorder(e);

            this.ForceRepaint = false;
            base.OnPaint(e);
        }

        private void DrawUpdatedCellsBorder(PaintEventArgs e)
        {
            if (this._updatedCoords.Count == 0) return;
            foreach (var lastCoord in this._updatedCoords)
            {
                var startLoc = this.GetCellStartLocation(lastCoord.x, lastCoord.y);
                var borderRec = new Rectangle(startLoc, new Size(this.CellSize, this.CellSize) - new Size(1, 1));
                var borderPen = new Pen(LastCellBorderColor, (3f*this.CellSize)/DEF_CELL_SIZE)
                                {
                                    DashStyle =
                                        DashStyle.Dash
                                };
                e.Graphics.DrawRectangle(borderPen, borderRec);
            }
        }

        private Point GetCellStartLocation(int x, int y)
        {
            return new Point((AXIS_SIZE*this.CellSize)/DEF_CELL_SIZE + (x*this.CellSize), (y*this.CellSize));
        }

        private void DrawCellLabels(GridUnit gridUnit, PaintEventArgs e, int x, int y)
        {
            if ((gridUnit == null) || this.Game.State.StructuresState.IsUnitEmpty(gridUnit.Coordinate)) return;

            var structureType = this.Game.State.StructuresState[gridUnit.Coordinate];
            if (structureType.Equals(StructureType.NotUsed)) return;

            var labels = new List<string> {structureType.ToString().Replace("_", " ")};

            var upgrades = this.Game.State.UpgradesState.GetActiveUpgrades(gridUnit.Coordinate);
            if (upgrades.Count > 0)
            {
                labels.Add("-----------------");
                labels.AddRange(upgrades.Select(upgradeType => upgradeType.ToString().Replace("_", " ")));
            }

            if (structureType.Equals(StructureType.City_Hall))
            {
                var policies = this.Game.State.PoliciesState.GetActivePolicies();
                if (policies.Count > 0)
                {
                    labels.Add("-----------------");
                    labels.AddRange(policies.Select(policyType => policyType.ToString().Replace("_", " ")));
                }
            }

            var basePoint = new PointF(
                AXIS_TEXT_OFFSET + (int) (0.3*this.CellSize) + (x*this.CellSize),
                CELL_LABEL_OFFSET + (y*this.CellSize));

            for (var i = 0; i < labels.Count; i++)
            {
                var label = labels[i];
                var point = basePoint + new SizeF(0, (LabelFont.Size + CELL_LABEL_OFFSET)*i);
                e.Graphics.DrawString(label, LabelFont, LabelBrush, point);
            }
        }

        private void DrawXAxisTick(PaintEventArgs e, int x)
        {
            e.Graphics.DrawString(x.ToString(CultureInfo.InvariantCulture), this.AxisFont, AxisBrush,
                (int) (this.CellSize*0.65) + (x*this.CellSize), this.ControlSize.Height - AXIS_SIZE + AXIS_TEXT_OFFSET);
        }

        private void DrawYAxisTick(PaintEventArgs e, int invY, int y)
        {
            e.Graphics.DrawString(invY.ToString(CultureInfo.InvariantCulture), this.AxisFont, AxisBrush,
                AXIS_TEXT_OFFSET, (int) (this.CellSize*0.45) + (y*this.CellSize));
        }

        private void DrawBackground(GridUnit gridUnit, PaintEventArgs e, Point startLoc)
        {
            var fullRec = new Rectangle(startLoc, new Size(this.CellSize, this.CellSize));
            var backColor = this.GetCellBackColor(gridUnit);
            var brush = this.GetCellBackBrush(gridUnit, backColor);
            e.Graphics.FillRectangle(brush, fullRec);
        }

        private void DrawBorder(GridUnit gridUnit, PaintEventArgs e, Point startLoc)
        {
            if (gridUnit == null) return;
            var borderRec = new Rectangle(startLoc, new Size(this.CellSize, this.CellSize) - new Size(1, 1));
            e.Graphics.DrawRectangle(new Pen(BorderColor, 1), borderRec);
        }

        private Brush GetCellBackBrush(GridUnit gridUnit, Color backColor)
        {
            return (gridUnit == null)
                ? new SolidBrush(backColor)
                : gridUnit.Level > this.Level
                    ? new HatchBrush(HatchStyle.DiagonalCross, Color.Black, backColor)
                    : this.Game.State.StructuresState.IsUnitEmpty(gridUnit.Coordinate)
                        ? (Brush) new HatchBrush(HatchStyle.Percent05, Color.Black, backColor)
                        : new SolidBrush(backColor);
        }

        private Color GetCellBackColor(GridUnit gridUnit)
        {
            Color backColor;
            if (gridUnit == null)
                backColor = SeaColor;
            else
            {
                var structureType = this.Game.State.StructuresState[gridUnit.Coordinate];
                backColor = structureType.Equals(StructureType.NotUsed)
                    ? GetSurfaceColor(gridUnit.SurfaceType)
                    : structureType.Equals(StructureType.City_Hall)
                        ? CityHallColor
                        : GetCategoryColor(this.Game.DomainInfo.Structures.StructureCategories[structureType]);
            }
            return backColor;
        }

        private static Color GetSurfaceColor(SurfaceType surfaceType)
        {
            Color backColor;
            switch (surfaceType)
            {
                case SurfaceType.Ocean:
                    backColor = SeaColor;
                    break;
                case SurfaceType.Hills:
                    backColor = HillsColor;
                    break;
                case SurfaceType.Hydro_River:
                case SurfaceType.River:
                    backColor = RiverColor;
                    break;
                case SurfaceType.Plains:
                case SurfaceType.Plains2:
                    backColor = PlainsColor;
                    break;
                default:
                    backColor = UnusedColor;
                    break;
            }
            return backColor;
        }

        private static Color GetCategoryColor(StructureCategory structureCat)
        {
            Color backColor;
            switch (structureCat)
            {
                case StructureCategory.Environment:
                    backColor = EnvironmentColor;
                    break;
                case StructureCategory.Residential:
                    backColor = ResidentialColor;
                    break;
                case StructureCategory.Economy:
                    backColor = EconomyColor;
                    break;
                case StructureCategory.Energy:
                    backColor = EnergyColor;
                    break;
                case StructureCategory.Wellbeing:
                    backColor = WellbeingColor;
                    break;
                default:
                    backColor = UnusedColor;
                    break;
            }
            return backColor;
        }

        #endregion
    }
}