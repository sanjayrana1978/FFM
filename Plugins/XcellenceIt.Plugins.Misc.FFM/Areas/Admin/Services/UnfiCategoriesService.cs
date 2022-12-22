using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using XcellenceIt.Plugins.Misc.FFM.Domain;

namespace XcellenceIt.Plugins.Misc.FFM.Services.UnfiServices
{
    public class UnfiCategoriesService 
    {
        #region Fields

        private readonly IRepository<UnfiCategories> _unfiCategoriesRepository;

        #endregion

        #region Ctor

        public UnfiCategoriesService(IRepository<UnfiCategories> unfiCategoriesrepository)
        {
            _unfiCategoriesRepository = unfiCategoriesrepository; 
        }

        #endregion

        #region Methods

        public async Task<IPagedList<UnfiCategories>> GetAllUnfiCategoriesAsync(int pageIndex = 0,
            int pageSize = int.MaxValue)
        {
            var unfiCategories = await _unfiCategoriesRepository.GetAllPagedAsync(query =>
            {
                return query;

            }, pageIndex, pageSize);

            return unfiCategories;
        }

        public async Task<UnfiCategories> GetUnfiCategoryByIdAsync(int unfiCategoriesId)
        {
            return await _unfiCategoriesRepository.GetByIdAsync(unfiCategoriesId, cache => default);
        }

        public async Task InsertUnfiCategoryAsync(UnfiCategories unfiCategories)
        {
            await _unfiCategoriesRepository.InsertAsync(unfiCategories);
        }

        public async Task DeleteUnfiCategoryAsync(UnfiCategories unfiCategories)
        {
            await _unfiCategoriesRepository.DeleteAsync(unfiCategories);
        }

        public virtual async Task<IList<UnfiCategories>> GetUnifiCategoryByIdsAsync(int[] unfiCategoryIds)
        {
            return await _unfiCategoriesRepository.GetByIdsAsync(unfiCategoryIds);
        }

        public virtual async Task DeleteUnfiCategoriesAsync(IList<UnfiCategories> unfiCategory)
        {
            await _unfiCategoriesRepository.DeleteAsync(unfiCategory);
        }

        public virtual UnfiCategories FindUnfiCategory(IList<UnfiCategories> source, int unfiCategoryId, int categoriesId)
        {
            foreach (var unfiCategories in source)
                if (unfiCategories.UnfiCategory == unfiCategoryId && unfiCategories.Categories == categoriesId)
                    return unfiCategories;

            return null;
        }

        #endregion
    }
}
