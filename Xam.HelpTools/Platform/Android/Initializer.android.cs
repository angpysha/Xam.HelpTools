using System;
using System.Collections.Generic;
using System.Text;
using Xam.HelpTools.Effects.BrushedtextColor;

namespace Xam.HelpTools.Platform.Android
{
    public static class Initializer
    {
        public static void Initialize()
        {
            Console.WriteLine($"{typeof(AndroidTextOptionsEffect).FullName} inited");
            Console.WriteLine($"{typeof(BrushedTextColorPlatformEffect).FullName} inited");
        }
    }
}
