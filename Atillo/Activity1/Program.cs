// See https://aka.ms/new-console-template for more information
string name = "John Carl O. Atillo";
int age = 20;
string address = "Inayawan, Cebu City";
string nickname = "JC";
string gender = "Male";
string yr = "BSCS 2A";
string[] programmingSkill = {"C", "Java"};
bool programmingExp = true;
string role = "Programmer";


Console.WriteLine("----------Demographics----------");
Console.WriteLine($"Name:\t\t{name}");
Console.WriteLine($"Age:\t\t{age}");
Console.WriteLine($"Address:\t{address}");
Console.WriteLine($"Nickname:\t{nickname}");
Console.WriteLine($"Gender:\t\t{gender}");
Console.WriteLine($"Yr:\t\t{yr}");
Console.WriteLine($"Programming Skills:");
for(int i = 0; i < programmingSkill.Length; i++){
    Console.WriteLine($"\t\t-- {programmingSkill[i]}");
}
Console.WriteLine("Programming Exp: " + (programmingExp? "Yes" : "No"));
Console.WriteLine($"Role:\t\t{role}");
