using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using static ErpDemo.Models;

namespace ErpDemo.Repository;

public static class OrderRepository
{
    // Récupère les commandes d'un client via Stored Procedure
    public static List<Order> GetByCustomer(int customerId)
    {
        var list = new List<Order>();

        using var conn = DbHelper.GetConnection();

        // SqlCommand avec le NOM de la stored procedure (pas le SQL directement)
        using var cmd = new SqlCommand("sp_GetOrdersByCustomer", conn);

        // Dit à ADO.NET que c'est une stored procedure et pas du SQL inline
        cmd.CommandType = CommandType.StoredProcedure;

        // Passe le paramètre à la stored procedure
        cmd.Parameters.AddWithValue("@CustomerId", customerId);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new Order
            {
                Id = reader.GetInt32(0),
                Reference = reader.GetString(1),
                Amount = reader.GetDecimal(2),
                Status = reader.GetString(3),
                OrderDate = reader.GetDateTime(4)
            });
        }
        return list;
    }

    // Insère une nouvelle commande
    public static void Add(Order o)
    {
        using var conn = DbHelper.GetConnection();
        using var cmd = new SqlCommand(
            "INSERT INTO Orders (CustomerId, Reference, Amount, Status)" +
            " VALUES (@CId, @Ref, @Amt, @Sta)", conn);
        cmd.Parameters.AddWithValue("@CId", o.CustomerId);
        cmd.Parameters.AddWithValue("@Ref", o.Reference);
        cmd.Parameters.AddWithValue("@Amt", o.Amount);
        cmd.Parameters.AddWithValue("@Sta", o.Status);
        cmd.ExecuteNonQuery();
    }

    // Supprime une commande par son Id
    public static void Delete(int id)
    {
        using var conn = DbHelper.GetConnection();
        using var cmd = new SqlCommand(
            "DELETE FROM Orders WHERE Id = @Id", conn);
        cmd.Parameters.AddWithValue("@Id", id);
        cmd.ExecuteNonQuery();
    }
}