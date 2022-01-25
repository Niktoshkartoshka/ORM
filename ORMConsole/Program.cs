using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using ORM.migrations;

namespace ORMConsole
{
    public class Program
    {
        public static void Main(string[] args)
        {
            MigrationKernel migrationKernel = new MigrationKernel(@"..\..\..\..\Test_ORM\database\migrations",
                                                                  @"..\..\..\..\Test_ORM\database\models");
            Console.ReadKey();
        }
    }
}
