using Darc_Euphoria.Euphoric;
using Darc_Euphoria.Euphoric.Config;
using Darc_Euphoria.Euphoric.Objects;
using Darc_Euphoria.Hacks.Injection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Darc_Euphoria.Hacks
{
    public static class Misc
    {
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

                Triggerbot.Start();
                Bunnyhop.Start();
                SkinChanger.Start();

                Local.NoArms = Settings.userSettings.VisualSettings.NoHands;
                Local.Flash = Settings.userSettings.MiscSettings.FlashAlpha;
                Local.PostProcessingDisable = Settings.userSettings.MiscSettings.NoPostProcessing;

                ClanTagChanger.Start();
                Rank.Start();
                ChatSpammer.Start();
                
                if (Settings.userSettings.VisualSettings.NoScope)
                {
                    if (Local.Scoped) Local.Scoped = false;
                }

                if (Settings.userSettings.VisualSettings.NoScope)
                {
                    if (Local.ActiveWeapon.ScopeLevel == 0)
                    {
                        Local.DrawViewModel = true;
                        Local.Fov = Settings.userSettings.MiscSettings.Fov;
                    }
                    else if (Local.ActiveWeapon.ScopeLevel == 1)
                    {
                        if (Local.ActiveWeapon.WeaponID != 8 && Local.ActiveWeapon.WeaponID != 39)
                            Local.DrawViewModel = false;
                        Local.Fov = 40;
                    }
                    else if (Local.ActiveWeapon.ScopeLevel == 2)
                    {
                        Local.DrawViewModel = false;
                        Local.Fov = 10;
                    }
                }
                else
                {
                    if (!Local.Scoped)
                    {
                        Local.DrawViewModel = true;
                        Local.Fov = Settings.userSettings.MiscSettings.Fov;
                    }
                    else if (Local.ActiveWeapon.ScopeLevel == 1)
                    {
                        if (Local.ActiveWeapon.WeaponID != 8 && Local.ActiveWeapon.WeaponID != 39)
                            Local.DrawViewModel = false;
                        Local.Fov = 40;
                    }
                    else if (Local.ActiveWeapon.ScopeLevel == 2)
                    {
                        Local.DrawViewModel = false;
                        Local.Fov = 10;
                    }
                }

                if (Settings.userSettings.MiscSettings._3rdPerson)
                {
                    if (WinAPI.GetAsyncKeyState(Settings.userSettings.MiscSettings._3rdPersonKey) > 0)
                    {
                        Local.ThirdPerson = !Local.ThirdPerson;
                        Thread.Sleep(500);
                    }
                }
                else if (Local.ThirdPerson)
                {
                    Local.ThirdPerson = false;
                }
            }

        }

    }
}
