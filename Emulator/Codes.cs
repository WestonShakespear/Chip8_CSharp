namespace Program
{
    
    public class Codes
    {
        public Dictionary<string, Func<string, int>> codes = new Dictionary<string, Func<string, int>>();

        System.Random random;

        public Codes()
        {
            codes["00E0"] = new Func<string, int>(CLEAR);
            codes["00EE"] = new Func<string, int>(RET);
            codes["1NNN"] = new Func<string, int>(JUMP);
            codes["2NNN"] = new Func<string, int>(SUB);
            codes["3XNN"] = new Func<string, int>(SKIPIF);
            codes["4XNN"] = new Func<string, int>(NSKIPIF);
            codes["5XY0"] = new Func<string, int>(SKIPCOMP);
            codes["9XY0"] = new Func<string, int>(NSKIPCOMP);
            codes["6XNN"] = new Func<string, int>(SETVX);
            codes["7XNN"] = new Func<string, int>(ADDVX);
            codes["8XY0"] = new Func<string, int>(MATH0);
            codes["8XY1"] = new Func<string, int>(MATH1);
            codes["8XY2"] = new Func<string, int>(MATH2);
            codes["8XY3"] = new Func<string, int>(MATH3);
            codes["8XY4"] = new Func<string, int>(MATH4);
            codes["8XY5"] = new Func<string, int>(MATH5);
            codes["8XY6"] = new Func<string, int>(MATH6);
            codes["8XY7"] = new Func<string, int>(MATH7);
            codes["8XYE"] = new Func<string, int>(MATHE);


            codes["ANNN"] = new Func<string, int>(SETI);
            codes["DXYN"] = new Func<string, int>(DRAW);
            codes["CXYN"] = new Func<string, int>(RAND);

            codes["FX15"] = new Func<string, int>(SETTIM);
            codes["FX07"] = new Func<string, int>(TIMSET);
            codes["FX1E"] = new Func<string, int>(IADDVX);
            codes["FX55"] = new Func<string, int>(SAVEREG);
            codes["FX65"] = new Func<string, int>(RECREG);
            codes["FX33"] = new Func<string, int>(BCDSTORE);

            random = new System.Random();
        }

        public int RAND(string code)
        {
            byte random_number = (byte)(random.Next(256));

            char reg = code[1];
            string value = code.Substring(2);

            byte value_number = ConvertNN(value);
            byte result = (byte)(random_number & value_number);

            SetRegisterCode(reg, result);

            Chip.PC += 2;
            return 0;
        }

        


        public int SAVEREG(string code)
        {
            Console.Write("SAVEREG ");

            Console.WriteLine();

            Chip.OutputREG();

            short address = Chip.I;

            Console.WriteLine("\r\n Startng at 0x{0}", address.ToString("X"));

            char reg = code[1];
            
            byte to = ConvertNN(reg.ToString());

            Console.WriteLine("Going to {0}", to);

            for (int i = 0; i <= to; i++)
            {
                char cur_reg = i.ToString()[0];
                byte cur_value = GetRegisterCode(cur_reg);

                Console.WriteLine("0x{0}  <-  V{1} : {2}", (address+i).ToString("X3"), cur_reg, cur_value);
                Chip.MEM[address+i] = cur_value;
            }

            Chip.PC += 2;

            Console.WriteLine();
            Console.WriteLine();

            Chip.OutputMEM(0x3E8, 0x3E8+4);
            Console.WriteLine();

            return 0;
        }

        public int SETTIM(string code)
        {
            Console.WriteLine("SETTIM ");

            char reg = code[1];

            byte value = GetRegisterCode(reg);

            Chip.DELAYTIMER = value;

            Chip.PC += 2;

            return 0;
        }

        public int TIMSET(string code)
        {
            Console.WriteLine("TIMSET ");

            char reg = code[1];

            SetRegisterCode(reg, Chip.DELAYTIMER);

            Chip.PC += 2;

            return 0;
        }

        public int IADDVX(string code)
        {
            char reg = code[1];

            Chip.I += GetRegisterCode(reg);

            Chip.PC += 2;
            return 0;
        }







        public int RECREG(string code)
        {
            Console.Write("RECREG ");

            short address = Chip.I;

            Console.WriteLine("\r\n Starting at {0}", address);

            char reg = code[1];
            
            byte to = ConvertNN(reg.ToString());

            Console.WriteLine("Going through 0-{0}", to);

            for (int i = 0; i <= to; i++)
            {
                char cur_reg = i.ToString()[0];
                byte cur_value = (byte)Chip.MEM[address+i];

                Console.WriteLine("0x{0} : {1}  ->  V{2}", (address+i).ToString("X3"), cur_value, cur_reg);
                SetRegisterCode(cur_reg, cur_value);
            }

            Chip.PC += 2;
            return 0;
        }

        public int BCDSTORE(string code)
        {
            Console.Write("BCDSTORE");

            short address = Chip.I;
            char reg = code[1];

            string value = (GetRegisterCode(reg)).ToString();

            Console.WriteLine("\r\n Parsing value: '{0}'", value);

            byte ones = (byte)Int32.Parse(value[value.Length - 1].ToString());
            byte tens = 0;
            byte hund = 0;

            if (value.Length > 1)
            {
                tens = (byte)Int32.Parse(value[value.Length - 2].ToString());
            }
            if (value.Length > 2)
            {
                hund = (byte)Int32.Parse(value[value.Length - 3].ToString());
            }

            Chip.MEM[address + 2] = ones;
            Chip.MEM[address + 1] = tens;
            Chip.MEM[address + 0] = hund;

            Console.WriteLine("Ones: {0}", ones);
            Console.WriteLine("Tens: {0}", tens);
            Console.WriteLine("Hund: {0}", hund);

            Chip.PC += 2;
            return 0;
        }


        public int MATH0(string code)
        {
            Console.Write("MATH0  ");

            char reg_x = code[1];
            char reg_y = code[2];

            byte value_y = GetRegisterCode(reg_y);
            SetRegisterCode(reg_x, value_y);

            Chip.PC += 2;

            return 0;
        }

        public int MATH1(string code)
        {
            Console.Write("MATH1  ");

            char reg_x = code[1];
            char reg_y = code[2];

            byte value_x = GetRegisterCode(reg_x);
            byte value_y = GetRegisterCode(reg_y);

            byte result = (byte)(value_x | value_y);

            Console.Write(" X: {0} Y: {1} OR: {2}", value_x, value_y, result);

            SetRegisterCode(reg_x, result);

            Chip.PC += 2;

            return 0;
        }

        public int MATH2(string code)
        {
            Console.Write("MATH2  ");

            char reg_x = code[1];
            char reg_y = code[2];

            byte value_x = GetRegisterCode(reg_x);
            byte value_y = GetRegisterCode(reg_y);

            byte result = (byte)(value_x & value_y);

            SetRegisterCode(reg_x, result);

            Chip.PC += 2;
            return 0;
        }

        public int MATH3(string code)
        {
            Console.Write("MATH3  ");

            char reg_x = code[1];
            char reg_y = code[2];

            byte value_x = GetRegisterCode(reg_x);
            byte value_y = GetRegisterCode(reg_y);

            byte result = (byte)(value_x ^ value_y);

            SetRegisterCode(reg_x, result);

            Chip.PC += 2;
            return 0;
        }

        public int MATH4(string code)
        {
            Console.Write("MATH4  ");

            char reg_x = code[1];
            char reg_y = code[2];

            byte value_x = GetRegisterCode(reg_x);
            byte value_y = GetRegisterCode(reg_y);

            byte result = (byte)(value_x + value_y);

            if (result > 255)
            {
                Chip.VF = 1;
                result -= 255;
            }

            SetRegisterCode(reg_x, result);

            Chip.PC += 2;
            return 0;
        }

        public int MATH5(string code)
        {
            Console.Write("MATH5  ");

            char reg_x = code[1];
            char reg_y = code[2];

            byte value_x = GetRegisterCode(reg_x);
            byte value_y = GetRegisterCode(reg_y);

            if (value_x > value_y)
            {
                Chip.VF = 1;
            }

            byte result = (byte)(value_x - value_y);

            SetRegisterCode(reg_x, result);

            Chip.PC += 2;
            return 0;
        }

        public int MATH6(string code)
        {
            Console.Write("MATH6  ");

            char reg_x = code[1];
            char reg_y = code[2];

            byte value_x = GetRegisterCode(reg_x);

            byte result = (byte)(value_x >> 1);

            SetRegisterCode(reg_x, result);

            Chip.PC += 2;
            return 0;
        }

        public int MATH7(string code)
        {
            Console.Write("MATH7  ");

            char reg_x = code[1];
            char reg_y = code[2];

            byte value_x = GetRegisterCode(reg_x);
            byte value_y = GetRegisterCode(reg_y);

            if (value_y > value_x)
            {
                Chip.VF = 1;
            }

            byte result = (byte)(value_y - value_x);

            SetRegisterCode(reg_x, result);

            Chip.PC += 2;
            return 0;
        }

        public int MATHE(string code)
        {
            Console.Write("MATHE  ");

            char reg_x = code[1];
            char reg_y = code[2];

            byte value_x = GetRegisterCode(reg_x);

            byte result = (byte)(value_x << 1);

            SetRegisterCode(reg_x, result);

            Chip.PC += 2;
            return 0;
        }


















        public int SKIPIF(string code)
        {
            Console.Write("SKIPIF");

            char register = code[1];
            string value = code.Substring(2);

            byte register_value = GetRegisterCode(register);
            byte value_number =  ConvertNN(value);

            short forward = 2;

            if (register_value == value_number)
            {
                forward += 2;
            }

            Chip.PC += forward;

            return 0;
        }

        public int NSKIPIF(string code)
        {
            Console.Write("NSKIPIF");

            char register = code[1];
            string value = code.Substring(2);

            byte register_value = GetRegisterCode(register);
            byte value_number =  ConvertNN(value);

            short forward = 4;

            if (register_value == value_number)
            {
                forward -= 2;
            }

            Chip.PC += forward;

            return 0;
        }

        public int SKIPCOMP(string code)
        {
            Console.Write("SKIPIF");

            char register_a = code[1];
            char register_b = code[2];

            byte reg_a_value = GetRegisterCode(register_a);
            byte reg_b_value = GetRegisterCode(register_b);

            short forward = 2;

            if (reg_a_value == reg_b_value)
            {
                forward += 2;
            }

            Chip.PC += forward;

            return 0;
        }

        public int NSKIPCOMP(string code)
        {
            Console.Write("SKIPIF");

            char register_a = code[1];
            char register_b = code[2];

            byte reg_a_value = GetRegisterCode(register_a);
            byte reg_b_value = GetRegisterCode(register_b);

            short forward = 4;

            if (reg_a_value == reg_b_value)
            {
                forward -= 2;
            }

            Chip.PC += forward;

            return 0;
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

            

            // Console.WriteLine("To {0}", value_number);

            Chip.PC = ConvertNNN(value);


            return -1;
        }

        public int RET(string code)
        {
            Console.Write("RET ");

            Chip.PC = Chip.STACK[Chip.SP];
            Chip.SP--;

            Chip.PC += 2;

            return 0;
        }

        public int SUB(string code)
        {
            Console.Write("SUB ");

            string value = code.Substring(1);
            short value_number = ConvertNNN(value);

            Chip.SP++;
            Chip.STACK[Chip.SP] = Chip.PC;

            Chip.PC = value_number;

            return 0;
        }

        public int SETVX(string code)
        {
            Console.Write("SETVX ");

            char register = code[1];
            string value = code.Substring(2);

            Console.Write(" X=0x{0}    Y=        NN=0x{1}", register, value);


            SetRegisterCode(register, ConvertNN(value));



            Chip.PC += 2;
            return 0;
        }

        public int ADDVX(string code)
        {
            Console.Write("ADDVX ");

            char register = code[1];
            string value = code.Substring(2);

            


            // Console.Write("    {0}", value_number);

            byte current_value = GetRegisterCode(register);
            byte addition = ConvertNN(value);
            int new_value = current_value + addition;

            Console.Write(" X=0x{0}    Y=        NN=0x{1}      '{2}' + '{3}' = '{4}'", register, value, current_value, addition, new_value);

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


            // Console.Write("    {0}", value_number);

            Chip.I = ConvertNNN(hex);

            Chip.PC += 2;
            return 0;
        }

        public int DRAW(string code)
        {
            Console.Write("DRAW ");

            char x = code[1];
            char y = code[2];
            string value = code[3].ToString();

            int x_value = GetRegisterCode(x) % 64;
            int y_value = GetRegisterCode(y) % 32;
            int n_value = ConvertNN(value);
            int start_address = Chip.I;

            Chip.VF = 0;

            Console.Write("  X=0x{0}    Y=0x{1}     N=0x{2}", x, y, value);
            Console.Write("    X:{0} Y:{1}", x_value, y_value);

            // Console.WriteLine("\r\nWriting sprite starting at {0}", start_address);

            for (int i = 0; i < n_value; i++)
            {
                int sprite_address = start_address + i;
                int sprite_data = Chip.MEM[sprite_address];

                string output = Convert.ToString(sprite_data, 2).PadLeft(8, '0');

                for (int b = 0; b < output.Length; b++)
                {
                    char l = output[b];
                    if (l == '1')
                    {
                        int loc = ((y_value + i) * 64) + (x_value+b);

                        if (Chip.SCREEN_MEM[loc] == 0)
                        {
                            Chip.SCREEN_MEM[loc] = 1;
                        }
                        else
                        {
                            Chip.SCREEN_MEM[loc] = 0;
                        }
                    }
                }



                // Console.WriteLine("    {0}", output);

                
            }

            


            // Chip.SCREEN_MEM[(GetRegisterCode(y) * 64) + GetRegisterCode(x)] = 1;
            
            


            Chip.PC += 2;
            return 0;
        }

        public byte ConvertNN(string value)
        {
            return (byte)Convert.ToInt32(value, 16);
        }

        public short ConvertNNN(string value)
        {
            return (short)Convert.ToInt32(value, 16);
        }

        public byte GetRegisterCode(char register)
        {
            if (register == '0')            { return Chip.V0; }
            else if (register == '1')       { return Chip.V1; }
            else if (register == '2')       { return Chip.V2; }
            else if (register == '3')       { return Chip.V3; }
            else if (register == '4')       { return Chip.V4; }
            else if (register == '5')       { return Chip.V5; }
            else if (register == '6')       { return Chip.V6; }
            else if (register == '7')       { return Chip.V7; }
            else if (register == '8')       { return Chip.V8; }
            else if (register == '9')       { return Chip.V9; }
            else if (register == 'A')       { return Chip.VA; }
            else if (register == 'B')       { return Chip.VB; }
            else if (register == 'C')       { return Chip.VC; }
            else if (register == 'D')       { return Chip.VD; }
            else if (register == 'E')       { return Chip.VE; }
            else if (register == 'F')       { return Chip.VF; }
            return (byte)0;
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