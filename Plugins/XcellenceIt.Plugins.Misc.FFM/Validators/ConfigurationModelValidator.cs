using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using XcellenceIt.Plugins.Misc.FFM.Models;

namespace XcellenceIt.Plugins.Misc.FFM.Validators
{
    public class ConfigurationModelValidator : BaseNopValidator<ConfigurationModel>
    {
        public ConfigurationModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Api).NotNull().WithMessageAwait(localizationService.GetResourceAsync("Plugins.Misc.FFM.Configuration.Api.Required"))
                .NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Plugins.Misc.FFM.Configuration.Api.Required"));
            RuleFor(x => x.RequestHeaderName).NotNull().WithMessageAwait(localizationService.GetResourceAsync("Plugins.Misc.FFM.Configuration.RequestHeaderName.Required"))
                .NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Plugins.Misc.FFM.Configuration.RequestHeaderName.Required"));
            RuleFor(x => x.PrimaryKey).NotNull().WithMessageAwait(localizationService.GetResourceAsync("Plugins.Misc.FFM.Configuration.PrimaryKey.Required"))
                .NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Plugins.Misc.FFM.Configuration.PrimaryKey.Required"));
            RuleFor(x => x.SecondaryKey).NotNull().WithMessageAwait(localizationService.GetResourceAsync("Plugins.Misc.FFM.Configuration.SecondaryKey.Required"))
                .NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Plugins.Misc.FFM.Configuration.SecondaryKey.Required"));
            RuleFor(x => x.PageSize).NotEqual(0).WithMessageAwait(localizationService.GetResourceAsync("Plugins.Misc.FFM.Configuration.PageSize.Required"));
            RuleFor(x => x.UNFINumber).GreaterThan(0).WithMessageAwait(localizationService.GetResourceAsync("Plugins.Misc.FFM.Configuration.UNFINumber.Required"));
            RuleFor(x => x.FtpHost).NotNull().WithMessageAwait(localizationService.GetResourceAsync("Plugins.Misc.FFM.Configuration.FtpHost.Required"))
                .NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Plugins.Misc.FFM.Configuration.FtpHost.Required"));
            RuleFor(x => x.FtpUserName).NotNull().WithMessageAwait(localizationService.GetResourceAsync("Plugins.Misc.FFM.Configuration.FtpUserName.Required"))
               .NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Plugins.Misc.FFM.Configuration.FtpUserName.Required"));
            RuleFor(x => x.FtpPassword).NotNull().WithMessageAwait(localizationService.GetResourceAsync("Plugins.Misc.FFM.Configuration.FtpPassword.Required"))
               .NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Plugins.Misc.FFM.Configuration.FtpPassword.Required"));
        }
    }
}
