using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography.X509Certificates;

namespace Vaccination
{
    public class Person
    {
        public string FirstName;
        public string LastName;
        public string Id;
        public int Age;
        public DateTime Date;
        public bool HealthCareEmpeloyee;
        public bool RiskGroup;
        public bool Infection;

    }

    public class Program
    {
        public static int NumberOfDoses;

        public static string UnderAgeVaccination;

        public static bool VaccinateChilderen;

        public static string InDataPath = @"C:\Users\Mohamed Jafari\source\repos\Inlämningsuppgift_4\Inlämningsuppgift_4\People.csv";

        public static string OutDataPath = @"C:\demo\vaccination.csv";



        public static void Main()
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            NumberOfDoses = 0;
            UnderAgeVaccination = "Nej";
            bool runing = true;

            while (runing)
            {
                DisplayMenu();


                int options = ShowMenu("Vad vill du göra?", new[]
            {
                    "Skapa prioritetsordning",
                    "Schemalägg vaccinationer",
                    "Ändra antal vaccindoser",
                    "Ändra åldersgräns",
                    "Ändra indatafil",
                    "Ändra utdatafil",
                    "Avsluta"
            });
                Console.Clear();

                if (options == 0)
                {
                    string[] InputLines = File.ReadAllLines(InDataPath);

                    if (File.Exists(OutDataPath))
                    {
                        int overWrite = ShowMenu("Filen existerar redan. Vill du skriva över?", new[]
                        {
                            "Ja",
                            "Nej"
                        });
                        if (overWrite == 0)
                        {
                            CreateVaccinationOrder(InputLines, NumberOfDoses, VaccinateChilderen);

                        }
                        else
                        {
                            Console.WriteLine("Du kan försöka igen!");
                        }
                    }
                    else
                    {

                        CreateVaccinationOrder(InputLines, NumberOfDoses, VaccinateChilderen);
                    }
                    Console.Clear();
                }
                else if (options == 1)
                {
                    Console.WriteLine("Brad Pitt is still working on it...");
                }

                else if (options == 2)
                {
                    ChangeNumberOfDoses();
                    Console.Clear();
                }

                else if (options == 3)
                {
                    UnderAge();
                    Console.Clear();
                }
                else if (options == 4)
                {
                    ChangeFilePath(InDataPath);
                    Console.Clear();
                }
                else if (options == 5)
                {
                    ChangeDirectoryPath(OutDataPath);
                    Console.Clear();
                }
                else
                {
                    runing = false;
                    Environment.Exit(1);
                }
            }

        }

        public static void DisplayMenu()
        {
            Console.WriteLine("Huvudmeny");
            Console.WriteLine("----------");
            Console.WriteLine();
            Console.WriteLine($"Antal tillgängliga vaccindoser: {NumberOfDoses}");
            Console.WriteLine($"Vaccinering under 18 år: {UnderAgeVaccination}");
            Console.WriteLine($"Indatafil: {InDataPath}");
            Console.WriteLine($"Utdatafil: {OutDataPath}");
        }
        public static void UnderAge()
        {
            int selectedAgeLimit = ShowMenu("Ska personer under 18 vaccineras?", new[]
                   {
                        "Ja",
                        "Nej"
                    });

            if (selectedAgeLimit == 0)
            {
                UnderAgeVaccination = "Ja";
                VaccinateChilderen = true;
            }
            else
            {
                UnderAgeVaccination = "Nej";
                VaccinateChilderen = false;
            }
        }
        public static void ChangeNumberOfDoses()
        {
            bool done = false;

            while (!done)
            {
                try
                {
                    Console.Write("Ange antal doser: ");
                    NumberOfDoses = int.Parse(Console.ReadLine());
                    done = true;
                }
                catch
                {
                    Console.WriteLine("Du matade in fel data, försök igen.");

                }
            }
        }
        public static string ChangeFilePath(string path)
        {
            try
            {
                bool done = false;
                Console.Write("Skirv din sökväg:");
                string newDataPath = Console.ReadLine();
                while (!done)
                {
                    if (!File.Exists(newDataPath))
                    {
                        Console.WriteLine($"{newDataPath} kunde inte hittas, försök igen");

                        Console.Write("Skirv din sökväg:");

                        newDataPath = Console.ReadLine();
                    }
                    else
                    {

                        path = newDataPath;
                        done = true;
                    }
                }
            }
            catch (Exception)
            {

                Console.WriteLine("Något gick fel. Försök igen");
            }

            return path;
        }
        public static string ChangeDirectoryPath(string path)
        {


            try
            {
                bool done = false;
                Console.Write("Ange sökväg:");
                path = Console.ReadLine();

                while (!done)
                {
                    if (Directory.Exists(path) || File.Exists(path))
                    {
                        OutDataPath = path;
                        done = true;
                    }
                    else if (!Directory.Exists(path))
                    {
                        Console.WriteLine($"Mappen kunde inte hittas, försök igen");

                        Console.Write("Ange sökväg:");

                        path = Console.ReadLine();

                    }
                }
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine("Mappen kunde inte hittas, vänligen försök igen");
            }
            catch (IOException)
            {

                Console.WriteLine("Ogiltigt namn, vänligen försök igen");
            }
            return path;


        }

