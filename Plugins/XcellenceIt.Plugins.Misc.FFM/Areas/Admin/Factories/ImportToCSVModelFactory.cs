using Nop.Web.Framework.Models.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;
using XcellenceIt.Plugins.Misc.FFM.Areas.Admin.Models.ImportToCSV;
using XcellenceIt.Plugins.Misc.FFM.Areas.Admin.Services;
using XcellenceIt.Plugins.Misc.FFM.Domain;

namespace XcellenceIt.Plugins.Misc.FFM.Areas.Admin.Factories
{
    public class ImportToCSVModelFactory
    {
        #region Fields

        private readonly ImportToCSVService _importToCSVService;

        #endregion

        #region Ctor

        public ImportToCSVModelFactory(ImportToCSVService importToCSVService)
        {
            _importToCSVService = importToCSVService;
        }

        #endregion

        #region Methods

        public virtual Task<ImportToCSVSearchModel> PrepareImportToCSVSearchModelAsync(ImportToCSVSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return Task.FromResult(searchModel);
        }

        public async Task<ImportToCSVListModel> PrepareImportToCSVListModelAsync(ImportToCSVSearchModel searchModel)
        {
            var importToCSVs = await _importToCSVService.GetAllImportToCSVAsync(pageIndex: searchModel.Page - 1,
                pageSize: searchModel.PageSize);
            var model = await new ImportToCSVListModel().PrepareToGridAsync(searchModel, importToCSVs, () =>
            {
                return importToCSVs.SelectAwait(async importToCSV =>
                {
                    return new ImportToCSVModel
                    {
                        Id = importToCSV.Id,
                        ProductName = importToCSV.ProductName,
                        Sku = importToCSV.Sku
                    };
                });
            });

            return model;
        }

        public Task<ImportToCSVModel> PrepareImportToCSVModelAsync(ImportToCSVModel model, ImportToCSV importToCSV, bool excludeProperties = false)
        {
            if (importToCSV != null)
            {
                if (model == null)
                {
                    model.ProductName = importToCSV.ProductName;
                    model.Sku = importToCSV.Sku;
                }
            }
            return Task.FromResult(model);
        }

        #endregion
    }
}
