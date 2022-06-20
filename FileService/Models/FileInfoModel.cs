﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileService.Models
{
    public class FileInfoModel
    {
        public string? Name { get; set; }
        public long SizeInByte { get; set; }
        public string? FileType { get; set; }
        public string? Link { get; set; }
    }
}