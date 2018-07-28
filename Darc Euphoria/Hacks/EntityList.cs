using Darc_Euphoria.Euphoric;
using Darc_Euphoria.Euphoric.Objects;
using System;
using System.Collections.Generic;
using System.IO;
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
        public static Entity player;
        public static Entity[] List;
        public static ItemObjects[] ItemList;
        public static bool Update = false;
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

                if (Update)
                {
                    List = Entity.EntityArray;
                    ItemList = ItemObjects.ItemList;
                    Update = false;
                }
                

                string MapPath = string.Format(@"{0}\csgo\maps\{1}.bsp", 
                    Memory.process.Modules[0].FileName.Substring(0, Memory.process.Modules[0].FileName.Length - 9), Local.MapName);

                if (Local.InGame && Local._bsp.FileName != MapPath)
                {
                    LoadMap(MapPath);
                }
            }
        }

        public static void LoadMap(string MapPath)
        {
            if (File.Exists(MapPath) && Local.ActiveWeapon.WeaponID != -1)
            {
                Local._bsp.Dispose();
                Local._bsp = new Euphoric.BspParsing.BSP(MapPath);
            }
        }

    }
}
