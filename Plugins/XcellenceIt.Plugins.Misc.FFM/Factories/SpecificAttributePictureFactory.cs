using XcellenceIt.Plugins.Misc.FFM.Domain;
using XcellenceIt.Plugins.Misc.FFM.Models.SpecificationAttributePicture;

namespace XcellenceIt.Plugins.Misc.FFM.Factories
{
    public partial class SpecificAttributePictureFactory : ISpecificAttributePictureFactory
    {
        /// <summary>
        /// Prepare the picture model
        /// </summary>
        /// <param name="picture">picture</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// </returns>
        public SpecificationAttributePictureModel PreparePictureModelsAsync(SpecificationAttributePictureModel model,
            SpecificationAttributePictures specificationAttributePicture,
            int specificationId,
            bool excludeProperties = false)
        {
            if (specificationAttributePicture != null)
            {
                model.PictureId = specificationAttributePicture.PictureId;
            }

            model.SpecificationAttributeId = specificationId;

            return model;
        }

    }
}
