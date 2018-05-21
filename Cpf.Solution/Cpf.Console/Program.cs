namespace Cpf.Console
{
    using System;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text.RegularExpressions;

    class Program
    {
        static void Main(string[] args)
        {
            Cpf cpf0 = new Cpf() { Value = "347.843.610-90" };
            Cpf cpf1 = new Cpf("34784361090");
            Cpf cpf2 = "347.843.610-90";
            Cpf cpf3 = 05253596569;
            Cpf cpf4 = new Cpf(052535965, 69);

            if (cpf0.Valid)
            {
                Console.Write(cpf0.CpfFormatted);
            }

            Console.ReadKey();
        }
    }

    public struct Cpf : IFormattable, IComparable<Cpf>, IEquatable<Cpf>, ISerializable
    {
        private static readonly string FormatCpf = @"^\d{3}\.\d{3}\.\d{3}-\d{2}$";
        private static readonly string LengthCpf = @"^\d{11}$";

        public string Value
        {
            get
            {
                return CpfFormatted;
            }
            set
            {
                this = new Cpf(value);
            }
        }
        public string CpfFormatted { get; private set; }
        public int? Base { get; private set; }
        public byte? Dig { get; private set; }
        public bool Valid { get; private set; }

        public Cpf(string cpf)
        {
            this = new Cpf();

            if (Separate(cpf))
            {
                ValidateCpf();
            }
        }

        public Cpf(int cpfBase, byte cpfDigito)
        {
            this = new Cpf();

            if (!default(int).Equals(cpfBase) && !default(byte).Equals(cpfDigito))
            {
                Base = cpfBase;
                Dig = cpfDigito;

                ValidateCpf();
            }
        }

        private static bool IsFormat(string cpf) => Regex.IsMatch(cpf, FormatCpf);
        private static bool IsLength(string cpf) => Regex.IsMatch(cpf, LengthCpf);

        private void ValidateCpf()
        {
            int[] multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            string baseCpf = $"{Base:000000000}";
            string digitoCpf;
            int soma = 0;
            int resto = 0;

            for (int i = 0; i < 9; i++)
            {
                soma += int.Parse(baseCpf[i].ToString()) * multiplicador1[i];
            }

            resto = soma % 11;

            if (resto < 2)
            {
                resto = 0;
            }
            else
            {
                resto = 11 - resto;
            }

            digitoCpf = resto.ToString();
            baseCpf = baseCpf + digitoCpf;
            soma = 0;

            for (int i = 0; i < 10; i++)
            {
                soma += int.Parse(baseCpf[i].ToString()) * multiplicador2[i];
            }

            resto = soma % 11;

            if (resto < 2)
            {
                resto = 0;
            }
            else
            {
                resto = 11 - resto;
            }

            digitoCpf = digitoCpf + resto.ToString();

            Valid = Dig.Value == Convert.ToByte(digitoCpf);

            CpfFormatted = $"{Base.Value:000,000,000}-{Dig.Value:00}";
        }

        private bool Separate(string cpf)
        {
            if (IsFormat(cpf))
            {
                string[] vet = cpf.Split('-');
                Base = int.Parse(vet.First().Replace(".", ""));
                Dig = byte.Parse(vet.Last());

                return true;
            }
            else if (IsLength(cpf))
            {
                Base = int.Parse(cpf.Substring(0, 9));
                Dig = byte.Parse(cpf.Substring(9));

                return true;
            }

            return false;
        }

        public override string ToString()
        {
            return CpfFormatted;
        }

        public bool Equals(Cpf other)
        {
            return Base == other.Base && Dig == other.Dig;
        }

        public int CompareTo(Cpf other)
        {
            if (this.Equals(other))
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Formatted", CpfFormatted);
            info.AddValue("Base", Base);
            info.AddValue("Dig", Dig);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return this.CpfFormatted;
        }

        public static implicit operator Cpf(string cpf)
        {
            return new Cpf(cpf);
        }

        public static implicit operator Cpf(long cpf)
        {
            return new Cpf(cpf.ToString("00000000000"));
        }

        public static implicit operator string(Cpf cpf)
        {
            return cpf.ToString();
        }

        public static implicit operator long(Cpf cpf)
        {
            return Convert.ToInt64($"{cpf.Base:000000000}{cpf.Dig:00}");
        }
    }
}
