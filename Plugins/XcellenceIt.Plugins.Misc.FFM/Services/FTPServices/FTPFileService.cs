using Nop.Core.Domain.Logging;
using Nop.Core.Infrastructure;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace XcellenceIt.Plugins.Misc.FFM.Services.FTPServices
{
    public class FTPFileService : IFTPFileService
    {
        #region Fields

        private readonly ILogger _logger;
        private readonly IFFMServices _fFMServices;
        private readonly ILocalizationService _localizationService;
        private readonly INopFileProvider _fileProvider;

        #endregion

        #region Ctor

        public FTPFileService(IFFMServices fFMServices,
            ILogger logger, ILocalizationService localizationService,
            INopFileProvider fileProvider)
        {
            _fFMServices = fFMServices;
            _logger = logger;
            _localizationService = localizationService;
            _fileProvider = fileProvider;
        }

        #endregion

        #region Utilities

        protected async Task<SftpClient> FtpConnection()
        {
            var ffmSettings = await _fFMServices.GetAllConfiguration();
            var connectionInfo = new ConnectionInfo(ffmSettings?.FtpHost, ffmSettings?.FtpUserName, new PasswordAuthenticationMethod(ffmSettings?.FtpUserName, ffmSettings?.FtpPassword));
            return new SftpClient(connectionInfo);
        }

        #endregion

        #region Methods 

        /// <summary>
        /// Upload file 
        /// </summary>
        /// <param name="folder">folder</param>
        /// <param name="filename">filename</param>
        /// <param name="contents">contents</param>
        /// <returns></returns>
        public async Task UploadFileAsync(string path, string ftpPath)
        {
            using var client = await FtpConnection();
            try
            {
                client.Connect();
                var streamWriter = (Stream)File.OpenRead(path);
                client.UploadFile(streamWriter, ftpPath);
                streamWriter.Close();
                await _logger.InsertLogAsync(LogLevel.Information, string.Format(await _localizationService.GetResourceAsync("Plugins.Misc.FFM.FTPOrderFileGenerate.Success"), path.Split('\\')[^1]));
            }
            catch (Exception exception)
            {
                await _logger.ErrorAsync(await _localizationService.GetResourceAsync("Plugins.Misc.FFM.FileGenerate.Error"), exception);
            }
            finally
            {
                client.Disconnect();
            }
        }

        /// <summary>
        /// Dowanload and store files from ftp server
        /// </summary>
        /// <param name="folders">folders</param> 
        public async Task DowanloadAndStoreFilesFromFtpServerAsync(string[] folders)
        {
            foreach (var folder in folders)
            {
                string directoryPath = _fileProvider.MapPath(string.Format("~/Plugins/Misc.FFM/FTPServerFiles/{0}", folder));
                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);
            }

            using var client = await FtpConnection();
            try
            {
                client.Connect();
                foreach (var folder in folders)
                {
                    var files = client.ListDirectory(string.Format("/{0}/", folder));
                    if (files == null) continue;

                    foreach (var file in files)
                    {
                        using Stream fileStream = File.OpenWrite(_fileProvider.MapPath(string.Format("~/Plugins/Misc.FFM/FTPServerFiles/{1}/{0}", file.Name, folder)));
                        client.DownloadFile(file.FullName, fileStream);
                        await _logger.InsertLogAsync(LogLevel.Information, string.Format(await _localizationService.GetResourceAsync("Plugins.Misc.FFM.FTPFilesDowanloads.Success"), file.FullName));
                    }
                }
            }
            catch (Exception exception)
            {
                await _logger.ErrorAsync(await _localizationService.GetResourceAsync("Plugins.Misc.FFM.FileDowanload.Error"), exception);
            }
            finally
            {
                client.Disconnect();
            }
        }
        #endregion
    }

}
