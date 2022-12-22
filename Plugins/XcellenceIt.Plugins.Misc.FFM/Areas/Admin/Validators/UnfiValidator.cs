using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using XcellenceIt.Plugins.Misc.FFM.Models.UNFI;

namespace XcellenceIt.Plugins.Misc.FFM.Areas.Admin.UnfiValidator
{
    public class UnfiValidator : BaseNopValidator<UnfiCategoriesModel>
    {
        public UnfiValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.UnfiCategory).NotNull().WithMessageAwait(localizationService.GetResourceAsync("Plugins.Misc.FFM.Company.Name.Required"))
                .NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Plugins.Misc.FFM.Company.Name.Required"));
        }
    }
}