        // Create an array of lines that should be saved to a CSV file after creating the vaccination order.
        //
        // Parameters:
        //
        // input: the lines from a CSV file containing population information
        // doses: the number of vaccine doses available
        // vaccinateChildren: whether to vaccinate people younger than 18
        public static string[] CreateVaccinationOrder(string[] input, int doses, bool vaccinateChildren)
        {
            List<Person> people = new List<Person>();

            foreach (string line in input)
            {

                try
                {

                    string[] parts = line.Split(',');
                    string personNummer = parts[0];

                    int year = 0;
                    int month = 0;
                    int day = 0;


                    //Converting 0 and 1 to integer becouse in order to convert them to a boolean variable we need to have int variable.
                    bool healthCare = Convert.ToBoolean(Convert.ToInt16(parts[3]));
                    bool riskGroup = Convert.ToBoolean(Convert.ToInt16(parts[4]));
                    bool infection = Convert.ToBoolean(Convert.ToInt16(parts[5]));


                    if (personNummer.Length <= 10)
                    {
                        int y = int.Parse(personNummer.Substring(0, 2));

                        if (y > 22)
                        {
                            personNummer = "19" + personNummer;
                            year = int.Parse(personNummer.Substring(0, 4));
                            month = int.Parse(personNummer.Substring(4, 2));
                            day = int.Parse(personNummer.Substring(6, 2));

                        }
                        else
                        {
                            personNummer = "20" + personNummer;
                            year = int.Parse(personNummer.Substring(0, 4));
                            month = int.Parse(personNummer.Substring(4, 2));
                            day = int.Parse(personNummer.Substring(6, 2));
                        }

                    }
                    else
                    {
                        year = int.Parse(personNummer.Substring(0, 4));
                        month = int.Parse(personNummer.Substring(4, 2));
                        day = int.Parse(personNummer.Substring(6, 2));

                    }
                    DateTime dateTime = new DateTime(year, month, day);
                    DateTime dateTimeNow = DateTime.Today;




                    Person p = new Person
                    {
                        Age = CalculateAge(dateTime, dateTimeNow),
                        Date = dateTime,
                        LastName = parts[1],
                        FirstName = parts[2],
                        HealthCareEmpeloyee = healthCare,
                        RiskGroup = riskGroup,
                        Infection = infection,
                        Id = parts[0]

                    };
                    people.Add(p);

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);

                }
            }


            people = people.OrderByDescending(p => p.HealthCareEmpeloyee).ThenByDescending(p => p.Age >= 65).ThenByDescending(p => p.RiskGroup).ThenBy(p => p.Date).ToList();

            List<string> peopleLines = new List<string>();

