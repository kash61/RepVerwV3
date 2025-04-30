using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace ReparaturVerwaltung
{
    public class Geraet
    {
        public int Id { get; set; } = 0;
        public string Name { get; set; } = string.Empty;
        public string Seriennummer { get; set; } = string.Empty;
    }

    public class Reparaturauftrag
    {
        public int AuftragId { get; set; }
        public Geraet Geraet { get; set; } = new Geraet();
        public string Fehlerbeschreibung { get; set; } = string.Empty;
        public string Status { get; set; } = "Offen";
        public DateTime Erstellungsdatum { get; set; } = DateTime.Now;
    }

    class Program
    {
        static List<Geraet> geraeteListe = new List<Geraet>();
        static List<Reparaturauftrag> auftragsListe = new List<Reparaturauftrag>();
        static int geraetCounter = 1;
        static int auftragCounter = 1;
        static string geraetePfad = "geraete.txt";
        static string auftraegePfad = "auftraege.txt";

        static readonly Dictionary<int, string> statusMap = new()
        {
            { 1, "Offen" },
            { 2, "In Bearbeitung" },
            { 3, "Warten auf Ersatzteil" },
            { 4, "Abgeschlossen" }
        };

        static void Main(string[] args)
        {
            IntroScreen();
            DatenLaden();
            Hauptmenue();
            DatenSpeichern();
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
            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("===== Byte & Bean Reparaturverwaltung =====\n");
                Console.ResetColor();

                Console.WriteLine("1. Gerät erfassen");
                Console.WriteLine("2. Reparaturauftrag erstellen");
                Console.WriteLine("3. Auftragsstatus anzeigen");
                Console.WriteLine("4. Bericht abgeschlossener Reparaturen anzeigen");
                Console.WriteLine("5. Gerät löschen");
                Console.WriteLine("6. Auftrag löschen");
                Console.WriteLine("7. Beenden\n");
                Console.Write("Auswahl: ");

                switch (Console.ReadLine())
                {
                    case "1": GeraetErfassen(); break;
                    case "2": ReparaturauftragErstellen(); break;
                    case "3": StatusAnzeigen(); break;
                    case "4": BerichtErstellen(); break;
                    case "5": GeraetLoeschen(); break;
                    case "6": AuftragLoeschen(); break;
                    case "7": return;
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
                Console.Clear();
                Console.WriteLine("=== Neues Gerät erfassen ===\n");
                Console.Write("Gerätename: ");
                string? name = Console.ReadLine();
                Console.Write("Seriennummer: ");
                string? sn = Console.ReadLine();

                geraeteListe.Add(new Geraet
                {
                    Id = geraetCounter++,
                    Name = name ?? string.Empty,
                    Seriennummer = sn ?? string.Empty
                });

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\nGerät erfolgreich erfasst!");
                Console.ResetColor();
            } while (WiederholenOderZurueck());
        }

        static void ReparaturauftragErstellen()
        {
            do
            {
                Console.Clear();
                Console.WriteLine("=== Reparaturauftrag erstellen ===\n");

                if (!geraeteListe.Any())
                {
                    Console.WriteLine("Keine Geräte vorhanden.");
                    Console.ReadKey();
                    return;
                }

                foreach (var g in geraeteListe)
                    Console.WriteLine($"{g.Id}: {g.Name} (SN: {g.Seriennummer})");

                Console.Write("\nGeräte-ID: ");
                if (!int.TryParse(Console.ReadLine(), out int id) || !geraeteListe.Any(g => g.Id == id))
                {
                    Console.WriteLine("Ungültige Eingabe.");
                    Console.ReadKey();
                    return;
                }

                var geraet = geraeteListe.First(g => g.Id == id);

                Console.Write("Fehlerbeschreibung: ");
                string? fehler = Console.ReadLine();

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

                if (Console.ReadLine()?.ToLower() == "j")
                {
                    Console.Write("Auftrags-ID: ");
                    if (int.TryParse(Console.ReadLine(), out int id) && auftragsListe.Any(a => a.AuftragId == id))
                    {
                        var auftrag = auftragsListe.First(a => a.AuftragId == id);
                        Console.WriteLine("\nNeuen Status wählen:");

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
                            auftrag.Status = statusMap[newStatus];
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

            foreach (var g in geraeteListe)
                Console.WriteLine($"{g.Id}: {g.Name} (SN: {g.Seriennummer})");

            Console.Write("\nGeräte-ID zum Löschen: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var g = geraeteListe.FirstOrDefault(x => x.Id == id);
                if (g != null && !auftragsListe.Any(a => a.Geraet.Id == id))
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
            return Console.ReadLine()?.ToLower() == "j";
        }

        static void DatenSpeichern()
        {
            using (var w = new StreamWriter(geraetePfad))
                foreach (var g in geraeteListe)
                    w.WriteLine($"{g.Id};{g.Name};{g.Seriennummer}");

            using (var w = new StreamWriter(auftraegePfad))
                foreach (var a in auftragsListe)
                    w.WriteLine($"{a.AuftragId};{a.Geraet.Id};{a.Fehlerbeschreibung};{a.Status};{a.Erstellungsdatum}");
        }

        static void DatenLaden()
        {
            if (File.Exists(geraetePfad))
                foreach (var line in File.ReadAllLines(geraetePfad))
                {
                    var p = line.Split(';');
                    geraeteListe.Add(new Geraet { Id = int.Parse(p[0]), Name = p[1], Seriennummer = p[2] });
                }

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

            if (geraeteListe.Any()) geraetCounter = geraeteListe.Max(g => g.Id) + 1;
            if (auftragsListe.Any()) auftragCounter = auftragsListe.Max(a => a.AuftragId) + 1;
        }
    }
}
