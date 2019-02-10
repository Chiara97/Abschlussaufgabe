namespace Abschlussarbeit
{
    partial class Stundenplan
    {
        public class Wpv
        {
            private string w_zeit;
            private string w_tag;
            private string w_beginn;
            private string w_bezeichnung;
            private string w_professor;

            public string getWpvZeit()
            {
                return w_zeit;
            }
            public string getWpvWochentag()
            {
                return w_tag;
            }
            public string getWpvBeginn()
            {
                return w_beginn;
            }
            public string getWpvBezeichnung()
            {
                return w_bezeichnung;
            }
            public string getWpvProfessor()
            {
                return w_professor;
            }
            public void setWpvZeit(string neuZeit)
            {
                w_zeit = neuZeit;
            }
            public void setWpvWochentag(string neuTag)
            {
                w_tag = neuTag;
            }
            public void setWpvBeginn(string neuBeginn)
            {
                w_beginn = neuBeginn;
            }
            public void setWpvBezeichnung(string neuBez)
            {
                w_bezeichnung = neuBez;
            }
            public void setWpvProfessor(string neuProf)
            {
                w_professor = neuProf;
            }
        }
    }
}