namespace Program
{
    public class Program
    {
        public static void Main(string[] args)
        {
            int option = 0;

            Chip chip = new Chip();

            string[] programs = {
                "IBM Logo.ch8",
                "test_opcode.ch8",
                "Trip8.ch8",
                "Sierpinski [Sergey Naydenov, 2010].ch8",
                "Particle Demo [zeroZshadow, 2008].ch8"
            };
            // chip.TestSCREEN(0);
            chip.ReadFileToROM(programs[0]);

            
            if (option == 0)
            {

                using (Game game = new Game(640*2, 320*2, "Test", chip))
                {
                    game.Run();
                }
            }
            else
            {
                chip.OutputPROGRAM(0x200+200);

                for (int i = 0; i < 40; i++)
                {
                    chip.Cycle();

               
                }
            }
            
            // chip.OutputFONT();

            // chip.TestSCREEN(0);
            // chip.OutputSCREEN();

            // chip.TestSCREEN(1);
            // chip.OutputSCREEN();
        }
    }
}