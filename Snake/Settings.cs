using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake
{
    internal class Settings
    {
        public static float height { get; set; }
        public static float width { get; set; }

        public static string directions;

        public Settings()
        {
            //storleken av varje cirkel x & y
            height = 16;
            width = 16;

            //start riktning för ormen
            directions = "left";
        }
    }
}
