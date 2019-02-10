namespace Abschlussarbeit
{
    partial class Stundenplan
    {
        public class Plan
        {
            private string sp_raum;
            private string sp_tag;
            private string sp_zeit;
            private string sp_fb;
            private int sp_semester;
            private string sp_vorlesung;
            private string sp_prof;
            public string getPlanRaum()
            {
                return sp_raum;
            }
            public string getPlanTag()
            {
                return sp_tag;
            }
            public string getPlanZeit()
            {
                return sp_zeit;
            }
            public string getPlanFB()
            {
                return sp_fb;
            }
            public int getPlanSemester()
            {
                return sp_semester;
            }
            public string getPlanVorlesung()
            {
                return sp_vorlesung;
            }
            public string getPlanProfessor()
            {
                return sp_prof;
            }
            public void setPlanRaum(string neuRaum)
            {
                sp_raum = neuRaum;
            }
            public void setPlanTag(string neuTag)
            {
                sp_tag = neuTag;
            }
            public void setPlanZeit(string neuZeit)
            {
                sp_zeit = neuZeit;
            }
            public void setPlanFB(string neuFB)
            {
                sp_fb = neuFB;
            }
            public void setPlanSemester(int neuSem)
            {
                sp_semester = neuSem;
            }
            public void setPlanVorlesung(string neuVor)
            {
                sp_vorlesung = neuVor;
            }
            public void setPlanProfessor(string neuProf)
            {
                sp_prof = neuProf;
            }
        }
    }
}