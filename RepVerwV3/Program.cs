// Importieren der benötigten .NET-Bibliotheken
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

// Definition des Namensraums
namespace ReparaturVerwaltung
{
    // Klasse zur Darstellung eines Geräts
    public class Geraet
    {
        public int Id { get; set; } = 0; // Eindeutige ID für jedes Gerät
        public string Name { get; set; } = string.Empty; // Name des Geräts
        public string Seriennummer { get; set; } = string.Empty; // Seriennummer des Geräts
    }

    // Klasse zur Darstellung eines Reparaturauftrags
    public class Reparaturauftrag
    {
        public int AuftragId { get; set; } // Eindeutige ID des Auftrags
        public Geraet Geraet { get; set; } = new Geraet(); // Zugehöriges Gerät
        public string Fehlerbeschreibung { get; set; } = string.Empty; // Fehlertext
        public string Status { get; set; } = "Offen"; // Standardstatus beim Erstellen
        public DateTime Erstellungsdatum { get; set; } = DateTime.Now; // Zeitpunkt der Erstellung
    }

    class Program
    {
        static List<Geraet> geraeteListe = new List<Geraet>(); // Alle Geräte
        static List<Reparaturauftrag> auftragsListe = new List<Reparaturauftrag>(); // Alle Aufträge
        static int geraetCounter = 1; // Laufnummer für neue Geräte
        static int auftragCounter = 1; // Laufnummer für neue Aufträge
        static string geraetePfad = "geraete.txt"; // Dateipfad für Geräte
        static string auftraegePfad = "auftraege.txt"; // Dateipfad für Aufträge

        // Statuszuordnungen
        static readonly Dictionary<int, string> statusMap = new()
        {
            { 1, "Offen" },
            { 2, "In Bearbeitung" },
            { 3, "Warten auf Ersatzteil" },
            { 4, "Abgeschlossen" }
        };

        static void Main(string[] args)
        {
            IntroScreen();      // Startanzeige
            DatenLaden();       // Geräte und Aufträge aus Datei einlesen
            Hauptmenue();       // Menü öffnen
            DatenSpeichern();   // Änderungen speichern
        }

        static void IntroScreen()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;

            string[] logo = new string[]
            {
                "  .----------------. .----------------. .----------------. .----------------.   .----------------.   .----------------. .----------------. .----------------. .-----------------.",
                " | .--------------. | .--------------. | .--------------. | .--------------. | | .--------------. | | .--------------. | .--------------. | .--------------. | .--------------. |",
                " | |   ______     | | |  ____  ____  | | |  _________   | | |  _________   | | | |    ___       | | | |   ______     | | |  _________   | | |      __      | | | ____  _____  | |",
                " | |  |_   _ \\    | | | |_  _||_  _| | | | |  _   _  |  | | | |_   ___  |  | | | |  .' _ '.     | | | |  |_   _ \\    | | | |_   ___  |  | | |     /  \\     | | ||_   \\|_   _| | |",
                " | |    | |_) |   | | |   \\ \\  / /   | | | |_/ | | \\_|  | | |   | |_  \\_|  | | | |  | (_) '___  | | | |    | |_) |   | | |   | |_  \\_|  | | |    / /\\ \\    | | |  |   \\ | |   | |",
                " | |    |  __'.   | | |    \\ \\/ /    | | |     | |      | | |   |  _|  _   | | | |  .`___'/ _/  | | | |    |  __'.   | | |   |  _|  _   | | |   / ____ \\   | | |  | |\\ \\| |   | |",
                " | |   _| |__) |  | | |    _|  |_    | | |    _| |_     | | |  _| |___/ |  | | | | | (___)  \\_  | | | |   _| |__) |  | | |  _| |___/ |  | | | _/ /    \\ \\_ | | | _| |_\\   |_  | |",
                " | |  |_______/   | | |   |______|   | | |   |_____|    | | | |_________|  | | | | `._____.\\__| | | | |  |_______/   | | | |_________|  | | ||____|  |____|| | ||_____|\\____| | |",
                " | |              | | |              | | |              | | |              | | | |              | | | |              | | |              | | |              | | |              | |",
                " | '--------------' | '--------------' | '--------------' | '--------------' | | '--------------' | | '--------------' | '--------------' | '--------------' | '--------------' |",
                "  '----------------' '----------------' '----------------' '----------------'   '----------------'   '----------------' '----------------' '----------------' '----------------' "
            };

