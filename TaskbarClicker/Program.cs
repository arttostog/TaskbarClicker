using System;
using System.Windows.Forms;

namespace TaskbarClicker
{
    internal static class Program
    {
        [STAThread]
        static void Main() => Application.Run(new Clicker());
    }
}
