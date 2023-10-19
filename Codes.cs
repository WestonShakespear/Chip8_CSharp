namespace Program
{
    
    public class Codes
    {
        public Dictionary<string, Func<string, int>> codes = new Dictionary<string, Func<string, int>>();

        public Codes()
        {
            codes["00E0"] = new Func<string, int>(CLEAR);
            codes["1NNN"] = new Func<string, int>(JUMP);
            codes["6XNN"] = new Func<string, int>(SETVX);
            codes["7XNN"] = new Func<string, int>(ADDVX);
            codes["ANNN"] = new Func<string, int>(SETI);
            codes["DXYN"] = new Func<string, int>(DRAW);
        }

        public int CLEAR(string code)
        {
            Console.Write("CLEAR");
            for (int i = 0; i < 32*64; i++)
            {   
                Chip.SCREEN_MEM[i] = 0;
            }

            Chip.PC += 2;

            return 0;
        }

        public int JUMP(string code)
        {
            Console.Write("JUMP ");

            string value = code.Substring(1);

            int value_number = Int32.Parse(value, System.Globalization.NumberStyles.HexNumber);

            Console.WriteLine("To {0}", value_number);

            Chip.PC = (short) (value_number);


            return -1;
        }

        public int SETVX(string code)
        {
            Console.Write("SETVX ");

            char register = code[1];
            string value = code.Substring(2);

            Console.Write(" X=0x{0}    Y=        NN=0x{1}", register, value);

            byte value_number = (byte)Int32.Parse(value, System.Globalization.NumberStyles.HexNumber);

            SetRegisterCode(register, value_number);



            Chip.PC += 2;
            return 0;
        }

        public int ADDVX(string code)
        {
            Console.Write("ADDVX ");

            char register = code[1];
            string value = code.Substring(2);

            Console.Write(" X=0x{0}    Y=        NN=0x{1}", register, value);

            int value_number = Int32.Parse(value, System.Globalization.NumberStyles.HexNumber);

            // Console.Write("    {0}", value_number);

            int current_value = GetRegisterCode(register);
            int new_value = current_value + value_number;

            SetRegisterCode(register, (byte)new_value);
            // Console.WriteLine(GetRegisterCode(register));


            Chip.PC += 2;
            return 0;
        }

        public int SETI(string code)
        {
            Console.Write("SETI ");
            string hex = code.Substring(1);
            Console.Write("  X=       Y=        NNN=0x{0}", hex);

            byte value_number = (byte)Int32.Parse(hex, System.Globalization.NumberStyles.HexNumber);

            // Console.Write("    {0}", value_number);

            Chip.I = value_number;

            Chip.PC += 2;
            return 0;
        }

        public int DRAW(string code)
        {
            Console.Write("DRAW ");

            char x = code[1];
            char y = code[2];
            string value = code[3].ToString();

            Console.Write("  X=0x{0}    Y=0x{1}     N=0x{2}", x, y, value);

            byte value_number = (byte)Int32.Parse(value, System.Globalization.NumberStyles.HexNumber);

            Console.Write("    X:{0} Y:{1} to {2}", GetRegisterCode(x), GetRegisterCode(y), value_number);

            Chip.SCREEN_MEM[(GetRegisterCode(y) * 64) + GetRegisterCode(x)] = 1;
            
            


            Chip.PC += 2;
            return 0;
        }

        public byte GetRegisterCode(char register)
        {
            if (register == '0')            return Chip.V0;
            else if (register == '1')       return Chip.V1;
            else if (register == '2')       return Chip.V2;
            else if (register == '3')       return Chip.V3;
            else if (register == '4')       return Chip.V4;
            else if (register == '5')       return Chip.V5;
            else if (register == '6')       return Chip.V6;
            else if (register == '7')       return Chip.V7;
            else if (register == '8')       return Chip.V8;
            else if (register == '9')       return Chip.V9;
            else if (register == 'A')       return Chip.VA;
            else if (register == 'B')       return Chip.VB;
            else if (register == 'C')       return Chip.VC;
            else if (register == 'D')       return Chip.VD;
            else if (register == 'E')       return Chip.VE;
            else if (register == 'F')       return Chip.VF;
            return 0;
        }

        public void SetRegisterCode(char register, byte value)
        {
            if (register == '0')            Chip.V0 = value;
            else if (register == '1')       Chip.V1 = value;
            else if (register == '2')       Chip.V2 = value;
            else if (register == '3')       Chip.V3 = value;
            else if (register == '4')       Chip.V4 = value;
            else if (register == '5')       Chip.V5 = value;
            else if (register == '6')       Chip.V6 = value;
            else if (register == '7')       Chip.V7 = value;
            else if (register == '8')       Chip.V8 = value;
            else if (register == '9')       Chip.V9 = value;
            else if (register == 'A')       Chip.VA = value;
            else if (register == 'B')       Chip.VB = value;
            else if (register == 'C')       Chip.VC = value;
            else if (register == 'D')       Chip.VD = value;
            else if (register == 'E')       Chip.VE = value;
            else if (register == 'F')       Chip.VF = value;
        }

    }

}