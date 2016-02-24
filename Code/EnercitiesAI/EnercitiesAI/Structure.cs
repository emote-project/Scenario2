using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmoteEnercitiesMessages;

namespace EnercitiesAI
{
    public class Structure
    {
        public const int NumberOfUpgradeSlots = 5;

        private StructureCategory category;
        private StructureType type;
        private List<Upgrade> upgrades;
        private List<Policy> policies;

        private Construction construction;

        private bool isPowerDown;
        private bool isOilDepleted;
        private int unlockLevel;

        private float buildingCost;
        private float buildTime;

        private int buildYear;
        private int buildDay;

        private int demolitionYear;
        private int demolitionDay;

        private float annualCost;
        private int homes;
        private float annualEnergy;
        private float annualOil;

        private float environmentScore;
        private float economyScore;
        private float wellbeingScore;

        public Structure()
        {
            category = 0;
            buildingCost = 0;
            buildTime = 0;
            unlockLevel = 1;

            //HACK: Initialise values, hardcoded
            buildYear = DayTime_Manager.StartYear;
            buildDay = 1;
            demolitionYear = 8888;
            DemolitionDay = 1;

            annualCost = 0;
            homes = 0;
            annualEnergy = 0;
            annualOil = 0;

            environmentScore = 0;
            economyScore = 0;
            wellbeingScore = 0;

            upgrades = new List<Upgrade>(NumberOfUpgradeSlots);
            policies = new List<Policy>();

            category = StructureCategory.NotUsed;
            type = StructureType.NotUsed;

            construction = null;
        }

        public Structure(StructureType type, StructureCategory category, float buildingcost, float buildtime, float annualcost,
            int homes, float annualenergy, float annualoil, float environmentscore, float economyscore, float wellbeingscore, int buildYear, int buildDay, int unlockLevel)
        {
            this.type = type;
            this.category = category;
            this.unlockLevel = unlockLevel;
            this.buildingCost = buildingcost;
            this.buildTime = buildtime;

            this.buildYear = buildYear;
            this.buildDay = buildDay;

            this.annualCost = annualcost;
            this.homes = homes;
            this.annualEnergy = annualenergy;
            this.annualOil = annualoil;

            this.environmentScore = environmentscore;
            this.economyScore = economyscore;
            this.wellbeingScore = wellbeingscore;

            this.upgrades = new List<Upgrade>(NumberOfUpgradeSlots);
            this.policies = new List<Policy>();

            this.construction = null;
        }

        public Structure Copy()
        {
            Structure s = new Structure();
            s.category = this.category;
            s.unlockLevel = this.unlockLevel;
            s.buildingCost = this.buildingCost;
            s.buildTime = this.buildTime;

            s.buildYear = this.buildYear;
            s.buildDay = this.buildDay;

            s.demolitionYear = this.demolitionYear;
            s.demolitionDay = this.demolitionDay;

            s.annualCost = this.annualCost;
            s.homes = this.homes;
            s.annualEnergy = this.annualEnergy;
            s.annualOil = this.annualOil;
            s.environmentScore = this.environmentScore;
            s.economyScore = this.economyScore;
            s.wellbeingScore = this.wellbeingScore;

            s.upgrades.AddRange(this.upgrades.ToArray());
            s.policies.AddRange(this.policies.ToArray());

            s.type = this.type;

            return s;
        }


        public void AddUpgrade(Upgrade upgrade)
        {
            PerceptionModule.PerformUpgrade(upgrade);
            if (upgrades.Count < NumberOfUpgradeSlots)
            {
                upgrades.Add(upgrade);
            }
        }

        public void AddPolicy(Policy policy)
        {
            //PerceptionModule.ImplementPolicy(policy.Type);
            policies.Add(policy);
        }

        #region Properties
        public StructureCategory Category
        {
            get
            {
                return category;
            }
        }

        public int UnlockLevel
        {
            get
            {
                return unlockLevel;
            }
        }

        public StructureType Type
        {
            get
            {
                return type;
            }
        }

        public float BuildingCost
        {
            get
            {
                return buildingCost;
            }
        }

        public float BuildTime
        {
            get
            {
                return buildTime;
            }
        }

        public int Homes
        {
            get
            {
                int result = 0;

                if (!this.IsUnderConstruction())
                {
                    int upgradesvalue = 0;
                    foreach (Upgrade u in this.upgrades)
                    {
                        if (!u.IsUpgrading())
                            upgradesvalue += u.Homes;
                    }

                    int policyvalue = GetPolicyValues().Homes;

                    result = homes + upgradesvalue + policyvalue;
                }

                if (isPowerDown)
                {
                    result = 0;
                }

                return result;
            }
        }

