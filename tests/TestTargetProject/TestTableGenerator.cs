////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System.Data;
using System.Data.SQLite;

namespace TestTargetProject
{
    public static class TestTableGenerator
    {
        public static SQLiteConnection CreateTestTable()
        {
            var connection = new SQLiteConnection("Data Source=:memory:");
            connection.Open();

            var c = connection.CreateCommand();
            c.CommandType = CommandType.Text;
            c.CommandText = "CREATE TABLE target (Value1 TEXT,Value2 INTEGER,Value3 INTEGER,Value4 INTEGER,Value5 INTEGER,Value6 REAL,Value7 REAL,Value8 REAL,Value9 TEXT,Value10 TEXT,Value11 INTEGER,Value12 TEXT,Value13 TEXT)";
            c.ExecuteNonQuery();

            c.CommandText = "INSERT INTO target VALUES ('TRUE',123,12345,12345678,123456789012345,123.45,123.45678901234,123.45678901234,'13B3385A-4D34-42F0-BBAD-6D900A66F191','01/18/2022 12:34:56.789','ValueB',7,'ABC')";
            c.ExecuteNonQuery();
            c.CommandText = "INSERT INTO target VALUES ('FALSE',124,12346,12345679,123456789012346,123.46,123.45678901235,123.45678901235,'13B3385A-4D34-42F0-BBAD-6D900A66F192','01/18/2022 12:34:56.780','ValueC',8,'ABD')";
            c.ExecuteNonQuery();
            c.CommandText = "INSERT INTO target VALUES ('TRUE',125,12347,12345670,123456789012347,123.47,123.45678901236,123.45678901236,'13B3385A-4D34-42F0-BBAD-6D900A66F193','01/18/2022 12:34:56.781','ValueD',11,'ABE')";
            c.ExecuteNonQuery();

            return connection;
        }
    }
}
