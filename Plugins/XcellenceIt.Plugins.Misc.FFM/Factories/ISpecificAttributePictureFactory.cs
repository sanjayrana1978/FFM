using XcellenceIt.Plugins.Misc.FFM.Domain;
using XcellenceIt.Plugins.Misc.FFM.Models.SpecificationAttributePicture;

namespace XcellenceIt.Plugins.Misc.FFM.Factories
{
    public partial interface ISpecificAttributePictureFactory
    {
        /// <summary>
        /// Prepare the picture model
        /// </summary>
        /// <param name="picture">picture</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// </returns>
        SpecificationAttributePictureModel PreparePictureModelsAsync(SpecificationAttributePictureModel model,
            SpecificationAttributePictures specificationAttributePicture,
            int specificationId,
            bool excludeProperties = false);
    }
}
