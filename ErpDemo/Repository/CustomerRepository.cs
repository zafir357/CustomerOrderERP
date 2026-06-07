using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using static ErpDemo.Models;

namespace ErpDemo.Repository
{
    public static class CustomerRepository
    {
        // Récupère tous les clients depuis la BDD
        // Retourne une liste d'objets Customer
        public static List<Customer> GetAll()
        {
            var list = new List<Customer>(); // liste vide qu'on va remplir

            // "using" = ferme automatiquement la connexion à la fin du bloc
            using var conn = DbHelper.GetConnection();

            // SqlCommand = la requête SQL à exécuter
            using var cmd = new SqlCommand(
                "SELECT Id, Name, Email, Phone, CreatedAt FROM Customers ORDER BY Name",
                conn); // on passe la connexion ouverte

            // ExecuteReader = exécute le SELECT et retourne les lignes une par une
            using var reader = cmd.ExecuteReader();

            // reader.Read() = passe à la ligne suivante, retourne false quand c'est fini
            while (reader.Read())
            {
                list.Add(new Customer
                {
                    Id = reader.GetInt32(0),   // colonne 0 = Id
                    Name = reader.GetString(1),   // colonne 1 = Name
                                                  // IsDBNull vérifie si la valeur est NULL dans la BDD
                    Email = reader.IsDBNull(2) ? "" : reader.GetString(2),
                    Phone = reader.IsDBNull(3) ? "" : reader.GetString(3),
                    CreatedAt = reader.GetDateTime(4)  // colonne 4 = CreatedAt
                });
            }
            return list;
        }

        // Insère un nouveau client dans la BDD
        public static void Add(Customer c)
        {
            using var conn = DbHelper.GetConnection();

            using var cmd = new SqlCommand(
                "INSERT INTO Customers (Name, Email, Phone) VALUES (@Name, @Email, @Phone)",
                conn);

            // AddWithValue = lie la valeur au paramètre de façon sécurisée
            cmd.Parameters.AddWithValue("@Name", c.Name);
            cmd.Parameters.AddWithValue("@Email", c.Email == "" ? DBNull.Value : c.Email);
            cmd.Parameters.AddWithValue("@Phone", c.Phone == "" ? DBNull.Value : c.Phone);

            // ExecuteNonQuery = exécute INSERT/UPDATE/DELETE (pas de résultat à lire)
            cmd.ExecuteNonQuery();
        }

        // Supprime un client ET toutes ses commandes
        // Utilise une TRANSACTION : soit tout réussit, soit rien n'est supprimé
        public static void Delete(int id)
        {
            using var conn = DbHelper.GetConnection();

            // BeginTransaction = démarre une transaction
            using var tx = conn.BeginTransaction();
            try
            {
                // (obligatoire à cause de la clé étrangère FK)
                using var cmd1 = new SqlCommand(
                    "DELETE FROM Orders WHERE CustomerId = @Id", conn, tx); // on passe la transaction
                cmd1.Parameters.AddWithValue("@Id", id);
                cmd1.ExecuteNonQuery();

                // Étape 2 : supprimer le client
                using var cmd2 = new SqlCommand(
                    "DELETE FROM Customers WHERE Id = @Id", conn, tx);
                cmd2.Parameters.AddWithValue("@Id", id);
                cmd2.ExecuteNonQuery();

                // Commit = valide les deux suppressions définitivement
                tx.Commit();
            }
            catch
            {
                // Rollback = annule tout si une étape a échoué
                tx.Rollback();
                throw; // relance l'exception pour que le Form l'affiche
            }
        }
    }
}
