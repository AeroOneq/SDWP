using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationLib.Database;
using AeroORMFramework;
using ApplicationLib.Models;

using FileLib;

namespace DatabaseController
{
    class Program
    {
        static void Main(string[] args)
        {
            /*CSFileParser cSFileParser = new CSFileParser();
            cSFileParser.GetMethodsTable("C:\\Users\\Aero\\Desktop\\Курсач\\SDWP\\SDWP\\ApplicationLib\\bin\\Debug\\ApplicationLib.dll");*/


            Connector connector = new Connector(DatabaseProperties.ConnectionString);
            connector.DeleteTable<Documentation>();
            connector.AddTable<Documentation>();
            /*UserInfo userInfo = new UserInfo
            {
                ID = 0,
                Login = "Aero",
                Password = "qwerty",
                Name = "Евгений",
                Surname = "Степанов",
                BirthDate = DateTime.Now,
                Email = "stepanov-ev@yandex.ru"
            };
            connector.Insert(userInfo);*/
            Console.WriteLine("Success");

            Console.ReadKey();
        }
    }
}
