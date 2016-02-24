using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmoteEnercitiesMessages;

namespace EnercitiesAI
{
    public class Policy
    {
        private PolicyType type;
        private float deploymentCost;
        private float deploymentTime;   //in gametime years
        private Dictionary<StructureType, GridValues> affectedStructures;
        private bool hasBeenDeployed;

        public Policy()
        {
            this.type = PolicyType.NotUsed;
            this.deploymentCost = 0;
            this.deploymentTime = 0;
            affectedStructures = new Dictionary<StructureType, GridValues>();
            hasBeenDeployed = false;
        }

        public Policy(PolicyType type, float cost, float time, Dictionary<StructureType, GridValues> structures)
        {
            this.type = type;
            this.deploymentCost = cost;
            this.deploymentTime = time;
            affectedStructures = structures;
            hasBeenDeployed = false;
        }

        

        public GridValues GetValuesFor(StructureType type)
        {
            GridValues result = new GridValues();

            //Every StructureType can be found only once in the affectedStructures
            if (affectedStructures.ContainsKey(type))
            {
                result = affectedStructures[type];
            }

            return result;
        }

        #region Properties
        public PolicyType Type
        {
            get
            {
                return type;
            }
        }

        public float DeploymentCost
        {
            get
            {
                return deploymentCost;
            }
        }

        public float DeploymentTime
        {
            get
            {
                return deploymentTime;
            }
        }

        public Dictionary<StructureType, GridValues> AffectedStructures
        {
            get
            {
                return affectedStructures;
            }
        }

        public bool HasBeenDeployed
        {
            get
            {
                return hasBeenDeployed;
            }
        }
        #endregion

    }

}
