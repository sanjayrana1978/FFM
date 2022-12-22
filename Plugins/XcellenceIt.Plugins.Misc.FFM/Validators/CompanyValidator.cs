using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using XcellenceIt.Plugins.Misc.FFM.Models.Company;

namespace XcellenceIt.Plugins.Misc.FFM.Validators
{
    public class CompanyValidator : BaseNopValidator<CompanyModel>
    {
        public CompanyValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotNull().WithMessageAwait(localizationService.GetResourceAsync("Plugins.Misc.FFM.Company.Name.Required"))
                .NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Plugins.Misc.FFM.Company.Name.Required"));
            RuleFor(x => x.Point).GreaterThan(0).WithMessageAwait(localizationService.GetResourceAsync("Plugins.Misc.FFM.Company.Point.Required"));
        }
    }
}
