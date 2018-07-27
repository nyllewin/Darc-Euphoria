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
            while (true)
            {
                if (gvar.isShuttingDown)
                {
                    while (gvar.SHUTDOWN != 5)
                        Thread.Sleep(1);
                    gvar.SHUTDOWN++;
                    break;
                }

                Thread.Sleep(10);

                if (!Local.InGame) continue;

                ClanTagChanger.Start();
                Rank.Start();

                if (Settings.userSettings.MiscSettings.ChatSpammer)
                {
                    Random r = new Random();
                    int t = r.Next(0, 9);
                    switch (t)
                    {
                        case 0: ClientCMD.Exec("say Get Mediocre, Go Euphoric."); break;
                        case 1: ClientCMD.Exec("say Darc Euphoira, an alright C# External Cheat."); break;
                        case 2: ClientCMD.Exec("say Get Mediocre, Get Darc Euphoria."); break;
                        case 3: ClientCMD.Exec("say Don't cry if I kill you. It's just a good gaming chair."); break;
                        case 4: ClientCMD.Exec("say If you say Darc Euphoria is shit, I won't argue, cause it is."); break;
                        case 5: ClientCMD.Exec("say Phansea owns Darc Euphoria and me."); break;
                        case 6: ClientCMD.Exec("say Why you mad? Is it because I rejected your confession?"); break;
                        case 7: ClientCMD.Exec("say If you kill me it's luck."); break;
                        case 8: ClientCMD.Exec("say If I kill you it's because you suck."); break;
                        default: break;
                    }
                    Thread.Sleep(20);
                }

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
                        Local.Fov = 0;
                        if (Local.ActiveWeapon.WeaponID != 8 && Local.ActiveWeapon.WeaponID != 39)
                            Local.DrawViewModel = false;
                    }
                    else if (Local.ActiveWeapon.ScopeLevel == 2)
                    {
                        Local.Fov = 10;
                        Local.DrawViewModel = false;
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
                        Local.Fov = 40;
                        if (Local.ActiveWeapon.WeaponID != 8 && Local.ActiveWeapon.WeaponID != 39)
                            Local.DrawViewModel = false;
                    }
                    else if (Local.ActiveWeapon.ScopeLevel == 2)
                    {
                        Local.Fov = 10;
                        Local.DrawViewModel = false;
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

                Local.Flash = Settings.userSettings.MiscSettings.FlashAlpha;
                Local.PostProcessingDisable = Settings.userSettings.MiscSettings.NoPostProcessing;
                Local.NoArms = Settings.userSettings.VisualSettings.NoHands;

            }

        }

    }
}
