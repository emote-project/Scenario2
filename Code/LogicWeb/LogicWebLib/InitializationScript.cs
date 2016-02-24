using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LogicWebLib
{
    public abstract class InitializationScript
    {
        public abstract void Run(out LogicFrame logicFrame, out LogicWebLib.LogicWeb logicWeb);

        public static InitializationScript LoadFromFile(string filePath)
        {
            var DLL = Assembly.LoadFile(filePath);
            foreach (Type type in DLL.GetExportedTypes())
            {
                if (type.IsSubclassOf(typeof(InitializationScript)))
                {
                    ConstructorInfo ctor = type.GetConstructor(new Type[] { });
                    object instance = ctor.Invoke(new object[] { });
                    return (InitializationScript)instance;
                }
            }
            return null;
        }
    }

    
}
