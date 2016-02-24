using System.Drawing;
using System.Windows.Forms;
using EnercitiesAI.AI.Game;

namespace EnercitiesAI.Forms
{
    public partial class WorldForm : Form
    {
        private const int CP_NOCLOSE_BUTTON = 0x200;
        private readonly int _cellSize;

        public WorldForm(int cellSize)
        {
            this._cellSize = cellSize;
            this.InitializeComponent();
        }

        public Game Game
        {
            set
            {
                //resizes the form
                var firstGame = this.worldPanel.Game == null;
                this.worldPanel.Game = value;
                if (firstGame)
                {
                    this.worldPanel.CellSize = this._cellSize;
                    this.Size = this.worldPanel.ControlSize + new Size(6, 28);
                }
                this.worldPanel.Invalidate();
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }
    }
}