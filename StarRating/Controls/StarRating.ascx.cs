using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI.WebControls;

namespace StarRating
{
    public partial class StarRating : System.Web.UI.UserControl
    {
        // Disse er public fordi de skal sættes fra rating siden
        public int articleId //Den artikel vi arbejder med
        {
            /*
             * Istedet for en lokal variabel, sætter vi et viewstate som bliver gemt ved pageload
             * Hvad er ViewStates: https://msdn.microsoft.com/en-us/library/ms972976.aspx#viewstate_topic4
             * Mere om ViewStates: https://msdn.microsoft.com/en-us/library/bb386448.aspx
             */
            get
            {
                // Hvad der sker når nogen tilspørger variablen
                return (int)ViewState["articleId"];
            }
            set
            {
                // Hvad der sker når nogen sætter en ny værdi til variablen
                ViewState["articleId"] = value;
            }
        }
        public decimal sumOfScore // Summen af alle stemmer
        {
            get
            {
                return (decimal)ViewState["sumOfScore"];
            }
            set
            {
                ViewState["sumOfScore"] = value;
            }
        }
        public decimal numOfVotes // Antallet af stemmer
        {
            get
            {
                return (decimal)ViewState["numOfVotes"];
            }
            set
            {
                ViewState["numOfVotes"] = value;
            }
        }
        public decimal numOfStars // Antallet af stemmer
        {
            get
            {
                // Hvis ikke der er valgt hvor mange så benytter vi 5 stjerner
                if (ViewState["numOfStars"] == null)
                    ViewState["numOfStars"] = (decimal)5;

                return (decimal)ViewState["numOfStars"];
            }
            set
            {
                ViewState["numOfStars"] = value;
            }
        }

