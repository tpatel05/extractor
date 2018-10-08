using DBTesting.App_Code;
using ncCore.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DBTesting.Models;


namespace DBTesting
{
    class Program
    {        
        static void Main(string[] args)
        {
            RunMain();
            Console.WriteLine("Done...");
            Console.ReadLine();
        }

        static void RunMain()
        {

            Console.WriteLine("Select a method to run(1 = Sproc, 2 = SELECTS, 3 = Entity, 4 = Serialize:");
            string strInput = Console.ReadLine();

            switch (strInput)
            {
                case "1":
                    Sproc();
                    break;
                case "2":
                    Selects();
                    break;
                case "3":
                    Entity();
                    break;
                case "4":
                    Serialize();
                    break;
                case "99":
                    Sproc();
                    Selects();
                    Entity();
                    Serialize();
                    break;
            }

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Play again? (y/n)");
            strInput = Console.ReadLine();

            if (strInput == "y")
            {
                RunMain();
            }

        }

    static void Sproc()
        {
            //set up start time
            DateTime dtStart = DateTime.Now;
            Console.WriteLine("Starting Sprocs Time: " + dtStart.ToLongTimeString());

            List<String[]> lstParameters = new List<String[]>();
            lstParameters.Add(new String[] { "@UserID", "6923" });
            lstParameters.Add(new String[] { "@PaymentStartDate", "7/1/2018" });
            lstParameters.Add(new String[] { "@PaymentEndDate", "8/30/2018" });
           
            //Run query
            DataSet ds = DataManager.ExecuteProcedure<DataSet>("X_sp_WCF_GetTransactions", lstParameters);
            Console.WriteLine(ds.Tables[0].Rows.Count + " found");

            //set end time
            DateTime dtEnd = DateTime.Now;
            Console.WriteLine("End Time: " + dtEnd.ToLongTimeString());

            //calculate difference
            TimeSpan tsLength = dtEnd - dtStart;
            Console.WriteLine("Time Spent: " + tsLength.TotalMilliseconds);
        }

        static void Selects()
        {
            //set up start time
            DateTime dtStart = DateTime.Now;
            Console.WriteLine("Starting Selects Time: " + dtStart.ToLongTimeString());

            //build results
            List<RCPmt> lstReslts = new List<RCPmt>();

            //Get Items_Master and Items based on UserId
            List<String[]> lstParameters = new List<String[]>();
            lstParameters.Add(new String[] { "@UserID", "6923" });
            lstParameters.Add(new String[] { "@PaymentStartDate", "8/1/2018" });
            lstParameters.Add(new String[] { "@PaymentEndDate", "9/30/2018" });
            DataSet ds = DataManager.Execute<DataSet>("SELECT M.ID as MasterID, I.ID as ItemID FROM X_USER_ORGANIZATIONS UO(NOLOCK) " +
                                                    "JOIN X_ITEM_MASTER M(NOLOCK) ON UO.OrganizationGUID = M.OrganizationGUID " +
                                                    "JOIN X_ITEMS I(NOLOCK) ON M.ID = I.MasterID " +
                                                    "WHERE UO.UserId = @UserID  " +
                                                    "AND(M.DatePaid >= @PaymentStartDate) " +
                                                    "AND(M.DatePaid < @PaymentEndDate)", lstParameters);
            var dsmCount = 0;
            var dsiCount = 0;
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                //Get MasterUDFields
                DataSet dsM = DataManager.Execute<DataSet>("SELECT * FROM X_Item_Master_UDFields WHERE MasterID = " + row["MasterID"].ToString());
                dsmCount = dsM.Tables[0].Rows.Count;
                //Get Item_Udfields
                DataSet dsI = DataManager.Execute<DataSet>("SELECT * FROM X_Items_UDFields WHERE ItemID = " + row["ItemID"].ToString());
                dsiCount = dsI.Tables[0].Rows.Count;
            }

            //set end time
            DateTime dtEnd = DateTime.Now;
            Console.WriteLine("End Selects Time: " + dtEnd.ToLongTimeString() + $"The first row count is {dsmCount} The second row count is {dsiCount}");

            //calculate difference
            TimeSpan tsLength = dtEnd - dtStart;
            Console.WriteLine("Time Spent Selects: " + tsLength.TotalMilliseconds);
        }

        static void Entity()
        {
            //set up start time
            DateTime dtStart = DateTime.Now;
            Console.WriteLine("Starting Entity Time: " + dtStart.ToLongTimeString());

            beta_XCaliberContext context = new beta_XCaliberContext();
            DateTime paymentStartDate = DateTime.Parse("07/01/2018");
            DateTime paymentEndDate = DateTime.Parse("08/30/2018");

            var mainData = from uo in context.XUserOrganizations
                join m in context.XItemMaster on uo.OrganizationGuid.ToString() equals m.OrganizationGuid
                join i in context.XItems on m.Id equals i.MasterId
              
                where uo.UserId == 6923 && m.DatePaid >= paymentStartDate && m.DatePaid < paymentEndDate
                select new MyNewClassWithEverything
                {
                    XItemsUdfields = context.XItemsUdfields.Where(p => p.ItemId == i.Id),
                    XItemMasterUdfields = context.XItemMasterUdfields.Where(z => z.MasterId == m.Id),
                    XitemMaster = m,
                    Xitems = i
                };

            DateTime dtEnd = DateTime.Now;
            Console.WriteLine("End Entity Time: " + dtEnd.ToLongTimeString() );
            TimeSpan tsLength = dtEnd - dtStart;
          
            Console.WriteLine($"Records returned count is {mainData.Count()}");
            Console.WriteLine("Time Spent Entity: " + tsLength.TotalMilliseconds);
        }

        static void Serialize()
        {
            //set up start time
            DateTime dtStart = DateTime.Now;
            Console.WriteLine("Starting serialize Time: " + dtStart.ToLongTimeString());

            //Code Here!!!

            //set end time
            DateTime dtEnd = DateTime.Now;
            Console.WriteLine("End serialize Time: " + dtEnd.ToLongTimeString());

            //calculate difference
            TimeSpan tsLength = dtEnd - dtStart;
            Console.WriteLine("Time Spent serialize: " + tsLength.TotalMilliseconds);
        }
    }
}
