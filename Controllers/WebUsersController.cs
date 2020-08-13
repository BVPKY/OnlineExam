using ExamDotNetMVC.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace ExamDotNetMVC.Controllers
{
    [Authorize]
    public class WebUsersController : Controller
    {
        private ExamDotNetMVCContext db = new ExamDotNetMVCContext();

        // GET: WebUsers
        public ActionResult Index()
        {
            return View();
        }

        // GET: WebUsers/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }

        //    var uri = "http://localhost:53349/api/UserDetails/" + id;

        //    using (HttpClient obj = new HttpClient())
        //    {
        //        obj.Timeout = TimeSpan.FromMilliseconds(15000);
        //        obj.MaxResponseContentBufferSize = 256000;
        //        //var response = obj.PostAsync(uri, request).Result;
        //        HttpResponseMessage response = null;
        //        try
        //        {
        //            response = obj.GetAsync(uri).Result;
        //        }
        //        catch (Exception ex)
        //        {
        //            throw ex;
        //        }

        //        if (id == null)
        //        {
        //            return HttpNotFound();
        //        }

        //        return View(JsonConvert.DeserializeObject<UserDetail>(response.Content.ReadAsStringAsync().Result));
        //    }
        //}

        // GET: WebUsers/Create
        public ActionResult Create()
        {
            return View();
        }


        public ActionResult ExamDetails(string Name)
        {
            ViewBag.Name = Name;

            return View(db.ExamDetails.Where(w => w.Name.Equals(Name)).ToList().FirstOrDefault());
        }

        //public ActionResult Final(string Name)
        //{
        //    return View(db.UserDetails.Where(w => w.Name.Equals(Name)).FirstOrDefault());
        //}
        //[HttpPost]
        public ActionResult Final([Bind(Include = "UserId,Name,Experience,Email,Phone,Status,UMarks")] UserDetail userDetail)
        {


           
            int AMarks = 0;

            var Marks = db.ExamDetails.Where(w => w.Name.Equals(userDetail.Name)).ToList();
            foreach (var VARIABLE in Marks)
            {
                AMarks = AMarks + int.Parse(VARIABLE.AMarks);
            }


            var umarks = userDetail.UMarks;
            
            var User = db.UserDetails.Where(w => w.Name.Equals(userDetail.Name)).FirstOrDefault();
            User.UMarks = AMarks.ToString();
            ViewBag.AMarks = AMarks;
            ViewBag.Name = User.Name;
            //if (userDetail.UserId != 0)
            //{
            //    if (umarks == "0")
            //    {
            //        userDetail.UMarks = AMarks.ToString();
            //    }
            //}


            if (ModelState.IsValid)
            {
                var uri = "http://localhost:53349/api/UserDetails/" + User.UserId;
                var json = JsonConvert.SerializeObject(User);
                var request = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
                request.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                using (HttpClient obj = new HttpClient())
                {
                    obj.Timeout = TimeSpan.FromMilliseconds(15000);
                    obj.MaxResponseContentBufferSize = 256000;

                    HttpResponseMessage response = null;
                    try
                    {
                        response = obj.PutAsync(uri, request).Result;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                    if (response.IsSuccessStatusCode)
                    {
                        MailMessage mail = new MailMessage();
                        //foreach (var address in (context.To).Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                        //{
                        //    mail.To.Add(address);
                        //}
                        mail.To.Add(User.Email);
                        mail.From = new MailAddress("pboyina@tollplus.com");
                        mail.Subject = "TollPlus Online Exam - Result";
                        string Body = "Hello, <b style='color: blue; '>" + User.Name + "</b>,<br />You have scored : <b style='color: green; '>" + User.UMarks + " </b> / 10. marks in the exam, <br /> <br /> <br /> Please contact Roja for further details.";
                        mail.Body = Body;
                        mail.IsBodyHtml = true;
                        SmtpClient smtp = new SmtpClient();
                        smtp.Host = "smtp.gmail.com";
                        smtp.Port = 587;
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new System.Net.NetworkCredential("pboyina@tollplus.com", "Satyavardhanhppl1$");
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                        return View();
                    }

                    throw new Exception(response.ReasonPhrase);
                }

            }
            return View();
        }


        // POST: WebUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserId,Name,Experience,Email,Phone,Status,UMarks")] UserDetail userDetail)
        {
            if (string.IsNullOrWhiteSpace(userDetail.Name))
            {
                ModelState.AddModelError("Name", "UserName is required Field");
                ModelState.AddModelError("", "Warning!!");
                return View();

            }
            if (userDetail.Experience==0)
            {
                ModelState.AddModelError("Experience", "Experience Must be selected");
                ModelState.AddModelError("", "Warning!!");
                return View();
              
            }
            if (string.IsNullOrWhiteSpace(userDetail.Email))
            {
                ModelState.AddModelError("Email", "Email is required Field");
                ModelState.AddModelError("", "Warning!!");
                return View();

            }
            
            if ( string.IsNullOrWhiteSpace(userDetail.Phone))
            {
                ModelState.AddModelError("Phone", "PhoneNo is required Field");
                ModelState.AddModelError("", "Warning!!");
                return View();

            }

            int count = 0;
            var name = userDetail.Name;
            ViewBag.Exp = userDetail.Experience;
            Session["Name"] = userDetail.Name;
            Session["Experience"] = userDetail.Experience;
            var uri = "http://localhost:53349/api/UserDetails/";
            var json = JsonConvert.SerializeObject(userDetail);
            var request = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            request.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            List<UserDetail> distinctWords = db.UserDetails.ToList();
            foreach (var user in distinctWords)
            {
                if (user.Name.Equals(userDetail.Name)|| user.Email.Equals(userDetail.Email)|| user.Phone.Equals(userDetail.Phone))
                {
                    count = 1;

                }
            }

            if (count != 0)
            {
                ModelState.AddModelError("", "May be UserName,Email or PhoneNo Already Exists...!");
                return View();
                //return RedirectToActionPermanent("Create", "WebUsers", new { Name = name });
            }
            else
            {


                using (HttpClient obj = new HttpClient())
                {
                    obj.Timeout = TimeSpan.FromMilliseconds(15000);
                    obj.MaxResponseContentBufferSize = 256000;
                    //var response = obj.PostAsync(uri, request).Result;
                    HttpResponseMessage response = null;
                    try
                    {
                        response = obj.PostAsync(uri, request).Result;

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                    if (response.IsSuccessStatusCode)
                    {
                        var ran = new Random();
                        var questions = db.AddQuestions.Where(w => w.QLevel.Equals(userDetail.Experience)).Take(10).ToList()
                            .OrderBy(q => ran.Next()).ToList();
                        foreach (var question in questions)
                        {
                            ExamDetail exam = new ExamDetail();
                            exam.Name = userDetail.Name;
                            exam.QuestionNo = question.Question;
                            exam.Answer = question.Answer;
                            exam.UserAnswer = "";
                            exam.Type = question.Type;
                            exam.OptionA = question.OptionA;
                            exam.OptionB = question.OptionB;
                            exam.OptionC = question.OptionC;
                            exam.OptionD = question.OptionD;
                            exam.Marks = question.Marks.ToString();
                            exam.Status = "UnChecked";
                            exam.AMarks = "0";
                            if (ModelState.IsValid)
                            {
                                
                                db.ExamDetails.Add(exam);
                                db.SaveChanges();

                            }

                        }
                        return RedirectToActionPermanent("ExamDetails", "WebUsers", new { Name = name });

                    }
                    return RedirectToActionPermanent("ExamDetails", "WebUsers", new { Name = name });
                }

            }

        }

        public ActionResult Edit1(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExamDetail examDetail = db.ExamDetails.Find(id);
            if (examDetail == null)
            {
                return HttpNotFound();
            }

            string Name = Session["Name"].ToString();
            ViewBag.ExamDetail = db.ExamDetails.Where(w => w.Name.Equals(Name)).ToList();
            return View(examDetail);
        }

        // POST: ExamDetails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit1([Bind(Include = "Id,Name,QuestionNo,Answer,Marks,AMarks,Status,Type,OptionA,OptionB,OptionC,OptionD,UserAnswer")] ExamDetail examDetail, FormCollection formCollection)
        {
            //ViewBag.res = db.ExamDetails.Where(x => x.Answer == x.UserAnswer).ToList();
            if (examDetail.Type == "MCQ")
            {

                //if (OptionA. == true)
                //{
                //OptionA
                //}
                
                if (examDetail.UserAnswer == examDetail.Answer)
                {

                    examDetail.AMarks = "1";
                }
                else if (examDetail.UserAnswer == null)
                {
                    examDetail.UserAnswer = "";
                }
            }
            else
            {
                if (examDetail.UserAnswer == null)
                {
                    examDetail.UserAnswer = "";
                }
                else
                {
                    examDetail.UserAnswer = formCollection["UserAnswer"];
                }

                if (examDetail.UserAnswer == examDetail.Answer)
                {
                    examDetail.AMarks = "1";
                }
            }
            
            ViewBag.ExamDetail = db.ExamDetails.Where(w => w.Name.Equals(examDetail.Name)).ToList();

            if (ModelState.IsValid)
            {
                var uri = "http://localhost:53349/api/ExamDetailsAPI/" + examDetail.Id;
                var json = JsonConvert.SerializeObject(examDetail);
                var request = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
                request.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                using (HttpClient obj = new HttpClient())
                {
                    obj.Timeout = TimeSpan.FromMilliseconds(15000);
                    obj.MaxResponseContentBufferSize = 256000;

                    HttpResponseMessage response = null;
                    try
                    {
                        response = obj.PutAsync(uri, request).Result;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Edit1/" + examDetail.Id);
                    }
                    throw new Exception(response.ReasonPhrase);

                }
            }
            return RedirectToAction("Edit1/" + examDetail.Id);
        }

        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
