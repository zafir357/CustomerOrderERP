using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace ErpDemo
{
    public static class DbHelper
    {

        public const string ConnStr =
            "Server=localhost;Database=ErpDemo;" +
            "Trusted_Connection=True;TrustServerCertificate=True;";

        // Ouvre et retourne une connexion SQL
        // Toujours appeler dans un "using" → ferme automatiquement même si erreur
        public static SqlConnection GetConnection()
        {
            var conn = new SqlConnection(ConnStr); 
            conn.Open();                           
            return conn;                           
        }
    }
}
