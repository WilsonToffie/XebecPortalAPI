﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XebecAPI.DTOs
{
    public class ApplicationPhaseDTO
    {
        public int Id { get; set; }
        public string Description {  get; set; }

        public string EmailTemplate { get; set; }
    }
}