            foreach (var line in logo)
                Console.WriteLine(line);

            Console.ResetColor();
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write("Initialisiere System");
            for (int i = 0; i < 3; i++) { Console.Write("."); Thread.Sleep(400); }
            Console.WriteLine("\n");

            Console.Write("Lade Module: ");
            int max = 40;
            for (int i = 0; i <= max; i++)
            {
                if (i < max / 3) Console.ForegroundColor = ConsoleColor.Blue;
                else if (i < 2 * max / 3) Console.ForegroundColor = ConsoleColor.Cyan;
                else Console.ForegroundColor = ConsoleColor.Green;

                string bar = "[" + new string('█', i) + new string(' ', max - i) + $"] {i * 100 / max}%";
                Console.Write("\r" + bar);
                Thread.Sleep(60);
            }

            Console.ResetColor();
            Console.WriteLine("\n\nSystemstart abgeschlossen. Made by Vinzent, Benjamin und Joshua.");
            Console.Beep(880, 150);
            Console.Beep(660, 150);
            Console.Beep(440, 200);
            Thread.Sleep(1000);
        }

        static void Hauptmenue()
        {
            while (true) // Endlosschleife bis "Beenden" gewählt wird
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("===== Byte & Bean Reparaturverwaltung =====\n");
                Console.ResetColor();
                
                // Menüoptionen anzeigen
                Console.WriteLine("1. Gerät erfassen");
                Console.WriteLine("2. Reparaturauftrag erstellen");
                Console.WriteLine("3. Auftragsstatus anzeigen");
                Console.WriteLine("4. Bericht abgeschlossener Reparaturen anzeigen");
                Console.WriteLine("5. Gerät löschen");
                Console.WriteLine("6. Auftrag löschen");
                Console.WriteLine("7. Beenden\n");
                Console.Write("Auswahl: ");
                
                // Eingabe auswerten und entsprechende Methode aufrufen
                switch (Console.ReadLine())
                {
                    case "1": GeraetErfassen(); break;
                    case "2": ReparaturauftragErstellen(); break;
                    case "3": StatusAnzeigen(); break;
                    case "4": BerichtErstellen(); break;
                    case "5": GeraetLoeschen(); break;
                    case "6": AuftragLoeschen(); break;
                    case "7": return; // Schleife verlassen → Programmende
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Ungültige Eingabe. Drücken Sie eine Taste.");
                        Console.ResetColor();
                        Console.ReadKey();
                        break;
                }
            }
        }

        static void GeraetErfassen()
        {
            do
            {
                // Eingabeformular für ein neues Gerät
                Console.Clear();
                Console.WriteLine("=== Neues Gerät erfassen ===\n");
                Console.Write("Gerätename: ");
                string? name = Console.ReadLine();
                Console.Write("Seriennummer: ");
                string? sn = Console.ReadLine();
                
                // Gerät zur Liste hinzufügen mit automatischer ID
                geraeteListe.Add(new Geraet
                {
                    Id = geraetCounter++,
                    Name = name ?? string.Empty,
                    Seriennummer = sn ?? string.Empty
                });

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\nGerät erfolgreich erfasst!");
                Console.ResetColor();
            } while (WiederholenOderZurueck()); // Wiederholung ermöglichen
        }

        static void ReparaturauftragErstellen()
        {
            do
            {
                Console.Clear();
                Console.WriteLine("=== Reparaturauftrag erstellen ===\n");
                
                // Wenn keine Geräte vorhanden sind, abbrechen
                if (!geraeteListe.Any())
                {
                    Console.WriteLine("Keine Geräte vorhanden.");
                    Console.ReadKey();
                    return;
                }
                
                // Geräteübersicht anzeigen
                foreach (var g in geraeteListe)
                    Console.WriteLine($"{g.Id}: {g.Name} (SN: {g.Seriennummer})");
                
                // Geräte-ID auswählen
                Console.Write("\nGeräte-ID: ");
                if (!int.TryParse(Console.ReadLine(), out int id) || !geraeteListe.Any(g => g.Id == id))
                {
                    Console.WriteLine("Ungültige Eingabe.");
                    Console.ReadKey();
                    return;
                }

                var geraet = geraeteListe.First(g => g.Id == id); // Gerät finden

                Console.Write("Fehlerbeschreibung: ");
                string? fehler = Console.ReadLine();
                
                // Reparaturauftrag anlegen und speichern
                auftragsListe.Add(new Reparaturauftrag
                {
                    AuftragId = auftragCounter++,
                    Geraet = geraet,
                    Fehlerbeschreibung = fehler ?? string.Empty
                });

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\nReparaturauftrag erstellt!");
                Console.ResetColor();
            } while (WiederholenOderZurueck());
        }

        static void StatusAnzeigen()
        {
            do
            {
                Console.Clear();
                Console.WriteLine("=== Auftragsstatus ===\n");
                
                // Alle Aufträge anzeigen, farbig je nach Status
                foreach (var a in auftragsListe)
                {
                    Console.ForegroundColor = a.Status switch
                    {
                        "Offen" => ConsoleColor.Red,
                        "In Bearbeitung" => ConsoleColor.Yellow,
                        "Warten auf Ersatzteil" => ConsoleColor.DarkYellow,
                        "Abgeschlossen" => ConsoleColor.Green,
                        _ => ConsoleColor.White
                    };

                    Console.WriteLine($"#{a.AuftragId} | Gerät: {a.Geraet.Name} | Fehler: {a.Fehlerbeschreibung} | Status: {a.Status}");
                    Console.ResetColor();
                }

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write("\nAuftrag bearbeiten? (j/n): ");
                Console.ResetColor();
                
                // Wenn ja → Status ändern
                if (Console.ReadLine()?.ToLower() == "j")
                {
                    Console.Write("Auftrags-ID: ");
                    if (int.TryParse(Console.ReadLine(), out int id) && auftragsListe.Any(a => a.AuftragId == id))
                    {
                        var auftrag = auftragsListe.First(a => a.AuftragId == id);
                        Console.WriteLine("\nNeuen Status wählen:");
                        
                        // Statusliste anzeigen
                        foreach (var s in statusMap)
                        {
                            Console.ForegroundColor = s.Key switch
                            {
                                1 => ConsoleColor.Red,
                                2 => ConsoleColor.Yellow,
                                3 => ConsoleColor.DarkYellow,
                                4 => ConsoleColor.Green,
                                _ => ConsoleColor.White
                            };
                            Console.WriteLine($"{s.Key}. {s.Value}");
                        }
                        Console.ResetColor();

                        Console.Write("Statusnummer: ");
                        if (int.TryParse(Console.ReadLine(), out int newStatus) && statusMap.ContainsKey(newStatus))
                        {
                            auftrag.Status = statusMap[newStatus]; // Neuen Status setzen
                            Console.WriteLine("Status geändert!");
                        }
                    }
                }
            } while (WiederholenOderZurueck());
        }

        static void BerichtErstellen()
        {
            Console.Clear();
            Console.WriteLine("=== Abgeschlossene Reparaturen ===\n");
            
            // Nur abgeschlossene Aufträge herausfiltern
            var abgeschlossen = auftragsListe.Where(a => a.Status == "Abgeschlossen").ToList();

            if (!abgeschlossen.Any())
            {
                Console.WriteLine("Keine abgeschlossenen Reparaturen.");
            }
            else
            {
                foreach (var a in abgeschlossen)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"#{a.AuftragId} | Gerät: {a.Geraet.Name} | Fehler: {a.Fehlerbeschreibung} | Fertig am: {a.Erstellungsdatum:dd.MM.yyyy HH:mm}");
                    Console.ResetColor();
                }
            }

            Console.WriteLine("\nDrücke eine Taste zum Zurückkehren...");
            Console.ReadKey();
        }

        static void GeraetLoeschen()
        {
            Console.Clear();
            Console.WriteLine("=== Gerät löschen ===\n");
            
            // Alle Geräte anzeigen
            foreach (var g in geraeteListe)
                Console.WriteLine($"{g.Id}: {g.Name} (SN: {g.Seriennummer})");

            Console.Write("\nGeräte-ID zum Löschen: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var g = geraeteListe.FirstOrDefault(x => x.Id == id);
                if (g != null && !auftragsListe.Any(a => a.Geraet.Id == id)) // Nur wenn kein Auftrag mit Gerät verknüpft ist
                {
                    geraeteListe.Remove(g);
                    Console.WriteLine("Gerät gelöscht.");
                }
                else Console.WriteLine("Nicht löschbar (existiert nicht oder mit Auftrag verknüpft).");
            }
            Console.ReadKey();
        }

        static void AuftragLoeschen()
        {
            Console.Clear();
            Console.WriteLine("=== Auftrag löschen ===\n");
            
            // Aufträge auflisten
            foreach (var a in auftragsListe)
                Console.WriteLine($"{a.AuftragId}: Gerät: {a.Geraet.Name}, Fehler: {a.Fehlerbeschreibung}");

            Console.Write("\nAuftrags-ID zum Löschen: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var a = auftragsListe.FirstOrDefault(x => x.AuftragId == id);
                if (a != null)
                {
                    auftragsListe.Remove(a);
                    Console.WriteLine("Auftrag gelöscht.");
                }
            }
            Console.ReadKey();
        }

        static bool WiederholenOderZurueck()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("\nWeitere Einträge hinzufügen? (j/n): ");
            Console.ResetColor();
            return Console.ReadLine()?.ToLower() == "j"; // true = wiederholen
        }

        static void DatenSpeichern()
        {   
            // Alle Geräte in Datei schreiben
            using (var w = new StreamWriter(geraetePfad))
                foreach (var g in geraeteListe)
                    w.WriteLine($"{g.Id};{g.Name};{g.Seriennummer}");
            
            // Alle Aufträge in Datei schreiben
            using (var w = new StreamWriter(auftraegePfad))
                foreach (var a in auftragsListe)
                    w.WriteLine($"{a.AuftragId};{a.Geraet.Id};{a.Fehlerbeschreibung};{a.Status};{a.Erstellungsdatum}");
        }

        static void DatenLaden()
        {   
            // Gerätedatei einlesen
            if (File.Exists(geraetePfad))
                foreach (var line in File.ReadAllLines(geraetePfad))
                {
                    var p = line.Split(';');
                    geraeteListe.Add(new Geraet { Id = int.Parse(p[0]), Name = p[1], Seriennummer = p[2] });
                }
            
            // Auftragsdatei einlesen
            if (File.Exists(auftraegePfad))
                foreach (var line in File.ReadAllLines(auftraegePfad))
                {
                    var p = line.Split(';');
                    var geraet = geraeteListe.FirstOrDefault(g => g.Id == int.Parse(p[1]));
                    if (geraet != null)
                    {
                        auftragsListe.Add(new Reparaturauftrag
                        {
                            AuftragId = int.Parse(p[0]),
                            Geraet = geraet,
                            Fehlerbeschreibung = p[2],
                            Status = p[3],
                            Erstellungsdatum = DateTime.Parse(p[4])
                        });
                    }
                }
            
            // Zähler aktualisieren
            if (geraeteListe.Any()) geraetCounter = geraeteListe.Max(g => g.Id) + 1;
            if (auftragsListe.Any()) auftragCounter = auftragsListe.Max(a => a.AuftragId) + 1;
        }
    }
}