            foreach (var p in people)
            {
                //We created a variable "shareDoses" to divide doses according to every persons condition.
                int shareDoses = 0;


                if (doses > 0)
                {
                    if (vaccinateChildren == false && p.Age <= 18)
                    {
                        shareDoses = 0;
                    }
                    else
                    {
                        if (p.Infection == true)
                        {
                            shareDoses = 1;
                            doses--;
                        }
                        else if (doses == 1 && p.Infection == false)
                        {
                            shareDoses = 0;
                        }
                        else
                        {
                            shareDoses = 2;
                            doses -= 2;
                        }
                    }
                }
                else
                {
                    shareDoses = 0;
                }




                string personNummer = p.Id;

                string firstPartOfId = "";
                string secondPartOfId = "";

                if (personNummer.Length <= 10)
                {
                    int y = int.Parse(personNummer.Substring(0, 2));
                    if (y > 22)
                    {
                        personNummer = "19" + personNummer;
                        firstPartOfId = personNummer.Substring(0, 8);
                        secondPartOfId = personNummer.Substring(8, 4);

                    }
                    else
                    {
                        personNummer = "20" + personNummer;
                        firstPartOfId = personNummer.Substring(0, 8);
                        secondPartOfId = personNummer.Substring(8, 4);
                    }

                }
                else
                {
                    firstPartOfId = p.Id.Substring(0, 8);
                    secondPartOfId = p.Id.Substring(9, 4);
                }




                peopleLines.Add($"{firstPartOfId}-{secondPartOfId},{p.LastName},{p.FirstName},{shareDoses}");
            }

