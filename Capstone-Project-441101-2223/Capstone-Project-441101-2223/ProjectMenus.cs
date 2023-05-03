using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Capstone_Project_441101_2223
{
    class StartMenu : ConsoleMenu
    {
        private ProjectManager _manager;


        public StartMenu(ProjectManager manager)
        {
            _manager = manager;

        }

        public override void CreateMenu()
        {
            _menuItems.Clear();
            _menuItems.Add(new AddNewProjectMenu(_manager));
            _menuItems.Add(new LoadTransactionsFromFile(_manager));
            if (_manager.projects.Count > 0)
            {
                _menuItems.Add(new SelectExistingProject(_manager));
                _menuItems.Add(new PortSummary(_manager));
            }
            _menuItems.Add(new ExitMenuItem(this));
        }

        public override string MenuText()
        {
            return "Menu";
        }
    }

    class AddNewProjectMenu : ConsoleMenu
    {
        ProjectManager _manager;

        public AddNewProjectMenu(ProjectManager manager)
        {
            _manager = manager;
        }
        public override void CreateMenu()
        {
            _menuItems.Clear();
            _menuItems.Add(new NewBuild(_manager));
            _menuItems.Add(new Renovation(_manager));
            _menuItems.Add(new ExitMenuItem(this));
        }
        public override string MenuText()
        {
            return "Add New Project";
        }

    }

    class LoadTransactionsFromFile : MenuItem
    {
        public ProjectManager _manager;

        public LoadTransactionsFromFile(ProjectManager manager)
        {
            _manager = manager;
        }
        
        public override string MenuText()
        {
            return "Load Transactions From File";
        }
        public override void Select()
        {
            string fileName = ConsoleHelpers.GetFileName("Please enter name of File you want to load");
            Array lines = ConsoleHelpers.GetLines(fileName);

            bool isCorrectFormat = ConsoleHelpers.Test(_manager,lines);
            if (!isCorrectFormat)
            {
                Console.WriteLine($"{fileName} could not be loaded as the format was incorrect");
            }
            else
            {
                Console.WriteLine("File Loaded");
            }
            
            _manager.End();
        }
    }

    class SelectExistingProject : ConsoleMenu
    {
        ProjectManager _manager;

        public SelectExistingProject(ProjectManager manager)
        {
            _manager = manager;
        }
        public override void CreateMenu()
        {
            _menuItems.Clear();

            foreach (Project project in _manager.projects)
            {
                _menuItems.Add(new SelectedProject(_manager, project));
            }

            _menuItems.Add(new ExitMenuItem(this));

        }
        public override string MenuText()
        {
            return "Select Existing Project";
        }
    }
    class SelectedProject : ConsoleMenu
    {
        ProjectManager _manager;
        private Project _project;

        public SelectedProject(ProjectManager manager, Project project)
        {
            _manager = manager;
            _project = project;
        }
        public override void CreateMenu()
        {
            _menuItems.Clear();
            _menuItems.Add(new AddTransaction(_manager, _project));
            _menuItems.Add(new DisplaySales(_manager, _project));
            _menuItems.Add(new DisplayPurchases(_manager, _project));
            _menuItems.Add(new Summary(_manager, _project));
            _menuItems.Add(new DeleteProject(_manager, _project));
            _menuItems.Add(new ExitMenuItem(this));

        }
        public override string MenuText()
        {
            return "Project " + _project.ID.ToString();
        }
    }

    class AddTransaction : ConsoleMenu
    {
        private ProjectManager _manager;
        private Project _project;


        public AddTransaction(ProjectManager projectManager, Project project)
        {
            _manager = projectManager;
            _project = project;

        }
        public override void CreateMenu()
        {
            _menuItems.Clear();
            _menuItems.Add(new AddSale(_manager, _project));
            _menuItems.Add(new AddPurchase(_manager, _project));
            _menuItems.Add(new ExitMenuItem(this));
        }
        public override string MenuText()
        {
            return "Add Transaction";
        }

    }

    class AddSale : MenuItem
    {
        private ProjectManager _manager;
        private Project _project;

        public AddSale(ProjectManager projectManager, Project project)
        {
            _manager = projectManager;
            _project = project;

        }
        public override string MenuText()
        {
            return "Add Sale";
        }
        public override void Select()
        {
            float s = ConsoleHelpers.GetNum("please enter Sale amount");
            if (_project.isLand)
            {
                Transactions transaction = new Transactions(_project.ID, 0, s, "S", true);  // Land Sale
                _manager.AddTransaction(transaction);
            }
            else
            {
                Transactions transaction = new Transactions(_project.ID, 0, s, "S", false);  // Renovation Sale
                _manager.AddTransaction(transaction);
            }

        }
    }
    class AddPurchase : MenuItem
    {
        private ProjectManager _manager;
        private Project _project;

        public AddPurchase(ProjectManager projectManager, Project project)
        {
            _manager = projectManager;
            _project = project;
        }
        public override string MenuText()
        {
            return "Add Purchase";
        }
        public override void Select()
        {
            float Input = ConsoleHelpers.GetNum("please enter Purchase amount");
            if (_project.isLand)
            {
                Transactions transaction = new Transactions(_project.ID, Input, 0, "P", true);  // Land purchase
                _manager.AddTransaction(transaction);
            }
            else
            {
                Transactions transaction = new Transactions(_project.ID, Input, 0, "P", false); // Renovation purchase
                _manager.AddTransaction(transaction);
            }
        }

    }

    class DisplaySales : MenuItem
    {
        private ProjectManager _manager;
        private Project _project;

        public DisplaySales(ProjectManager projectManager, Project project)
        {
            _manager = projectManager;
            _project = project;

        }
        public override string MenuText()
        {
            return "Display Sales";
        }
        public override void Select()
        {
            foreach (Transactions transactions in _manager.transactions)
            {
                if (transactions.transactionSale > 0 && transactions._ID == _project.ID)
                {
                    transactions.DisplaySales();
                }
            }
            _manager.End();
        }
    }
    class DisplayPurchases : MenuItem
    {
        private ProjectManager _manager;
        private Project _project;

        public DisplayPurchases(ProjectManager projectManager, Project project)
        {
            _manager = projectManager;
            _project = project;
        }
        public override string MenuText()
        {
            return "Display Purhases";
        }
        public override void Select()
        {
            foreach(Transactions transactions in _manager.transactions)
            {
                if (transactions._ID == _project.ID)
                {
                    transactions.DisplayPurchases();
                }
            }
            _manager.End();
        }
    }

    class Summary : MenuItem
    {
        ProjectManager _manager;
        Project _project;


        public Summary(ProjectManager projectManager, Project project)
        {
            _manager = projectManager;
            _project = project;
        }

        public override string MenuText()
        {
            return "Project Summary";
        }
        public override void Select()
        {
            float totalsale = 0;
            float totalpurchase = 0;
            float totalrefund = 0;
            foreach (Transactions transactions in _manager.transactions)
            {
                if (transactions._ID == _project.ID)
                {
                    totalsale = transactions.transactionSale + totalsale;
                    totalpurchase = transactions.transactionPurchase + totalpurchase;
                    totalrefund = transactions.transactionVATReturn + totalrefund;
                }
            }
            totalsale = MathF.Round(totalsale, 2);
            totalpurchase = MathF.Round(totalpurchase, 2);
            totalrefund = MathF.Round(totalrefund, 2);
            float totalprofit = totalsale - totalpurchase + totalrefund;
            Console.WriteLine("ID = " + _project.ID + " , Sales = " + totalsale + " , Purchases = " + totalpurchase + " , Refund = " + totalrefund + " , Profit = " + MathF.Round(totalprofit, 2));

            _manager.End();
        }
    }
    class DeleteProject : MenuItem
    {
        private ProjectManager _manager;
        private Project _project;

        public DeleteProject(ProjectManager projectManager, Project project)
        {
            _manager = projectManager;
            _project = project;
        }
        public override string MenuText()
        {
            return "Delete Project " + _project.ID.ToString();
        }
        public override void Select()
        {
            for (int i = 0; i < _manager.transactions.Count; i++)
            {
                Transactions transaction = _manager.transactions[i];
                if (_project.ID == transaction._ID)
                {
                    _manager.transactions.Remove(transaction);
                }
            }

            Console.WriteLine("Project " + _project.ID + " has been Removed");
            _manager.projects.Remove(_project);
            _manager.End();
            new StartMenu(_manager).Select();

        }
    }



    class NewBuild : MenuItem
    {
        ProjectManager _manager;

        public NewBuild(ProjectManager manager)
        {
            _manager = manager;
        }

        public override string MenuText()
        {
            return "New Build";
        }
        public override void Select()
        {
            Project project = new Project(true);
            _manager.AddProject(project);
            float Input = ConsoleHelpers.GetNum("please enter amount for land purchase");
            Transactions transaction = new Transactions(project.ID, Input, 0, "L", true);  //L = land first purchase
            _manager.AddTransaction(transaction);
            Console.WriteLine("A new project has been created with ID " + project.ID);
        }
    }
    class Renovation : MenuItem
    {
        ProjectManager _manager;

        public Renovation(ProjectManager manager)
        {
            _manager = manager;
        }
        public override string MenuText()
        {
            return "Renovation";
        }
        public override void Select()
        {
            Project project = new Project(false);
            _manager.AddProject(project);
            float Input = ConsoleHelpers.GetNum("please enter amount for Building purchase");
            int id = project.ID;
            Transactions transaction = new Transactions(id, Input, 0, "R", false);  //Renovation first purchase
            _manager.AddTransaction(transaction);
            Console.WriteLine("A new project has been created with ID " + project.ID);
        }
    }


    class PortSummary : MenuItem
    {
        private ProjectManager _manager;

        public PortSummary(ProjectManager manager)
        {
            _manager = manager;
        }

        public override string MenuText()
        {
            return "Portfolio Summary";
        }
        public override void Select()
        {
            float Allsale = 0;
            float Allpurchase = 0;
            float Allrefund = 0;
            foreach (Project project in _manager.projects)
            {
                float totalsale = 0;
                float totalpurchase = 0;
                float totalrefund = 0;
                foreach (Transactions transactions in _manager.transactions)
                {
                    if (transactions._ID == project.ID)
                    {
                        totalsale = transactions.transactionSale + totalsale;
                        totalpurchase = transactions.transactionPurchase + totalpurchase;
                        totalrefund = transactions.transactionVATReturn + totalrefund;
                    }
                }
                totalsale = MathF.Round(totalsale, 2);
                totalpurchase = MathF.Round(totalpurchase, 2);
                totalrefund = MathF.Round(totalrefund, 2);
                float totalprofit = totalsale - totalpurchase + totalrefund;
                Console.WriteLine("ID = " + project.ID + " , Sales = " + totalsale + " , Purchases = " + totalpurchase + " , Refund = " + totalrefund + " , Profit = " + MathF.Round(totalprofit, 2));
                Allsale = totalsale + Allsale;
                Allpurchase = totalpurchase + Allpurchase;
                Allrefund = totalrefund + Allrefund;
            }
            float AllProfit = Allsale - Allpurchase + Allrefund;
            Console.WriteLine("All , Sales = " + Allsale + " , Purchases = " + Allpurchase + " , Refund = " + Allrefund + " , Profit = " + MathF.Round(AllProfit, 2));
            _manager.End();
        }
    }
}


