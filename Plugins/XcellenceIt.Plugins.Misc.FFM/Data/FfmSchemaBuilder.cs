using FluentMigrator;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Messages;
using Nop.Data.Extensions;
using Nop.Data.Mapping;
using Nop.Data.Migrations;
using Nop.Services.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using XcellenceIt.Plugins.Misc.FFM.Domain;
using XcellenceIt.Plugins.Misc.FFM.Services;
using XcellenceIt.Plugins.Misc.FFM.Services.FileServices;

namespace XcellenceIt.Plugins.Misc.FFM.Data
{

    [NopMigration("2022/10/07 01:40:10", "Misc.FFM base schema", MigrationProcessType.Update)]
    public class FfmSchemaBuilder : Migration
    {
        protected IMigrationManager _migrationManager;
        private readonly IFFMServices _fFMServices;
        private readonly IOrderFileServices _orderFileServices;
        private readonly ICustomerAttributeService _customerAttributeService;

        #region Ctor
        public FfmSchemaBuilder(IMigrationManager migrationManager,
               IFFMServices fFMServices,
               IOrderFileServices orderFileServices,
               ICustomerAttributeService customerAttributeService)
        {
            _migrationManager = migrationManager;
            _fFMServices = fFMServices;
            _orderFileServices = orderFileServices;
            _customerAttributeService = customerAttributeService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override async void Up()
        {
            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(ProductImagesUrls))).Exists())
                Create.TableFor<ProductImagesUrls>();

            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(Company))).Exists())
                Create.TableFor<Company>();

            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(SpecificationAttributePictures))).Exists())
                Create.TableFor<SpecificationAttributePictures>();

            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(ProductAdditionalInfo))).Exists())
                Create.TableFor<ProductAdditionalInfo>();

            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(UnfiCategories))).Exists())
                Create.TableFor<UnfiCategories>();
            
            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(ImportToCSV))).Exists())
                Create.TableFor<ImportToCSV>();

            if (!Schema.Table("XIT_Company").Exists())
                Create.TableFor<Company>();
            else
                AddTableColumn<Company>();
           
            //message template
            if (Schema.Table(NameCompatibilityManager.GetTableName(typeof(MessageTemplate))).Exists())
                await _fFMServices.AddDefaultFFMEmailMessageTemplate();

            await _orderFileServices.InsertScheduleTaskAsync();

        }

        /// <summary>
        /// Collect the Down migration expressions
        /// </summary>
        public override async void Down()
        {
            if (Schema.Table(NameCompatibilityManager.GetTableName(typeof(ProductImagesUrls))).Exists())
                Delete.Table("ProductImagesUrls");

            if (Schema.Table(NameCompatibilityManager.GetTableName(typeof(Company))).Exists())
                Delete.Table("XIT_Company");
        }

        public partial class BaseNameCompatibility : INameCompatibility
        {
            public Dictionary<Type, string> TableNames => new Dictionary<Type, string>
            {
                {typeof(Company), "XIT_Company" },
                {typeof(SpecificationAttributePictures),"XIT_SpecificationAttributePictures" },
                {typeof(ProductAdditionalInfo),"XIT_ProductAdditionalInfo" },
                {typeof(UnfiCategories),"XIT_UnfiCategories" },
                {typeof(ImportToCSV),"XIT_ImportToCSV" }
            };
            public Dictionary<(Type, string), string> ColumnName => new Dictionary<(Type, string), string>
            {
                //do nothing
            };
        }

        public void AddTableColumn<TEntity>()
        {
            var classPropertiesWihoutCacheKey = typeof(TEntity).GetProperties().Where(x => x.Name != nameof(BaseEntity.Id)).ToList();

            foreach (var item in classPropertiesWihoutCacheKey)
            {
                var column = Schema.Table(NameCompatibilityManager.GetTableName(typeof(TEntity))).Column(item.Name).Exists();
                if (!column)
                {
                    if (item.PropertyType == typeof(string))
                    {
                        //add new column
                        Alter.Table(NameCompatibilityManager.GetTableName(typeof(TEntity)))
                        .AddColumn(item.Name).AsString(int.MaxValue).Nullable();
                    }
                }
            }
        }

        #endregion
    }
}
