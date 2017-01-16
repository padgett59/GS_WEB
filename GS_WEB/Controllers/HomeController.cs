using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Caching;
using System.Linq;
using MVC.GSWeb.Models;
using System.Collections.Generic;


namespace MVC.GSWeb.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(byte newsEnrolled = 0)
        {
            ViewData["enrollStatus"] = newsEnrolled; 
            return View();
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DownloadEbook(FormCollection formCollection)
        {

            //foreach (string _formData in formCollection)
            //{
            //    ViewData[_formData] = formCollection[_formData];
            //}
            //return View();

            Guid itemNumber = Guid.Parse(formCollection["item_number"]);
            string supportEmail = formCollection["payer_email"];
            
            string newsletterAdd = formCollection["custom"];
            string itemName = formCollection["item_name"];
            string Payer_Id = formCollection["payer_id"];
            string Txn_Id = formCollection["txn_id"];

            //Vaildate trNumber and set the complete date
            OpenItemNumber openItem = new OpenItemNumber();
            Boolean purchaseValid = DownloadValid(itemNumber, out openItem);
            if (openItem != null)
            {
                EbookOrder ebookOrder = new EbookOrder();
                ebookOrder.BuyDate = DateTime.Now;
                ebookOrder.ItemNumber = itemNumber;
                ebookOrder.SupportEmail = supportEmail;
                if (itemName.IndexOf("PDF") >= 0) { ebookOrder.Type = "PDF"; }
                if (itemName.IndexOf("e-Book") >= 0) { ebookOrder.Type = "Ebook"; }
                ViewBag.Type = ebookOrder.Type;
                ebookOrder.NewsletterSubscribe = (newsletterAdd.IndexOf("supportAndNewsletter") >= 0);
                ebookOrder.Payer_Id = Payer_Id;
                ebookOrder.Txn_Id = Txn_Id;
                this.GsDataContext().EbookOrders.Add(ebookOrder);
                if (ebookOrder.NewsletterSubscribe)
                {
                    if (IsEmailUnique(ebookOrder.SupportEmail))
                    {
                        //Add to newletter subscribers
                        NewReader newReader = new NewReader(ebookOrder.SupportEmail, ebookOrder.SupportEmail);
                        newReader.DateAdded = System.DateTime.Now.ToShortDateString();
                        this.GsDataContext().NewReaders.Add(newReader);
                    }
                }
                this.GsDataContext().SaveChanges();
            }

            return View(openItem);
        }

        private Boolean DownloadValid (Guid trNumber, out OpenItemNumber openItemNumber)
        {
            openItemNumber = this.GsDataContext().OpenItemNumbers.FirstOrDefault(oi => oi.ItemNumber == trNumber && oi.Completed == null);
            if (openItemNumber != null)
            {
                //Set completed
                openItemNumber.Completed = DateTime.Now;
                this.GsDataContext().SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        public ActionResult PreviewBook()
        {
            return View();
        }

        public ActionResult OrderThanks()
        {
            return View();
        }

        public ActionResult AddReviewThanks()
        {
            return View();
        }

        public ActionResult BuyProof()
        {
            ProofOrder newProofOrder = new ProofOrder();
            return View(newProofOrder);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BuyProof(ProofOrder proofOrder)
        {
            if (proofOrder.Step != 3) //Final)
            {
                //Caculate Total and show
                ModelState.Clear();
                proofOrder.Total = (proofOrder.Quantity * Constants.PROOF_COST) + 1.99M;
                //Add extra shipping for more than 1 
                if (proofOrder.Quantity > 1)
                {
                    proofOrder.Total += (proofOrder.Quantity - 1) * .99M;
                }
                proofOrder.Step = 2; //Update;
                return View(proofOrder);
            }
            else
            {
                //Save and notify
                proofOrder.DateOrdered = DateTime.Now;
                this.GsDataContext().ProofOrders.Add(proofOrder);
                
                //Add to newletter list if not yet enrolled
                if (proofOrder.NewsEnroll && IsEmailUnique(proofOrder.Email))
                {
                    NewReader subscriber = new NewReader(String.Format("{0} {1}",proofOrder.FirstName, proofOrder.LastName), proofOrder.Email);
                    this.GsDataContext().NewReaders.Add(subscriber);
                }
                this.GsDataContext().SaveChanges();

                //Notify for invoincing 
                GS_WEB.Models.Notify.OrderReceived(proofOrder);

                return RedirectToAction("OrderThanks");
            }
        }

        public ActionResult BuyEbook()
        {
            OpenItemNumber openItemNumber = new OpenItemNumber();
            this.GsDataContext().OpenItemNumbers.Add(openItemNumber);
            this.GsDataContext().SaveChanges();
            //ViewBag.emailRegex = @"^([0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$";
            return View(openItemNumber);
        }
        
        public ActionResult TakeQuiz()
        {
            //Get the Quiz categories
            Session["QuestionSet"] = null;
            ActiveQuiz activeQuiz = new ActiveQuiz();
            activeQuiz.CurrentQuestionId = -1;
            return View(activeQuiz);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TakeQuiz(ActiveQuiz activeQuiz, FormCollection formCollection)
        {
            //Check for quiz reset
            var resetQuizFlag = formCollection["resetQuiz"];
            if (Boolean.Parse(resetQuizFlag))
            {
                ActiveQuiz activeQuiz2 = new ActiveQuiz();
                activeQuiz2.CurrentQuestionId = -1;
                if (activeQuiz.CategoryId > 0) activeQuiz2.CategoryId = activeQuiz.CategoryId;
                if (activeQuiz.QuizId > 0) activeQuiz2.QuizId = activeQuiz.QuizId;
                activeQuiz = activeQuiz2;
                activeQuiz2 = null;
                ModelState.Clear(); //Clear model cache
                Session["QuestionSet"] = null;
            }

            //string QuizId = formCollection["QuizId"];
            if (!activeQuiz.QuestionComplete)
            {
                //User selected Quiz
                if (activeQuiz.QuizId > 0 && null == activeQuiz.QuestionSet)
                {
                    //Load the Question Sets for the selected quiz
                    activeQuiz.GetQuestionSet();
                    activeQuiz.CurrentQuestionId = 0;
                }
                //User selected Category
                else if (activeQuiz.QuizId == 0)
                {
                    //Get the Quizzes for the category selected
                    Session["QuizInfoList"] = null;
                    activeQuiz.GetQuizList();
                }
            }
            else
            {
                //User completed question
                //Reload Question Set
                activeQuiz.GetQuestionSet();
                activeQuiz.QuestionComplete = false;
                activeQuiz.CurrentQuestionId++;
                activeQuiz.QuizComplete = activeQuiz.CurrentQuestionId > activeQuiz.QuestionSet.Count;
                if (activeQuiz.QuizComplete)
                {
                    Session["QuestionSet"] = null;
                }
            }

            return View(activeQuiz);
        }

        public ActionResult NewsLetter(byte newsEnrolled = 0)
        {
            ViewData["enrollStatus"] = newsEnrolled;
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult NewsLetter(NewReader newReader, FormCollection formCollection)
        {
            if (formCollection["mode"] != "remove")
            {
                if (ModelState.IsValid)
                {
                    //Check to see if the email is unique
                    if (IsEmailUnique(newReader.Email))
                    {
                        //Add user
                        try
                        {
                            newReader.DateAdded = System.DateTime.Now.ToShortDateString();
                            this.GsDataContext().NewReaders.Add(newReader);
                            this.GsDataContext().SaveChanges();

                            //Notify 
                            GS_WEB.Models.Notify.WelcomeSubscriber(newReader);
                            ViewData["enrollStatus"] = Constants.NEW_READER_ADDED;
                        }
                        catch
                        {
                            ViewData["enrollStatus"] = Constants.NEW_READER_ADD_ERROR;
                        }

                        string addUser = string.Empty;
                    }
                    else
                    {
                        //Raise 'already taken' error
                        ViewData["enrollStatus"] = Constants.NEW_READER_EXISTS;
                    }
                }
                else
                {
                    ViewData["enrollStatus"] = Constants.NEW_READER_ADD_ERROR;
                }
                return View(newReader);

            }
            else
            {
                //Unsubscribe Reader
                NewReader removeReader = this.GsDataContext().NewReaders.FirstOrDefault(r => r.Email.ToLower() == newReader.Email.ToLower());
                if (removeReader != null)
                {
                    this.GsDataContext().NewReaders.Remove(removeReader);
                    ViewData["enrollStatus"] = Constants.READER_REMOVED;
                }
                else
                {
                    ViewData["enrollStatus"] = Constants.REMOVE_NOTFOUND;
                }
                return View(newReader);
            }
        }

        public ActionResult Support()
        {
            return View();
        }
        public ActionResult Blog()
        {
            return View();
        }

        public ActionResult Reviews()
        {
            var reviewList = this.GsDataContext().ReaderReviews.Where(r => r.Approved).ToList();
            return View(reviewList);
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult BuyBook()
        {
            return View();
        }

         public ActionResult AddReview()
        {
            ReaderReview readerReview = new ReaderReview();
            return View(readerReview);
        }

        [AcceptVerbs(HttpVerbs.Post)]
         public ActionResult AddReview(ReaderReview readerReview)
        {
            if (readerReview.Step != 3) //Final)
            {
                //Caculate Total and show
                ModelState.Clear();
                readerReview.Step = 2; //Update;
                return View(readerReview);
            }
            else
            {
                //Save and notify
                readerReview.DateAdded = DateTime.Now.ToShortDateString();
                this.GsDataContext().ReaderReviews.Add(readerReview);

                //Add to newletter list if not yet enrolled
                if (readerReview.NewsEnroll && IsEmailUnique(readerReview.Email))
                {
                    NewReader subscriber = new NewReader(String.Format("{0} {1}", readerReview.FirstName, readerReview.LastName), readerReview.Email);
                    this.GsDataContext().NewReaders.Add(subscriber);
                }
                
                this.GsDataContext().SaveChanges();

                //Notify for checking and approving the review
                GS_WEB.Models.Notify.CheckReview(readerReview);
                return RedirectToAction("AddReviewThanks");
            }
        }
       
        public bool IsEmailUnique(string eMail)
        {
            return !this.GsDataContext().NewReaders.Any(nr => nr.Email == eMail);
        }
    }
}