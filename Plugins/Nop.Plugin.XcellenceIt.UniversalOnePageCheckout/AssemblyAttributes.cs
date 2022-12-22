using System;

namespace Nop.Plugin.XcellenceIt.UniversalOnePageCheckout
{
    public class AssemblyAttributes
    {
        [AttributeUsage(AttributeTargets.Assembly)]
        public sealed class BuildDateAttribute : Attribute
        {
            public BuildDateAttribute(string value)
            {
                DateTime = Convert.ToDateTime(value);
            }
            public DateTime DateTime { get; }
        }
    }
}
