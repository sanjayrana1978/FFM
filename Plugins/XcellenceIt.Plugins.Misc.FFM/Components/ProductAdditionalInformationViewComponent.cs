using Microsoft.AspNetCore.Mvc;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Components;
using System.Threading.Tasks;
using XcellenceIt.Plugins.Misc.FFM.Factories;
using XcellenceIt.Plugins.Misc.FFM.Models.ProducstAdditinalInfo;
using XcellenceIt.Plugins.Misc.FFM.Services;
using XcellenceIt.Plugins.Misc.FFM.Services.ProductAdditionalInfomation;

namespace XcellenceIt.Plugins.Misc.FFM.Components
{
    [ViewComponent(Name = DefaultFFMStrings.ProductAdditinalInfo)]
    public class ProductAdditionalInformationViewComponent : NopViewComponent
    {
        private readonly IFFMServices _fFMServices;
        private readonly IProductAdditionalInfoService _productAdditionalInfoService;
        private readonly IProductAdditionalInfoFactory _productAdditionalInfoFactory;


        public ProductAdditionalInformationViewComponent(IFFMServices fFMServices, 
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

            var modeldata = (ProductModel)additionalData;
            var productAdditionalInfo = await _productAdditionalInfoService.GetProductAdditionalInfoByIdAsync(modeldata.Id);
            var model = _productAdditionalInfoFactory.PrepareProductAdditionalInfoModelAsync(new ProductAdditionalInfoModel(), productAdditionalInfo, modeldata.Id);

            return View(model);
        }
    }
}
