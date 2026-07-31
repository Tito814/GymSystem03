using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.BLL.Services.Interfaces
{
    public  interface IAttachmentService
    {
        Task<string?> UploadAsync(Stream fileStream, string fileName, string folderName, CancellationToken ct = default);
        bool Delete(string fileName, string folderName);
        (Stream Stream, string ContentType)? GetFile(string fileName, string folderName);
    }
}
