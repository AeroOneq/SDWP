using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataStructures;
using AeroORMFramework;
using DataBase;

namespace DatabaseController
{
    class Program
    {
        static void Main(string[] args)
        {
            Connector connector = new Connector(Properties.ConnectionString);
            connector.DeleteTable<UserInfo>();
            connector.AddTable<UserInfo>();
            UserInfo userInfo = new UserInfo
            {
                ID = 0,
                Login = "Aero",
                Password = "qwerty",
                Name = "Евгений",
                Surname = "Степанов",
                BirthDate = DateTime.Now,
                Email = "stepanov-ev@yandex.ru",
                UserDocs = new List<int>()
                {
                    1,2,3,4
                },
                SharedDocs = new List<int>()
                {
                    4,3,2,1
                }
            };
            connector.Insert(userInfo);
            Console.ReadKey();
        }
    }
}
