using Nop.Core;

namespace XcellenceIt.Plugins.Misc.FFM.Domain
{
    public class Company : BaseEntity
    {
        /// <summary>
        /// Gets or sets the customer identifier
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the customer role identifier
        /// </summary>
        public int CustomerRoleId { get; set; }

        /// <summary>
        /// Gets or sets the Point
        /// </summary>
        public int Point { get; set; }

        ///<summary>
        ///Gets or sets the subsidized Code identifier
        ///</summary>
        public string SubsidizedCode { get; set; }
    }
}
