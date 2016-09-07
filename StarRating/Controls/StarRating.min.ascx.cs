using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI.WebControls;

namespace StarRating
{
    public partial class StarRatingMin : System.Web.UI.UserControl
    {
        public int articleId
        {
            get { return (int)ViewState["articleId"];  }
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
            get { 
                if (ViewState["numOfStars"] == null)
                    ViewState["numOfStars"] = (decimal)5;
                return (decimal)ViewState["numOfStars"];
            }
            set { ViewState["numOfStars"] = value;  }
        }

        private SqlConnection conn;
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

                string icon = (avarageScore <= i) ? "glyphicon-star-empty" : "glyphicon-star" ;
                starBtn.Text = "<i class='glyphicon glyphicon " + icon + "'></i>";

                Article_Rating.Controls.Add(starBtn);
            }
        }

        private void updateTextScore(decimal score)
        {
            Article_Score.Text = "<em>Score/Stemmer: " + score.ToString("0.0") + " / " +  this.numOfVotes + "</em>";
        }

        protected void submitRating(object s, CommandEventArgs e)
        {
            try {
                using (conn = new SqlConnection(ConfigurationManager.ConnectionStrings["Conn"].ConnectionString)) 
                {
                    int userScore = Convert.ToInt32(e.CommandArgument);
                    string userSessionID = HttpContext.Current.Session.SessionID;

                    SqlCommand newScore = new SqlCommand(@"SELECT COUNT(Id) AS count FROM Rating WHERE SessionID = @session AND ArticleId = @article", conn);
                    newScore.Parameters.AddWithValue("session", userSessionID);
                    newScore.Parameters.AddWithValue("article", this.articleId);

                    conn.Open(); //Åben forbindelse til database
                    object num = newScore.ExecuteScalar();
                    if ((int)num == 0)
                    {
                        newScore.CommandText = @"INSERT INTO Rating VALUES (@value, @session, @article)";
                        newScore.Parameters.AddWithValue("value", userScore);
                        newScore.ExecuteNonQuery();

                        this.numOfVotes++;
                        this.sumOfScore += userScore;
                        Article_Message.Text = "Din stemme på " + userScore + " er gemt. <em>Hvis du vil ændre din stemme kan du klikke igen.</em>";
                    }
                    else
                    {
                        newScore.CommandText = @"DECLARE @oldVote int;UPDATE Rating SET score = @value, @oldVote = score WHERE SessionID = @session AND ArticleId = @article;SELECT @oldVote";
                        newScore.Parameters.AddWithValue("value", userScore);
                        object oldVote = newScore.ExecuteScalar();

                        this.sumOfScore -= Convert.ToInt32(oldVote);
                        this.sumOfScore += userScore;
                        Article_Message.Text = "Det ser ud til at du allerede har stemt, men bare rolig. Din stemme på " + userScore + " er gemt.";
                    }
                    this.updateRatingView();
                }
            } catch { throw new HttpException(500, "Der skete en fejl"); }
        }

        private void updateRatingView()
        {
            this.updateTextScore(this.avarageScore);
            this.addRatingStars();
        }

    }
}