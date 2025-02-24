using System;
using System.Collections.Generic;
using System.Linq;

class UserData
{
    public string firstName { get; set; }
    public string lastName { get; set; }
    public string middleName { get; set; }
    public string contactNumber { get; set; }
    public string email { get; set; }
    public string userName { get; set; }
    public string password { get; set; }
    public string role { get; set; }
}

class Program
{
    static HashSet<UserData> registeredUsers = new HashSet<UserData>() // Stores users
        {
        new UserData { firstName = "John", lastName = "Doe", middleName = "A", contactNumber = "1234567890", email = "john.doe@email.com", userName = "johndoe", password = "pass123", role = "user" },
        new UserData { firstName = "Jane", lastName = "Smith", middleName = "B", contactNumber = "0987654321", email = "jane.smith@email.com", userName = "janesmith", password = "pass123", role = "admin" },
        new UserData { firstName = "Mike", lastName = "Brown", middleName = "C", contactNumber = "1112223333", email = "mike.brown@email.com", userName = "mikebrown", password = "pass123", role = "user" },
        new UserData { firstName = "Sarah", lastName = "Johnson", middleName = "D", contactNumber = "2223334444", email = "sarah.johnson@email.com", userName = "sarahjohnson", password = "pass123", role = "admin" },
        new UserData { firstName = "Chris", lastName = "Evans", middleName = "E", contactNumber = "3334445555", email = "chris.evans@email.com", userName = "chrisevans", password = "pass123", role = "user" },
        new UserData { firstName = "Emma", lastName = "Watson", middleName = "F", contactNumber = "4445556666", email = "emma.watson@email.com", userName = "emmawatson", password = "pass123", role = "user" },
        new UserData { firstName = "Robert", lastName = "Downey", middleName = "G", contactNumber = "5556667777", email = "robert.downey@email.com", userName = "robertdowney", password = "pass123", role = "admin" },
        new UserData { firstName = "Scarlett", lastName = "Johansson", middleName = "H", contactNumber = "6667778888", email = "scarlett.j@email.com", userName = "scarlettj", password = "pass123", role = "user" },
        new UserData { firstName = "Mark", lastName = "Ruffalo", middleName = "I", contactNumber = "7778889999", email = "mark.r@email.com", userName = "markruffalo", password = "pass123", role = "user" },
        new UserData { firstName = "Tom", lastName = "Holland", middleName = "J", contactNumber = "8889990000", email = "tom.h@email.com", userName = "tomholland", password = "pass123", role = "admin" },
    };

    static void Main()
    {
        
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=======Main Menu=======");
            Console.WriteLine("[1] Register");
            Console.WriteLine("[2] Login");
            Console.WriteLine("[3] Exit");
            Console.Write("Select an option: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Register();
                    break;
                case "2":
                    Login();
                    break;
                case "3":
                    Console.WriteLine("Thank you for using our system. Goodbye!");
                    return;
                default:
                    Console.WriteLine("Invalid choice. Try again.");
                    break;
            }
        }
    }

    static void Register()
    {
        Console.WriteLine("=========Register New User=========");
        Console.Write("Enter First Name: ");
        string firstName = Console.ReadLine();
        Console.Write("Enter Last Name: ");
        string lastName = Console.ReadLine();
        Console.Write("Enter Middle Name: ");
        string middleName = Console.ReadLine();
        Console.Write("Enter Contact Number: ");
        string contactNumber = Console.ReadLine();

        Console.Write("Enter Email: ");
        string registerEmail = Console.ReadLine();
        
        if (registeredUsers.Any(x => x.email == registerEmail))
        {
            Console.WriteLine("Email already exists. Try again.");
            return;
        }

        Console.Write("Enter Username: ");
        string registerUsername = Console.ReadLine();

        if (registeredUsers.Any(x => x.userName == registerUsername))
        {
            Console.WriteLine("Username already exists. Try again.");
            return;
        }

        Console.Write("Enter Password: ");
        string registerPassword = Console.ReadLine();

        if (string.IsNullOrEmpty(registerPassword))
        {
            Console.WriteLine("Password cannot be empty. Try again.");
            return;
        }

        Console.Write("Enter role [User/Admin]: ");
        string role = Console.ReadLine().ToLower();

        if (role != "user" && role != "admin")
        {
            Console.WriteLine("Invalid role");
            return;
        }

        UserData newUser = new UserData
        {
            firstName = firstName,
            lastName = lastName,
            middleName = middleName,
            contactNumber = contactNumber,
            email = registerEmail,
            userName = registerUsername,
            password = registerPassword,
            role = role
        };
        registeredUsers.Add(newUser);

        Console.WriteLine("Registration successful!");
    }

    static void Login()
    {
        int attempts = 0;
        Console.WriteLine("===== Login =====");

        while (attempts < 3)
        {
            Console.Write("Username: ");
            string username = Console.ReadLine();
            Console.Write("Password: ");
            string password = Console.ReadLine();

            UserData foundUser = registeredUsers.FirstOrDefault(user => user.userName == username && user.password == password);

            if (foundUser != null)
            {
                string token = new string(password.Reverse().ToArray()) + username;
                Console.WriteLine($"Login Successful! Welcome {username}, Generated Token: {token}.");
                UserMenu(foundUser);
                return;
            }
            else
            {
                attempts++;
                Console.WriteLine("Invalid Username or Password");
                Console.WriteLine($"Attempts left: {3 - attempts}");
            }
        }

        Console.WriteLine("Too many failed attempts. Login locked.");
    }

    static void UserMenu(UserData loggedInUser)
    {
        string choice;
        do
        {
            Console.WriteLine("\n===== Menu =====");
            Console.WriteLine("[1] View Profile");
            Console.WriteLine("[2] View All Profiles (Admin only)");
            Console.WriteLine("[3] Change Password");
            Console.WriteLine("[4] Logout");
            Console.Write("Enter Choice: ");
            choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    DisplayUserProfile(loggedInUser);
                    break;
                case "2":
                    if (loggedInUser.role == "admin")
                    {
                        ViewAllProfiles();
                    }
                    else
                    {
                        Console.WriteLine("Feature locked");
                    }
                    break;
                case "3":
                    ChangePassword(loggedInUser);
                    break;
                case "4":
                    Console.WriteLine("Logging out...");
                    break;
                default:
                    Console.WriteLine("Invalid Choice");
                    break;
            }
        } while (choice != "4");
    }

    static void DisplayUserProfile(UserData user)
    {
        Console.WriteLine("\n===== User Profile =====");
        Console.WriteLine("First Name: " + user.firstName);
        Console.WriteLine("Last Name: " + user.lastName);
        Console.WriteLine("Middle Name: " + user.middleName);
        Console.WriteLine("Contact Number: " + user.contactNumber);
        Console.WriteLine("Email: " + user.email);
        Console.WriteLine("Username: " + user.userName);
        Console.WriteLine("Role: " + user.role);
    }

    static void ViewAllProfiles()
    {
        foreach (var user in registeredUsers)
        {
            Console.WriteLine($"Username: {user.userName}, Role: {user.role}");
        }
    }

    static void ChangePassword(UserData user)
    {
        Console.Write("Enter your current password: ");
        string currentPassword = Console.ReadLine();

        if (currentPassword != user.password)
        {
            Console.WriteLine("Incorrect password. Try again.");
            return;
        }

        Console.Write("Enter new password: ");
        string newPassword = Console.ReadLine();
        if (string.IsNullOrEmpty(newPassword))
        {
            Console.WriteLine("Password cannot be empty.");
            return;
        }

        user.password = newPassword;
        Console.WriteLine("Password changed successfully!");
    }
}
