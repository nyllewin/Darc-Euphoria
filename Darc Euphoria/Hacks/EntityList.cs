using Darc_Euphoria.Euphoric;
using Darc_Euphoria.Euphoric.Objects;
using Darc_Euphoria.Hacks.Injection;
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
        public static void Start()
        {
            gvar.SHUTDOWN++;
            while (true)
            {
                if (gvar.isShuttingDown)
                {
                    gvar.SHUTDOWN--;
                    break;
                }

                Thread.Sleep(1);

                if (!Local.InGame) continue;
            
                List = Entity.EntityArray;
                ItemList = ItemObjects.ItemList;

                ESP.matrix = Memory.Read<Matrix4x4>(Memory.client + Offsets.dwViewMatrix);

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
                Local._bsp = new Euphoric.BspParsing.BSP(MapPath);
                ClientCMD.Exec(String.Format("clear; echo Map File {0} Loaded!", Local._bsp.FileName));
            }
        }

    }
}
