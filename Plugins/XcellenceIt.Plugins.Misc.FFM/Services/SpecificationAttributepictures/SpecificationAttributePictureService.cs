using Nop.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using XcellenceIt.Plugins.Misc.FFM.Domain;

namespace XcellenceIt.Plugins.Misc.FFM.Services.SpecificationAttributepictures
{
    public class SpecificationAttributePictureService : ISpecificationAttributePictureService
    {
        private readonly IRepository<SpecificationAttributePictures> _specificationAttributePicturesrepository;

        public SpecificationAttributePictureService(IRepository<SpecificationAttributePictures> specificationAttributePicturesrepository)
        {
            _specificationAttributePicturesrepository = specificationAttributePicturesrepository;
        }

        /// <summary>
        /// Inserts a specificationAttribute  picture
        /// </summary>
        /// <param name="specificationAttributePicture">specificationAttribute picture</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertPictureAsync(SpecificationAttributePictures specificationAttributePicture)
        {
            await _specificationAttributePicturesrepository.InsertAsync(specificationAttributePicture);
        }


        /// <summary>
        /// Updates a specificationAttribute picture
        /// </summary>
        /// <param name="specificationAttributePicture">specificationAttribute picture</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdatePictureAsync(SpecificationAttributePictures specificationAttributePicture)
        {
            await _specificationAttributePicturesrepository.UpdateAsync(specificationAttributePicture);
        }

        /// <summary>
        /// get a specification attribute Picture
        /// </summary>
        /// <param name="specificationattributeId"> specification attribute picture</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task<SpecificationAttributePictures> GetPicturesBySpecificationAttributeId(int specificationattributeId)
        {
            return  await _specificationAttributePicturesrepository.Table.FirstOrDefaultAsync(x => x.SpecificationAttributeId == specificationattributeId);

        }

        /// <summary>
        /// Deletes a specification attribute Picture
        /// </summary>
        /// <param name="productSpecificationAttributePicture"> specification attribute picture</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteProductSpecificationAttributePictureAsync(SpecificationAttributePictures productSpecificationAttributePicture)
        {
            if (productSpecificationAttributePicture == null)
                throw new ArgumentNullException(nameof(productSpecificationAttributePicture));

            await _specificationAttributePicturesrepository.DeleteAsync(productSpecificationAttributePicture);
        }

    }
}
