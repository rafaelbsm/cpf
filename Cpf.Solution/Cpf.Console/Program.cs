namespace Cpf.Console
{
    using System;

    class Program
    {
        static void Main(string[] args)
        {
            Cpf cpf0 = new Cpf() { Value = "347.843.610-90" };
            Cpf cpf1 = new Cpf("34784361090");
            Cpf cpf2 = new Cpf(347843610, 90);
            Cpf cpf3 = new Cpf(34784361090);
            Cpf cpf4 = "347.843.610-90";
            Cpf cpf5 = "34784361090";
            Cpf cpf6 = 34784361090;

            if (cpf0.Valid)
            {
                Console.Write(cpf0.CpfFormatted);
            }

            Console.ReadKey();
        }
    }
}
