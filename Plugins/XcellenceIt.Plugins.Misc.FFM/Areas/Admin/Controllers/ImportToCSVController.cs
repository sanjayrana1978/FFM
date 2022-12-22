using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Framework.Mvc;
using System;
using System.Threading.Tasks;
using XcellenceIt.Plugins.Misc.FFM.Areas.Admin.Factories;
using XcellenceIt.Plugins.Misc.FFM.Areas.Admin.Models.ImportToCSV;
using XcellenceIt.Plugins.Misc.FFM.Areas.Admin.Services;

namespace XcellenceIt.Plugins.Misc.FFM.Areas.Admin.Controllers
{
    public class ImportToCSVController : BaseAdminController
    {
        #region Fields

        private readonly ImportToCSVModelFactory _importToCSVModelFactory;
        private readonly ImportToCSVService _importToCSVService;
        private readonly IPermissionService _permissionService;
        private readonly INotificationService _notificationService;
        private readonly ILocalizationService _localizationService;
        private readonly CustomImportManager _customImportManager;

        #endregion

        #region Ctor

        public ImportToCSVController(ImportToCSVModelFactory importToCSVModelFactory,
            ImportToCSVService importToCSVService,
            IPermissionService permissionService,
            INotificationService notificationService,
            ILocalizationService localizationService,
            CustomImportManager customImportManager)
        {
            _importToCSVModelFactory = importToCSVModelFactory;
            _importToCSVService = importToCSVService;
            _permissionService = permissionService;
            _notificationService = notificationService;
            _localizationService = localizationService;
            _customImportManager = customImportManager;
        }

        #endregion

        #region Methods

        public async Task<IActionResult> List()
        {
            //prepare model
            var model = await _importToCSVModelFactory.PrepareImportToCSVSearchModelAsync(new ImportToCSVSearchModel());

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> List(ImportToCSVSearchModel searchModel)
        {
            //prepare model
            var model = await _importToCSVModelFactory.PrepareImportToCSVListModelAsync(searchModel);

            return Json(model);
        }

        public async Task<IActionResult> ImportToCSVUpdate(ImportToCSVModel model)
        {
            var importToCsv = await _importToCSVService.GetImportToCSVByIdAsync(model.Id);
            if (importToCsv == null)
                return RedirectToAction("List");

            var check = await _importToCSVService.GetImportToCSVByNameAndSKU(model.ProductName, model.Sku);
            if (check == null)
            {
                importToCsv.ProductName = model.ProductName;
                importToCsv.Sku = model.Sku;
                await _importToCSVService.UpdateImportToCSVAsync(importToCsv);
            }

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteImportToCSV(int id)
        {
            var category = await _importToCSVService.GetImportToCSVByIdAsync(id);
            await _importToCSVService.DeleteImportToCSVAsync(category);
            return new NullJsonResult();
        }

        [HttpPost]
        public virtual async Task<IActionResult> ImportToCSV(IFormFile importexcelfile)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            try
            {
                if (importexcelfile != null && importexcelfile.Length > 0)
                {
                    await _customImportManager.ImportToCSVAsync(importexcelfile.OpenReadStream());
                }
                else
                {
                    _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Common.UploadFile"));

                    return RedirectToAction("List");
                }

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Products.Imported"));

                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                await _notificationService.ErrorNotificationAsync(exc);

                return RedirectToAction("List");
            }
        }

        #endregion
    }
}
