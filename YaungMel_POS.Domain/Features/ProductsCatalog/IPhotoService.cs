using CloudinaryDotNet.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YaungMel_POS.shared.Responses;

namespace YaungMel_POS.Domain.Features.ProductsCatalog
{
    public interface IPhotoService
    {
        Task<ImageUploadResult> UploadPhotoAsync(Stream photoStream, string fileName);

        Task<DeletionResult> DeletePhotoAsync(string publicId);
    }
}