        // Disse variabler skal kun bruges i denne klasse
        private SqlConnection conn; //Forbindelse til database
        private decimal avarageScore //Gennemsnittet
        {
            get
            {
                //Vi kan ikke dividerer med 0
                if (this.numOfVotes == 0)
                {
                    return 0;
                }
                else
                {
                    // Udregn gennemsnit for score
                    return this.sumOfScore / this.numOfVotes;
                }
                /*
                 * Linien nedeunder gør det samme som ovenstående. Kan du regne den ud???
                 * return (this.numOfVotes == 0) ? 0 : this.sumOfScore / this.numOfVotes;
                */
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            /*
             *  Mere om egne User Controls : https://msdn.microsoft.com/en-us/library/3457w616.aspx
             *  "this." er ikke nødvendigt, men jeg har valgt at bruge 'this.' foran variabler fra klassen 
             *  idet at ordet 'this' relaterer til denne klasse vi er i (StarRating)
             *  og uden this når det er lokale variabler der kun virker inden for deres scope, altså fra '{' til '}'
             *  det gør også at man kan bruge det samme navn til to variabler, en i scoppet og en i klassen.
             */

            // Tilføj nuværende score som tekst
            this.updateTextScore(this.avarageScore);

            // Tilføj de fem stjerner
            this.addRatingStars();
        }

        private void addRatingStars()
        {
            // Vi sikre os at vores control uden på siden er tom, inden vi begynder med at tilføje stjerner
            this.Article_Rating.Controls.Clear();

            // Tilføj det antal stjerner vi vil have
            for (int i = 0; i < this.numOfStars; i++)
            {
                // Opret et link til stjernen.
                LinkButton starBtn = new LinkButton();
                // Tilføj værdien til stjernen 1..2..3...
                starBtn.CommandArgument = (i + 1).ToString();
                // Tilføj et ID til knappen, for at forhindre fejl, ved pageload
                starBtn.ID = "starRatingBtn" + (i + 1).ToString();

                // Hvis den nuværende score er højere end den stjerne vi er nået til
                string icon;
                if (avarageScore <= i)
                {
                    // Flot ikon der viser en tom stjerne
                    icon = "glyphicon-star-empty";
                }
                else
                {
                    // Endnu et flot ikon der viser en fuld stjerne... hik!
                    icon = "glyphicon-star";
                }
                /*
                 * Linien nedeunder gør det samme som ovenstående. Kan du regne den ud???
                 * string icon = (avarageScore <= i) ? "glyphicon-star-empty" : "glyphicon-star" ;
                */
                starBtn.Text = "<i class='glyphicon glyphicon " + icon + "'></i>";

                // Tilføj klik event til stjernen
                starBtn.Command += new CommandEventHandler(this.submitRating);

                // Tilføj stjernen til siden  
                Article_Rating.Controls.Add(starBtn);
            }
            // Bum... Alle stjerne er tilføjet.
        }

        private void updateTextScore(decimal score)
        {
            // Tilføj en tekst så man kan se selve score i tal
            Article_Score.Text = "<em>Score/Stemmer: " + score.ToString("0.0") + " / " +  this.numOfVotes + "</em>";
        }

        protected void submitRating(object s, CommandEventArgs e)
        {
            try
            {

                // Sørg for at forbindelsen bliver oprettet inden vi tillspørger databasen
                using (conn = new SqlConnection(ConfigurationManager.ConnectionStrings["Conn"].ConnectionString))
                {
                    // Værdien af den stjerne som brugeren har klikket på
                    int userScore = Convert.ToInt32(e.CommandArgument);
                    /*
                     * Istedet for SessionID som kun holder 15 uden brug af siden, 
                     * kunne det også være brugerens Id fra en bruger tabel.
                     * Lav selv ændringerner i din database
                     */
                    string userSessionID = HttpContext.Current.Session.SessionID;

                    // Opret en ny sql forespørgelse
                    SqlCommand newScore = new SqlCommand(@"
                        SELECT COUNT(Id) AS count 
                        FROM Rating 
                        WHERE SessionID = @session
                        AND ArticleId = @article
                    ", conn);

                    // Husk at tilføje værdier til de to parametre
                    newScore.Parameters.AddWithValue("session", userSessionID);
                    newScore.Parameters.AddWithValue("article", this.articleId);

                    conn.Open(); //Åben forbindelse til database
                    
                    //Udfør SQL og returner en værdi
                    object userVote = newScore.ExecuteScalar();

                    //Hvis brugeren ikke allerede har stemt, skal resultatet af stemmer gerne være 0
                    if ((int)userVote == 0)
                    {
                        // Vi er Good-2-Go og kan gemme stemmen

                        /* 
                         * Vi genbruger den SQL forespørgelse som vi lige har brugt.
                         * Derfor sætter vi kun ændringer og nye ting
                         */

                        // Opdater SQL sætningen
                        newScore.CommandText = @"
                            INSERT INTO Rating 
                            VALUES (@value, @session, @article)
                        ";

                        // Husk at tilføje det nye parameter
                        newScore.Parameters.AddWithValue("value", userScore);

                        // Udfør for at gemme i databasen
                        newScore.ExecuteNonQuery();

                        /*
                         * Hele dette afsnit er i stedet for at lave et redirect for at vise ændringerne.
                         * Det kræver både tid og resourcer at reloade siden når vi ikke har brug for det.
                         */

                        // Opdater vores ViewState variabler, for at reflektere de ændringer vi lige har lavet i databasen
                        this.numOfVotes++;
                        this.sumOfScore += userScore;

                        // Sæt en pæn besked så brugeren ved hvad der er sket
                        Article_Message.Text = "Din stemme på " + userScore + " er gemt. <em>Hvis du vil ændre din stemme kan du klikke igen.</em>";
                    }
                    else
                    {
                        newScore.CommandText = @"
                                DECLARE @oldVote int

                                UPDATE Rating 
                                SET score = @value, @oldVote = score 
                                WHERE SessionID = @session
                                AND ArticleId = @article

                                --to receive the old value
                                SELECT @oldVote 
                        ";

                        // Husk at opdatere parametret med den nye værdi
                        newScore.Parameters.AddWithValue("value", userScore);
                        
                        // Udfør og returner den gamle værdi så vi kan reflekterer ændringerne istedet for at reloade siden
                        object oldVote = newScore.ExecuteScalar();

                        //Træk den gamle score fra og opdater med den nye
                        this.sumOfScore -= Convert.ToInt32(oldVote);
                        this.sumOfScore += userScore;

                        // Brugeren har allerede stemt
                        Article_Message.Text = "Det ser ud til at du allerede har stemt, men bare rolig. Din stemme på " + userScore + " er gemt.";
                    }

                    // Opdater det som brugeren ser
                    this.updateRatingView();

                    conn.Close(); //Bare for god ordens skyld, selvom using tager sig af det

                }
            }
            catch
            {
                throw new HttpException(500, "Der skete en fejl");
            }
        }

        private void updateRatingView()
        {
            // Opdater det visuelle på siden så brugeren kan se ændringerne
            this.updateTextScore(this.avarageScore);
            this.addRatingStars();
        }

    }
}