using Nop.Core;

namespace XcellenceIt.Plugins.Misc.FFM.Domain
{
    public class ImportToCSV : BaseEntity
    {
        /// <summary>
        /// Gets or sets the ProductName identifier
        /// </summary>
        public string  ProductName { get; set; }

        /// <summary>
        /// Gets or sets the Sku identifier
        /// </summary>
        public string  Sku { get; set; }
    }
}
