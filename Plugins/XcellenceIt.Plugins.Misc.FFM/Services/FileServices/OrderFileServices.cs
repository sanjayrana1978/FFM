using Nop.Core;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.ScheduleTasks;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Web.Factories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XcellenceIt.Plugins.Misc.FFM.Models.FilesReadingModels;
using XcellenceIt.Plugins.Misc.FFM.Services.FTPServices;

namespace XcellenceIt.Plugins.Misc.FFM.Services.FileServices
{
    public class OrderFileServices : IOrderFileServices
    {
        #region Fields

        private readonly INopFileProvider _fileProvider;
        private readonly IOrderModelFactory _orderModelFactory;
        private readonly IRepository<Order> _orderRepository;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly ICustomProductServices _customProductServices;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly ICurrencyService _currencyService;
        private readonly IFTPFileService _fTPFileService;
        private readonly ILogger _logger;
        private readonly ILocalizationService _localizationService;
        private readonly IFFMServices _fFMServices;
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IWorkContext _workContext;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly IEmailAccountService _emailAccountService;
        private readonly IMessageTemplateService _messageTemplateService;
        private readonly IMessageTokenProvider _messageTokenProvider;
        private readonly IStoreService _storeService;
        private readonly ILanguageService _languageService;
        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IShipmentService _shipmentService;


        #endregion

        #region Ctor

        public OrderFileServices(INopFileProvider fileProvider,
            IOrderModelFactory orderModelFactory,
            IRepository<Order> orderRepository,
            IStateProvinceService stateProvinceService,
            ICustomProductServices customProductServices,
            ISettingService settingService,
            IStoreContext storeContext,
            ICurrencyService currencyService,
            IFTPFileService fTPFileService,
            ILogger logger,
            ILocalizationService localizationService,
            IFFMServices fFMServices,
            IOrderService orderService,
            ICustomerService customerService,
            IWorkflowMessageService workflowMessageService,
            IWorkContext workContext,
            EmailAccountSettings emailAccountSettings,
            IMessageTemplateService messageTemplateService,
            IEmailAccountService emailAccountService,
            IMessageTokenProvider messageTokenProvider,
            IStoreService storeService,
            ILanguageService languageService,
            IScheduleTaskService scheduleTaskService,
            IOrderProcessingService orderProcessingService,
            IShipmentService shipmentService)
        {
            _fileProvider = fileProvider;
            _orderModelFactory = orderModelFactory;
            _orderRepository = orderRepository;
            _stateProvinceService = stateProvinceService;
            _customProductServices = customProductServices;
            _settingService = settingService;
            _storeContext = storeContext;
            _currencyService = currencyService;
            _fTPFileService = fTPFileService;
            _logger = logger;
            _localizationService = localizationService;
            _fFMServices = fFMServices;
            _orderService = orderService;
            _customerService = customerService;
            _workflowMessageService = workflowMessageService;
            _workContext = workContext;
            _emailAccountSettings = emailAccountSettings;
            _messageTemplateService = messageTemplateService;
            _emailAccountService = emailAccountService;
            _messageTokenProvider = messageTokenProvider;
            _storeService = storeService;
            _languageService = languageService;
            _scheduleTaskService = scheduleTaskService;
            _orderProcessingService = orderProcessingService;
            _shipmentService = shipmentService;
        }

        #endregion

        #region utilities

        ///// <summary>
        ///// Get active message templates by the name
        ///// </summary>
        ///// <param name="messageTemplateName">Message template name</param>
        ///// <param name="storeId">Store identifier</param>
        ///// <returns>
        ///// A task that represents the asynchronous operation
        ///// The task result contains the list of message templates
        ///// </returns>
        protected virtual async Task<IList<MessageTemplate>> GetActiveMessageTemplatesAsync(string messageTemplateName, int storeId)
        {
            //get message templates by the name
            var messageTemplates = await _messageTemplateService.GetMessageTemplatesByNameAsync(messageTemplateName, storeId);

            //no template found
            if (!messageTemplates?.Any() ?? true)
                return new List<MessageTemplate>();

            //filter active templates
            messageTemplates = messageTemplates.Where(messageTemplate => messageTemplate.IsActive).ToList();

            return messageTemplates;
        }

