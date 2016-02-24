using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace EnercitiesAI.Domain.World
{
    [Serializable]
    [XmlType(AnonymousType = true)]
    [XmlRoot("grid", Namespace = "", IsNullable = false)]
    public class WorldGrid : IDisposable
    {
        private readonly Dictionary<Coordinate, HashSet<Coordinate>> _neighbourUnits =
            new Dictionary<Coordinate, HashSet<Coordinate>>();

        private GridUnit[,] _cellUnits;

        [XmlElement("unit", Form = XmlSchemaForm.Unqualified)]
        public GridUnit[] Units
        {
            get
            {
                var list = new List<GridUnit>();
                foreach (var gridUnit in this._cellUnits)
                    list.Add(gridUnit);
                return list.ToArray();
            }
            set { this.Init(value); }
        }

        [XmlAttribute("width")]
        public int Width { get; set; }

        [XmlAttribute("height")]
        public int Height { get; set; }

        [XmlAttribute("unitsizex")]
        public int UnitSizeX { get; set; }

        [XmlAttribute("unitsizey")]
        public int UnitSizeY { get; set; }

        [XmlIgnore]
        public GridUnit this[int x, int y]
        {
            get
            {
                return (this._cellUnits != null) && (x < this.Width) && (y < this.Height)
                    ? this._cellUnits[x, y]
                    : null;
            }
        }

        [XmlIgnore]
        public GridUnit this[Coordinate coord]
        {
            get { return this[coord.x, coord.y]; }
        }

        #region IDisposable Members

        public void Dispose()
        {
            this._cellUnits = null;
            this._neighbourUnits.Clear();
        }

        #endregion

        public HashSet<Coordinate> GetNeighbourUnits(Coordinate coord)
        {
            return this._neighbourUnits.ContainsKey(coord)
                ? this._neighbourUnits[coord]
                : null;
        }

        private void Init(GridUnit[] gridUnits)
        {
            //creates cell map
            this._cellUnits = new GridUnit[this.Width, this.Height];

            //creates x-y indexes for the units
            foreach (var gridUnit in gridUnits)
                this._cellUnits[gridUnit.X, gridUnit.Y] = gridUnit;
                
            //creates neighbours list
            foreach (var gridUnit in gridUnits)
            {
                var coord = new Coordinate(gridUnit.X,gridUnit.Y);
                this._neighbourUnits[coord] = this.CalcNeighbourUnits(coord);
            }
        }

        private HashSet<Coordinate> CalcNeighbourUnits(Coordinate coord)
        {
            //gets up, down, left, right coords
            var possibleNeighbours = new List<Coordinate>();
            if (coord.y > 0)
                possibleNeighbours.Add(new Coordinate(coord.x, coord.y - 1));
            if (coord.y < this.Height - 1)
                possibleNeighbours.Add(new Coordinate(coord.x, coord.y + 1));
            if (coord.x > 0)
                possibleNeighbours.Add(new Coordinate(coord.x - 1, coord.y));
            if (coord.x < this.Width - 1)
                possibleNeighbours.Add(new Coordinate(coord.x + 1, coord.y));

            //includes valid (not null) units only
            return
                new HashSet<Coordinate>(possibleNeighbours.Where(possibleNeighbour => this[possibleNeighbour] != null));
        }
    }
}