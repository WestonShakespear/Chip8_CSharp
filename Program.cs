namespace Program
{
    public class Program
    {
        public static void Main(string[] args)
        {
            
            using (Game game = new Game(640*2, 320*2, "Test"))
            {
                game.Run();
            }
            // Chip chip = new Chip();
            
            // chip.ReadFileToROM("IBM Logo.ch8");

            // for (int i = 0; i < 1000; i++)
            // {
            //     chip.Cycle();
            // }
            // chip.OutputPROGRAM(0x200+200);
            // chip.OutputFONT();

            // chip.TestSCREEN(0);
            // chip.OutputSCREEN();

            // chip.TestSCREEN(1);
            // chip.OutputSCREEN();
        }
    }
}