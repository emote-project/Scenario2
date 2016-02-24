using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmoteEnercitiesMessages;

namespace EnercitiesAI
{
    public class Upgrade
    {
        private UpgradeType type;
        private float researchCost;
        private GridValues values;
        private float researchTime;   //in gametime years

        public Upgrade()
        {
            this.type = UpgradeType.NotUsed;
            this.researchCost = 0;
            this.researchTime = 0;
            values = new GridValues();
        }

        public Upgrade(UpgradeType type, float researchcost, float time, float annualcost, int homes,
            float annualenergy, float annualoil, float environmentscore, float economyscore, float wellbeingscore)
        {
            this.type = type;
            this.researchCost = researchcost;
            this.researchTime = time;
            values = new GridValues(annualcost, annualenergy, annualoil, environmentscore, economyscore, wellbeingscore, homes);
        }

        //FEATURE
        public float PreviewIncrement()
        {
            return values.WellbeingScore + values.EnvironmentScore + values.EconomyScore + (values.AnnualOil > 0 ? 5 * values.AnnualOil : values.AnnualOil) + values.AnnualEnergy;
        }

        

        public Upgrade Copy()
        {
            return new Upgrade(type, researchCost, researchTime, values.AnnualCost, values.Homes, values.AnnualEnergy, values.AnnualOil, values.EnvironmentScore, values.EconomyScore, values.WellbeingScore);
        }

        #region Properties
        public UpgradeType Type
        {
            get
            {
                return type;
            }
        }

        public float ResearchCost
        {
            get
            {
                return researchCost;
            }
        }

        public float ResearchTime
        {
            get
            {
                return researchTime;
            }
        }

        public float AnnualCost
        {
            get
            {
                return values.AnnualCost;
            }
        }

        public int Homes
        {
            get
            {
                return values.Homes;
            }
        }

        public float AnnualEnergy
        {
            get
            {
                return values.AnnualEnergy;
            }
        }

        public float AnnualOil
        {
            get
            {
                return values.AnnualOil;
            }
        }

        public float EnvironmentScore
        {
            get
            {
                return values.EnvironmentScore;
            }
        }

        public float EconomyScore
        {
            get
            {
                return values.EconomyScore;
            }
        }

        public float WellbeingScore
        {
            get
            {
                return values.WellbeingScore;
            }
        }
        #endregion
    }
}
