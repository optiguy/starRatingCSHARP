using System;
using System.Configuration;
using System.Data.Linq;
using System.Linq;
using System.Web;

namespace StarRating
{
    public partial class RatingLinq : System.Web.UI.Page
    {
        // Opret variabler som siden altid skal bruge
        private DataClassesDataContext db;
        private int articleId;

        protected void Page_Load(object sender, EventArgs e)
        {
            Session.Add("init", 0); //Til at slå bruger til og fra

            //Hvis siden bliver loader første gang
            if (Page.IsPostBack) return;

            try
            {
                try { this.articleId = Convert.ToInt32(Request.QueryString["articleId"].ToString()); }
                catch
                {
                    //Hvis vi besøger artikel siden, uden at have et gyldigt artikel id.
                    //throw new HttpException(404, "Artiklen findes ikke!");
                    this.articleId = 1; //Dette er kun til demo
                }

                //Sørg for at forbindelsen bliver oprettet inden vi tillspørger databasen
                using (db = new DataClassesDataContext())
                {

                    /* LINQ: https://msdn.microsoft.com/en-us/library/bb397906(v=vs.110).aspx
                     * Getting started med SQL til LINQ: https://msdn.microsoft.com/en-us/library/bb399398(v=vs.110).aspx
                     * 101 LINQ Examples : https://msdn.microsoft.com/da-dk/vstudio/ee908647.aspx
                     * LINQ Query keywords: https://msdn.microsoft.com/en-us/library/bb310804.aspx
                     */

                    try
                    {
                        /*
                         * Fra Artikel databasen tager vi den rette artikel, hvis den er aktiv
                         * derefter opbygger vi vores kolonner, hvor vi så samtidig tæller ratings
                         * Hvis der ingen stemmer er vil SUM være null, derfor sætter vi selv tallet 0
                         */
                        var article =
                            (from art in db.Articles
                             where art.Status == true && art.Id == this.articleId
                             select new
                             {
                                 art.Id,
                                 art.Title,
                                 art.Content,
                                 art.DateCreated,
                                 numOfVotes = art.Ratings.Count(),
                                 sumOfScore = art.Ratings.Count() == 0 ? 0 : art.Ratings.Sum(r => r.Score)
                             }).Single();

                        // Vis de enkeltke værdier fra databasen
                        Article_Title.Text = article.Title;
                        Article_Date.Text = article.DateCreated.ToString();
                        Article_Content.Text = article.Content;
                        Article_StarRating.articleId = Convert.ToInt32(article.Id);
                        Article_StarRating.sumOfScore = Convert.ToDecimal(article.sumOfScore);
                        Article_StarRating.numOfVotes = Convert.ToDecimal(article.numOfVotes);
                        Article_StarRating.numOfStars = 10;
                    }
                    catch { throw new HttpException(404, "Artiklen findes ikke!"); }
                }
            }
            catch (Exception error) { throw new HttpException(500, "Der skete en fejl!"); }
        }

    }
}