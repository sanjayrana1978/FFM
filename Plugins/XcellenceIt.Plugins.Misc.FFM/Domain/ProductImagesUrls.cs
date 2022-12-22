using Nop.Core;

namespace XcellenceIt.Plugins.Misc.FFM.Domain
{
    /// <summary>
    /// Represents a Product ImagesUrls
    /// </summary>
    public class ProductImagesUrls : BaseEntity
    {
        /// <summary>
        /// Gets or sets the Product identifier
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Gets or sets ImageUrl 
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Gets or sets Picture identifier
        /// </summary>
        public int PictureId { get; set; }
    }
}
