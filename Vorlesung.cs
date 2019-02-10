namespace Abschlussarbeit
{
    partial class Stundenplan
    {
        public class Vorlesung
        {
            private string v_nr;
            private string v_fachbereich;
            private int v_semester;
            private string v_bezeichnung;
            private string v_beschreibung;
            private string v_professor;
            private int v_plätze;

            public string getVorlesungNr()
            {
                return v_nr;
            }
            public string getVorlesungFachbereich()
            {
                return v_fachbereich;
            }
            public int getVorlesungSemester()
            {
                return v_semester;
            }
            public string getVorlesungBezeichnung()
            {
                return v_bezeichnung;
            }
            public string getVorlesungBeschreibung()
            {
                return v_beschreibung;
            }
            public string getVorlesungProfessor()
            {
                return v_professor;
            }
            public int getVorlesungPlätze()
            {
                return v_plätze;
            }
            public void setVorlesungNr(string neuNr)
            {
                v_nr = neuNr;
            }
            public void setVorlesungFachbereich(string neuFB)
            {
                v_fachbereich = neuFB;
            }
            public void setVorlesungSemester(int neuSemester)
            {
                v_semester = neuSemester;
            }
            public void setVorlesungBezeichnung(string neuBez)
            {
                v_bezeichnung = neuBez;
            }
            public void setVorlesungBeschreibung(string neuBesch)
            {
                v_beschreibung = neuBesch;
            }
            public void setVorlesungProfessor(string neuProf)
            {
                v_professor = neuProf;
            }
            public void setVorlesungPlätze(int neuPlätze)
            {
                v_plätze = neuPlätze;
            }
        }
    }
}