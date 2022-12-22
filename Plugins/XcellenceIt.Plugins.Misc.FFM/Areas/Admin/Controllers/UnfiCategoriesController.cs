using DocumentFormat.OpenXml.EMMA;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XcellenceIt.Plugins.Misc.FFM.Areas.Admin.Factories;
using XcellenceIt.Plugins.Misc.FFM.Domain;
using XcellenceIt.Plugins.Misc.FFM.Factories;
using XcellenceIt.Plugins.Misc.FFM.Models.UNFI;
using XcellenceIt.Plugins.Misc.FFM.Services.UnfiServices;

namespace XcellenceIt.Plugins.Misc.FFM.Areas.Admin.Controllers
{
    public class UnfiCategoriesController : BaseAdminController
    {
        #region Fields

        private readonly UnfiCategoriesFactory _unfiCategoriesFactory;
        private readonly UnfiCategoriesService _unfiCategoriesService;
        private readonly ICategoryService _categoryService;
        private readonly INotificationService _notificationService;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Ctor

        public UnfiCategoriesController(UnfiCategoriesFactory unfiCategoriesFactory, UnfiCategoriesService unfiCategoriesService, ICategoryService categoryService, INotificationService notificationService, ILocalizationService localizationService)
        {
            _unfiCategoriesFactory = unfiCategoriesFactory;
            _unfiCategoriesService = unfiCategoriesService;
            _categoryService = categoryService;
            _notificationService = notificationService;
            _localizationService = localizationService;
        }

        #endregion

        #region UNFI Categories

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var model = await _unfiCategoriesFactory.PrepareUnfiCategoriesSearchModelAsync(new UnfiCategoriesSearchModel());

            return View("~/Plugins/Misc.FFM/Areas/Admin/Views/UnfiCategories/List.cshtml", model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(UnfiCategoriesSearchModel unfiCategoriesSearchModel)
        {
            //prepare model
            var model = await _unfiCategoriesFactory.PrepareUnfiCategoriesListModelAsync(unfiCategoriesSearchModel);
            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> CreateUnfiCategoryMapping(UnfiCategoriesModel unfiCategories)
        {
            if (ModelState.IsValid)
            {
                //var categories = await _unfiCategoriesService.GetAllUnfiCategoriesAsync();
                foreach (var category in unfiCategories.UnfiCategory)
                {
                    var unficategories = (await _unfiCategoriesService.GetAllUnfiCategoriesAsync()).ToList();
                    if (_unfiCategoriesService.FindUnfiCategory(unficategories, category, unfiCategories.Categories) != null)
                        continue;
                    //insert the new unficategory category mapping
                    await _unfiCategoriesService.InsertUnfiCategoryAsync(new UnfiCategories
                    {
                        Categories = unfiCategories.Categories,
                        UnfiCategory = category
                    });

                    //Unpublish UNFI categories
                    var unifiCategory = await _categoryService.GetCategoryByIdAsync(category);
                    unifiCategory.Published = false;
                    await _categoryService.UpdateCategoryAsync(unifiCategory);

                }
                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Misc.FFM.UnfiCategories.Add"));
            }

            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual async Task<IActionResult> UnfiCategoryDelete(int id)
        {
            var category = await _unfiCategoriesService.GetUnfiCategoryByIdAsync(id);
            await _unfiCategoriesService.DeleteUnfiCategoryAsync(category);
            return new NullJsonResult();
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected(ICollection<int> selectedIds)
        {

            if (selectedIds == null || selectedIds.Count == 0)
                return NoContent();

            var productReviews = await _unfiCategoriesService.GetUnifiCategoryByIdsAsync(selectedIds.ToArray());

            await _unfiCategoriesService.DeleteUnfiCategoriesAsync(productReviews);

            return Json(new { Result = true });
        }

        #endregion
    }
}
