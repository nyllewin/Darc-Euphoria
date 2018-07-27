using Darc_Euphoria.Euphoric;
using Darc_Euphoria.Euphoric.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Darc_Euphoria.Euphoric.Structs;

namespace Darc_Euphoria.Hacks
{
    public static class EntityList
    {
        public static Entity[] List;
        public static ItemObjects[] ItemList;
        public static bool update = false;


        public static void Start()
        {
            while (true)
            {
                if (gvar.isShuttingDown)
                {
                    while (gvar.SHUTDOWN != 2)
                        Thread.Sleep(1);

                    gvar.SHUTDOWN++;
                    break;
                }

                Thread.Sleep(10);
                if (!Local.InGame) continue;

                List = Entity.EntityList;
                ItemList = ItemObjects.ItemList;

                if (update)
                {
                    if (Local.MapFile != null)
                    {
                        Local._bsp = new Euphoric.BspParsing.BSP(Local.MapFile);
                        Thread.Sleep(1000);
                    }
                    update = false;
                }
            }

        }

    }
}
