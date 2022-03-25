using ConaviWeb.Data.Repositories;
using ConaviWeb.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace ConaviApi.WS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EfirmaController : ControllerBase
    {
        private readonly ISourceFileRepository _sourceFileRepository;
        public EfirmaController(ISourceFileRepository sourceFileRepository)
        {
            _sourceFileRepository = sourceFileRepository;
        }
        //[HttpGet]
        //public async Task<ActionResult> DownloadFile()
        //{
        //    var filePath = "/var/www/FirmaWeb/wwwroot/doc/EFirma/Original/RH/test_api.pdf"; // Here, you should validate the request and the existance of the file.

        //    var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
        //    return File(bytes, "text/plain", Path.GetFileName(filePath));
        //}

        #region Upload  
        [HttpPost("UploadPost")]
        public IActionResult Upload([Required] List<IFormFile> formFiles,[FromForm] [Required] string idUser)
        {
            try
            {
                UploadFile(formFiles, Convert.ToInt32(idUser));

                return Ok(new { formFiles.Count, Size = SizeConverter(formFiles.Sum(f => f.Length)) , Response = "success"});
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region Upload File  
        public void UploadFile(List<IFormFile> files, int idUser)
        {
            var target = "/var/www/FirmaWeb/wwwroot/doc/EFirma/Original/Recursos_Humanos/";
            SourceFile sourceFile = new SourceFile();
            Directory.CreateDirectory(target);
            var success = false;
            files.ForEach(async file =>
            {
                if (file.Length <= 0) return;

                var filePath = Path.Combine(target, file.FileName);
                var hash = GetHashDocument(file);
                sourceFile.FileName = file.FileName;
                sourceFile.FilePath = "doc/EFirma/Original/Recursos_Humanos";
                sourceFile.Hash = hash;
                sourceFile.IdUser = idUser;
                sourceFile.IdPartition = 100;
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                success = await _sourceFileRepository.InsertSourceFile(sourceFile);
            });
        }
        #endregion

        #region Size Converter  
        public string SizeConverter(long bytes)
        {
            var fileSize = new decimal(bytes);
            var kilobyte = new decimal(1024);
            var megabyte = new decimal(1024 * 1024);
            var gigabyte = new decimal(1024 * 1024 * 1024);

            switch (fileSize)
            {
                case var _ when fileSize < kilobyte:
                    return $"Less then 1KB";
                case var _ when fileSize < megabyte:
                    return $"{Math.Round(fileSize / kilobyte, 0, MidpointRounding.AwayFromZero):##,###.##}KB";
                case var _ when fileSize < gigabyte:
                    return $"{Math.Round(fileSize / megabyte, 2, MidpointRounding.AwayFromZero):##,###.##}MB";
                case var _ when fileSize >= gigabyte:
                    return $"{Math.Round(fileSize / gigabyte, 2, MidpointRounding.AwayFromZero):##,###.##}GB";
                default:
                    return "n/a";
            }
        }
        #endregion

        #region Get Hash

        public string GetHashDocument(IFormFile file)
        {
            byte[] fileBytes;
            SHA512 shaM = new SHA512Managed();
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                fileBytes = ms.ToArray();
            }
            //fileByte = shaM.ComputeHash(file.OpenReadStream());
            string hashFile = BitConverter.ToString(shaM.ComputeHash(fileBytes));

            return hashFile;
        }

        #endregion
    }
}
