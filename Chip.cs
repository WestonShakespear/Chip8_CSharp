using System;
using System.IO;

namespace Program
{
    public class Chip
    {
        public static short PC = 0x200;
        public static short I = 0;

        // Create Stack
        public static byte DELAYTIMER = 0;
        public static byte SOUNDTIMER = 0;
        public static byte V0 = 0;
        public static byte V1 = 0;
        public static byte V2 = 0;
        public static byte V3 = 0;
        public static byte V4 = 0;
        public static byte V5 = 0;
        public static byte V6 = 0;
        public static byte V7 = 0;
        public static byte V8 = 0;
        public static byte V9 = 0;
        public static byte VA = 0;
        public static byte VB = 0;
        public static byte VC = 0;
        public static byte VD = 0;
        public static byte VE = 0;
        public static byte VF = 0;

        public static byte SP = 0;

        public static short[] MEM = new short[4096];
        public static short[] STACK = new short[16];
        public static byte[] SCREEN_MEM = new byte[32*64];


        public int loaded = 0;

        private Codes codes;

        public Chip()
        {

            WriteFONT();

            codes = new Codes();
        }   

        public void Cycle()
        {
            if (loaded == 0)
                return;

            
            string code = Fetch();
            if (code == "0000")
            {
                loaded = 0;
                return;
            }

            if (DELAYTIMER > 0)
            {
                DELAYTIMER--;
            }

            
            Console.Write("| PC: 0x{0} #{1} | CODE: 0x{2} | ", PC.ToString("X2"), PC, code);
            Execute(code);
            Console.WriteLine();

            

            
            
        }

        public string Fetch()
        {
            string opcode = "";

            string a = MEM[PC].ToString("X2");
            string b = MEM[PC+1].ToString("X2");

            opcode = a + b;

            return opcode;
        }

        

        public void Execute(string opcode)
        {
            // Console.WriteLine();
            string matchCode = "";

            foreach (KeyValuePair<string, Func<string, int>> entry in codes.codes)
            {
                int match = 0;

                for (int i = 0; i < 4; i++)
                {
                    char codeL = entry.Key[i];
                    char opL = opcode[i];

                    if (codeL == 'X' || codeL == 'Y' || codeL == 'N') continue;

                    if (codeL != opL)
                    {
                        match = 1;
                        break;
                    }
                }

                if (match == 0)
                {
                    matchCode = entry.Key;
                    break;
                }
            }

            if (matchCode.Length != 0)
            {
                Console.Write("TYPE: 0x{0} | ", matchCode);
                codes.codes[matchCode].DynamicInvoke(opcode);
            }
            
        }







        public static void OutputMEM(int start = 0, int end = 4096)
        {
            int line = 0;
            int row = 0;

            for (int i = start; i < end; i++)
            {
                if (line == 0)
                {
                    Console.Write(" {0}: | ", (row).ToString("D3"));
                    row++;
                }

                Console.Write("0x{0}  ", MEM[i].ToString("X3"));
                line++;

                if (line % 5 == 0)
                {
                    Console.Write("  |  ");
                    for (int b = i - 4; b <=  i; b++)
                    {
                        Console.Write("0x{0}  ", b.ToString("X3"));
                    }
                    Console.WriteLine();
                    Console.Write(" {0}: | ", (row).ToString("D3"));
                    row++;
                }
                
            }
        }

        public void OutputSCREEN()
        {
            Console.WriteLine();
            for (int i = 0; i < 32*64; i++)
            {
                Console.Write("{0}  ", SCREEN_MEM[i].ToString());
                if ((i+1) % 64 == 0)
                {
                    Console.WriteLine();
                }
            }
        }

        public void TestSCREEN(byte f)
        {
            for (int i = 0; i < 32*64; i++)
            {
                if ((i) % 64 != 0)
                {
                    if (f == 0)
                    {
                        f = 1;
                    }
                    else
                    {
                        f = 0;   
                    }
                }
                
                SCREEN_MEM[i] = f;
            }
        }


        public void WriteMEM(short[] input, int start = 0, int end = 4096)
        {
            int index = 0;
            for (int i = start; i < end; i++)
            {
                MEM[i] = input[index++];
            }
        }

        public void OutputFONT()
        {
            OutputMEM(0x050, 0x09F+1);
        }

        public void OutputPROGRAM(short end = 0xFFF+1)
        {
            OutputMEM(0x200, end);
        }

        public void WriteFONT()
        {
            WriteMEM(Rom.FONT, 0x050, 0x09F+1);
        }

        public static void OutputREG()
        {
            Console.WriteLine("    V0 = {0}", V0);
            Console.WriteLine("    V1 = {0}", V1);
            Console.WriteLine("    V2 = {0}", V2);
            Console.WriteLine("    V3 = {0}", V3);
            Console.WriteLine("    V4 = {0}", V4);
            Console.WriteLine("    V5 = {0}", V5);
            Console.WriteLine("    V6 = {0}", V6);
            Console.WriteLine("    V7 = {0}", V7);
            Console.WriteLine("    V8 = {0}", V8);
            Console.WriteLine("    V9 = {0}", V9);
            Console.WriteLine("    VA = {0}", VA);
            Console.WriteLine("    VB = {0}", VB);
            Console.WriteLine("    VC = {0}", VC);
            Console.WriteLine("    VD = {0}", VD);
            Console.WriteLine("    VE = {0}", VE);
            Console.WriteLine("    VF = {0}", VF);
        }


        public void ReadFileToROM(string file_path)
        {
            Console.WriteLine("Loading...");
            using(BinaryReader reader = new BinaryReader(File.Open(file_path, FileMode.Open)))
            {
                byte[] v = reader.ReadBytes(0xFFF - 0x200);
                short programSpaceCounter = 0x200;

                for (int i = 0; i < v.Length; i++)
                {
                    MEM[programSpaceCounter] = v[i];
                    programSpaceCounter++;

                    if (i % 2 == 0)
                    {
                        string a = "";
                        string b = "";

                        a =  v[i].ToString("X2");

                        if (i+1 < v.Length)
                        {
                            b = v[i+1].ToString("X2");
                        }

                        string command = a + b;
                        Console.WriteLine("  {0}  0x{1}", i, command);
                    }
                }
            } 
            loaded = 1;
        }
    }
    

    
}