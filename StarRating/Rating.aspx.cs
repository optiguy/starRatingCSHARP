using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace StarRating
{
    public partial class Rating : System.Web.UI.Page
    {
        // Opret variabler som siden altid skal bruge
        private SqlConnection conn;
        private int articleId;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Brugeren får kun tildelt en sessionID hvis de har en session. Dette er bare til demostrationen her
            Session.Add("init", 0);

            // Hvis det er et postback, så stopper vi koden her
            if (Page.IsPostBack) return;

            try
            {
                try
                {
                    // Hent id fra adresse linie og gem inden i denne klasse
                    this.articleId = Convert.ToInt32(Request.QueryString["articleId"].ToString());
                }
                catch
                {
                    //Hvis vi besøger artikel siden, uden at have et gyldigt artikel id.
                    //throw new HttpException(404, "Artiklen findes ikke!");
                    this.articleId = 1;
                }

                //Sørg for at forbindelsen bliver oprettet inden vi tillspørger databasen
                using (conn = new SqlConnection(ConfigurationManager.ConnectionStrings["Conn"].ConnectionString))
                {
                    //Opret SQL til at hente den enkelte artikel ud sammen med antallet af stemmer og summen af stemmerne
                    SqlCommand ArticleQuery = new SqlCommand(@"

                            SELECT a.Id, a.Title, a.DateCreated,

                            (SELECT Content from Article WHERE Id = @id) as Content, 

                            COALESCE(SUM(r.Score),0) as sumOfScore,

                            COUNT(r.Id) as numOfVotes

                            FROM Article as a

                            LEFT JOIN Rating as r ON a.Id = r.ArticleId

                            WHERE a.Id = @id 
                            AND a.Status = 1

                            GROUP BY a.Id, a.Title, a.DateCreated
                        ", conn);

                    //Sørg for at sikre mod SQL injection ved at tilføje alle dynamiske værædier som parametre
                    ArticleQuery.Parameters.AddWithValue("id", this.articleId);

                    //Åben forbindelse for at udføre vores query.
                    conn.Open();

                    //Udfør din Sequel Language Query og giv os resultatet
                    SqlDataReader ArticleData = ArticleQuery.ExecuteReader();

                    //Hvis der er række(r) og vi kan læse den første
                    if (ArticleData.HasRows && ArticleData.Read())
                    {
                        //Vis de enkelte værdier fra databasen
                        Article_Title.Text = ArticleData["Title"].ToString();
                        Article_Date.Text = ArticleData["DateCreated"].ToString();
                        Article_Content.Text = ArticleData["Content"].ToString();

                        /*
                         * Sæt vores hjemmebyggede star rating op (Dette er det nye!!!)
                         * Marker f.eks ordet 'articleId' og tryk F12 for at gå til deklerationen
                         * eller marker ordet og tryk alt+F12 for quick looks
                         * Du finder filerne under Controls/StarRating
                         */
                        Article_StarRating.articleId = Convert.ToInt32(ArticleData["Id"]);
                        Article_StarRating.sumOfScore = Convert.ToDecimal(ArticleData["sumOfScore"]);
                        Article_StarRating.numOfVotes = Convert.ToDecimal(ArticleData["numOfVotes"]);
                    }
                    else
                    {
                        throw new HttpException(404, "Artiklen findes ikke!");
                    }

                    //Luk forbindelsen for at frigøre resourcer. (Bliver også gjort automatisk ved at bruge using)
                    conn.Close();

                }
            }
            catch (Exception error)
            {
                Response.Write(error.Message);
                throw new HttpException(500, "Der skete en fejl!");
            }
        }

    }
}