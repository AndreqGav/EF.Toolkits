using System;

namespace EF.Toolkits.Tests.Sql
{
    public class AppDbFunctions
    {
        public static string GetName(int id) => throw new InvalidOperationException();

        public static string GetNameSqlUp() =>
            "CREATE OR REPLACE FUNCTION GetName(id integer)\n" +
            "RETURNS text AS $$\n" +
            "BEGIN\n" +
            "RETURN (SELECT \"Name\" FROM \"Animals\" WHERE \"Id\" = id);\n" +
            " END;\n" +
            "$$ LANGUAGE plpgsql;";

        public static string GetNameSqlDown() => "DROP FUNCTION IF EXISTS GetName";
    }
}