using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWIFileReader
{
    class Program
    {
        internal class Sale{
            public string SaleId { get; set; }
            public float Price { get; set; }
            public string SalesmanName { get; set; }

        }
        internal class Salesman
        {
            public string CPF { get; set; }
            public string Name { get; set; }
            public string Salary { get; set; }
            public List<Sale> SalesList { get; set; }
            public float SalesValue
            {
                get
                {
                    var value = 0f;
                    foreach(Sale sale in SalesList)
                    {
                        value += sale.Price;
                    }

                    return value;
                }
            }
        }
        static void Main(string[] args)
        {
            if (Directory.Exists("../../data/in"))
            {
                var files = Directory.GetFiles("../../data/in", @"*.csv");

                for (int i = 0; i < files.Count(); i++)
                {

                    string filename = files[i].Substring(files[i].IndexOf("\\") + 1);
                    List<Sale> salesInTheFile = new List<Sale>();
                    List<Salesman> salesmanInTheFile = new List<Salesman>();
                    int numberOfClientsInTheFile = 0;

                    using (StreamReader reader = new StreamReader(files[i]))
                    {

                        while (!reader.EndOfStream)
                        {

                            string line = reader.ReadLine();
                            try
                            {
                                string[] lineValues = line.Split('ç');
                                if (lineValues.Count() > 0)
                                {
                                    if (lineValues[0] == "001")
                                    {
                                        if (lineValues.Count() == 4) // Verify if it has all information or discard the salesman from the count
                                        {
                                            Salesman salesman = new Salesman()
                                            {
                                                CPF = lineValues[1],
                                                Name = lineValues[2],
                                                Salary = lineValues[3],
                                                SalesList = new List<Sale>(),
                                            };

                                            salesmanInTheFile.Add(salesman);
                                        }
                                    }
                                    else if (lineValues[0] == "002")
                                    {
                                        numberOfClientsInTheFile++;
                                    }
                                    else if (lineValues[0] == "003")
                                    {
                                        if (lineValues.Count() == 4) //Check if it has all the sale information or discard the data
                                        {

                                            float finalPrice = 0f;
                                            string lineTwoValueTwoFormated = lineValues[2].Substring(1, lineValues[2].ToString().Length - 2);
                                            string[] saleItemsInformation = lineTwoValueTwoFormated.Split(',');
                                            foreach (string item in saleItemsInformation)
                                            {
                                                string[] itemInformation = item.Split('-');
                                                if (itemInformation.Count() == 3)//Check if it has all the sale information or discard the data
                                                {
                                                    var price = float.Parse(itemInformation[2].Replace('.', ',')) * Convert.ToInt32(itemInformation[1]);
                                                    finalPrice += price;
                                                }
                                            }

                                            Sale sale = new Sale()
                                            {
                                                SaleId = lineValues[1],
                                                SalesmanName = lineValues[3],
                                                Price = finalPrice,
                                            };

                                            salesInTheFile.Add(sale);
                                        }
                                    }
                                    else
                                    {
                                        continue; //Invalid identifier
                                    }

                                }

                            }
                            catch (Exception msg)
                            {
                                Console.WriteLine(msg);
                            }
                        }
                    }

                    AddSalesToSalesMan(salesInTheFile, salesmanInTheFile);
                    string mostExpansiveSale = GetMostExpansiveSale(salesInTheFile);
                    Salesman worstSalesman = GetWorstSalesman(salesmanInTheFile);
                    string[] lines = { "quantidade de clientes: " + numberOfClientsInTheFile, "quantidade de vendedores:" + salesInTheFile.Count(),
                                     "venda mais cara: " + mostExpansiveSale, "pior vendedor: " + "nome: " + worstSalesman.Name + " cpf: " + worstSalesman.CPF};
                    // WriteAllLines creates a file, writes a collection of strings to the file,
                    // and then closes the file.  You do NOT need to call Flush() or Close().
                    System.IO.File.WriteAllLines(@"../../data/out/" + filename + ".txt", lines);

                }
            }
            else
            {
                Console.WriteLine("The directory doesn't exist.");
            }
        }

        public static void AddSalesToSalesMan(List<Sale> salesList, List<Salesman> salesmanList)
        {
            foreach(Sale sale in salesList)
            {
                string salesmanName = sale.SalesmanName.ToLower().Trim();
                Salesman salesmanByName = salesmanList.Where(x => x.Name.ToLower().Trim() == salesmanName).FirstOrDefault();
                if(salesmanByName != null)
                {
                    salesmanByName.SalesList.Add(sale);
                }
            }
        }

        public static string GetMostExpansiveSale(List<Sale> salesList)
        {
            Sale mostExpansiveSale = salesList.OrderByDescending(x => x.Price).FirstOrDefault();
            if(mostExpansiveSale != null)
                return mostExpansiveSale.SaleId;

            return "No sale found";
        }

        public static Salesman GetWorstSalesman(List<Salesman> salesmanList)
        {
            Salesman worstSalesman = salesmanList.OrderByDescending(x => x.SalesValue).LastOrDefault();
            if(worstSalesman != null)
            {
                return worstSalesman;
            }

            return new Salesman();
        }
    }
}
