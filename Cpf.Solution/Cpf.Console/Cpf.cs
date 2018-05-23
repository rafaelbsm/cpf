namespace Cpf.Console
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text.RegularExpressions;

    public struct Cpf : IFormattable, IComparable<Cpf>, IEquatable<Cpf>, ISerializable
    {
        public static readonly string FormatCpf = @"^\d{3}\.\d{3}\.\d{3}-\d{2}$";
        private static readonly string LengthCpf = @"^\d{11}$";

        public string CpfFormatted { get; private set; }
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
        public int? Base { get; private set; }
        public byte? Dig { get; private set; }
        public bool Valid { get; private set; }


        public Cpf(string cpf)
        {
            this = new Cpf();

            Separate(cpf);
            ValidateCpf();
            Format();
        }

        public Cpf(long cpf)
        {
            this = cpf;

            ValidateCpf();
            Format();
        }

        public Cpf(int cpfBase, byte cpfDigito)
        {
            this = new Cpf();
            Base = cpfBase;
            Dig = cpfDigito;

            ValidateCpf();
            Format();
        }

        private static bool IsFormat(string cpf) => Regex.IsMatch(cpf, FormatCpf);
        private static bool IsLength(string cpf) => Regex.IsMatch(cpf, LengthCpf);

        private ICollection<byte> GetCpfBaseCalcArray(int? digit = null)
        {
            return $"{Base:000000000}{digit}".Select(c => Convert.ToByte(c.ToString())).ToList();
        }

        private void ValidateCpf()
        {
            int digit = CalculateDigit(GetCpfBaseCalcArray(), 10);

            Valid = Dig.Value == (digit * 10) + CalculateDigit(GetCpfBaseCalcArray(digit), 11);
        }

        private int CalculateDigit(IEnumerable<byte> baseCpf, int multiplier)
        {
            int digit = 0;

            for (int i = 0; i < baseCpf.Count(); i++)
            {
                digit += baseCpf.ElementAt(i) * (multiplier--);
            }

            digit = digit % 11;

            if (digit < 2)
            {
                return 0;
            }
            else
            {
                return 11 - digit;
            }
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

        private void Format()
        {
            if (Valid)
            {
                CpfFormatted = $"{Base.Value:000,000,000}-{Dig.Value:00}";
            }
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
            return new Cpf($"{cpf:00000000000}");
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
