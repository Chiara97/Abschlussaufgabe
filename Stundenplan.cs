namespace Abschlussarbeit
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;

    partial class Stundenplan
    {
        public static void Main()
        {
            // Einlesen der Vorgabendateien in Listen
            Globals.anzFachbereiche = EinlesenFachbereiche(@".\fachbereich.txt");
            Globals.anzProfessoren = EinlesenProfessoren(@".\professor.txt");
            Globals.anzRäume = EinlesenRäume(@".\raum.txt");
            Globals.anzVorlesungen = EinlesenVorlesungen(@".\vorlesung.txt");
            Globals.anzZeiten = EinlesenZeiten(@".\zeit.txt");
            Globals.anzWpv = EinlesenWpv(@".\wpv.txt");
            // Gesamtstundenplan in neuem Thread berechnen
            var thr = new Thread(StundenplanBerechnen);
            thr.Start();
            var stopuhr = Stopwatch.StartNew();
            // Warten bis Berechnung abgeschlossen ist oder Anzahl Millisekunden vergangen sind 
            while (stopuhr.ElapsedMilliseconds <= 30000 && thr.IsAlive == true)
            {
            }
            stopuhr.Stop();
            // Ausgabe der Stundenpläne nach vorgegebener Sortierung 
            if (thr.IsAlive == false)
            {
                StundenplanAusgeben();
            }
            else
            {
                Console.WriteLine("Die Berechnung hat zu lange gedauert und wurde ohne Ergebnis abgebrochen.");
            }
        }

        // Stundenplanberechnung: Suche je Fachbereich, Semester, Block und Raum eine Vorlesung mit Dozent
        public static void StundenplanBerechnen()
        {
            for (int f = 0; f < Globals.anzFachbereiche; f++)
            {
                for (int s = 1; s <= 7; s++)
                {
                    for (int z = 0; z < Globals.anzZeiten; z++)
                    {
                        for (int r = 0; r < Globals.anzRäume; r++)
                        {
                            // Check ob der Raum noch frei ist, ohne Belegung zu ändern 
                            if (RaumVerfügbar(r, Globals.listeZeiten[z].getZeitName(), false) == true)
                            {
                                for (int v = 0; v < Globals.anzVorlesungen; v++)
                                {
                                    // Check ob Vorlesung dem gesuchten Fachbereich und Semester entspricht und der Raum groß genug ist 
                                    if (Globals.listeVorlesungen[v].getVorlesungFachbereich() == Globals.listeFachbereiche[f].getFachbereichName() && Globals.listeVorlesungen[v].getVorlesungSemester() == s && Globals.listeVorlesungen[v].getVorlesungPlätze() <= Globals.listeRäume[r].getRaumPlätze())
                                    {
                                        for (int p = 0; p < Globals.anzProfessoren; p++)
                                        {
                                            // Check ob der Dozent zu der Vorlesung passt 
                                            if (Globals.listeVorlesungen[v].getVorlesungProfessor() == Globals.listeProfessoren[p].getProfName())
                                            {
                                                // Check ob der Dozent zum gewählten Block Zeit hat 
                                                if (ProfessorVerfügbar(p, Globals.listeZeiten[z].getZeitName()) == true)
                                                {
                                                    // Raum als belegt setzen 
                                                    RaumVerfügbar(r, Globals.listeZeiten[z].getZeitName(), true);
                                                    Plan plan = new Plan();
                                                    plan.setPlanRaum(Globals.listeRäume[r].getRaumName());
                                                    plan.setPlanTag(Globals.listeZeiten[z].getZeitWochentag());
                                                    plan.setPlanZeit(Globals.listeZeiten[z].getZeitBeginn());
                                                    plan.setPlanFB(Globals.listeVorlesungen[v].getVorlesungFachbereich());
                                                    plan.setPlanSemester(Globals.listeVorlesungen[v].getVorlesungSemester());
                                                    plan.setPlanVorlesung(Globals.listeVorlesungen[v].getVorlesungBezeichnung());
                                                    plan.setPlanProfessor(Globals.listeProfessoren[p].getProfName());
                                                    Globals.listePläne.Add(plan);
                                                    Globals.anzPläne++;
                                                    // Fertig geplante Vorlesung aus der Vorlesungsliste entfernen 
                                                    Globals.listeVorlesungen.RemoveAt(v);
                                                    // Vorlesung ist zugeordnet, for Professor kann verlassen werden 
                                                    break;
                                                }
                                            }
                                        }
                                        // Nach jeder erfolgreichen Zuordnung ist listeVorlesung.Count um 1 weniger
                                        if (Globals.anzVorlesungen != Globals.listeVorlesungen.Count)
                                        {
                                            // Vorlesung ist zugeordnet, for Vorlesung kann verlassen werden
                                            break;
                                        }
                                    }
                                }
                                // Nach jeder erfolgreichen Zuordnung wird der lokale Zähler reduziert, bis alle Vorlesungen verarbeitet sind 
                                if (Globals.anzVorlesungen != Globals.listeVorlesungen.Count)
                                {
                                    Globals.anzVorlesungen = Globals.listeVorlesungen.Count;
                                    // Vorlesung ist zugeordnet, for Raum kann verlassen werden
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            // Wieviele Vorlesungen von der Gesamtanzahl konnten zugewiesen werden
            Console.WriteLine(Globals.anzPläne + " von " + (Globals.anzVorlesungen + Globals.anzPläne) + " Vorlesungen zugeordnet");
            Console.WriteLine();
        }

        public static void StundenplanAusgeben()
        {
            // Abfrage nach Ausgabeart
            Console.WriteLine("Stundenplan für einen Raum <r>, Fachbereich <f> oder Dozent <d>?");
            Console.WriteLine("Beliebige andere Eingabe für eine Gesamtliste.");
			string eingabe = Console.ReadLine();
            Console.WriteLine();
            if (eingabe == "r")
            {
                AusgabeRaum();
            }
            else if (eingabe == "f")
            {
                AusgabeFachbereich();
            }
            else if (eingabe == "d")
            {
                AusgabeProfessor();
            }
            else
            {
                Console.WriteLine("Alle Vorlesungen nach Zeit und Raum:");
                Console.WriteLine();
                for (int i = 0; i < Globals.anzPläne; i++)
                {
                    Console.WriteLine(Globals.listePläne[i].getPlanTag() + " - " + Globals.listePläne[i].getPlanZeit());
                    Console.WriteLine("Raum: " + Globals.listePläne[i].getPlanRaum());
                    Console.WriteLine(Globals.listePläne[i].getPlanFB() + " - " + Globals.listePläne[i].getPlanSemester()+". Semester");
                    Console.WriteLine(Globals.listePläne[i].getPlanVorlesung());
                    Console.WriteLine(Globals.listePläne[i].getPlanProfessor());
                    Console.WriteLine();
                }
            }
            // Hier könnten die nicht verarbeiteten Vorlesungen ausgegeben werden 
            // AusgabeNichtZugewiesene();
        }

        public static void AusgabeRaum()
        {
            bool raumTreffer = false;
            Console.WriteLine("Geben Sie einen Raum ein:");
			string eingabe = Console.ReadLine();
            Console.WriteLine();
            Console.WriteLine("<<< Stundenplan für Raum " + eingabe + " >>>");
            Console.WriteLine();
            // Ausgabe sortieren nach Zeit
            for (int z = 0; z < Globals.anzZeiten; z++)
            {
                for (int i = 0; i < Globals.anzPläne; i++)
                {
                    // Entspricht der aktuelle Listeneintrag dem gewünschten Raum 
                    if (Globals.listePläne[i].getPlanRaum() == eingabe)
                    {
                        // Entspricht der aktuelle Listeneintrag dem Tag und der Zeit 
                        if (Globals.listePläne[i].getPlanTag() == Globals.listeZeiten[z].getZeitWochentag() && Globals.listePläne[i].getPlanZeit() == Globals.listeZeiten[z].getZeitBeginn())
                        {
                            raumTreffer = true; // der eingegebene Raum existiert
                            Console.WriteLine(Globals.listePläne[i].getPlanTag() + " - " + Globals.listePläne[i].getPlanZeit());
                            Console.WriteLine(Globals.listePläne[i].getPlanFB() + " - " + Globals.listePläne[i].getPlanSemester()+". Semester");
                            Console.WriteLine(Globals.listePläne[i].getPlanVorlesung());
                            Console.WriteLine(Globals.listePläne[i].getPlanProfessor());
                            Console.WriteLine();
                        }
                    }
                }
            }
            // Es wurde ein nicht existierender Raum eingegeben 
            if (raumTreffer == false)
            {
                Console.WriteLine(">>> Raum " + eingabe + " nicht gefunden, folgende Räume sind vorhanden:");
                Console.WriteLine();
                for (int i = 0; i < Globals.anzRäume; i++)
                {
                    Console.WriteLine(Globals.listeRäume[i].getRaumName());
                }
            }
        }

        public static void AusgabeFachbereich()
        {
            bool fbTreffer = false;
            Console.WriteLine("Geben Sie einen Fachbereich ein:");
			string eingabe = Console.ReadLine();
			int eingabe2 = 0;
            while (eingabe2 < 1 || eingabe2 > 7)
            {
                Console.WriteLine("Geben Sie das Semester (1-7) ein:");
                eingabe2 = Int32.Parse(Console.ReadLine());
            }
            for (int f = 0; f < Globals.anzFachbereiche; f++)
            {
                // Existiert der eingegebene Fachbereich 
                if (Globals.listeFachbereiche[f].getFachbereichName() == eingabe)
                {
                    fbTreffer = true; // der eingegebene Fachbereich existiert
                    Console.WriteLine();
                    Console.WriteLine("<<< Stundenplan für Fachbereich " + eingabe + " - " + eingabe2 + ". Semester >>>");
                    Console.WriteLine();
                    // Ausgabe sortieren nach Zeit 
                    for (int z = 0; z < Globals.anzZeiten; z++)
                    {
                        for (int i = 0; i < Globals.anzPläne; i++)
                        {
                            // Entspricht der aktuelle Listeneintrag dem gewünschten Fachbereich und Semester 
                            if (Globals.listePläne[i].getPlanFB() == eingabe && Globals.listePläne[i].getPlanSemester() == eingabe2)
                            {
                                // Entspricht der aktuelle Listeneintrag dem Tag und Zeit
                                if (Globals.listePläne[i].getPlanTag() == Globals.listeZeiten[z].getZeitWochentag() && Globals.listePläne[i].getPlanZeit() == Globals.listeZeiten[z].getZeitBeginn())
                                {
                                    Console.WriteLine(Globals.listePläne[i].getPlanTag() + " - " + Globals.listePläne[i].getPlanZeit());
                                    Console.WriteLine("Raum: " + Globals.listePläne[i].getPlanRaum());
                                    Console.WriteLine(Globals.listePläne[i].getPlanVorlesung());
                                    Console.WriteLine(Globals.listePläne[i].getPlanProfessor());
                                    Console.WriteLine();
                                    // Die aktuell zugewiesene Kombi Tag/Zeit kann aus der Liste der WPVs entfernt werden
                                    WpvListeBereinigen(Globals.listeZeiten[z].getZeitName());
                                }
                            }
                        }
                    }
                }
            }
            // Nicht existierenden Fachbereich eingegeben
            if (fbTreffer == false)
            {
                Console.WriteLine(">>> Fachbereich " + eingabe + " nicht gefunden, folgende Fachbereiche sind vorhanden:");
                Console.WriteLine();
                for (int i = 0; i < Globals.anzFachbereiche; i++)
                {
                    Console.WriteLine(Globals.listeFachbereiche[i].getFachbereichName());
                }
            }
            // Ausgabe der noch möglichen WPVs 
            else
            {
                Console.WriteLine();
                Console.WriteLine("Nach Stundenplan belegbare WPVs:");
                Console.WriteLine();
                for (int i = 0; i < Globals.listeWpv.Count; i++)
                {
                    int a = Globals.listeWpv[i].getWpvWochentag().Length;
                    Console.WriteLine(Globals.listeWpv[i].getWpvWochentag().PadRight(10, ' ') + " - " + Globals.listeWpv[i].getWpvBeginn() + ": " + Globals.listeWpv[i].getWpvBezeichnung());
                    Console.WriteLine("                    " + Globals.listeWpv[i].getWpvProfessor());
                }
            }
        }

        public static void AusgabeProfessor()
        {
            bool profTreffer = false;
            Console.WriteLine("Geben Sie einen Dozentennamen ein:");
			string eingabe = Console.ReadLine();
            Console.WriteLine();
            Console.WriteLine("<<< Stundenplan für Dozent " + eingabe + " >>>");
            Console.WriteLine();
            // Ausgabe sortiert nach Tag/Zeit 
            for (int z = 0; z < Globals.anzZeiten; z++)
            {
                for (int i = 0; i < Globals.anzPläne; i++)
                {
                    // Entspricht der aktuelle Listeneintrag dem gewünschten Dozent 
                    if (Globals.listePläne[i].getPlanProfessor() == eingabe)
                    {
                        // Entspricht der aktuelle Listeneintrag dem gewünschten Kombi Tag/Zeit 
                        if (Globals.listePläne[i].getPlanTag() == Globals.listeZeiten[z].getZeitWochentag() && Globals.listePläne[i].getPlanZeit() == Globals.listeZeiten[z].getZeitBeginn())
                        {
                            profTreffer = true; // der eingegebene Dozent existiert
                            Console.WriteLine(Globals.listePläne[i].getPlanTag() + " - " + Globals.listePläne[i].getPlanZeit());
                            Console.WriteLine("Raum: " + Globals.listePläne[i].getPlanRaum());
                            Console.WriteLine(Globals.listePläne[i].getPlanFB() + " - " + Globals.listePläne[i].getPlanSemester()+". Semester");
                            Console.WriteLine(Globals.listePläne[i].getPlanVorlesung());
                            Console.WriteLine();
                        }
                    }
                }
            }
            // Nicht existierenden Dozent eingegeben 
            if (profTreffer == false)
            {
                Console.WriteLine(">>> Dozent " + eingabe + " nicht gefunden, folgende Dozenti sind vorhanden:");
                Console.WriteLine();
                for (int i = 0; i < Globals.anzProfessoren; i++)
                {
                    Console.WriteLine(Globals.listeProfessoren[i].getProfName());
                }
            }
        }

        public static void WpvListeBereinigen(string zeit)
        {
            for (int i = 0; i < Globals.listeWpv.Count; i++)
            {
                if (Globals.listeWpv[i].getWpvZeit() == zeit)
                {
                    Globals.listeWpv.RemoveAt(i);
                }
            }
        }

        public static void AusgabeNichtZugewiesene()
        {
            if (Globals.anzVorlesungen > 0)
            {
                Console.WriteLine("--------------------------------------------------");
                Console.WriteLine();
                Console.WriteLine("Nicht zugeordnete Vorlesungen:");
                Console.WriteLine();
                for (int i = 0; i < Globals.anzVorlesungen; i++)
                {
                    Console.WriteLine(Globals.listeVorlesungen[i].getVorlesungFachbereich() + " - " + Globals.listeVorlesungen[i].getVorlesungSemester()+". Semester");
                    Console.WriteLine(Globals.listeVorlesungen[i].getVorlesungBezeichnung());
                    Console.WriteLine(Globals.listeVorlesungen[i].getVorlesungProfessor());
                    Console.WriteLine();
                }
            }
        }

        // Prüfung, ob Prof verfügbar ist. Wenn ja, wird die Verfügbarkeit für diese Zeit aus der globalen Liste entfernt 
        public static bool ProfessorVerfügbar(int p, string zeit)
        {
            if (zeit == "Mo1" && Globals.listeProfessoren[p].getProfMo1() == "x")
            {
                Globals.listeProfessoren[p].setProfMo1("");
                return true;
            }
            else if (zeit == "Mo2" && Globals.listeProfessoren[p].getProfMo2() == "x")
            {
                Globals.listeProfessoren[p].setProfMo2("");
                return true;
            }
            else if (zeit == "Mo3" && Globals.listeProfessoren[p].getProfMo3() == "x")
            {
                Globals.listeProfessoren[p].setProfMo3("");
                return true;
            }
            else if (zeit == "Mo4" && Globals.listeProfessoren[p].getProfMo4() == "x")
            {
                Globals.listeProfessoren[p].setProfMo4("");
                return true;
            }
            else if (zeit == "Mo5" && Globals.listeProfessoren[p].getProfMo5() == "x")
            {
                Globals.listeProfessoren[p].setProfMo5("");
                return true;
            }
            else if (zeit == "Di1" && Globals.listeProfessoren[p].getProfDi1() == "x")
            {
                Globals.listeProfessoren[p].setProfDi1("");
                return true;
            }
            else if (zeit == "Di2" && Globals.listeProfessoren[p].getProfDi2() == "x")
            {
                Globals.listeProfessoren[p].setProfDi2("");
                return true;
            }
            else if (zeit == "Di3" && Globals.listeProfessoren[p].getProfDi3() == "x")
            {
                Globals.listeProfessoren[p].setProfDi3("");
                return true;
            }
            else if (zeit == "Di4" && Globals.listeProfessoren[p].getProfDi4() == "x")
            {
                Globals.listeProfessoren[p].setProfDi4("");
                return true;
            }
            else if (zeit == "Di5" && Globals.listeProfessoren[p].getProfDi5() == "x")
            {
                Globals.listeProfessoren[p].setProfDi5("");
                return true;
            }
            else if (zeit == "Mi1" && Globals.listeProfessoren[p].getProfMi1() == "x")
            {
                Globals.listeProfessoren[p].setProfMi1("");
                return true;
            }
            else if (zeit == "Mi2" && Globals.listeProfessoren[p].getProfMi2() == "x")
            {
                Globals.listeProfessoren[p].setProfMi2("");
                return true;
            }
            else if (zeit == "Mi3" && Globals.listeProfessoren[p].getProfMi3() == "x")
            {
                Globals.listeProfessoren[p].setProfMi3("");
                return true;
            }
            else if (zeit == "Mi4" && Globals.listeProfessoren[p].getProfMi4() == "x")
            {
                Globals.listeProfessoren[p].setProfMi4("");
                return true;
            }
            else if (zeit == "Mi5" && Globals.listeProfessoren[p].getProfMi5() == "x")
            {
                Globals.listeProfessoren[p].setProfMi5("");
                return true;
            }
            else if (zeit == "Do1" && Globals.listeProfessoren[p].getProfDo1() == "x")
            {
                Globals.listeProfessoren[p].setProfDo1("");
                return true;
            }
            else if (zeit == "Do2" && Globals.listeProfessoren[p].getProfDo2() == "x")
            {
                Globals.listeProfessoren[p].setProfDo2("");
                return true;
            }
            else if (zeit == "Do3" && Globals.listeProfessoren[p].getProfDo3() == "x")
            {
                Globals.listeProfessoren[p].setProfDo3("");
                return true;
            }
            else if (zeit == "Do4" && Globals.listeProfessoren[p].getProfDo4() == "x")
            {
                Globals.listeProfessoren[p].setProfDo4("");
                return true;
            }
            else if (zeit == "Do5" && Globals.listeProfessoren[p].getProfDo5() == "x")
            {
                Globals.listeProfessoren[p].setProfDo5("");
                return true;
            }
            else if (zeit == "Fr1" && Globals.listeProfessoren[p].getProfFr1() == "x")
            {
                Globals.listeProfessoren[p].setProfFr1("");
                return true;
            }
            else if (zeit == "Fr2" && Globals.listeProfessoren[p].getProfFr2() == "x")
            {
                Globals.listeProfessoren[p].setProfFr2("");
                return true;
            }
            else if (zeit == "Fr3" && Globals.listeProfessoren[p].getProfFr3() == "x")
            {
                Globals.listeProfessoren[p].setProfFr3("");
                return true;
            }
            else if (zeit == "Fr4" && Globals.listeProfessoren[p].getProfFr4() == "x")
            {
                Globals.listeProfessoren[p].setProfFr4("");
                return true;
            }
            else if (zeit == "Fr5" && Globals.listeProfessoren[p].getProfFr5() == "x")
            {
                Globals.listeProfessoren[p].setProfFr5("");
                return true;
            }
            else
            {
                return false;
            }
        }

        // Prüfung, ob Raum verfügbar ist. Mit 3. Parameter = true, wird die Verfügbarkeit für diese Zeit aus der globalen Liste Räume entfernt 
        public static bool RaumVerfügbar(int r, string zeit, bool ändern)
        {
            if (zeit == "Mo1" && Globals.listeRäume[r].getRaumMo1() == "x")
            {
                if (ändern == true)
                {
                    Globals.listeRäume[r].setRaumMo1("");
                }
                return true;
            }
            else if (zeit == "Mo2" && Globals.listeRäume[r].getRaumMo2() == "x")
            {
                if (ändern == true)
                {
                    Globals.listeRäume[r].setRaumMo2("");
                }
                return true;
            }
            else if (zeit == "Mo3" && Globals.listeRäume[r].getRaumMo3() == "x")
            {
                if (ändern == true)
                {
                    Globals.listeRäume[r].setRaumMo3("");
                }
                return true;
            }
            else if (zeit == "Mo4" && Globals.listeRäume[r].getRaumMo4() == "x")
            {
                if (ändern == true)
                {
                    Globals.listeRäume[r].setRaumMo4("");
                }
                return true;
            }
            else if (zeit == "Mo5" && Globals.listeRäume[r].getRaumMo5() == "x")
            {
                if (ändern == true)
                {
                    Globals.listeRäume[r].setRaumMo5("");
                }
                return true;
            }
            else if (zeit == "Di1" && Globals.listeRäume[r].getRaumDi1() == "x")
            {
                if (ändern == true)
                {
                    Globals.listeRäume[r].setRaumDi1("");
                }
                return true;
            }
            else if (zeit == "Di2" && Globals.listeRäume[r].getRaumDi2() == "x")
            {
                if (ändern == true)
                {
                    Globals.listeRäume[r].setRaumDi2("");
                }
                return true;
            }
            else if (zeit == "Di3" && Globals.listeRäume[r].getRaumDi3() == "x")
            {
                if (ändern == true)
                {
                    Globals.listeRäume[r].setRaumDi3("");
                }
                return true;
            }
            else if (zeit == "Di4" && Globals.listeRäume[r].getRaumDi4() == "x")
            {
                if (ändern == true)
                {
                    Globals.listeRäume[r].setRaumDi4("");
                }
                return true;
            }
            else if (zeit == "Di5" && Globals.listeRäume[r].getRaumDi5() == "x")
            {
                if (ändern == true)
                {
                    Globals.listeRäume[r].setRaumDi5("");
                }
                return true;
            }
            else if (zeit == "Mi1" && Globals.listeRäume[r].getRaumMi1() == "x")
            {
                if (ändern == true)
                {
                    Globals.listeRäume[r].setRaumMi1("");
                }
                return true;
            }
            else if (zeit == "Mi2" && Globals.listeRäume[r].getRaumMi2() == "x")
            {
                if (ändern == true)
                {
                    Globals.listeRäume[r].setRaumMi2("");
                }
                return true;
            }
            else if (zeit == "Mi3" && Globals.listeRäume[r].getRaumMi3() == "x")
            {
               if (ändern == true)
                {
                     Globals.listeRäume[r].setRaumMi3("");
                }
                return true;
            }
            else if (zeit == "Mi4" && Globals.listeRäume[r].getRaumMi4() == "x")
            {
                if (ändern == true)
                {
                    Globals.listeRäume[r].setRaumMi4("");
                }
                return true;
            }
            else if (zeit == "Mi5" && Globals.listeRäume[r].getRaumMi5() == "x")
            {
                if (ändern == true)
                {
                    Globals.listeRäume[r].setRaumMi5("");
                }
                return true;
            }
            else if (zeit == "Do1" && Globals.listeRäume[r].getRaumDo1() == "x")
            {
                if (ändern == true)
                {
                    Globals.listeRäume[r].setRaumDo1("");
                }
                return true;
            }
            else if (zeit == "Do2" && Globals.listeRäume[r].getRaumDo2() == "x")
            {
                if (ändern == true)
                {
                    Globals.listeRäume[r].setRaumDo2("");
                }
                return true;
            }
            else if (zeit == "Do3" && Globals.listeRäume[r].getRaumDo3() == "x")
            {
                if (ändern == true)
                {
                    Globals.listeRäume[r].setRaumDo3("");
                }
                return true;
            }
            else if (zeit == "Do4" && Globals.listeRäume[r].getRaumDo4() == "x")
            {
                if (ändern == true)
                {
                    Globals.listeRäume[r].setRaumDo4("");
                }
                return true;
            }
            else if (zeit == "Do5" && Globals.listeRäume[r].getRaumDo5() == "x")
            {
                if (ändern == true)
                {
                    Globals.listeRäume[r].setRaumDo5("");
                }
                return true;
            }
            else if (zeit == "Fr1" && Globals.listeRäume[r].getRaumFr1() == "x")
            {
                if (ändern == true)
                {
                    Globals.listeRäume[r].setRaumFr1("");
                }
                return true;
            }
            else if (zeit == "Fr2" && Globals.listeRäume[r].getRaumFr2() == "x")
            {
                if (ändern == true)
                {
                    Globals.listeRäume[r].setRaumFr2("");
                }
                return true;
            }
            else if (zeit == "Fr3" && Globals.listeRäume[r].getRaumFr3() == "x")
            {
                if (ändern == true)
                {
                    Globals.listeRäume[r].setRaumFr3("");
                }
                return true;
            }
            else if (zeit == "Fr4" && Globals.listeRäume[r].getRaumFr4() == "x")
            {
                if (ändern == true)
                {
                    Globals.listeRäume[r].setRaumFr4("");
                }
                return true;
            }
            else if (zeit == "Fr5" && Globals.listeRäume[r].getRaumFr5() == "x")
            {
                if (ändern == true)
                {
                    Globals.listeRäume[r].setRaumFr5("");
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public static int EinlesenFachbereiche(string datei)
        {
            var file = new System.IO.StreamReader(datei);
            string lese;
            int i = 0;
            while ((lese = file.ReadLine()) != null)
            {
                // 1.Zeile der Datei wird übersprungen, da Überschriftenzeile 
                if (i > 0)
                {
                    Fachbereich fachbereich = new Fachbereich();
                    string[] leseFachbereiche = lese.Split(";");
                    fachbereich.setFachbereichName(leseFachbereiche[0]);
                    fachbereich.setFachbereichBez(leseFachbereiche[1]);
                    fachbereich.setFachbereichStudenten(Int32.Parse(leseFachbereiche[2]));
                    Globals.listeFachbereiche.Add(fachbereich);
                    i++;
                }
                else
                {
                    i++;
                }
            }
            file.Close();
            // -1 wegen Überschriftenzeile 
            return i-1;
        }

        public static int EinlesenProfessoren(string datei)
        {
            var file = new System.IO.StreamReader(datei);
            string lese;
            int i = 0;
            while ((lese = file.ReadLine()) != null)
            {
                // 1.Zeile der Datei wird übersprungen, da Überschriftenzeile
                if (i > 0)
                {
                    Professor prof = new Professor();
                    string[] leseProfs = lese.Split(";");
                    prof.setProfName(leseProfs[0]);
                    prof.setProfMo1(leseProfs[1]);
                    prof.setProfMo2(leseProfs[2]);
                    prof.setProfMo3(leseProfs[3]);
                    prof.setProfMo4(leseProfs[4]);
                    prof.setProfMo5(leseProfs[5]);
                    prof.setProfDi1(leseProfs[6]);
                    prof.setProfDi2(leseProfs[7]);
                    prof.setProfDi3(leseProfs[8]);
                    prof.setProfDi4(leseProfs[9]);
                    prof.setProfDi5(leseProfs[10]);
                    prof.setProfMi1(leseProfs[11]);
                    prof.setProfMi2(leseProfs[12]);
                    prof.setProfMi3(leseProfs[13]);
                    prof.setProfMi4(leseProfs[14]);
                    prof.setProfMi5(leseProfs[15]);
                    prof.setProfDo1(leseProfs[16]);
                    prof.setProfDo2(leseProfs[17]);
                    prof.setProfDo3(leseProfs[18]);
                    prof.setProfDo4(leseProfs[19]);
                    prof.setProfDo5(leseProfs[20]);
                    prof.setProfFr1(leseProfs[21]);
                    prof.setProfFr2(leseProfs[22]);
                    prof.setProfFr3(leseProfs[23]);
                    prof.setProfFr4(leseProfs[24]);
                    prof.setProfFr5(leseProfs[25]);
                    Globals.listeProfessoren.Add(prof);
                    i++;
                }
                else
                {
                    i++;
                }
            }
            file.Close();
            // -1 wegen Überschriftenzeile 
            return i-1;
        }
        public static int EinlesenRäume(string datei)
        {
            var file = new System.IO.StreamReader(datei);
            string lese;
            int i = 0;
            while ((lese = file.ReadLine()) != null)
            {
                // 1.Zeile der Datei wird übersprungen, da Überschriftenzeile 
                if (i > 0)
                {
                    Raum raum = new Raum();
                    string[] leseRäume = lese.Split(";");
                    raum.setRaumName(leseRäume[0]);
                    raum.setRaumPlätze(Int32.Parse(leseRäume[1]));
                    raum.setRaumMo1(leseRäume[2]);
                    raum.setRaumMo2(leseRäume[3]);
                    raum.setRaumMo3(leseRäume[4]);
                    raum.setRaumMo4(leseRäume[5]);
                    raum.setRaumMo5(leseRäume[6]);
                    raum.setRaumDi1(leseRäume[7]);
                    raum.setRaumDi2(leseRäume[8]);
                    raum.setRaumDi3(leseRäume[9]);
                    raum.setRaumDi4(leseRäume[10]);
                    raum.setRaumDi5(leseRäume[11]);
                    raum.setRaumMi1(leseRäume[12]);
                    raum.setRaumMi2(leseRäume[13]);
                    raum.setRaumMi3(leseRäume[14]);
                    raum.setRaumMi4(leseRäume[15]);
                    raum.setRaumMi5(leseRäume[16]);
                    raum.setRaumDo1(leseRäume[17]);
                    raum.setRaumDo2(leseRäume[18]);
                    raum.setRaumDo3(leseRäume[19]);
                    raum.setRaumDo4(leseRäume[20]);
                    raum.setRaumDo5(leseRäume[21]);
                    raum.setRaumFr1(leseRäume[22]);
                    raum.setRaumFr2(leseRäume[23]);
                    raum.setRaumFr3(leseRäume[24]);
                    raum.setRaumFr4(leseRäume[25]);
                    raum.setRaumFr5(leseRäume[26]);
                    raum.setRaumBeamer(leseRäume[27]);
                    raum.setRaumWhiteboard(leseRäume[28]);
                    Globals.listeRäume.Add(raum);
                    i++;
                }
                else
                {
                    i++;
                }
            }
            file.Close();
            // -1 wegen Überschriftenzeile 
            return i-1;
        }

        public static int EinlesenVorlesungen(string datei)
        {
            var file = new System.IO.StreamReader(datei);
            string lese;
            int i = 0;
            while ((lese = file.ReadLine()) != null)
            {
                // 1.Zeile der Datei wird übersprungen, da Überschriftenzeile 
                if (i > 0)
                {
                    Vorlesung vorlesung = new Vorlesung();
                    string[] leseVorlesungen = lese.Split(";");
                    vorlesung.setVorlesungNr(leseVorlesungen[0]);
                    vorlesung.setVorlesungFachbereich(leseVorlesungen[1]);
                    vorlesung.setVorlesungSemester(Int32.Parse(leseVorlesungen[2]));
                    vorlesung.setVorlesungBezeichnung(leseVorlesungen[3]);
                    vorlesung.setVorlesungBeschreibung(leseVorlesungen[4]);
                    vorlesung.setVorlesungProfessor(leseVorlesungen[5]);
                    vorlesung.setVorlesungPlätze(Int32.Parse(leseVorlesungen[6]));
                    Globals.listeVorlesungen.Add(vorlesung);
                    i++;
                }
                else
                {
                    i++;
                }
            }
            file.Close();
            // -1 wegen Überschriftenzeile 
            return i-1;
        }

        public static int EinlesenZeiten(string datei)
        {
            var file = new System.IO.StreamReader(datei);
            string lese;
            int i = 0;
            while ((lese = file.ReadLine()) != null)
            {
                // 1.Zeile der Datei wird übersprungen, da Überschriftenzeile 
                if (i > 0)
                {
                    Zeit zeit = new Zeit();
                    string[] leseZeiten = lese.Split(";");
                    zeit.setZeitName(leseZeiten[0]);
                    zeit.setZeitWochentag(leseZeiten[1]);
                    zeit.setZeitBeginn(leseZeiten[2]);
                    Globals.listeZeiten.Add(zeit);
                    i++;
                }
                else
                {
                    i++;
                }
            }
            file.Close();
            // -1 wegen Überschriftenzeile 
            return i-1;
        }

        public static int EinlesenWpv(string datei)
        {
            var file = new System.IO.StreamReader(datei);
            string lese;
            int i = 0;
            while ((lese = file.ReadLine()) != null)
            {
                // 1.Zeile der Datei wird übersprungen, da Überschriftenzeile 
                if (i > 0)
                {
                    Wpv wpv = new Wpv();
                    string[] leseWpv = lese.Split(";");
                    wpv.setWpvZeit(leseWpv[0]);
                    wpv.setWpvWochentag(leseWpv[1]);
                    wpv.setWpvBeginn(leseWpv[2]);
                    wpv.setWpvBezeichnung(leseWpv[3]);
                    wpv.setWpvProfessor(leseWpv[4]);
                    Globals.listeWpv.Add(wpv);
                    i++;
                }
                else
                {
                    i++;
                }
            }
            file.Close();
            // -1 wegen Überschriftenzeile
            return i-1;
        }
    }
}
