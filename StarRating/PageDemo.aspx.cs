using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace StarRating
{
    public partial class PageDemo : System.Web.UI.Page
    {

        private DataClassesDataContext db;

        protected void Page_Load(object sender, EventArgs e)
        {

            /* Opsætning af pagination */
            Pagination.totalRecords = 100;
            Pagination.perPage = 4;


        }
    }
}