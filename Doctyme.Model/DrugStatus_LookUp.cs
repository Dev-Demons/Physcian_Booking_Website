﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctyme.Model
{
    [Table("DrugStatus_LookUp")]
    public class DrugStatus_LookUp 

    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DrugStatus_LookUpID { get; set; }
        [StringLength(200)]
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int? CreatedBy { get; set; }
     
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
       
        public DateTime? UpdatedDate { get; set; }
    }
}