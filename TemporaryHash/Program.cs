using System;
using BCrypt.Net;

class Program
{
    static void Main()
    {
        Console.Write("Enter your password to hash: ");
        string plainTextPassword = Console.ReadLine();

        // Hash the password
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(plainTextPassword);
        Console.WriteLine($"Hashed Password: {hashedPassword}");

        // Copy the hashed password and update it in SQL Server manually
    }
}
