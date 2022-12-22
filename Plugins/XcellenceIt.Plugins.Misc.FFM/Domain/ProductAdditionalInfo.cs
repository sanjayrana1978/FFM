using Nop.Core;

namespace XcellenceIt.Plugins.Misc.FFM.Domain
{
    public class ProductAdditionalInfo : BaseEntity
    {
        /// <summary>
        /// Gets or sets Product identifier
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Gets or sets the full description of Ingredients
        /// </summary>
        public string IngredientsInfomation { get; set; }

        /// <summary>
        /// Gets or sets the full description of Nutritional
        /// </summary>
        public string NutritionalInfomation { get; set; }


    }
}
