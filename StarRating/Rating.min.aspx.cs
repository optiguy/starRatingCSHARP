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
    public partial class RatingMin : System.Web.UI.Page
    {
        private SqlConnection conn;
        private int articleId;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack) return;

            try
            {
                try { this.articleId = Convert.ToInt32(Request.QueryString["articleId"].ToString()); }
                catch { this.articleId = 1; }

                using (conn = new SqlConnection(ConfigurationManager.ConnectionStrings["Conn"].ConnectionString))
                {
                    SqlCommand ArticleQuery = new SqlCommand(@"SELECT a.Id, a.Title, a.DateCreated, (SELECT Content from Article WHERE Id = @id) as Content, COALESCE(SUM(r.Score),0) as sumOfScore, COUNT(r.Id) as numOfVotes FROM Article as a LEFT JOIN Rating as r ON a.Id = r.ArticleId WHERE a.Id = @id AND a.Status = 1 GROUP BY a.Id, a.Title, a.DateCreated", conn);
                    ArticleQuery.Parameters.AddWithValue("id", this.articleId);

                    conn.Open();
                    SqlDataReader ArticleData = ArticleQuery.ExecuteReader();
                    if (ArticleData.HasRows && ArticleData.Read())
                    {
                        Article_Title.Text = ArticleData["Title"].ToString();
                        Article_Date.Text = ArticleData["DateCreated"].ToString();
                        Article_Content.Text = ArticleData["Content"].ToString();
                        Article_StarRating.articleId = Convert.ToInt32(ArticleData["Id"]);
                        Article_StarRating.sumOfScore = Convert.ToDecimal(ArticleData["sumOfScore"]);
                        Article_StarRating.numOfVotes = Convert.ToDecimal(ArticleData["numOfVotes"]);
                    }
                    else throw new HttpException(404, "Artiklen findes ikke!");
                }
            }
            catch { throw new HttpException(500, "Der skete en fejl!"); }
        }

    }
}