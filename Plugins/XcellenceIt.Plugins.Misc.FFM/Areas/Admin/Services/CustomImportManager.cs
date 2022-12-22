using ClosedXML.Excel;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Services.ExportImport.Help;
using Nop.Services.Localization;
using Nop.Services.Logging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using XcellenceIt.Plugins.Misc.FFM.Domain;
using static Nop.Services.ExportImport.ImportManager;

namespace XcellenceIt.Plugins.Misc.FFM.Areas.Admin.Services
{
    public class CustomImportManager
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly ImportToCSVService _importToCSVService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Ctor

        public CustomImportManager(CatalogSettings catalogSettings,
            ImportToCSVService importToCSVService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService)
        {
            _catalogSettings = catalogSettings;
            _importToCSVService = importToCSVService;
            _customerActivityService = customerActivityService;
            _localizationService = localizationService;
        }

        #endregion

        #region Fields

        /// <summary>
        /// Import products from XLSX file
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task ImportToCSVAsync(Stream stream)
        {
            // get the first worksheet in the workbook
            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheets.FirstOrDefault();
            if (worksheet == null)
                throw new NopException("No worksheet found");

            //the columns
            var properties = GetPropertiesByExcelCells<ImportToCSV>(worksheet);

            var manager = new PropertyManager<ImportToCSV>(properties, _catalogSettings);
            var iRow = 2;

            while (true)
            {
                var allColumnsAreEmpty = manager.GetProperties
                    .Select(property => worksheet.Row(iRow).Cell(property.PropertyOrderPosition))
                    .All(cell => cell?.Value == null || string.IsNullOrEmpty(cell.Value.ToString()));

                if (allColumnsAreEmpty)
                    break;

                manager.ReadFromXlsx(worksheet, iRow);

                var importToCSV = await _importToCSVService.GetImportToCSVByNameAndSKU(manager.GetProperty("ProductName").StringValue, manager.GetProperty("sku").StringValue);

                var isNew = importToCSV == null;

                importToCSV ??= new ImportToCSV();


                foreach (var property in manager.GetProperties)
                {
                    switch (property.PropertyName)
                    {
                        case "ProductName":
                            importToCSV.ProductName = property.StringValue;
                            break;
                        case "sku":
                            importToCSV.Sku = property.StringValue;
                            break;
                    }
                }

                if (isNew)
                    await _importToCSVService.InsertImportToCSVAsync(importToCSV);
                else
                    await _importToCSVService.UpdateImportToCSVAsync(importToCSV);

                iRow++;
            }
        }

        #endregion
    }
}
