using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.IO;
using System.Threading.Tasks;

namespace Lychee.Extension.ChunkUploader
{
    public class Uploader
    {
        private readonly UploaderOptions options;

        public Uploader(IOptions<UploaderOptions> options)
        {
            this.options = options.Value;
            if (string.IsNullOrEmpty(this.options.TemporayFolder))
            {
                this.options.TemporayFolder = Path.Combine(this.options.SaveFolder, "Temp");
            }
        }

        public async Task<string> Upload(IFormFile file)
        {
            // 分片文件保存文件夹
            var saveFolder = Path.Combine(this.options.TemporayFolder, "");

            // 当前分片保存路径
            var chunkSavePath = Path.Combine(saveFolder, $"");

            if (File.Exists(chunkSavePath))
            {
                // 该分片已上传
            }
            else
            {
                if (!Directory.Exists(saveFolder))
                {
                    Directory.CreateDirectory(saveFolder);
                }

                using var write = File.Create(chunkSavePath);
                using var uploadStream = file.OpenReadStream();
                await uploadStream.CopyToAsync(write);
            }

            var chunks = Directory.GetFiles(saveFolder, $".*");

            if (chunks.Length == 0)
            {
            }

            await Task.CompletedTask;
            return string.Empty;
        }
    }
}