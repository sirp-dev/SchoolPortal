using SchoolPortal.Web.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Dtos
{
    public class SubjectListPrintoutDto
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        public int Id { get; set; }


        public int? SubjectId { get; set; }
        public Subject Subject { get; set; }

        [Display(Name = " Test Score:")]
        //[Range(0, 30)]
        public decimal? TestScore { get; set; }

        [Display(Name = "Exam Score:")]
        //[Range(0, 70)]
        public decimal? ExamScore { get; set; }

        [Display(Name = "2nd Test Score:")]
        public decimal? TestScore2 { get; set; }

        [Display(Name = "Project:")]
        public decimal? Project { get; set; }

        [Display(Name = "Class Exercise:")]
        public decimal? ClassExercise { get; set; }

        [Display(Name = "Assessment:")]
        public decimal? Assessment { get; set; }

        [Display(Name = "Total CA:")]
        public decimal? TotalCA { get; set; }

        [Display(Name = "Total Score:")]
        public decimal? TotalScore { get; set; }

        public decimal? HighestTotalScore { get; set; }



        public decimal? LowestTotalScore { get; set; }



        public int EnrollmentId { get; set; }
        public Enrollment Enrollments { get; set; }

        public bool IsOffered { get; set; }

        [Display(Name = "Grading Option")]
        public GradingOption GradingOption { get; set; }



        [Display(Name = "Grade:")]
        public string Grade
        {
            get
            {

                if (this.GradingOption == Entities.GradingOption.SSS)
                {
                    foreach (var item in db.GradingDetails.Where(x => x.Grading.Name == GradingOption.SSS.ToString()))
                    {
                        if (this.TotalScore == Convert.ToDecimal(0.00))
                        {
                            return "-";
                        }
                        else if (this.TotalScore >= item.LowerLimit && this.TotalScore <= item.UpperLimit)
                        {
                            return item.Grade;
                        }
                    }
                }
                else if (this.GradingOption == Entities.GradingOption.JSS)
                {
                    foreach (var item in db.GradingDetails.Where(x => x.Grading.Name == GradingOption.JSS.ToString()))
                    {

                        if (this.TotalScore == Convert.ToDecimal(0.00))
                        {
                            return "-";
                        }
                        else if (this.TotalScore >= item.LowerLimit && this.TotalScore <= item.UpperLimit)
                        {
                            return item.Grade;
                        }

                    }

                }
                else if (this.GradingOption == Entities.GradingOption.PRI)
                {
                    foreach (var item in db.GradingDetails.Where(x => x.Grading.Name == GradingOption.PRI.ToString()))
                    {
                        if (this.TotalScore == Convert.ToDecimal(0.00))
                        {
                            return "-";
                        }
                        else if (this.TotalScore >= item.LowerLimit && this.TotalScore <= item.UpperLimit)
                        {
                            return item.Grade;
                        }

                    }

                }
                else if (this.GradingOption == Entities.GradingOption.NUR)
                {
                    foreach (var item in db.GradingDetails.Where(x => x.Grading.Name == GradingOption.NUR.ToString()))
                    {
                        if (this.TotalScore == Convert.ToDecimal(0.00))
                        {
                            return "-";
                        }
                        else if (this.TotalScore >= item.LowerLimit && this.TotalScore <= item.UpperLimit)
                        {
                            return item.Grade;
                        }

                    }

                }
                else if (this.GradingOption == Entities.GradingOption.PG)
                {
                    try
                    {
                        string vcv = GradingOption.PG.ToString();
                        var g = db.Gradings.Where(x => x.Name == GradingOption.PG.ToString());
                        var itemq = db.GradingDetails.Where(x => x.Grading.Name == GradingOption.PG.ToString());
                        int dfd = itemq.Count();

                        foreach (var item in db.GradingDetails.Where(x => x.Grading.Name == GradingOption.PG.ToString()))
                        {
                            if (this.TotalScore == Convert.ToDecimal(0.00))
                            {
                                return "-";
                            }
                            else if (this.TotalScore >= item.LowerLimit && this.TotalScore <= item.UpperLimit)
                            {
                                return item.Grade;
                            }
                        }
                    }
                    catch (Exception c)
                    {

                    }
                }

                else if (this.GradingOption == Entities.GradingOption.PRE)
                {
                    foreach (var item in db.GradingDetails.Where(x => x.Grading.Name == GradingOption.PRE.ToString()))
                    {
                        if (this.TotalScore == Convert.ToDecimal(0.00))
                        {
                            return "-";
                        }
                        else if (this.TotalScore >= item.LowerLimit && this.TotalScore <= item.UpperLimit)
                        {
                            return item.Grade;
                        }

                    }

                }

                return "Grade Option Not Set";



            }

        }


        [Display(Name = "Remark:")]
        public string Remark
        {


            get
            {

                if (this.GradingOption == Entities.GradingOption.SSS)
                {
                    foreach (var item in db.GradingDetails.Where(x => x.Grading.Name == GradingOption.SSS.ToString()))
                    {
                        if (this.TotalScore == Convert.ToDecimal(0.00))
                        {
                            return "-";
                        }
                        else if (this.TotalScore >= item.LowerLimit && this.TotalScore <= item.UpperLimit)
                        {
                            return item.Remark;
                        }
                    }
                }
                else if (this.GradingOption == Entities.GradingOption.JSS)
                {
                    foreach (var item in db.GradingDetails.Where(x => x.Grading.Name == GradingOption.JSS.ToString()))
                    {
                        if (this.TotalScore == Convert.ToDecimal(0.00))
                        {
                            return "-";
                        }
                        else if (this.TotalScore >= item.LowerLimit && this.TotalScore <= item.UpperLimit)
                        {
                            return item.Remark;
                        }

                    }

                }
                else if (this.GradingOption == Entities.GradingOption.PRI)
                {
                    foreach (var item in db.GradingDetails.Where(x => x.Grading.Name == GradingOption.PRI.ToString()))
                    {
                        if (this.TotalScore == Convert.ToDecimal(0.00))
                        {
                            return "-";
                        }
                        else if (this.TotalScore >= item.LowerLimit && this.TotalScore <= item.UpperLimit)
                        {
                            return item.Remark;
                        }

                    }

                }
                else if (this.GradingOption == Entities.GradingOption.NUR)
                {
                    foreach (var item in db.GradingDetails.Where(x => x.Grading.Name == GradingOption.NUR.ToString()))
                    {
                        if (this.TotalScore == Convert.ToDecimal(0.00))
                        {
                            return "-";
                        }
                        else if (this.TotalScore >= item.LowerLimit && this.TotalScore <= item.UpperLimit)
                        {
                            return item.Remark;
                        }

                    }

                }

                else if (this.GradingOption == Entities.GradingOption.PG)
                {
                    try
                    {
                        foreach (var item in db.GradingDetails.Where(x => x.Grading.Name == GradingOption.PG.ToString()))
                        {
                            if (this.TotalScore == Convert.ToDecimal(0.00))
                            {
                                return "-";
                            }
                            else if (this.TotalScore >= item.LowerLimit && this.TotalScore <= item.UpperLimit)
                            {
                                return item.Remark;
                            }

                        }
                    }
                    catch (Exception c)
                    {

                    }

                }
                else if (this.GradingOption == Entities.GradingOption.PRE)
                {
                    foreach (var item in db.GradingDetails.Where(x => x.Grading.Name == GradingOption.PRE.ToString()))
                    {
                        if (this.TotalScore == Convert.ToDecimal(0.00))
                        {
                            return "-";
                        }
                        else if (this.TotalScore >= item.LowerLimit && this.TotalScore <= item.UpperLimit)
                        {
                            return item.Remark;
                        }

                    }

                }

                return "Grade Option Not Set";



            }
        }




    }
}