        /// <summary>
        /// Prepare StringLength 
        /// </summary>
        /// <param name="str">str</param>
        /// <param name="length">length</param>
        /// <returns></returns>
        protected string PrepareStringLengthWise(string str, int length)
        {
            if (string.IsNullOrEmpty(str))
                return string.Empty;

            return str.Length > length ? str[..(length - 1)] : str;
        }

        /// <summary>
        /// PrepareState
        /// </summary>
        /// <param name="state">state</param>
        /// <param name="length">length</param>
        /// <returns></returns>
        protected async Task<string> PrepareState(string state, int length)
        {
            if (string.IsNullOrEmpty(state))
                return string.Empty;

            string abbreviation = string.Empty;

            if (!string.IsNullOrEmpty(state))
                abbreviation = state[..length];

            var stateProvince = await _customProductServices.GetStateProvinceByStateProvinceName(state);

            if (stateProvince != null)
                if (stateProvince.Abbreviation != null)
                    abbreviation = stateProvince.Abbreviation;

            return abbreviation;
        }

        /// <summary>
        /// Prepare File Name
        /// </summary>
        /// <param name="fileName">fileName</param>
        /// <param name="fileId">fileId</param>
        /// <param name="unfiNumber">unfiNumber</param>
        protected static string PrepareFileName(string fileName, int fileId, string unfiNumber)
        {
            if (fileName.Equals(DefaultFFMStrings.OrderTextFile))
                return "Order." + fileId + ".txt";

            if (fileName.Equals(DefaultFFMStrings.ConfirmationTextFile))
                return unfiNumber + ".O" + fileId + "A";

            if (fileName.Equals(DefaultFFMStrings.InvoiceTextFile))
                return unfiNumber + ".S" + fileId + "A";

            if (fileName.Equals(DefaultFFMStrings.TrackingTextFile))
                return unfiNumber + ".T" + fileId + "S";

            return string.Empty;
        }

