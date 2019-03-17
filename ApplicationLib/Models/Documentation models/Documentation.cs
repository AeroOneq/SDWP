﻿using System;
using System.Collections.Generic;
using AeroORMFramework;

namespace ApplicationLib.Models
{
    public class Documentation
    { 
        [PrimaryKey]
        [CanBeNull(false)]
        [AutoincrementID]
        public int ID { get; set; }

        [CanBeNull(false)]
        public string Name { get; set; }
        [CanBeNull(false)]
        public int AuthorID { get; set; }
        [CanBeNull(false)]
        public string AuthorName { get; set; }
        [CanBeNull(false)]
        public DateTime CreationDate { get; set; }
        [CanBeNull(false)]
        public DateTime UpdatedAt { get; set; }
        [CanBeNull(false)]
        [SetAzureSQLDataType("int")]
        public Access Access { get; set; }
    }
}
