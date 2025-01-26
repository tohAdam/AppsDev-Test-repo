// See https://aka.ms/new-console-template for more information
Console.WriteLine("John Carl Atillo");
Console.Write("Enter num1: ");
int num1 = Convert.ToInt32(Console.ReadLine());

Console.Write("Enter num2: ");
int num2 = Convert.ToInt32(Console.ReadLine());
Console.Write("Choose an operator (+ - * /): ");
string op = Console.ReadLine();
double result = 0;

if (string.IsNullOrEmpty(op))
{
    Console.WriteLine("Invalid Input: No operator provided.");
    return;
}

switch(op[0]){
    case '+':
        result = num1 + num2;
        break;
    case '-':
        result = num1 - num2;
        break;
    case '*':
        result = num1 * num2;
        break;
    case '/':
        if(num2 == 0){
            Console.WriteLine("Undefined.");
            break;
        }
        result = (double)num1/num2;
        break;
    default:
        Console.WriteLine("Invalid Input.");
        return;
}

Console.WriteLine($"Result: {result}");