        ///// <summary>
        ///// Ensure language is active
        ///// </summary>
        ///// <param name="languageId">Language identifier</param>
        ///// <param name="storeId">Store identifier</param>
        ///// <returns>
        ///// A task that represents the asynchronous operation
        ///// The task result contains the return a value language identifier
        ///// </returns>
        protected virtual async Task<int> EnsureLanguageIsActiveAsync(int languageId, int storeId)
        {
            //load language by specified ID
            var language = await _languageService.GetLanguageByIdAsync(languageId);

            if (language == null || !language.Published)
            {
                //load any language from the specified store
                language = (await _languageService.GetAllLanguagesAsync(storeId: storeId)).FirstOrDefault();
            }

            if (language == null || !language.Published)
            {
                //load any language
                language = (await _languageService.GetAllLanguagesAsync()).FirstOrDefault();
            }

            if (language == null)
                throw new Exception("No active language could be loaded");

            return language.Id;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Insert schedule task
        /// </summary>
        public async Task InsertScheduleTaskAsync()
        {
            var updateProductsFromApiTaskschedularTask = await _scheduleTaskService.GetTaskByTypeAsync("XcellenceIt.Plugins.Misc.FFM.ScheduleTask.UpdateProductsFromApiTask");
            string updateProductsFromApiTaskschedularTaskName = "XcellenceIt.Plugins.Misc.FFM.ScheduleTask.UpdateProductsFromApiTask";
            if (updateProductsFromApiTaskschedularTask == null)
                await _scheduleTaskService.InsertTaskAsync(new Nop.Core.Domain.ScheduleTasks.ScheduleTask()
                {
                    Name = DefaultFFMStrings.UpdateProductsFromApiTask,
                    Seconds = 21600,
                    Type = updateProductsFromApiTaskschedularTaskName,
                    Enabled = true
                });

            var readFilesFromFTPschedularTask = await _scheduleTaskService.GetTaskByTypeAsync("XcellenceIt.Plugins.Misc.FFM.ScheduleTask.ReadFilesFromFTP");
            string readFilesFromFTPschedularName = "XcellenceIt.Plugins.Misc.FFM.ScheduleTask.ReadFilesFromFTP";
            if (readFilesFromFTPschedularTask == null)
                await _scheduleTaskService.InsertTaskAsync(new Nop.Core.Domain.ScheduleTasks.ScheduleTask()
                {
                    Name = DefaultFFMStrings.ReadFilesFromFTP,
                    Seconds = 900,
                    Type = readFilesFromFTPschedularName,
                    Enabled = true
                });

            //AddRemove Reward point task
            if (await _scheduleTaskService.GetTaskByTypeAsync(DefaultFFMStrings.AddRemoveRewardPointTaskType) == null)
            {
                await _scheduleTaskService.InsertTaskAsync(new Nop.Core.Domain.ScheduleTasks.ScheduleTask
                {
                    Enabled = true,
                    Seconds = 43200,
                    Name = DefaultFFMStrings.AddRemoveRewardPointTaskName,
                    Type = DefaultFFMStrings.AddRemoveRewardPointTaskType,
                });
            }
        }

        /// <summary>
        /// Delete schedule task Read Files From FTP
        /// </summary>
        public virtual async Task DeleteScheduleTaskAsync()
        {
            var updateProductsFromApiTaskschedularTask = await _scheduleTaskService.GetTaskByTypeAsync("XcellenceIt.Plugins.Misc.FFM.ScheduleTask.UpdateProductsFromApiTask");
            if (updateProductsFromApiTaskschedularTask != null)
                await _scheduleTaskService.DeleteTaskAsync(updateProductsFromApiTaskschedularTask);

            var readFilesFromFTPschedularTask = await _scheduleTaskService.GetTaskByTypeAsync("XcellenceIt.Plugins.Misc.FFM.ScheduleTask.ReadFilesFromFTP");
            if (readFilesFromFTPschedularTask != null)
                await _scheduleTaskService.DeleteTaskAsync(readFilesFromFTPschedularTask);

            //AddRemove Reward point task
            var rewardpointtask = await _scheduleTaskService.GetTaskByTypeAsync(DefaultFFMStrings.AddRemoveRewardPointTaskType);
            if (rewardpointtask != null)
                await _scheduleTaskService.DeleteTaskAsync(rewardpointtask);

        }

        /// <summary>
        /// Delete Message Template
        /// </summary>
        public virtual async Task DeleteMessageTemplate()
        {
            var orderplaceconfirmationmessageTemplate = (await _messageTemplateService.GetMessageTemplatesByNameAsync(DefaultFFMStrings.OrderPlaceConfirmationMessageTemplate)).FirstOrDefault();
            if (orderplaceconfirmationmessageTemplate != null)
                await _messageTemplateService.DeleteMessageTemplateAsync(orderplaceconfirmationmessageTemplate);
        }

        /// <summary>
        ///  Create text order file
        /// </summary>
        /// <param name="path">path</param>
        /// <param name="contents">contents</param>
        /// <param name="fileName">fileName</param>
        public async Task CreateCutomerOrderTextFile(string path, List<string> contents, string fileName)
        {
            string directoryPath = _fileProvider.MapPath(string.Format("Plugins/Misc.FFM/FTPServerFiles/{0}", path.Split('\\')[^2]));

            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            try
            {
                FileStream fileStream = new(path, FileMode.Create);
                StreamWriter streamWriter = new(fileStream, Encoding.UTF8);

                foreach (var content in contents)
                    await streamWriter.WriteLineAsync(content);

                streamWriter.Close();

                await _logger.InsertLogAsync(LogLevel.Information, string.Format(await _localizationService.GetResourceAsync("Plugins.Misc.FFM.OrderFileGenerate.Success"), fileName));
            }
            catch (Exception exception)
            {
                await _logger.ErrorAsync(await _localizationService.GetResourceAsync("Plugins.Misc.FFM.FileGenerate.Error"), exception);
            }
        }

        /// <summary>
        /// Get customer orders by customerId
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns>Orders list</returns>
        public async Task<List<Order>> GetCustomerOrdersByCustomerId(int customerId)
        {
            return await _orderRepository.Table.Where(o => o.CustomerId == customerId).OrderBy(s => s.CreatedOnUtc).ToListAsync();
        }

        /// <summary>
        /// Manage customer order text file
        /// </summary>
        /// <param name="order">order</param> 
        /// <param name="unfiNumber">unfiNumber</param>  
        public async Task ManageCustomerOrderTextFile(Order order, string unfiNumber)
        {
            var orders = await GetCustomerOrdersByCustomerId(order.CustomerId);

            if (orders == null)
                return;

            string fileName = string.Format("Order.{0}.txt", order.CustomerId);
            var contents = new List<string> { };

            // Order counts
            contents.Add(orders.Count.ToString());

            foreach (var orderItem in orders)
            {
                var orderDetails = await _orderModelFactory.PrepareOrderDetailsModelAsync(orderItem);
                string leaveBlank = ",";
                string type = "FIL";

                // YOUR CUSTOMER NUMBER(Provided by Honest Green for each warehouse)
                string customer = unfiNumber + leaveBlank;

                // BLANK(LEAVE BLANK)
                customer += leaveBlank;

                // PURCHASE ORDER NUMBER(Must be unique for each order -cannot use same PO w /in 90 days)
                customer += orderDetails.Id + leaveBlank;

                // BLANK(LEAVE BLANK)
                customer += leaveBlank;

                // TYPE(Always use FIL for fulfillment)
                customer += type + leaveBlank;
                contents.Add(customer);

                // The second line of an order is used for fulfillment orders and is required if the type is FIL. The fields are as follows: 
                string address = "";

                // SHIP-TO NAME (30 characters MAX, required field) 
                string name = orderDetails.ShippingAddress.FirstName
                              + " "
                              + orderDetails.ShippingAddress.LastName;

                address = name.Length >= 30 ? name[..29] : name + leaveBlank;

                // SHIP-TO ADDRESS 1 (30 characters MAX, required field)  
                address += PrepareStringLengthWise(orderDetails.ShippingAddress.Address1, 30)
                           + leaveBlank;

                // SHIP-TO ADDRESS 2 (30 characters MAX, optional field) 
                address += PrepareStringLengthWise(orderDetails.ShippingAddress.Address2, 30)
                           + leaveBlank;

                // SHIP-TO CITY (18 characters MAX, required field) 
                address += PrepareStringLengthWise(orderDetails.ShippingAddress.City, 18)
                           + leaveBlank;

                // SHIP-TO STATE (2 characters MAX, state code, required field)
                address += await PrepareState(orderDetails.ShippingAddress.StateProvinceName, 2)
                           + leaveBlank;

                // SHIP-TO ZIP CODE (5 or 10 chars, required field, 99999-9999) 
                address += PrepareStringLengthWise(orderDetails.ShippingAddress.ZipPostalCode, 10)
                           + leaveBlank;

                // SHIP-TO PHONE NUMBER (9999999999) 
                address += orderDetails.ShippingAddress.PhoneNumber
                           + leaveBlank;

                // SHIP VIA CODE (Always use A) 
                address += "A"
                           + leaveBlank;

                // STORE NUMBER (Always use 5001 for fulfillment) 
                address += "5001";
                contents.Add(address);


                //The rest of the lines for an order are the products.The fields are: 
                foreach (var product in orderDetails.Items)
                {
                    //UNFI PRODUCT NUMBER(7 Digit Number – with the leading 0(if applicable))
                    string productDetails = product.Sku + leaveBlank;

                    //BLANK(LEAVE BLANK)
                    productDetails += leaveBlank;

                    //QUANTITY(99999)
                    productDetails += product.Quantity + leaveBlank;

                    //BLANK(LEAVE BLANK)
                    productDetails += leaveBlank;

                    //BLANK(LEAVE BLANK)
                    productDetails += leaveBlank;

                    ////BLANK(LEAVE BLANK)
                    //productDetails += leaveBlank;

                    contents.Add(productDetails);
                }

                //***EOF***
                contents.Add("***EOF***");
            }

            string path = _fileProvider.MapPath(string.Format("/Plugins/Misc.FFM/FTPServerFiles/Orders/Order.{0}.txt", order.CustomerId));

            await CreateCutomerOrderTextFile(path, contents, fileName);
            await _fTPFileService.UploadFileAsync(path, string.Format("Orders/{0}", fileName));

            FileMoveOnBackup(DefaultFFMStrings.OrderFolderName, fileName);
        }

        /// <summary>
        /// Get Unfi Number
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetUnfiNumberAsync()
        {
            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var ffmSettings = await _settingService.LoadSettingAsync<FFMSettings>(storeScope);
            return ffmSettings.UNFINumber != 0 ? ffmSettings.UNFINumber.ToString() : "";
        }

        /// <summary>
        /// Prepare confirmation model
        /// </summary>
        /// <param name="contents">contents</param>
        /// <returns>ConfirmationFileModel</returns>
        public ConfirmationFileReadModel PrepareConfirmationModel(string[] contents)
        {
            var l1 = contents[0].Split(",");
            var l2 = contents[1].Split(",");
            var trimquote = new char[] { '"' };

            var model = new ConfirmationFileReadModel();

            // Line 1
            model.SalesOrder = l1.ElementAtOrDefault(0);
            model.ShipToName = l1.ElementAtOrDefault(1);
            model.ShipToAddress1 = l1.ElementAtOrDefault(2);
            model.ShipToAddress2 = l1.ElementAtOrDefault(3);
            model.ShipToCityAndState = l1.ElementAtOrDefault(4);
            model.ShipToZipCode = l1.ElementAtOrDefault(5);
            model.CustomerPO = l1.ElementAtOrDefault(6).Trim(trimquote);
            model.ShipViaDescription = l1.ElementAtOrDefault(7);
            model.SalesOrderDate = l1.ElementAtOrDefault(8);

            // Line 2
            model.SubTotalAmount = l2.ElementAtOrDefault(0);
            model.FreightAmount = l2.ElementAtOrDefault(1);
            model.MiscChargeAmount = l2.ElementAtOrDefault(2);
            model.VolumeDiscountAmount = l2.ElementAtOrDefault(3);
            model.ProjectedTotalAmount = l2.ElementAtOrDefault(4);

            // Line 3
            model.ProductDetails = new List<ProductDetails>();
            var productDetails = new ProductDetails();

            foreach (var productdetails in contents.Skip(2))
            {
                var pro = productdetails.Split(",");
                productDetails.SNPartNumber = pro.ElementAtOrDefault(0);
                productDetails.UPCNumber = pro.ElementAtOrDefault(1);
                productDetails.PartDescription = pro.ElementAtOrDefault(2);
                productDetails.QuantityOrdered = pro.ElementAtOrDefault(3);
                productDetails.RetailPrice = pro.ElementAtOrDefault(4);
                productDetails.RegWholesale = pro.ElementAtOrDefault(5);
                productDetails.SaleWholesale = pro.ElementAtOrDefault(6);
                productDetails.ExtendedPrice = pro.ElementAtOrDefault(7);
                productDetails.UNFIPOLineNumber = pro.ElementAtOrDefault(8);
                productDetails.OnHandStatus = pro.ElementAtOrDefault(9);
                model.ProductDetails.Add(productDetails);
            }

            return model;
        }

        /// <summary>
        /// Prepare invlice model
        /// </summary>
        /// <param name="contents">contents</param>
        /// <returns>InvoiceFileReadModel</returns>
        public InvoiceFileReadModel PrepareInvoiceModel(string[] contents)
        {
            var l1 = contents[0].Split(",");
            var l2 = contents[1].Split(",");
            var l3 = contents[2].Split(",");

            var model = new InvoiceFileReadModel();

            // Line 1
            model.Invoice = l1.ElementAtOrDefault(0);
            model.ShipToName = l1.ElementAtOrDefault(1);
            model.ShipToAddress1 = l1.ElementAtOrDefault(2);
            model.ShipToAddress2 = l1.ElementAtOrDefault(3);
            model.ShipToCityAndState = l1.ElementAtOrDefault(4);
            model.ShipToZipCode = l1.ElementAtOrDefault(5);
            model.PhoneNumber = l1.ElementAtOrDefault(6);
            model.CustomerPO = l1.ElementAtOrDefault(7);
            model.ShipViaDescription = l1.ElementAtOrDefault(8);
            model.SalesOrder = l1.ElementAtOrDefault(9);
            model.ShippingDate = l1.ElementAtOrDefault(0);

            // Line 2
            model.Zero = l2.ElementAtOrDefault(0);

            // Line 3
            model.SubTotalAmount = l3.ElementAtOrDefault(0);
            model.FreightAmount = l3.ElementAtOrDefault(1);
            model.MiscChargeAmount = l3.ElementAtOrDefault(2);
            model.VolumeDiscountAmount = l3.ElementAtOrDefault(3);
            model.TaxAmount = l3.ElementAtOrDefault(4);
            model.InvoiceAmount = l3.ElementAtOrDefault(5);

            // Line 4
            model.ProductDetails = new List<InvoiceProductDetails>();
            var productDetails = new InvoiceProductDetails();

            foreach (var productdetails in contents.Skip(3))
            {
                var pro = productdetails.Split(",");
                productDetails.SNPartNumber = pro.ElementAtOrDefault(0);
                productDetails.UPCNumber = pro.ElementAtOrDefault(1);
                productDetails.PartDescription = pro.ElementAtOrDefault(2);
                productDetails.QuantityOrdered = pro.ElementAtOrDefault(3);
                productDetails.QuantityShipped = pro.ElementAtOrDefault(4);
                productDetails.UnitPrice = pro.ElementAtOrDefault(5);
                productDetails.RegWholesale = pro.ElementAtOrDefault(6);
                productDetails.SaleWholesale = pro.ElementAtOrDefault(7);
                productDetails.ExtendedPrice = pro.ElementAtOrDefault(8);
                productDetails.UNFIPOLineNumber = pro.ElementAtOrDefault(9);

                model.ProductDetails.Add(productDetails);
            }

            return model;
        }

        /// <summary>
        /// Prepare tracking model
        /// </summary>
        /// <param name="contents">contents</param>
        /// <returns>TrackingFileReadModel</returns>
        public TrackingFileReadModel PrepareTrackingModel(string[] contents)
        {
            var l1 = contents[0].Split(",");
            var l2 = contents[1].Split(",");

            var model = new TrackingFileReadModel();

            model.Invoice = l1.ElementAtOrDefault(0);
            model.ShipToName = l1.ElementAtOrDefault(1);
            model.ShipToAddress1 = l1.ElementAtOrDefault(2);
            model.ShipToAddress2 = l1.ElementAtOrDefault(3);
            model.ShipToCityAndState = l1.ElementAtOrDefault(4);
            model.ShipToZipCode = l1.ElementAtOrDefault(5);
            model.Zip = l1.ElementAtOrDefault(6);
            model.Blank = l1.ElementAtOrDefault(7);
            model.PONumber = l1.ElementAtOrDefault(8);
            model.A = l1.ElementAtOrDefault(9);
            model.UNFIOrder = l1.ElementAtOrDefault(10);
            model.DateShipped = l1.ElementAtOrDefault(11);

            // Line 2
            model.TrackingValue = l2;

            return model;
        }

        /// <summary>
        /// FileRead 
        /// </summary>
        /// <param name="folderName"></param>
        public string[] FileRead(string folderName)
        {
            return Directory.GetFiles(_fileProvider.MapPath(string.Format("~/Plugins/Misc.FFM/FTPServerFiles/{0}", folderName)));
        }

        /// <summary>
        /// Manage file read scheduler task
        /// </summary>
        public async Task ManageFileReadSchedulerTask()
        {
            //Store file on plugin static folder from ftp server.
            await _fTPFileService.DowanloadAndStoreFilesFromFtpServerAsync(new string[]
            {
                DefaultFFMStrings.ConfirmationsFolderName,
                DefaultFFMStrings.TrackingFolderName,
                DefaultFFMStrings.ShippingFolderName
            });

            // Read ConfirmationFiles
            string[] files = FileRead(DefaultFFMStrings.ConfirmationsFolderName);
            foreach (var filename in files)
            {
                string fileName = filename.Split("\\")[^1];
                await SendOrderConformationMailToCustomer(DefaultFFMStrings.ConfirmationsFolderName, fileName);
            }

            //Read TrackingFiles
            string[] trakingfiles = FileRead(DefaultFFMStrings.TrackingFolderName);
            foreach (var filename in trakingfiles)
            {
                string fileName = filename.Split("\\")[^1];
                await ManageTrackingFile(DefaultFFMStrings.TrackingFolderName, fileName);
            }
        }

        /// <summary>
        /// Send Order Conformation Mail and order Cancel Mail to Customer 
        /// </summary>
        /// <returns>Send a mail to customer</returns>
        public async Task SendOrderConformationMailToCustomer(string folderName, string fileName)
        {
            var contents = await _fFMServices.ReadOrderconformationFile(folderName, fileName);
            var confirmationFileReadModel = PrepareConfirmationModel(contents);

            var order = await _orderService.GetOrderByIdAsync(Convert.ToInt32(confirmationFileReadModel.CustomerPO));

            if (order == null)
            {
                await _logger.ErrorAsync(string.Format(await _localizationService.GetResourceAsync("Plugins.Misc.FFM.OrderNotFound.Error"), fileName, folderName));
                return;
            }

            var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);

            var store = await _storeService.GetStoreByIdAsync(order.StoreId) ?? await _storeContext.GetCurrentStoreAsync();
            int languageId = await EnsureLanguageIsActiveAsync((await _workContext.GetWorkingLanguageAsync()).Id, store.Id);

            //messageTemplates
            bool isCancledOrder = confirmationFileReadModel.ProductDetails.Any(m => m.QuantityOrdered.Contains('0'));
            MessageTemplate messageTemplates;

            if (isCancledOrder)
            {
                messageTemplates = (await GetActiveMessageTemplatesAsync(DefaultFFMStrings.OrderCancelMessageTemplate, store.Id))?.FirstOrDefault();
                await _orderProcessingService.CancelOrderAsync(order, true);
            }
            else
            {
                messageTemplates = (await GetActiveMessageTemplatesAsync(DefaultFFMStrings.OrderPlaceConfirmationMessageTemplate, store.Id))?.FirstOrDefault();
            }

            //tokens
            var tokens = new List<Token>();
            await _messageTokenProvider.AddOrderTokensAsync(tokens, order, languageId);
            tokens.Add(new Token("Store.Name", store.Name));
            tokens.Add(new Token("Store.URL", store.Url));

            //defaultEmailAccount
            var defaultEmailAccountId = (await _settingService.LoadSettingAsync<EmailAccountSettings>()).DefaultEmailAccountId;
            var defaultEmailAccount = await _emailAccountService.GetEmailAccountByIdAsync(defaultEmailAccountId);

            //Customer Full Name
            var toName = await _customerService.GetCustomerFullNameAsync(customer);

            await _workflowMessageService.SendNotificationAsync(messageTemplates, defaultEmailAccount, languageId, tokens, customer.Email, toName);

            // Move file on backup folder
            FileMoveOnBackup(DefaultFFMStrings.ConfirmationsFolderName, fileName);
        }

