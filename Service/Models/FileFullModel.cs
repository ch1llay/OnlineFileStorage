﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models;

namespace Service.Models
{
    public class FileFullModel
    {
        public string? Name { get; set; }
        public long SizeInByte { get; set; }
        public string? FileType { get; set; }
        public byte[] Content { get; set; }
        public SaveType FileSaveType { get; set; }
    }
}
