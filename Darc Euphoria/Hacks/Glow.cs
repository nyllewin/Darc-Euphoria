using Darc_Euphoria.Euphoric.Config;
using Darc_Euphoria.Euphoric.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Darc_Euphoria.Euphoric.Structs;
using static Darc_Euphoria.Euphoric.MathFuncs;
using Darc_Euphoria.Euphoric;

namespace Darc_Euphoria.Hacks
{
    class Glow
    {
        public static GlowColor glowColor = new GlowColor();
        public static GlowSettings glowSettings = new GlowSettings();
        public static Settings.UserSettings.Visuals visuals = new Settings.UserSettings.Visuals();
        public static Settings.UserSettings.VisColors visColors = new Settings.UserSettings.VisColors();

        public static void Start()
        {
            int GlowObjectPtr = Memory.Read<int>(Memory.client + Offsets.dwGlowObjectManager);
            while (true)
            {
                if (gvar.isShuttingDown)
                {
                    while (gvar.SHUTDOWN != 4)
                        Thread.Sleep(1);
                    gvar.SHUTDOWN++;
                    break;
                }

                Thread.Sleep(10);

                visuals = Settings.userSettings.VisualSettings;
                visColors = Settings.userSettings.VisualColors;
                glowSettings = new GlowSettings(true, true, visuals.FullBloom);

                if (!Local.InGame) continue;
                if (!visuals.Enabled) continue;
                if (!visuals.Glow && !visuals.PseudoChams) continue;

                foreach (Entity player in EntityList.List)
                {
                    if (!visuals.DisplayTeam && player.isTeam) continue;

                    if (visuals.PseudoChams)
                    {
                        if (player.isTeam)
                        {
                            glowColor = visColors.Team_Chams.toGlow();
                            player.renderColor = visColors.Team_Chams.toRender();
                        }
                        else
                        {
                            glowColor = visColors.Enemy_Chams.toGlow();
                            player.renderColor = visColors.Enemy_Chams.toRender();
                        }
                    }
                    else
                    {
                        if (player.isTeam)
                        {
                            if (!visuals.DisplayTeam) continue;

                            if (player.Visible) glowColor = visColors.Team_Glow_Visible.toGlow();
                            else glowColor = visColors.Team_Glow_Hidden.toGlow();
                        }
                        else
                        {
                            if (player.Visible) glowColor = visColors.Enemy_Glow_Visible.toGlow();
                            else glowColor = visColors.Enemy_Glow_Hidden.toGlow();
                        }
                    }
                    
                    Memory.Write<GlowColor>(GlowObjectPtr + (player.GlowIndex * 0x38 + 0x4), glowColor);
                    Memory.Write<GlowSettings>(GlowObjectPtr + (player.GlowIndex * 0x38 + 0x24), glowSettings);

                    if (visuals.PseudoChams)
                        Memory.Write<int>(GlowObjectPtr + (player.GlowIndex * 0x38 + 0x2C), 1);
                }
                
            }
        }

    }
}
