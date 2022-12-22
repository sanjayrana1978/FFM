using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;
using Nop.Web.Models.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XcellenceIt.Plugins.Misc.FFM.Factories;
using XcellenceIt.Plugins.Misc.FFM.Models.ProducstAdditinalInfo;
using XcellenceIt.Plugins.Misc.FFM.Services;
using XcellenceIt.Plugins.Misc.FFM.Services.ProductAdditionalInfomation;

namespace XcellenceIt.Plugins.Misc.FFM.Components
{
    [ViewComponent(Name = DefaultFFMStrings.DisplyFrontSideIngredientInfo)]
    public class FrontSideDisplayIngredientsViewComponent : NopViewComponent
    {
        private readonly IFFMServices _fFMServices;
        private readonly IProductAdditionalInfoService _productAdditionalInfoService;
        private readonly IProductAdditionalInfoFactory _productAdditionalInfoFactory;

        public FrontSideDisplayIngredientsViewComponent(IFFMServices fFMServices, 
            IProductAdditionalInfoService productAdditionalInfoService,
            IProductAdditionalInfoFactory productAdditionalInfoFactory)
        {
            _fFMServices = fFMServices;
            _productAdditionalInfoService = productAdditionalInfoService;
            _productAdditionalInfoFactory = productAdditionalInfoFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {
            if (!await _fFMServices.IsPluginEnable())
                return Content("");

            var modeldata = (ProductDetailsModel)additionalData;
            var productAdditionalInfo = await _productAdditionalInfoService.GetProductAdditionalInfoByIdAsync(modeldata.Id);
            var model = _productAdditionalInfoFactory.PrepareProductAdditionalInfoModelAsync(new ProductAdditionalInfoModel(), productAdditionalInfo, modeldata.Id);

            return View(model);
        }
    }
}
