using System;

namespace EF.Toolkits.Tests.Sql
{
    public class AppDbFunctions
    {
        public static string GetName(int id) => throw new InvalidOperationException();

        public static string GetNameSqlUp() => "CREATE OR REPLACE FUNCTION GetName (@id integer)";
        
        public static string GetNameSqlDown() => "DROP FUNCTION IF EXISTS GetName";
    }
}