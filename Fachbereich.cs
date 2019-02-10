namespace Abschlussarbeit
{
    partial class Stundenplan
    {
        public class Fachbereich
        {
            private string fb_name;
            private string fb_bez;
            private int fb_studenten;
            public string getFachbereichName()
            {
                return fb_name;
            }
            public string getFachbereichBezeichnung()
            {
                return fb_bez;
            }
            public int getFachbereichStudenten()
            {
                return fb_studenten;
            }
            public void setFachbereichName(string neuFB)
            {
                fb_name = neuFB;
            }
            public void setFachbereichBez(string neuBez)
            {
                fb_bez = neuBez;
            }
            public void setFachbereichStudenten(int neuAnz)
            {
                fb_studenten = neuAnz;
            }
        }
    }
}