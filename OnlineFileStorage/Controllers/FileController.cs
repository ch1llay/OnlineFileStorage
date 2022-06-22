﻿using DataAccess.Models;
using Domain.Interfaces;
using Service.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using OnlineFileStorage.Utilities;
using Service.Interfaces;

namespace FileStorageOnline.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class FileController : Controller
    {
        private IFileInfoRepository _fileRepository;
        private IFileService _fileService;                                       // здесь храним прогресс загрузки каждого файла
        static Dictionary<Guid, int> fileProgress = new Dictionary<Guid, int>(); // TODO: по-хорошему это надо хранить в базе,
                                                                                 // в рамках тестового задания была выбрана данная реализация

        public FileController(IFileInfoRepository fileRepository, IFileService fileService)
        {
            _fileRepository = fileRepository;
            _fileService = fileService;
            
        }

        //[HttpPost("/loadManyFirstVariant")]
        ////                                                 путь обхода с нормальной загрузкой нескольких файлов и возвращением списка [{filename:status}]
        //                                                    // и оставить 2 вариант который ближе к тз
        //public async Task<IActionResult> OnPostUploadAsync(IFormFileCollection files)
        //{
        //    long size = files.Sum(f => f.Length);
        //    var filesForLoading = files.Select(x => new FileInfoServiceModel
        //    {
        //        Name = x.FileName,
        //        FileType = x.ContentType,
        //        SizeInByte = x.Length
        //    }).ToList();

        //    var filesWithIds = await _fileService.GetFileInfoServicesInTimeLoading(filesForLoading);
        //    var fullFiles = filesWithIds.Select(x => new Tuple<Guid, IFormFile>(x.Id, files.First(m => m.FileName == x.Name && m.Length == x.SizeInByte)));
        //    foreach (var file in fullFiles)
        //    {
        //        LoadingHelper.NewLoadInit(file.Item1, file.Item2.Length);
        //        new Thread(file_ =>
        //        {
        //            var file = (Tuple<Guid, IFormFile>) file_;
        //            using (var memoryStream = new MemoryStream())
        //            {
        //                using (Stream stream = file.Item2.OpenReadStream())
        //                {
        //                    LoadingHelper.UpdateStatus(file.Item1, Status.Progress);
        //                    long count = file.Item2.Length;
        //                    while (stream.Position < count)
        //                    {
        //                        memoryStream.Write(stream.Read())
        //                        var f = new DbFileData
        //                        {
        //                            FileInfoId = 
        //                            Content = memoryStream.ToArray()
        //                        };
        //                        _fileService.LoadFile(f);
        //                    }
        //                }
        //            }
        //        }
        //        ).Start(file);

        //        return Ok(filesWithIds);

            

        //    }

        //    // Process uploaded files
        //    // Don't rely on or trust the FileName property without validation.

        //    return Ok(new { count = files.Count, size });
        //}

        [HttpGet("/getUrl")]
        [SwaggerResponse((int)HttpStatusCode.OK, "DbFileModel", typeof(DbFileInfo))]
        public async Task<IActionResult> GetUrlForFile([FromQuery] Guid fileId)
        {
            if (fileId == Guid.Empty)
            {
                return BadRequest();
            }

            var url = $"{Request.Host.Value}/file/downloadFile/{await _fileService.GetUrlByFileId(fileId)}";
            return Ok(url);

        }

        [HttpGet("/downloadFile/{uri}")]
        [SwaggerResponse((int)HttpStatusCode.OK, "DbFileModel", typeof(DbFileInfo))]
        public async Task<IActionResult> Download(string uri)
        {
            if (!Guid.TryParse(uri, out Guid _))
            {
                return BadRequest();
            }
            var file = await _fileService.GetFileFullModelByLink(uri);
            if (file == null)
            {
                return NotFound("Ссылка не существует либо не действительна");
            }

            return File(file.Content, file.FileType, file.Name);

        }

        [HttpPost("/LoadOne")]
        //[SwaggerResponse((int)HttpStatusCode.OK, "Guid", typeof(Guid))]
        public async Task<IActionResult> FileLoad(IFormFile formFile)
        {
            using (var memoryStream = new MemoryStream())
            {
                formFile.CopyTo(memoryStream);
                var file = new FileFullModel
                {
                    Name = formFile.FileName,
                    SizeInByte = formFile.Length,
                    FileType = formFile.ContentType,
                    Content = memoryStream.ToArray()
                };
                var id = await _fileService.LoadFile(file);
                return Ok(id);
            };
            
        }
        [HttpGet]
        [SwaggerResponse((int)HttpStatusCode.OK, "DbFileModel", typeof(DbFileInfo))]
        public async Task<IActionResult> Get()
        {
            return Ok((await _fileService.GetAllFilesInfo()));
        }

    }
}