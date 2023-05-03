using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Capstone_Project_441101_2223
{
    class ProjectManager
    {
        public List<Project> projects { get; private set; }
        public List<Transactions> transactions { get; private set; }


        public ProjectManager()
        {
            projects = new List<Project>();
            transactions = new List<Transactions>();
        }

        public void AddProject(Project project)
        {
            if (!projects.Contains(project))
            {
                projects.Add(project);
            }
        }
        public void AddTransaction(Transactions transaction)
        {
            if (!transactions.Contains(transaction))
            {
                transactions.Add(transaction);
            }
        }
        public void End()
        {
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
        }


    }

    class Project
    {
        static int nextID = 100;
        public int ID { get; private set; }
        public bool isLand;

        public Project(bool land)
        {
            ID = nextID;
            nextID++;
            isLand = land;
        }

        public Project(int iD, bool Land)
        {
            ID = iD;
            isLand = Land;
        }
    }

    class Transactions
    {
        public string transactionType { get; private set; }
        public float transactionSale { get; private set; }
        public float transactionPurchase { get; private set; }
        public float transactionVATReturn { get; private set; }
        public int _ID { get; private set; }
        public float profit { get; private set; }
        float _purchase;
        public Transactions(int projID, float purchase, float sale, string type, bool needTaxReturn)
        {
            _ID = projID;
            transactionPurchase = purchase;
            transactionSale = sale;
            transactionType = type;
            _purchase = purchase;
            profit = transactionSale - transactionPurchase;
            if (needTaxReturn)
            {
                float ExVat = _purchase / 1.2f;
                transactionVATReturn = _purchase - ExVat;

            }
        }

        public void DisplayPurchases()
        {
            if(transactionType == "P" || transactionType == "L" || transactionType == "R")
            {
                Console.WriteLine(transactionType + " (" + transactionPurchase + ") , VAT Return = " + MathF.Round(transactionVATReturn, 2));
            }
            
        }

        public void DisplaySales()
        {
            if(transactionType == "S")
            {
                Console.WriteLine(transactionType + " (" + transactionSale + ")");
            }
            
        }        
    }

}
