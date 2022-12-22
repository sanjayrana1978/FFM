using System.Threading.Tasks;
using XcellenceIt.Plugins.Misc.FFM.Domain;

namespace XcellenceIt.Plugins.Misc.FFM.Services.SpecificationAttributepictures
{
    public interface ISpecificationAttributePictureService
    {

        /// <summary>
        /// Inserts a specificationAttribute  picture
        /// </summary>
        /// <param name="specificationAttributePicture">specificationAttribute picture</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertPictureAsync(SpecificationAttributePictures specificationAttributePicture);


        /// <summary>
        /// Updates a specificationAttribute picture
        /// </summary>
        /// <param name="specificationAttributePicture">specificationAttribute picture</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdatePictureAsync(SpecificationAttributePictures specificationAttributePicture);

        /// <summary>
        /// get a specification attribute Picture
        /// </summary>
        /// <param name="specificationattributeId"> specification attribute picture</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task<SpecificationAttributePictures> GetPicturesBySpecificationAttributeId(int specificationattributeId);

        /// <summary>
        /// Deletes a specification attribute Picture
        /// </summary>
        /// <param name="specificationattributeId"> specification attribute picture</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteProductSpecificationAttributePictureAsync(SpecificationAttributePictures productSpecificationAttributePicture);
    }
}
