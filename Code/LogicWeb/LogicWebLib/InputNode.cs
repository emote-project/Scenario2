using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicWebLib
{
    /// <summary>
    /// A specialized logic node which state Active/NotActive depends on a custom source (thalamus signals or such)
    /// </summary>
    public abstract class InputNode : Node
    {
        protected InputNode(string description)
            : base(description)
        {}

    }
}
