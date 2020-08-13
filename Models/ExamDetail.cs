using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ExamDotNetMVC.Models
{
    public class ExamDetail
    {
        [Key] public int Id { get; set; }
        public string Name { get; set; }
        public string QuestionNo { get; set; }
        public string Type { get; set; }
        public string OptionA { get; set; }
        public string OptionB { get; set; }
        public string OptionC { get; set; }
        public string OptionD { get; set; }
        public string UserAnswer { get; set; }
        public string Answer { get; set; }
        public string Marks { get; set; }
        public string AMarks { get; set; }
        public string Status { get; set; }
      
    }
}