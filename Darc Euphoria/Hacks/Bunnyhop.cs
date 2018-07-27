using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Darc_Euphoria.Euphoric;
using Darc_Euphoria.Euphoric.Config;
using Darc_Euphoria.Euphoric.Objects;

namespace Darc_Euphoria.Hacks
{
    class Bunnyhop
    {
        public static void Start()
        {
            while (true)
            {
                if (gvar.isShuttingDown)
                {
                    while (gvar.SHUTDOWN != 1)
                        Thread.Sleep(1);

                    gvar.SHUTDOWN++;
                    break;
                }

                Thread.Sleep(10);
                if (gvar.isMenu) continue;
                if (!Local.InGame) continue;
                if (!Settings.userSettings.MiscSettings.BunnyHop) continue;
                if ((WinAPI.GetAsyncKeyState(32) & 0x8000) <= 0) continue;
                if (Local.Flags != 257 && Local.Flags != 263) continue;

                Local.Jump();
            }
        }
    }
}
