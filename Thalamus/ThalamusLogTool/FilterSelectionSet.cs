using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace ThalamusLogTool
{
    internal sealed class FilterSelectionSets : ApplicationSettingsBase
    {
        [UserScopedSetting()]
        [SettingsSerializeAs(SettingsSerializeAs.Binary)]
        [DefaultSettingValue("")]
        public System.Collections.Generic.Dictionary<string, List<string>> Sets
        { 
            get
            {
                return ((System.Collections.Generic.Dictionary<string, List<string>>)this["Sets"]);
            }
            set
            {
                this["Sets"] = (System.Collections.Generic.Dictionary<string, List<string>>)value;
            } 
        }
    }
}
