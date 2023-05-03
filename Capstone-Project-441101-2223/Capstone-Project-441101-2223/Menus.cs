using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Capstone_Project_441101_2223
{
    struct DataLoadedFromFile
    {
        public string Type;        
        public float Quantity;
        public int ID;
    }
    static class ConsoleHelpers
    {
        public static int GetIntegerInRange(int pMin, int pMax, string pMessage)
        {
            if (pMin > pMax)
            {
                throw new Exception($"Minimum value {pMin} cannot be greater than maximum value {pMax}");
            }

            int result;

            do
            {
                Console.WriteLine(pMessage);
                Console.WriteLine($"Please enter a number between {pMin} and {pMax} inclusive.");

                string userInput = Console.ReadLine();

                try
                {
                    result = int.Parse(userInput);
                }
                catch
                {
                    Console.WriteLine($"{userInput} is not a number");
                    continue;
                }

                if (result >= pMin && result <= pMax)
                {
                    return result;
                }
                Console.WriteLine($"{result} is not between {pMin} and {pMax} inclusive.");
            } while (true);
        }
        public static float GetNum(string pMessage)
        {
            float result;
            do
            {
                Console.WriteLine(pMessage);
                string userInput = Console.ReadLine();

                try
                {
                    result = float.Parse(userInput);
                }
                catch
                {
                    Console.WriteLine($"{userInput} is not a number");
                    continue;
                }
                if (result <= 0)
                {
                    Console.WriteLine("Number must be larger than 0");
                }
                else
                {
                    return result;
                }

            } while (true);
        }

        public static string GetFileName(string pMessage)
        {
            string filename;
            do
            {
                Console.WriteLine(pMessage);
                filename = Console.ReadLine();
                if (File.Exists(filename))
                {
                    return filename;
                }
                else
                {
                    Console.WriteLine($"{filename} does not exist");
                    continue;
                }

            } while (true);
        }

       
        public static Array GetLines(string filename)
        {                   

            int numberOfLines = 0;
            StreamReader reader = new StreamReader(filename);
            while (!reader.EndOfStream)
            {
                reader.ReadLine();
                numberOfLines++;
            }
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            string[] lines = new string[numberOfLines];
            int lineIndex = 0;
            while (!reader.EndOfStream)
            {
                lines[lineIndex] = reader.ReadLine();
                lineIndex++;
            }
            reader.Close();
            return lines;
        }
        public static bool Test(ProjectManager manager, Array pLine)
        {
            DataLoadedFromFile[] data = new DataLoadedFromFile[pLine.Length];

            char[] splitChars = { ',', '(', ')', '=', ';', ' ' };
            string[] firstStrings = { "Land", "Renovation", "Purchase", "Sale" };
            string[] typeStrings = { "L", "S", "R", "P" };
            
            int lineNumber = 0;
            foreach (string line in pLine)
            {

                string[] separateStrings = line.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);
                
                Console.WriteLine(separateStrings[0]);
                Console.WriteLine(separateStrings[1]);
                Console.WriteLine(separateStrings[2]);



                if (firstStrings.Contains(separateStrings[0])) // format 2
                {
                    data[lineNumber].Type = separateStrings[0];
                    if (!IsInt(separateStrings[1], manager))
                    {
                        return false;
                    }
                    else
                    {
                        data[lineNumber].ID = int.Parse(separateStrings[1]);
                    }
                    
                }
                else if (IsInt(separateStrings[0], manager)) // format 1
                {
                    data[lineNumber].ID = int.Parse(separateStrings[0]);

                    if (!typeStrings.Contains(separateStrings[1]))
                    {
                        return false;
                    }
                    else
                    {
                        data[lineNumber].Type = separateStrings[0];
                    }
                }
                else
                {
                    return false;
                }

                if (!IsFloat(separateStrings[2]))
                {
                    return false;
                }
                else
                {
                    data[lineNumber].Quantity = float.Parse(separateStrings[2]);
                }
                
                lineNumber++;
            }
            AddProjectsFromFile(manager,data);
            return true;
        }

        public static bool DoesProjectExist(int pID, ProjectManager manager)
        {
            foreach (Project project in manager.projects)
            {
                if (project.ID == pID)
                {
                    return true;
                }
            }
            Console.WriteLine($" File could not be loaded as their is a project with the same ID {pID}");
            return false;
        }
        public static bool IsFloat(string pAmount)
        {
            if (float.TryParse(pAmount, out float value))
            {
                return true;
            }
            return false;
        }
        public static bool IsInt(string input, ProjectManager projectManager)
        {
            if (int.TryParse(input, out int value))
            {
                if (DoesProjectExist(value, projectManager))
                {
                    return true;
                }
            }
            return false;
        }

        public static void AddProjectsFromFile(ProjectManager projectManager, DataLoadedFromFile[] data)
        {      

            int i = 0;
            foreach(DataLoadedFromFile loadedFromFile in data)
            {
                if(data[i].Type.Length > 1)
                {
                    switch (data[i].Type)
                    {
                        case "Land":
                            data[i].Type = "L";
                            break;
                        case "Renovation":
                            data[i].Type = "R";
                            break;
                        case "Purchase":
                            data[i].Type = "P";
                            break;
                        case "Sale":
                            data[i].Type = "S";
                            break;
                        default:
                            throw new Exception("Somthing has gone wrong");
                    }
                }

                if (data[i].Type == "L")
                {
                    Project project = new Project(data[i].ID, true);
                    projectManager.AddProject(project);
                    Transactions transactions = new Transactions(data[i].ID, data[i].Quantity, 0, data[i].Type, true);
                    projectManager.AddTransaction(transactions);
                }
                else if (data[i].Type == "R")
                {
                    Project project = new Project(data[i].ID, false);
                    projectManager.AddProject(project);
                    Transactions transactions = new Transactions(data[i].ID, data[i].Quantity, 0, data[i].Type, false);
                    projectManager.AddTransaction(transactions);
                }
                else if (data[i].Type == "P")
                {
                    foreach (Project project in projectManager.projects)
                    {
                        if (project.ID == data[i].ID)
                        {
                            if (project.isLand)
                            {
                                Transactions transactions = new Transactions(data[i].ID, data[i].Quantity, 0, data[i].Type, true);
                                projectManager.AddTransaction(transactions);
                            }
                            else
                            {
                                Transactions transactions = new Transactions(data[i].ID, data[i].Quantity, 0, data[i].Type, false);
                                projectManager.AddTransaction(transactions);
                            }
                            break;
                        }
                    }
                }
                else if (data[i].Type == "S")
                {
                    Transactions transactions = new Transactions(data[i].ID, 0, data[i].Quantity, data[i].Type, false);
                    projectManager.AddTransaction(transactions);
                }
                i++;
            }            
        }

    }
        abstract class ConsoleMenu : MenuItem
        {
            protected List<MenuItem> _menuItems = new List<MenuItem>();

            public bool IsActive { get; set; }

            public abstract void CreateMenu();

            public override void Select()
            {
                IsActive = true;
                do
                {
                    CreateMenu();
                    string output = $"{MenuText()}{Environment.NewLine}";
                    int selection = ConsoleHelpers.GetIntegerInRange(1, _menuItems.Count, this.ToString()) - 1;
                    _menuItems[selection].Select();
                } while (IsActive);
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(MenuText());
                for (int i = 0; i < _menuItems.Count; i++)
                {
                    sb.AppendLine($"{i + 1}. {_menuItems[i].MenuText()}");
                }
                return sb.ToString();
            }
        }

        abstract class MenuItem
        {
            public abstract string MenuText();
            public abstract void Select();


        }

        class ExitMenuItem : MenuItem
        {
            private ConsoleMenu _menu;

            public ExitMenuItem(ConsoleMenu parentItem)
            {
                _menu = parentItem;
            }

            public override string MenuText()
            {
                return "Exit";
            }

            public override void Select()
            {
                _menu.IsActive = false;
            }
        }
    }