            File.WriteAllLines(OutDataPath, peopleLines);


           
            return peopleLines.ToArray();
        }

        public static int ShowMenu(string prompt, IEnumerable<string> options)
        {
            if (options == null || options.Count() == 0)
            {
                throw new ArgumentException("Cannot show a menu for an empty list of options.");
            }

            Console.WriteLine(prompt);

            // Hide the cursor that will blink after calling ReadKey.
            Console.CursorVisible = false;

            // Calculate the width of the widest option so we can make them all the same width later.
            int width = options.Max(option => option.Length);

            int selected = 0;
            int top = Console.CursorTop;
            for (int i = 0; i < options.Count(); i++)
            {
                // Start by highlighting the first option.
                if (i == 0)
                {
                    Console.BackgroundColor = ConsoleColor.Blue;
                    Console.ForegroundColor = ConsoleColor.White;
                }

                var option = options.ElementAt(i);
                // Pad every option to make them the same width, so the highlight is equally wide everywhere.
                Console.WriteLine("- " + option.PadRight(width));

                Console.ResetColor();
            }
            Console.CursorLeft = 0;
            Console.CursorTop = top - 1;

            ConsoleKey? key = null;
            while (key != ConsoleKey.Enter)
            {
                key = Console.ReadKey(intercept: true).Key;

                // First restore the previously selected option so it's not highlighted anymore.
                Console.CursorTop = top + selected;
                string oldOption = options.ElementAt(selected);
                Console.Write("- " + oldOption.PadRight(width));
                Console.CursorLeft = 0;
                Console.ResetColor();

                // Then find the new selected option.
                if (key == ConsoleKey.DownArrow)
                {
                    selected = Math.Min(selected + 1, options.Count() - 1);
                }
                else if (key == ConsoleKey.UpArrow)
                {
                    selected = Math.Max(selected - 1, 0);
                }

                // Finally highlight the new selected option.
                Console.CursorTop = top + selected;
                Console.BackgroundColor = ConsoleColor.Blue;
                Console.ForegroundColor = ConsoleColor.White;
                string newOption = options.ElementAt(selected);
                Console.Write("- " + newOption.PadRight(width));
                Console.CursorLeft = 0;
                // Place the cursor one step above the new selected option so that we can scroll and also see the option above.
                Console.CursorTop = top + selected - 1;
                Console.ResetColor();
            }

            // Afterwards, place the cursor below the menu so we can see whatever comes next.
            Console.CursorTop = top + options.Count();

            // Show the cursor again and return the selected option.
            Console.CursorVisible = true;
            return selected;
        }
        public static int CalculateAge(DateTime birthDate, DateTime now)
        {
            int age = now.Year - birthDate.Year;

            if (now.Month < birthDate.Month || now.Month == birthDate.Month && now.Day < birthDate.Day || birthDate > now.AddYears(-age))
            {
                age--;
            }
            return age;
        }


    }

    [TestClass]
    public class ProgramTests
    {
        [TestMethod]
        public void InfectionTest()
        {
            // Arrange
            string[] input =
            {
                "19720906-1111,Elba,Idris,0,0,1",
                "8102032222,Efternamnsson,Eva,1,1,0"
            };
            int doses = 10;
            bool vaccinateChildren = false;


            // Act
            string[] output = Program.CreateVaccinationOrder(input, doses, vaccinateChildren);

            // Assert
            Assert.AreEqual(output.Length, 2);
            Assert.AreEqual("19810203-2222,Efternamnsson,Eva,2", output[0]);
            Assert.AreEqual("19720906-1111,Elba,Idris,1", output[1]);
        }

        [TestMethod]
        public void VaccinationChildrenTest()
        {
            string[] input =
            {

                "0602032222,Efternamnsson,Eva,1,1,1",
                "20051231-1337,Nazario,Ronaldo,0,0,0"
            };

            int doses = 10;
            bool vaccinateChildren = true;

            string[] output = Program.CreateVaccinationOrder(input, doses, vaccinateChildren);

            Assert.AreEqual(output.Length, 2);
            Assert.AreEqual("20060203-2222,Efternamnsson,Eva,1", output[0]);
            Assert.AreEqual("20051231-1337,Nazario,Ronaldo,2", output[1]);

        }
        [TestMethod]
        public void VaccinationChildrenTest2()
        {
            string[] input =
            {

                "0602032222,Efternamnsson,Eva,1,1,1",
                "20051231-1337,Nazario,Ronaldo,0,0,0"
            };

            int doses = 10;
            bool vaccinatChildren = false;

            string[] output = Program.CreateVaccinationOrder(input, doses, vaccinatChildren);

            Assert.AreEqual(output.Length, 2);
            Assert.AreEqual("20060203-2222,Efternamnsson,Eva,0", output[0]);
            Assert.AreEqual("20051231-1337,Nazario,Ronaldo,0", output[1]);

        }
        [TestMethod]
        public void OldestVaccinationTest()
        {
            string[] input =
            {

                "8102032222,Efternamnsson,Eva,1,0,0",
                "20051231-1337,Nazario,Ronaldo,0,0,0",
                "6503301234,Henke,Larsson,1,0,0"
            };

            int doses = 10;
            bool vaccinatChildren = false;

            string[] output = Program.CreateVaccinationOrder(input, doses, vaccinatChildren);

            Assert.AreEqual(output.Length, 3);
            Assert.AreEqual("19650330-1234,Henke,Larsson,2", output[0]);
            Assert.AreEqual("19810203-2222,Efternamnsson,Eva,2", output[1]);
            Assert.AreEqual("20051231-1337,Nazario,Ronaldo,0", output[2]);


        }
        [TestMethod]
        public void MissingDataTest()
        {
            string[] input =
            {

                "8102032222,Efternamnsson,Eva,0,0,",
                "20051231-1337,Nazario,Ronaldo,0,0,1",
                "6503301234,Henke,Larsson,1,1,0"
            };

            int doses = 10;
            bool vaccinatChildren = true;

            string[] output = Program.CreateVaccinationOrder(input, doses, vaccinatChildren);

            Assert.AreEqual(output.Length, 2);
            Assert.AreEqual("19650330-1234,Henke,Larsson,2", output[0]);
            Assert.AreEqual("20051231-1337,Nazario,Ronaldo,1", output[1]);


        }
        [TestMethod]
        public void Over65YearsOldTest()
        {
            string[] input =
            {

                "5502032222,Efternamnsson,Eva,0,1,0",
                "20051231-1337,Nazario,Ronaldo,0,0,0",
                "6503301234,Henke,Larsson,0,1,0"
            };

            int doses = 10;
            bool vaccinatChildren = true;

            string[] output = Program.CreateVaccinationOrder(input, doses, vaccinatChildren);

            Assert.AreEqual(output.Length, 3);
            Assert.AreEqual("19550203-2222,Efternamnsson,Eva,2", output[0]);
            Assert.AreEqual("19650330-1234,Henke,Larsson,2", output[1]);
            Assert.AreEqual("20051231-1337,Nazario,Ronaldo,2", output[2]);


        }
    }
}