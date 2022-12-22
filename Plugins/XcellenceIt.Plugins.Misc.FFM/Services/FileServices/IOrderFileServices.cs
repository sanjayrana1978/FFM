using Nop.Core.Domain.Orders;
using System.Collections.Generic;
using System.Threading.Tasks;
using XcellenceIt.Plugins.Misc.FFM.Models.FilesReadingModels;

namespace XcellenceIt.Plugins.Misc.FFM.Services.FileServices
{
    public interface IOrderFileServices
    {
        /// <summary>
        ///  Create text order file
        /// </summary>
        /// <param name="path">path</param>
        /// <param name="contents">contents</param>
        /// <param name="fileName">fileName</param>
        Task CreateCutomerOrderTextFile(string path, List<string> contents, string fileName);

        /// <summary>
        /// Get customer orders by customerId
        /// </summary>
        /// <param name="customerId">customerId</param>
        /// <returns>Orders list</returns>
        Task<List<Order>> GetCustomerOrdersByCustomerId(int customerId);

        /// <summary>
        /// Get UnfiNumber  
        /// </summary>
        /// <returns>UnfiNumber</returns>
        Task<string> GetUnfiNumberAsync();

        /// <summary>
        /// Delete schedule task Read Files From FTP
        /// </summary>
        Task DeleteScheduleTaskAsync();

        /// <summary>
        /// Delete Message Template
        /// </summary>
        Task DeleteMessageTemplate();

        /// <summary>
        /// Manage customer order text file
        /// </summary>
        /// <param name="order">order</param> 
        /// <param name="unfiNumber">unfiNumber</param> 
        Task ManageCustomerOrderTextFile(Order order, string unfiNumber);

        /// <summary>
        /// Prepare confirmation model
        /// </summary>
        /// <param name="contents">contents</param>
        /// <returns>ConfirmationFileReadModel</returns>
        ConfirmationFileReadModel PrepareConfirmationModel(string[] contents);

        /// <summary>
        /// Prepare invoice model
        /// </summary>
        /// <param name="contents">contents</param>
        /// <returns>InvoiceFileReadModel</returns>
        InvoiceFileReadModel PrepareInvoiceModel(string[] contents);

        /// <summary>
        /// Prepare tracking model
        /// </summary>
        /// <param name="contents">contents</param>
        /// <returns>TrackingFileReadModel</returns>
        TrackingFileReadModel PrepareTrackingModel(string[] contents);

        /// <summary>
        /// Send Order Conformation Mail to Customew
        /// </summary>
        /// <returns>Send a mail to customer</returns>
        Task SendOrderConformationMailToCustomer(string folderName, string fileName);

        /// <summary>
        /// Manage file read scheduler task
        /// </summary>
        Task ManageFileReadSchedulerTask();

        /// <summary>
        /// FileRead
        /// </summary>
        /// <param name="folderName"></param>
        /// <returns>string[]</returns>
        string[] FileRead(string folderName);

        /// <summary>
        /// File move on backup
        /// </summary>
        /// <param name="folderName">folderName</param>
        /// <param name="FileName">FileName</param> 
        void FileMoveOnBackup(string folderName, string FileName);

        /// <summary>
        /// Insert schedule task
        /// </summary>
        Task InsertScheduleTaskAsync();

        //Task SendOrderCancelMailToCustomer(string folderName, string fileName, ConfirmationFileReadModel confirmationFileReadModel);

    }
}
