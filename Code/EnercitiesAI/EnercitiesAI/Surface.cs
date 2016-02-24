using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmoteEnercitiesMessages;
namespace EnercitiesAI
{
    public class Surface
    {
        private SurfaceType surfaceType;

        private int environmentScore;
        private int economyScore;
        private int wellbeingScore;

        public Surface()
        {
            this.surfaceType = SurfaceType.NotUsed;
            this.environmentScore = 0;
            this.economyScore = 0;
            this.wellbeingScore = 0;
        }

        public Surface(SurfaceType type, int environmentscore, int economyscore, int wellbeingscore)
        {
            this.surfaceType = type;
            this.environmentScore = environmentscore;
            this.economyScore = economyscore;
            this.wellbeingScore = wellbeingscore;
        }

        public Surface Copy()
        {
            return new Surface(this.surfaceType, this.environmentScore, this.economyScore, this.wellbeingScore);
        }

        #region Properties
        public SurfaceType Type
        {
            get
            {
                return surfaceType;
            }
        }

        public int EnvironmentScore
        {
            get
            {
                return environmentScore;
            }
        }

        public int EconomyScore
        {
            get
            {
                return economyScore;
            }
        }

        public int WellbeingScore
        {
            get
            {
                return wellbeingScore;
            }
        }
        #endregion
    }
}
