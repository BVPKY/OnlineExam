using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExamDotNetMVC.Models
{
    public class AddQuestion
    {
        [Key] public int Questionid { get; set; }
        public string Question { get; set; }
        public string Type { get; set; }
        public string Answer { get; set; }
        public string OptionA { get; set; }
        public string OptionB { get; set; }
        public string OptionC { get; set; }
        public string OptionD { get; set; }
        public int QLevel { get; set; }
        public int Marks { get; set; }
        public string Explaination { get; set; }
    }

    //public class Type
    //{
    //    public int ID { get; set; }
    //    public string Pattern { get; set; }
    //}
}