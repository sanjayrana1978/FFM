using System.Collections.Generic;
using System.Threading.Tasks;

namespace XcellenceIt.Plugins.Misc.FFM.Services.FTPServices
{
    public interface IFTPFileService
    {
        /// <summary>
        /// Upload file 
        /// </summary>
        /// <param name="path">path</param>
        /// <param name="ftpPath">ftpPath</param> 
        Task UploadFileAsync(string path, string ftpPath);

        /// <summary>
        /// Dowanload and store files from ftp server
        /// </summary>
        /// <param name="folders">folders</param> 
        Task DowanloadAndStoreFilesFromFtpServerAsync(string[] folders);
    }
}