        /// <summary>
        /// File move on backup
        /// </summary>
        /// <param name="folderName">folderName</param>
        /// <param name="fileName">fileName</param> 
        public void FileMoveOnBackup(string folderName, string fileName)
        {
            string directoryPath = _fileProvider.MapPath(string.Format("Plugins/Misc.FFM/FTPServerFiles/Backup{0}", folderName));

            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            string sourceFile = _fileProvider.MapPath(string.Format("Plugins/Misc.FFM/FTPServerFiles/{1}/{0}", fileName, folderName));
            string destinationFile = _fileProvider.MapPath(string.Format(string.Format("Plugins/Misc.FFM/FTPServerFiles/Backup{1}/{0}", fileName, folderName)));
            File.Move(sourceFile, destinationFile, true);
        }

        /// <summary>
        /// Manage Tracking file 
        /// </summary>
        public async Task ManageTrackingFile(string folderName, string fileName)
        {
            try
            {
                var contents = await _fFMServices.ReadOrderconformationFile(folderName, fileName);
                var trackingFileReadModel = PrepareTrackingModel(contents);

                var order = await _orderService.GetOrderByIdAsync(Convert.ToInt32(trackingFileReadModel.PONumber));
                if (order == null)
                {
                    await _logger.ErrorAsync(string.Format(await _localizationService.GetResourceAsync("Plugins.Misc.FFM.OrderNotFound.Error"), fileName, folderName));
                    return;
                }

                var trackingNo = trackingFileReadModel.TrackingValue.Skip(1);
                var shippeddate = Convert.ToDateTime(trackingFileReadModel.DateShipped);

                var shipment = new Shipment
                {
                    OrderId = order.Id,
                    TrackingNumber = string.Join(",", trackingNo),
                    ShippedDateUtc = shippeddate,
                    CreatedOnUtc = DateTime.UtcNow
                };
                await _shipmentService.InsertShipmentAsync(shipment);

                var orderItems = await _orderService.GetOrderItemsAsync(order.Id);
                foreach(var item in orderItems)
                {
                    var shipmentItem = new ShipmentItem
                    {
                        ShipmentId = shipment.Id,
                        OrderItemId=item.Id,
                        Quantity = item.Quantity
                    };

                    await _shipmentService.InsertShipmentItemAsync(shipmentItem);
                }

                //Change orderStaus 
                order.OrderStatusId = 20;

                //Change Shipping Staus 
                order.ShippingStatusId = 30;
                await _orderService.UpdateOrderAsync(order);

                // Move file on backup folder
                FileMoveOnBackup(DefaultFFMStrings.TrackingFolderName, fileName);
            }
            catch (Exception exception)
            {
                await _logger.ErrorAsync(string.Format(await _localizationService.GetResourceAsync("Plugins.Misc.FFM.TackingFile.Error"), fileName, folderName), exception);
            }
        }

        #endregion
    }
}

