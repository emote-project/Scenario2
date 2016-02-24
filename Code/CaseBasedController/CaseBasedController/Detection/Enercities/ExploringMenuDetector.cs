using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaseBasedController.Detection.Enercities
{
    public class ExploringMenuDetector : BaseFeatureDetector
    {
        public enum MenuType { CONSTRUCTION, POLICY, UPGRADE };

        public MenuType WatchedMenuType {get; set;}

        private MenuType _selectedMenuType;
        private bool _showed = false;
        private bool _selected = false;


        public ExploringMenuDetector(MenuType watchedMenuType)
        {
            WatchedMenuType = watchedMenuType;
        }


        public override double ActivationLevel
        {
            get
            {
                lock (this.locker)
                {
                    return IsActive ? 1 : 0;
                }
            }
        }

        public override bool IsActive
        {
            get 
            {
                lock (this.locker)
                {
                    return WatchedMenuType == _selectedMenuType && (_showed || _selected);
                }
            }
        }

        public override void Dispose()
        {
            perceptionClient.BuildMenuTooltipShowedEvent -= perceptionClient_BuildMenuTooltipShowedEvent;
            perceptionClient.BuildingMenuToolSelectedEvent -= perceptionClient_BuildingMenuToolSelectedEvent;
            perceptionClient.BuildingMenuToolUnselectedEvent -= perceptionClient_BuildingMenuToolUnselectedEvent;
            perceptionClient.BuildMenuTooltipClosedEvent -= perceptionClient_BuildMenuTooltipClosedEvent;

            perceptionClient.PoliciesMenuShowedEvent -= perceptionClient_PoliciesMenuShowedEvent;
            perceptionClient.PoliciesMenuClosedEvent -= perceptionClient_PoliciesMenuClosedEvent;

            perceptionClient.UpgradesMenuShowedEvent -= perceptionClient_UpgradesMenuShowedEvent;
            perceptionClient.UpgradesMenuClosedEvent -= perceptionClient_UpgradesMenuClosedEvent;
        }

        protected override void AttachEvents()
        {
            perceptionClient.BuildMenuTooltipShowedEvent += perceptionClient_BuildMenuTooltipShowedEvent;
            perceptionClient.BuildingMenuToolSelectedEvent += perceptionClient_BuildingMenuToolSelectedEvent;
            perceptionClient.BuildingMenuToolUnselectedEvent += perceptionClient_BuildingMenuToolUnselectedEvent;
            perceptionClient.BuildMenuTooltipClosedEvent += perceptionClient_BuildMenuTooltipClosedEvent;

            perceptionClient.PoliciesMenuShowedEvent += perceptionClient_PoliciesMenuShowedEvent;
            perceptionClient.PoliciesMenuClosedEvent += perceptionClient_PoliciesMenuClosedEvent;
            perceptionClient.PolicyTooltipShowedEvent += perceptionClient_PolicyTooltipShowedEvent;
            perceptionClient.PolicyTooltipClosedEvent += perceptionClient_PolicyTooltipClosedEvent;

            perceptionClient.UpgradesMenuShowedEvent += perceptionClient_UpgradesMenuShowedEvent;
            perceptionClient.UpgradesMenuClosedEvent += perceptionClient_UpgradesMenuClosedEvent;
            perceptionClient.UpgradeTooltipShowedEvent += perceptionClient_UpgradeTooltipShowedEvent;
            perceptionClient.UpgradeTooltipClosedEvent += perceptionClient_UpgradeTooltipClosedEvent;
        }

        

        #region Upgrade menu events
        void perceptionClient_UpgradesMenuClosedEvent(object sender, EventArgs e)
        {
            _selectedMenuType = MenuType.UPGRADE;
            _selected = false;
            CheckActivationChanged();
        }

        void perceptionClient_UpgradesMenuShowedEvent(object sender, EventArgs e)
        {
            _selectedMenuType = MenuType.UPGRADE;
            _selected = true;
            CheckActivationChanged();
        }
        void perceptionClient_UpgradeTooltipClosedEvent(object sender, EventArgs e)
        {
            _selectedMenuType = MenuType.UPGRADE;
            _showed = false;
            CheckActivationChanged();
        }

        void perceptionClient_UpgradeTooltipShowedEvent(object sender, Thalamus.MenuEventArgs e)
        {
            _selectedMenuType = MenuType.UPGRADE;
            _showed = true;
            CheckActivationChanged();
        }
        #endregion

        #region policy menu events
        void perceptionClient_PoliciesMenuClosedEvent(object sender, EventArgs e)
        {
            _selectedMenuType = MenuType.POLICY;
            _showed = false;
            CheckActivationChanged();
        }

        void perceptionClient_PoliciesMenuShowedEvent(object sender, EventArgs e)
        {
            _selectedMenuType = MenuType.POLICY;
            _showed = true;
            CheckActivationChanged();
        }

        void perceptionClient_PolicyTooltipClosedEvent(object sender, EventArgs e)
        {
            _selectedMenuType = MenuType.POLICY;
            _selected = false;
            CheckActivationChanged();
        }

        void perceptionClient_PolicyTooltipShowedEvent(object sender, Thalamus.MenuEventArgs e)
        {
            _selectedMenuType = MenuType.POLICY;
            _selected = true;
            CheckActivationChanged();
        }
        #endregion

        #region Construction menu events
        void perceptionClient_BuildingMenuToolSelectedEvent(object sender, Thalamus.MenuEventArgs e)
        {
            _selectedMenuType = MenuType.CONSTRUCTION;
            _selected = true;
            CheckActivationChanged();
        }

        void perceptionClient_BuildMenuTooltipClosedEvent(object sender, Thalamus.MenuEventArgs e)
        {
            _selectedMenuType = MenuType.CONSTRUCTION;
            _showed = false;
            CheckActivationChanged();
        }

        void perceptionClient_BuildingMenuToolUnselectedEvent(object sender, Thalamus.MenuEventArgs e)
        {
            _selectedMenuType = MenuType.CONSTRUCTION;
            _selected = false;
            CheckActivationChanged();
        }

        void perceptionClient_BuildMenuTooltipShowedEvent(object sender, Thalamus.MenuEventArgs e)
        {
            _selectedMenuType = MenuType.CONSTRUCTION;
            _showed  = true;
            CheckActivationChanged();
        }
        #endregion
        


    }
}
