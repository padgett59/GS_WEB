using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Linq.Mapping;

namespace MVC.GSWeb.Models
{
    public class GsDataContext : DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //Map entities to tables
            modelBuilder.Entity<NewReader>().ToTable("Readers");
            modelBuilder.Entity<Quiz>().ToTable("Quizzes");
        }

        //DbSet instances must match pluralized table names unless mapped above
        public DbSet<NewReader> NewReaders { get; set; }
        public DbSet<QuizCategory> QuizCategories { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<QuizQuestion> QuizQuestions { get; set; }
        public DbSet<QuizAnswer> QuizAnswers { get; set; }
        public DbSet<OpenItemNumber> OpenItemNumbers { get; set; }
        public DbSet<EbookOrder> EbookOrders { get; set; }
        public DbSet<ProofOrder> ProofOrders { get; set; }
        public DbSet<ReaderReview> ReaderReviews { get; set; }
    }

    public class ProofOrder
    {
        public ProofOrder()
        {
            this.NewsEnroll = true;
            this.Step = 1;
        }

        [Required(ErrorMessage = "Please enter your first name before proceeding.")]
        [StringLength(50, MinimumLength = 2)]
        public String FirstName { get; set; }

        [Required(ErrorMessage = "Please enter your last name before proceeding.")]
        [StringLength(50, MinimumLength = 2)]
        public String LastName { get; set; }

        [Key]
        [Required(ErrorMessage = "Please enter your email address before proceeding.")]
        [RegularExpression(@"^([0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$", ErrorMessage = "Please be sure to enter a valid email address.")]
        public String Email { get; set; }

        [Required(ErrorMessage = "Please enter your address before proceeding.")]
        [StringLength(100, MinimumLength = 2)]
        public String Address1 { get; set; }

        public String Address2 { get; set; }

        [Required(ErrorMessage = "Please enter your city before proceeding.")]
        [StringLength(60, MinimumLength = 2)]
        public String City { get; set; }

        [Required(ErrorMessage = "Please enter your state before proceeding.")]
        public String State { get; set; }

        [Required(ErrorMessage = "Please enter your Zip Code before proceeding.")]
        [RegularExpression(@"^\d{5}(?:[-\s]?\d{4})?$", ErrorMessage = "Please be sure to enter a valid zip code.")]
        public String Zip { get; set; }

        public Int16 Quantity { get; set; }
        public DateTime DateOrdered { get; set; }

        public decimal Total { get; set; }

        [NotMapped]
        public Boolean NewsEnroll { get; set; }
        [NotMapped]
        public Byte Step { get; set; }
    }

    public class NewReader
    {
        public NewReader() { }

            public NewReader(String name, String email)
        {
            this.ReaderName = name;
            this.Email = email;
            this.DateAdded = DateTime.Now.ToShortDateString();
        }
        
        [Required (ErrorMessage="Please enter your name before proceeding.")]
        [StringLength(50, MinimumLength = 3)]
        public String ReaderName { get; set; }

        [Key]
        [Required(ErrorMessage = "Please enter your email address before proceeding.")]
        [RegularExpression(@"^([0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$", ErrorMessage = "Please be sure to enter a valid email address.")]
        public String Email { get; set; }
        public String DateAdded { get; set; }

    }

    //[Serializable]
    //public class ReaderReviewList
    //{
    //    public ReaderReviewList() { 
    //    }
    //    public ReaderReviewList(List<ReaderReview> reviewList)
    //    {
    //        this.ReviewList = reviewList;
    //    }
    //    public List<ReaderReview> ReviewList { get; set; }
    //}

    [Serializable]
    public class ReaderReview
    {
        public ReaderReview() 
        {
            this.NewsEnroll = true;
            this.Step = 1;
        }

        public String DateAdded { get; set; }

        [Key]
        [Required(ErrorMessage = "Please enter your email address before proceeding.")]
        [RegularExpression(@"^([0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$", ErrorMessage = "Please be sure to enter a valid email address.")]
        public String Email { get; set; }

        [Required(ErrorMessage = "Please enter your First name before proceeding.")]
        [StringLength(50, MinimumLength = 3)]
        public String FirstName { get; set; }

        [StringLength(50, MinimumLength = 3)]
        public String LastName { get; set; }

        [Required(ErrorMessage = "Please select your State before proceeding.")]
        public String State { get; set; }

        [Required(ErrorMessage = "Please enter your City before proceeding.")]
        [StringLength(40, MinimumLength = 3)]
        public String City { get; set; }

        [Required(ErrorMessage = "Please enter your review before proceeding.")]
        [StringLength(1000, MinimumLength = 3)]
        public String Review { get; set; }

        [Required(ErrorMessage = "Please select your Rating before proceeding.")]
        public byte Rating { get; set; }

        public Boolean Approved { get; set; }

        [NotMapped]
        public Boolean NewsEnroll { get; set; }

        [NotMapped]
        public Byte Step { get; set; }

    }

    [Serializable]
    public class Quiz
    {
        [Key]
        public Int16 QuizId { get; private set; }
        public Int16 CategoryId { get; private set; }
        public String QuizName { get; private set; }
        public Byte Difficulty { get; private set; }
    }

    public class QuizCategory
    {
        [Key]
        public Int16 CategoryId { get; private set; }
        public String Category { get; private set; }
    }

    [Serializable]
    public class QuizQuestion
    {
        [Key]
        public Int16 QuestionId { get; set; }
        public Int16 QuizId { get; set; }
        public String Question { get; set; }
        public Byte QuestionTypeId { get; set; }
        [NotMapped]
        public Boolean Answered { get; set; }
    }

    [Serializable]
    public class QuizAnswer
    {
        [Key]
        public Int16 AnswerId { get; set; }
        public Int16 QuestionId { get; set; }
        public String Answer { get; set; }
        public Boolean CorrectAnswer { get; set; }
        public Boolean Selected { get; set; }
    }

    [Serializable]
    public class QuizInfo

    {
        public QuizInfo (Int16 qId, String qName)
        {
            this.QuizId = qId;
            this.QuizName = qName;
        }

        [Key]
        public Int16 QuizId { get; set; }
        public String QuizName { get; set; }
    }

    [Serializable]
    public class QuestionSet
    {
        public QuestionSet()
        {
        }

        public QuestionSet(Int16 pQuId)
        {
            GsDataContext dc = new GsDataContext();
            //Get the question for passed questionId
            this.Question = dc.QuizQuestions.Where(q => q.QuestionId == pQuId).First();
            //Get list of answers for passed questionId
            this.AnswerList = dc.QuizAnswers.Where(a => a.QuestionId == pQuId).ToList();
        }

        [Key]
        public QuizQuestion Question { get; set; }
        public List<QuizAnswer> AnswerList { get; set; }
    }

    public enum EbookType
    {
        Pdf,
        Epub
    }

    [Serializable]
    public class OpenItemNumber
    {
        public OpenItemNumber()
        {
            this.ItemNumber = Guid.NewGuid();
            this.Started = DateTime.Now;
            this.Completed = null;
        }

        [Key]
        public Guid ItemNumber { get; set; }
        public DateTime Started { get; set; }
        public DateTime? Completed { get; set; }
    }

    [Serializable]
    public class EbookOrder
    {
        public EbookOrder()
        {
        }

        [Key]
        public Int32 EbookOrderId { get; set; }
        public Guid ItemNumber { get; set; }

        //Match valid email address or empty string
        [RegularExpression(@"^([0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$", ErrorMessage = "Please be sure to enter a valid email address.")]
        public String SupportEmail { get; set; }

        public Boolean NewsletterSubscribe { get; set; }
        public String Type { get; set; }
        public DateTime BuyDate{ get; set; }
        public String Payer_Id { get; set; }
        public String Txn_Id { get; set; }
    }

    [Serializable]
    public class ActiveQuiz
    {
        public ActiveQuiz()
        {
            GetCachedCategoryList();
            LoadQuizState();
            this.QuestionCount = 0;
        }

        public Int16 CategoryId { get; set; }
        public Int16 QuizId { get; set; }
        public Int16 CurrentQuestionId { get; set; }
        public Int16 QuestionCount { get; set; }
        public List<QuizCategory> CategoryList { get; set; }
        public List<QuizInfo> QuizInfoList { get; set; }
        public List<QuestionSet> QuestionSet { get; set; }
        public Boolean QuestionComplete { get; set; }
        public Boolean QuizComplete { get; set; }
        public Int16 CorrectAnswers { get; set; }
        public Int16 AnsweredCorrectly { get; set; }
        public String QuizName { get; set; }

        private GsDataContext dc = new GsDataContext();

        public void GetQuestionSet()
        {
            List<QuestionSet> qSetList = new List<QuestionSet>();
            if (null != HttpContext.Current.Session["QuestionSet"])
            {
                this.QuestionSet = (List<QuestionSet>) HttpContext.Current.Session["QuestionSet"];
            }
            else
            {
                List<QuizQuestion> questionList = dc.QuizQuestions.Where(q => q.QuizId == this.QuizId).ToList();
                questionList.ForEach(q =>
                {
                    QuestionSet qSet = new QuestionSet(q.QuestionId);
                    qSetList.Add(qSet);
                });
                this.QuestionSet = qSetList;
                HttpContext.Current.Session["QuestionSet"] = qSetList;
            }
            this.QuestionCount = (Int16)(this.QuestionSet.Count);
            this.GetQuizList();
        }

        public void GetQuizList()
        {
            GetCachedCategoryList();
            if (null == HttpContext.Current.Session["QuizInfoList"])
            {
                List<QuizInfo> quizInfoList = new List<QuizInfo>();
                dc.Quizzes.Where(q => q.CategoryId == this.CategoryId).ToList().ForEach(q =>quizInfoList.Add(new QuizInfo(q.QuizId, q.QuizName)));
                HttpContext.Current.Session["QuizInfoList"] = quizInfoList;
            }
            this.QuizInfoList = (List<QuizInfo>)HttpContext.Current.Session["QuizInfoList"];
        }

        private void GetCachedCategoryList()
        {
            if (null == this.CategoryList)
            {
                if (HttpContext.Current.Cache["CategoryList"] == null)
                {
                    //HttpContext.Current.Cache["CategoryList"] = new GsDataContext().QuizCategories.ToList();
                    List<short> categoriesWithQuestions = new GsDataContext().Quizzes.Select(q => q.CategoryId).Distinct().ToList();
                    List<QuizCategory> categoriesInUse = new GsDataContext().QuizCategories.Where(qc => categoriesWithQuestions.Contains(qc.CategoryId)).ToList();
                    HttpContext.Current.Cache["CategoryList"] = categoriesInUse;
                }
                this.CategoryList = (List<QuizCategory>)HttpContext.Current.Cache["CategoryList"];
            }
        }

        private void LoadQuizState()
        {
            //Do we have a QuidId yet?
            if (null != HttpContext.Current.Session["QuizId"])
            {
                this.GetQuestionSet();
            }
        }
    }
}