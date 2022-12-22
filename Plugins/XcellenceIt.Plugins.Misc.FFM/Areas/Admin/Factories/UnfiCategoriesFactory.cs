using Nop.Services.Catalog;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Framework.Models.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;
using XcellenceIt.Plugins.Misc.FFM.Areas.Admin.Models.UNFI;
using XcellenceIt.Plugins.Misc.FFM.Domain;
using XcellenceIt.Plugins.Misc.FFM.Models.UNFI;
using XcellenceIt.Plugins.Misc.FFM.Services.UnfiServices;

namespace XcellenceIt.Plugins.Misc.FFM.Areas.Admin.Factories
{
    public class UnfiCategoriesFactory
    {
        #region Fields

        private readonly UnfiCategoriesService _unfiCategoriesService;
        private readonly ICategoryService _categoryService;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;

        #endregion

        #region Ctor

        public UnfiCategoriesFactory(UnfiCategoriesService unfiCategoriesService,
            ICategoryService categoryService,
            IBaseAdminModelFactory baseAdminModelFactory)
        {
            _unfiCategoriesService = unfiCategoriesService;
            _categoryService = categoryService;
            _baseAdminModelFactory = baseAdminModelFactory;
        }

        #endregion

        #region Methods

        public async Task<UnfiCategoriesSearchModel> PrepareUnfiCategoriesSearchModelAsync(UnfiCategoriesSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            await _baseAdminModelFactory.PrepareCategoriesAsync(searchModel.unfiCategories.AllCategories, false);
            return searchModel;
        }

        public async Task<UnfiCategoriesListModel> PrepareUnfiCategoriesListModelAsync(UnfiCategoriesSearchModel searchModel)
        {
            var unfiCategories = await _unfiCategoriesService.GetAllUnfiCategoriesAsync(pageIndex: searchModel.Page - 1,
                pageSize: searchModel.PageSize);
            var categories = await _categoryService.GetAllCategoriesAsync();
            
            var model = await new UnfiCategoriesListModel().PrepareToGridAsync(searchModel, unfiCategories, () =>
            {
                return unfiCategories.SelectAwait(async unifiCategory =>
                {
                    return new UnfiCategoriesModel
                    {
                        Id = unifiCategory.Id,
                        CategoryName = (await _categoryService.GetCategoryByIdAsync(unifiCategory.Categories)).Name,
                        UnfiCategoryName = (await _categoryService.GetCategoryByIdAsync(unifiCategory.UnfiCategory)).Name,
                    };
                });
            });
            //await _baseAdminModelFactory.PrepareCategoriesAsync(searchModel.unfiCategories.AllCategories, false);
            return model;
        }

        #endregion
    }
}
