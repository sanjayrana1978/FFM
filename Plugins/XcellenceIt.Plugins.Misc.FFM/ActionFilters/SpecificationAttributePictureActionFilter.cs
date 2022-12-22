using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Services.Media;
using Nop.Web.Models.Catalog;
using System;
using System.Threading.Tasks;
using XcellenceIt.Plugins.Misc.FFM.Services.ProductAdditionalInfomation;
using XcellenceIt.Plugins.Misc.FFM.Services.SpecificationAttributepictures;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Routing;

namespace XcellenceIt.Plugins.Misc.FFM.ActionFilters
{
    public class SpecificationAttributePictureActionFilter : IAsyncActionFilter
    {
        ISpecificationAttributePictureService _specificationAttributepictureService = EngineContext.Current.Resolve<ISpecificationAttributePictureService>();
        IPictureService _pictureservice = EngineContext.Current.Resolve<IPictureService>();
        ISpecificationAttributeService _specificationAttributeservice = EngineContext.Current.Resolve<ISpecificationAttributeService>();
        IProductService _productService = EngineContext.Current.Resolve<IProductService>();
        IProductAdditionalInfoService _productAdditionalInfoService = EngineContext.Current.Resolve<IProductAdditionalInfoService>();

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (context.HttpContext.Request == null)
                return;


            var controllerModel = context.Controller as Controller;

            var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            var actionName = actionDescriptor?.ActionName;
            var controllerName = actionDescriptor?.ControllerName;

            if (string.IsNullOrEmpty(actionName) || string.IsNullOrEmpty(controllerName))
                return;

            //Delete Product additional infomation  Picture
            if (controllerName.Equals("Product") && actionName.Equals("Delete") && context.HttpContext.Request.Method == "POST")
            {
                _ = int.TryParse(context.ActionArguments["Id"].ToString(), out int productid);

                if (productid > 0)
                {
                    int productdataid = (await _productService.GetProductByIdAsync(productid)).Id;
                    var productdata = _productAdditionalInfoService.GetProductAdditionalInfoByProductIdAsync(productdataid);
                    if (productdata != null)
                    {
                        await _productAdditionalInfoService.DeleteProductAdditionalInfoAsync(productdata);
                    }
                }

            }

            //Delete Specification Attribute Picture
            if (controllerName.Equals("SpecificationAttribute") && actionName.Equals("DeleteSpecificationAttribute") && context.HttpContext.Request.Method == "POST")
            {
                _ = int.TryParse(context.ActionArguments["Id"].ToString(), out int spcattributeId);

                if (spcattributeId > 0)
                {
                    int attributeId = (await _specificationAttributeservice.GetSpecificationAttributeByIdAsync(spcattributeId)).Id;
                    var picturedata = await _specificationAttributepictureService.GetPicturesBySpecificationAttributeId(attributeId);
                    if (picturedata != null)
                    {
                        await _specificationAttributepictureService.DeleteProductSpecificationAttributePictureAsync(picturedata);
                    }
                }

            }

            //Delete multiple Specific Attribute Picture
            if (controllerName.Equals("SpecificationAttribute") && actionName.Equals("DeleteSelectedSpecificationAttributes") && context.HttpContext.Request.Method == "POST")
            {
                var specificationpictureid = context.ActionArguments["selectedIds"] as ICollection<int>;
                var attributeId = (await _specificationAttributeservice.GetSpecificationAttributeByIdsAsync(specificationpictureid.ToArray()));
                foreach (var item in attributeId)
                {
                    var picturedata = await _specificationAttributepictureService.GetPicturesBySpecificationAttributeId(item.Id);
                    if (picturedata != null)
                    {
                        await _specificationAttributepictureService.DeleteProductSpecificationAttributePictureAsync(picturedata);
                    }
                }


            }


            //if (controllerName.Equals("Home") && actionName.Equals("Index") && context.HttpContext.Request.Method == "GET")
            //{
            //    // var ffmSettings = await _settingService.LoadSettingAsync<FFMSettings>();
            //    // if (ffmSettings.RedirectToCategoryId > 0)
            //    {
            //        //var categoryid = await _categoryService.GetCategoryByIdAsync(ffmSettings.RedirectToCategoryId);
            //        //var sename = await _urlRecordService.GetSeNameAsync(categoryid);

            //        //return new RedirectToRouteResult("RegisterResult", new { resultId = (int)UserRegistrationType.EmailValidation, returnUrl });
            //        //return new RedirectToRouteResult("RegisterResult", new { sename });
            //        //return RedirectToRoutePermanent("Category", new { SeName = await _urlRecordService.GetSeNameAsync(category) });
            //        //context.Result = new RedirectToRouteResult("Category", new { SeName = sename });

            //        //context.Result = new RedirectToActionResult("Maintenance", "States", null);

            //        //context.Result = new RedirectToRouteResult(
            //        //new RouteValueDictionary {{ "Controller", "PublicFFM" },
            //        //                  { "Action", "RedirectToCategory" }, { "sename", "sename"} });

            //        //if (context.Result != null)
            //        //    await next();
            //        //context.Result = new RedirectToRouteResult("SystemLogin", routeValues);

            //        //var values = new RouteValueDictionary(new
            //        //{
            //        //    action = "RedirectToCategory",
            //        //    controller = "PublicFFM",
            //        //    code = "0",
            //        //    values = sename
            //        //});
            //        //context.Result = new RedirectToRouteResult(values);

            //        //return RedirectToRouteResult(
            //        //new RouteValueDictionary {{ "Controller", "PublicFFM" },
            //        //                  { "Action", "RedirectToCategory" }, { "sename", sename} });
            //    }
            //}

            if (controllerName.Equals("Blog") && actionName.Equals("List") && context.HttpContext.Request.Method == "GET")
            {
                //context.Result = new RedirectToActionResult("Maintenance", "States", null);
                context.Result = new RedirectToRouteResult(
                    new RouteValueDictionary {{ "Controller", "PublicFFM" },
                                      { "Action", "RedirectToCategory" }, { "sename", "notebooks"} });
            }
            await next();

            if (controllerName.Equals("Product") && actionName.Equals("ProductDetails"))
            {
                var model = controllerModel.ViewData.Model as ProductDetailsModel;

                foreach (var specificationattributegroupdata in model.ProductSpecificationModel.Groups)
                {
                    var specificationattribute = specificationattributegroupdata.Attributes;

                    foreach (var specificationattributedata in specificationattribute)
                    {
                        var Picture = await _specificationAttributepictureService.GetPicturesBySpecificationAttributeId(specificationattributedata.Id);

                        if (Picture == null)
                            continue;

                        var pictureUrl = await _pictureservice.GetPictureUrlAsync(Picture.PictureId);
                        specificationattributedata.CustomProperties.Add("PictureUrl", pictureUrl);

                    }
                }
            }
        }
    }
}