        public float AnnualCost
        {
            get
            {
                float result = 0;

                if (!this.IsUnderConstruction())
                {
                    float upgradesvalue = 0;
                    foreach (Upgrade u in this.upgrades)
                    {
                        if (!u.IsUpgrading())
                            upgradesvalue += u.AnnualCost;
                    }

                    float policyvalue = GetPolicyValues().AnnualCost;

                    result = annualCost + upgradesvalue + policyvalue;
                }

                if (isOilDepleted && this.annualOil < 0 && result < 0)
                {
                    result = (float)Math.Round(result / 2.0f);
                }

                if (isPowerDown)
                {
                    result = 0;
                }

                return result;
            }
        }

        public float AnnualEnergy
        {
            get
            {
                float result = 0;

                if (!this.IsUnderConstruction())
                {
                    float upgradesvalue = 0;
                    foreach (Upgrade u in this.upgrades)
                    {
                        if (!u.IsUpgrading())
                            upgradesvalue += u.AnnualEnergy;
                    }

                    float policyvalue = GetPolicyValues().AnnualEnergy;

                    result = annualEnergy + upgradesvalue + policyvalue;
                }

                if (isOilDepleted && this.annualOil < 0 && result > 0)
                {
                    result = (float)Math.Round(result / 2.0f);
                }

                return result;
            }
        }

        public float AnnualOil
        {
            get
            {
                float result = 0;

                if (!this.IsUnderConstruction())
                {
                    float upgradesvalue = 0;
                    foreach (Upgrade u in this.upgrades)
                    {
                        if (!u.IsUpgrading())
                            upgradesvalue += u.AnnualOil;
                    }

                    float policyvalue = GetPolicyValues().AnnualOil;

                    result = annualOil + upgradesvalue + policyvalue;
                }

                if (isPowerDown)
                {
                    result = 0;
                }

                //Oil should never be > 0 !!!
                result = Mathf.Clamp(result, result, 0);

                return result;
            }
        }

        public float EnvironmentScore
        {
            get
            {
                float result = 0;

                if (!this.IsUnderConstruction())
                {
                    float upgradesvalue = 0;
                    foreach (Upgrade u in this.upgrades)
                    {
                        if (!u.IsUpgrading())
                            upgradesvalue += u.EnvironmentScore;
                    }

                    float policyvalue = GetPolicyValues().EnvironmentScore;

                    result = environmentScore + upgradesvalue + policyvalue;
                }

                if (isOilDepleted && this.annualOil < 0 && result > 0)
                {
                    result = (float)Math.Round(result / 2.0f);
                }

                return result;
            }
        }

        public float EconomyScore
        {
            get
            {
                float result = 0;

                if (!this.IsUnderConstruction())
                {
                    float upgradesvalue = 0;

                    foreach (Upgrade u in this.upgrades)
                    {
                        if (!u.IsUpgrading())
                            upgradesvalue += u.EconomyScore;
                    }

                    float policyvalue = GetPolicyValues().EconomyScore;

                    result = economyScore + upgradesvalue + policyvalue;
                }

                if (isOilDepleted && this.annualOil < 0 && result > 0)
                {
                    result = (float)Mathf.Round(result / 2.0f);
                }

                if (isPowerDown)
                {
                    result = 0;
                }

                return result;
            }
        }

        public float WellbeingScore
        {
            get
            {
                float result = 0;

                if (!this.IsUnderConstruction())
                {
                    float upgradesvalue = 0;
                    foreach (Upgrade u in this.upgrades)
                    {
                        if (!u.IsUpgrading())
                            upgradesvalue += u.WellbeingScore;
                    }

                    float policyvalue = GetPolicyValues().WellbeingScore;

                    result = wellbeingScore + upgradesvalue + policyvalue;
                }

                if (isOilDepleted && this.annualOil < 0 && result > 0)
                {
                    result = (float)Mathf.Round(result / 2.0f);
                }

                if (isPowerDown && annualEnergy < 0)
                {
                    result = 0;
                }

                return result;
            }
        }

        public bool IsPowerDown
        {
            get
            {
                return isPowerDown;
            }

            set
            {
                isPowerDown = value;
            }
        }

        public bool IsOilDepleted
        {
            get
            {
                return isOilDepleted;
            }

            set
            {
                isOilDepleted = value;
            }
        }

        public int BuildYear
        {
            get
            {
                return buildYear;
            }
        }

        public int BuildDay
        {
            get
            {
                return buildDay;
            }
        }

        public int DemolitionYear
        {
            get
            {
                return demolitionYear;
            }
            set
            {
                demolitionYear = value;
            }
        }

        public int DemolitionDay
        {
            get
            {
                return demolitionDay;
            }
            set
            {
                demolitionDay = value;
            }
        }

        public int FreeUpgradeSlots
        {
            get
            {
                return NumberOfUpgradeSlots - upgrades.Count;
            }
        }

        public Upgrade[] CurrentUpgrades
        {
            get
            {
                return upgrades.ToArray();
            }
        }
        #endregion

        private GridValues GetPolicyValues()
        {
            GridValues result = new GridValues();
            foreach (Policy p in policies)
            {
                if (!p.IsBeingDeployed())
                    result += p.GetValuesFor(this.type);
            }

            return result;
        }
    }
}
