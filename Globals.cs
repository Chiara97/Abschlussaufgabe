namespace Abschlussarbeit
{
    using System.Collections.Generic;
    partial class Stundenplan
    {
        static class Globals
        {
            public static int anzFachbereiche;
            public static int anzProfessoren;
            public static int anzRäume;
            public static int anzVorlesungen;
            public static int anzZeiten;
            public static int anzPläne;
            public static int anzWpv;
            public static List<Fachbereich> listeFachbereiche = new List<Fachbereich>();
            public static List<Professor> listeProfessoren = new List<Professor>();
            public static List<Raum> listeRäume = new List<Raum>();
            public static List<Vorlesung> listeVorlesungen = new List<Vorlesung>();
            public static List<Zeit> listeZeiten = new List<Zeit>();
            public static List<Plan> listePläne = new List<Plan>();
            public static List<Wpv> listeWpv = new List<Wpv>();
        }
    }
}