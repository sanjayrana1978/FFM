using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using XcellenceIt.Plugins.Misc.FFM.Domain;
using System.Linq;

namespace XcellenceIt.Plugins.Misc.FFM.Areas.Admin.Services
{
    public class ImportToCSVService 
    {
        #region Fields

        private readonly IRepository<ImportToCSV> _importToCSVrepository;

        #endregion

        #region Ctor

        public ImportToCSVService(IRepository<ImportToCSV> importToCSVrepository)
        {
            _importToCSVrepository = importToCSVrepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets all Import to CSV
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<IPagedList<ImportToCSV>> GetAllImportToCSVAsync(int pageIndex = 0,
           int pageSize = int.MaxValue)
        {
            var unfiCategories = await _importToCSVrepository.GetAllPagedAsync(query =>
            {
                return query;

            }, pageIndex, pageSize);

            return unfiCategories;
        }

        /// <summary>
        /// Get Import to CSV by identifier
        /// </summary>
        /// <param name="importToCSVId"></param>
        /// <returns></returns>
        public async Task<ImportToCSV> GetImportToCSVByIdAsync(int importToCSVId)
        {
            return await _importToCSVrepository.GetByIdAsync(importToCSVId, cache => default);
        }

        /// <summary>
        /// Insert Import to CSV
        /// </summary>
        /// <param name="importToCSV"></param>
        /// <returns></returns>
        public async Task InsertImportToCSVAsync(ImportToCSV importToCSV)
        {
            await _importToCSVrepository.InsertAsync(importToCSV);
        }

        /// <summary>
        /// Updates Import to CSV
        /// </summary>
        /// <param name="importToCSV">Manufacturer</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task UpdateImportToCSVAsync(ImportToCSV importToCSV)
        {
            await _importToCSVrepository.UpdateAsync(importToCSV);
        }

        /// <summary>
        /// Delete Import to CSV
        /// </summary>
        /// <param name="importToCSV"></param>
        /// <returns></returns>
        public async Task DeleteImportToCSVAsync(ImportToCSV importToCSV)
        {
            await _importToCSVrepository.DeleteAsync(importToCSV);
        }

        /// <summary>
        /// Gets multiple Import to CSV
        /// </summary>
        /// <param name="importToCSVIds"></param>
        /// <returns></returns>
        public virtual async Task<IList<ImportToCSV>> GetImportToCSVByIdsAsync(int[] importToCSVIds)
        {
            return await _importToCSVrepository.GetByIdsAsync(importToCSVIds);
        }

        /// <summary>
        /// Deletes multiple Import to CSV
        /// </summary>
        /// <param name="importToCSV"></param>
        /// <returns></returns>
        public virtual async Task DeleteImportToCSVsAsync(IList<ImportToCSV> importToCSV)
        {
            await _importToCSVrepository.DeleteAsync(importToCSV);
        }

        public async Task<ImportToCSV> GetImportToCSVByNameAndSKU(string name , string sku)
        {
            var query = await (from p in _importToCSVrepository.Table
                        where p.ProductName == name && p.Sku == sku
                        select p).FirstOrDefaultAsync();
            return query;
        }

        #endregion
    }
}
