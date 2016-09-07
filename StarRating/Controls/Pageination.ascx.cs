using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace StarRating.Controls
{
    public partial class Pageination : System.Web.UI.UserControl
    {

        public int perPage = 5;
        public int totalRecords;
        private int currentPage = 1;
        private int totalPages
        {
            get
            {
                return totalRecords / perPage;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if( !string.IsNullOrEmpty(Request.QueryString["side"]) )
            {
                currentPage = Convert.ToInt32(Request.QueryString["side"]);
            }
            createLinks();
        }

        private void createLinks()
        {
            HtmlGenericControl ul = new HtmlGenericControl("ul");
            
            ul.Attributes.Add("class", "pageination");
            
            //Sidste side
            LinkButton prevPage = new LinkButton();
            prevPage.Text = "Sidste side";
            if(currentPage == 1)
            {
                prevPage.Enabled = false;
            }
            prevPage.PostBackUrl = HttpContext.Current.Request.Url.AbsolutePath + "?side=" + (currentPage - 1).ToString();
            HtmlGenericControl prevListItem = new HtmlGenericControl("li");
            prevListItem.Controls.Add(prevPage);
            ul.Controls.Add(prevListItem);

            for (int i = 1; i <= totalPages; i++ )
            {
                HtmlGenericControl li = new HtmlGenericControl("li");
                
                LinkButton pagelink = new LinkButton();
                pagelink.Text = i.ToString();
                if(currentPage == i)
                {
                    pagelink.Enabled = false;
                }
                pagelink.PostBackUrl = HttpContext.Current.Request.Url.AbsolutePath + "?side=" + i.ToString();
                li.Controls.Add(pagelink);
                ul.Controls.Add(li);
            }

            // Næste side
            LinkButton nextPage = new LinkButton();
            nextPage.Text = "Næste side";
            if (currentPage == totalPages)
            {
                nextPage.Enabled = false;
            }
            nextPage.PostBackUrl = HttpContext.Current.Request.Url.AbsolutePath + "?side=" + (currentPage + 1).ToString();
            HtmlGenericControl nextListItem = new HtmlGenericControl("li");
            nextListItem.Controls.Add(nextPage);
            ul.Controls.Add(nextListItem);

            //Tilføj pagination til siden
            pageination_links.Controls.Add(ul);
            
            /*
            <li>
                <a href="#" aria-label="Previous">
                    <span aria-hidden="true">&laquo;</span>
                </a>
            </li>
            <li><a href="#">1</a></li>
            <li><a href="#">2</a></li>
            <li><a href="#">3</a></li>
            <li><a href="#">4</a></li>
            <li><a href="#">5</a></li>
            <li>
                <a href="#" aria-label="Next">
                    <span aria-hidden="true">&raquo;</span>
                </a>
            </li>
             */
        }

    }
}