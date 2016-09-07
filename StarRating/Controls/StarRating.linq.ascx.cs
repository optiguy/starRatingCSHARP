using System;
using System.Data.Linq;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace StarRating
{
    public partial class StarRatingLinq : System.Web.UI.UserControl
    {
        private DataClassesDataContext db;
        public int articleId
        {
            get { return (int)ViewState["articleId"]; }
            set { ViewState["articleId"] = value; }
        }
        public decimal sumOfScore
        {
            get { return (decimal)ViewState["sumOfScore"]; }
            set { ViewState["sumOfScore"] = value; }
        }
        public decimal numOfVotes
        {
            get { return (decimal)ViewState["numOfVotes"]; }
            set { ViewState["numOfVotes"] = value; }
        }
        public decimal numOfStars
        {
            get
            {
                if (ViewState["numOfStars"] == null)
                    ViewState["numOfStars"] = (decimal)5;
                return (decimal)ViewState["numOfStars"];
            }
            set { ViewState["numOfStars"] = value; }
        }

        private decimal avarageScore
        {
            get { return (this.numOfVotes == 0) ? 0 : this.sumOfScore / this.numOfVotes; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.updateTextScore(this.avarageScore);
            this.addRatingStars();
        }

        private void addRatingStars()
        {
            this.Article_Rating.Controls.Clear();
            for (int i = 0; i < this.numOfStars; i++)
            {
                LinkButton starBtn = new LinkButton();
                starBtn.CommandArgument = (i + 1).ToString();
                starBtn.ID = "starRatingBtn" + (i + 1).ToString();
                starBtn.Command += new CommandEventHandler(this.submitRating);

                string icon = (avarageScore <= i) ? "glyphicon-star-empty" : "glyphicon-star";
                starBtn.Text = "<i class='glyphicon glyphicon " + icon + "'></i>";

                Article_Rating.Controls.Add(starBtn);
            }
        }

        private void updateTextScore(decimal score)
        {
            Article_Score.Text = "<em>Score/Stemmer: " + score.ToString("#0.00") + " / " + this.numOfVotes + "</em>";
        }

        protected void submitRating(object s, CommandEventArgs e)
        {
            try
            {
                using (db = new DataClassesDataContext())
                {
                    int userScore = Convert.ToInt32(e.CommandArgument);
                    string userSessionID = HttpContext.Current.Session.SessionID;

                    try
                    {
                        // Check på om brugeren allerede har stemt.
                        var userVote = (from v in db.Ratings
                                        where v.SessionID == userSessionID && v.ArticleId == this.articleId
                                        select v).Single();

                        this.sumOfScore -= Convert.ToInt32(userVote.Score);
                        userVote.Score = userScore; // Sæt opdateringen i kø til databasen
                        this.sumOfScore += userScore;
                        Article_Message.Text = "Det ser ud til at du allerede har stemt, men bare rolig. Din stemme på " + userScore + " er gemt.";
                    }
                    catch
                    {
                        // Opret en ny stemme
                        Rating vote = new Rating
                        {
                            Score = userScore,
                            SessionID = userSessionID,
                            ArticleId = this.articleId
                        };
                        //Tilføj oprettelse til køen
                        db.Ratings.InsertOnSubmit(vote);

                        this.numOfVotes++;
                        this.sumOfScore += userScore;
                        Article_Message.Text = "Din stemme på " + userScore + " er gemt. <em>Hvis du vil ændre din stemme kan du klikke igen.</em>";
                    }

                    // Udfør køen (insert eller update)
                    db.SubmitChanges();

                    this.updateRatingView();
                }
            }
            catch { throw new HttpException(500, "Der skete en fejl"); }
        }

        private void updateRatingView()
        {
            this.updateTextScore(this.avarageScore);
            this.addRatingStars();
        }

    }
}