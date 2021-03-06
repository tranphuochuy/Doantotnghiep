﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DOAN_CHuyenNGanh.Models
{
    public class FocusExams
    {
        [Key]
        [Column(Order = 1)]
        public string Id { get; set; }

        public string Name { get; set; }

        public string Mark { get; set; }

        public DateTime DateTime { get; set; }

        public string Grade { get; set; }

        public Subject Subject { get; set; }
        [Key]
        [Column(Order = 2)]
        public string SubjectId { get; set; }

        public Year Year { get; set; }
        [Key]
        [Column(Order = 3)]
        public string YearId { get; set; }

        public Semester Semester { get; set; }
        [Key]
        [Column(Order = 4)]
        public string SemesterId { get; set; }
    }
}