
// N   4  bit
// NN  8  bit
// NNN 12 bit

Console.WriteLine("Hex Test");

string[] N = {"2", "9", "C", "F"};

string[] NN = { "01", "10", "5C", "84"};

string[] NNN = { "001", "228", "5AC", "8B4"};


foreach (string N_i in N)
{

    byte v = (byte)Convert.ToInt32(N_i, 16);

    Console.WriteLine("The hex value '{0}' is '{1}'", N_i, v);
}
Console.WriteLine("\r\n");

foreach (string N_i in NN)
{

    byte v = (byte)Convert.ToInt32(N_i, 16);

    Console.WriteLine("The hex value '{0}' is '{1}'", N_i, v);
}
Console.WriteLine("\r\n");

foreach (string N_i in NNN)
{

    short v = (short)Convert.ToInt32(N_i, 16);

    Console.WriteLine("The hex value '{0}' is '{1}'", N_i, v);

    int vi = Convert.ToInt32(N_i, 16);

    Console.WriteLine("The hex value '{0}' is '{1}'", N_i, (short)vi);
}