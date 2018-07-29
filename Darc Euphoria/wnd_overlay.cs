using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using AlphaMode = SharpDX.Direct2D1.AlphaMode;
using Factory = SharpDX.Direct2D1.Factory;
using TextAntialiasMode = SharpDX.Direct2D1.TextAntialiasMode;

using Darc_Euphoria.Euphoric;
using Darc_Euphoria.Hacks.Injection;
using Darc_Euphoria.Euphoric.Config;
using Darc_Euphoria.Hacks;
using Darc_Euphoria.Euphoric.Objects;
using static Darc_Euphoria.Euphoric.Structs;
using System.Diagnostics;

namespace Darc_Euphoria
{
    public partial class wnd_overlay : Form
    {
        public const int GWL_EXSTYLE = -20;
        public const int WS_EX_LAYERED = 0x80000;
        public const int WS_EX_TRANSPARENT = 0x20;
        public const int LWA_ALPHA = 0x2;
        public const int LWA_COLORKEY = 0x1;

        private static RenderTarget Device;

        Thread SharpDXThread = new Thread(new ThreadStart(dxThread))
        {
            Priority = ThreadPriority.Highest,
        };

        public wnd_overlay()
        {
            this.Paint += Wnd_overlay_Paint;

            int initialStyle = WinAPI.GetWindowLong(this.Handle, -20);
            WinAPI.SetWindowLong(this.Handle, -20,  initialStyle | 0x80000 | 0x20 | 0x80);

            InitializeComponent();

            var factory = new Factory();
            var renderProp = new HwndRenderTargetProperties()
            {
                Hwnd = this.Handle,
                PixelSize = new Size2(this.Width, this.Height),
                PresentOptions = PresentOptions.Immediately,
            };

            Device = new WindowRenderTarget(factory,
                new RenderTargetProperties(new PixelFormat(Format.B8G8R8A8_UNorm, AlphaMode.Premultiplied)),
                renderProp);

            this.Visible = false;
            factory.Dispose();
            
            updateWnd.RunWorkerAsync();
            SharpDXThread.Start();
        }

        public static int _RED_TO_GREEN = 765;

        private void Wnd_overlay_Paint(object sender, PaintEventArgs e)
        {
            Margins margins = new Margins()
            {
                Left = 0,
                Top = 0,
                Right = this.Width,
                Bottom = this.Height,
            };

            WinAPI.DwmExtendFrameIntoClientArea(this.Handle, ref margins);
        }

        private void updateWnd_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                if (gvar.isShuttingDown)
                {
                    break;
                }

                if (_RED_TO_GREEN > 0)
                    _RED_TO_GREEN--;
                else
                    _RED_TO_GREEN = 765;

                this.Invoke((MethodInvoker)delegate ()
                {
                    if (gvar.isShuttingDown == true)
                    {
                        this.Close();
                    }

                    if (gvar.OverlaySize != Size)
                    {
                        Rect rect = new Rect();
                        WinAPI.GetClientRect(Memory.process.MainWindowHandle, out rect);
                        this.Size = new Size(rect.Right, rect.Bottom);
                        int initialStyle = WinAPI.GetWindowLong(this.Handle, -20);
                        WinAPI.SetWindowLong(this.Handle, -20, initialStyle | 0x80000 | 0x20 | 0x80);
                        gvar.OverlaySize = Size;
                        isReady = true;
                    }

                    Point point = new Point();
                    WinAPI.ClientToScreen(Memory.process.MainWindowHandle, out point);
                    this.Location = point;
                    gvar.OverlayPoint = Location;
                });
                Thread.Sleep(100);
            }
        }

        private void wnd_overlay_Resize(object sender, EventArgs e)
        {
            var factory = new Factory();
            var renderProp = new HwndRenderTargetProperties()
            {
                Hwnd = this.Handle,
                PixelSize = new Size2(this.Width, this.Height),
                PresentOptions = PresentOptions.Immediately,
            };

            Device = new WindowRenderTarget(factory,
                new RenderTargetProperties(new PixelFormat(Format.B8G8R8A8_UNorm, AlphaMode.Premultiplied)),
                renderProp);
            factory.Dispose();
        }


        public static void DrawText(string text, int x, int y)
        {
            SolidColorBrush dBrushBack = new SolidColorBrush(Device, Color.FromArgb(1, 1, 1).toRawColor4());
            SolidColorBrush dBrush = new SolidColorBrush(Device, Settings.userSettings.VisualColors.World_Text.toRawColor4());

            if (text == "Darc Euphoria")
            {
                if (_RED_TO_GREEN > 510)
                {
                    dBrush.Color = Color.FromArgb(_RED_TO_GREEN - 510, 255 - (_RED_TO_GREEN - 510), 0).toRawColor4();
                }
                else if (_RED_TO_GREEN > 255)
                {
                    dBrush.Color = Color.FromArgb(0, _RED_TO_GREEN - 255, 255 - (_RED_TO_GREEN - 255)).toRawColor4();
                }
                else
                {
                    dBrush.Color = Color.FromArgb(255 - _RED_TO_GREEN, 0, _RED_TO_GREEN).toRawColor4();
                }
            }
            else
            {
                Device.DrawText(text, gvar.textFormat, MathFuncs.StringSize(text, x-1, y), dBrushBack);
                Device.DrawText(text, gvar.textFormat, MathFuncs.StringSize(text, x+1, y), dBrushBack);
                Device.DrawText(text, gvar.textFormat, MathFuncs.StringSize(text, x, y-1), dBrushBack);
                Device.DrawText(text, gvar.textFormat, MathFuncs.StringSize(text, x, y+1), dBrushBack);
            }

            Device.DrawText(text, gvar.textFormat, MathFuncs.StringSize(text, x, y), dBrush);
            dBrushBack.Dispose();
            dBrush.Dispose();
        }

        private static bool isReady = false;
        
        private static void dxThread()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            gvar.SHUTDOWN++;
            while (gvar.isRunning)
            {
                if (gvar.isShuttingDown)
                {
                    gvar.SHUTDOWN--;
                    break;
                }
                var frameLength = 1000f / gvar.Fps;

                if (gvar.RefreshID == int.MaxValue)
                    gvar.RefreshID = 0;

                gvar.RefreshID++;

                try
                {
                    #region Begin Device
                    Device.BeginDraw();
                    Device.Clear(new RawColor4(0, 0, 0, 0));
                    Device.TextAntialiasMode = TextAntialiasMode.Aliased;
                    Device.AntialiasMode = AntialiasMode.Aliased;
                    #endregion

                    if (!isReady)
                    {
                        Device.EndDraw();
                        continue;
                    }

                    if (Settings.userSettings.MiscSettings.Watermark)
                        DrawText("Darc Euphoria", 4, 4);

                    if (Settings.userSettings.MiscSettings.LocalTime)
                    {
                        if (Settings.userSettings.MiscSettings.Watermark)
                        {
                            DrawText(DateTime.Now.ToString("h:mm:ss tt"), 4, (int)(MathFuncs.MeasureString("DarcEuphoria").Height) - 4);
                        }
                        else
                        {
                            DrawText(DateTime.Now.ToString("h:mm:ss tt"), 4, 4);
                        }


                    }

                    if (!Local.InGame)
                    {
                        Device.EndDraw();
                        continue;
                    }

                    ESP.Start(Device);

                    if ((Settings.userSettings.VisualSettings.SniperCrosshair && Local.ActiveWeapon.isSniper()) || 
                        Settings.userSettings.VisualSettings.RecoilCrosshair)
                    {
                        using (SolidColorBrush brush = new SolidColorBrush(Device, Color.White.toRawColor4()))
                        {
                            var radAngle = Settings.userSettings.MiscSettings.Fov * (3.14f / 180f);
                            var radHFov = 2 * Math.Atan(Math.Tan(radAngle / 2f) * gvar.AspectRatio);
                            var hFov = radHFov * (180f / 3.14f);
                            var perc = gvar.OverlaySize.Width / hFov;

                            var rcsPunchVec = Local.PunchAngle * .59999f;
                            rcsPunchVec *= -1;
                            rcsPunchVec *= (float)perc;
                            rcsPunchVec *= Local.ScopeScale;

                            var point = new RawVector2(gvar.OverlaySize.Width / 2 + rcsPunchVec.x, gvar.OverlaySize.Height / 2 - rcsPunchVec.y);


                            var p1 = point;
                            var p2 = point;
                            var p3 = point;
                            var p4 = point;
                            p1.X -= 5;
                            p2.X += 5;
                            p3.Y -= 5;
                            p4.Y += 5;
                            Device.DrawLine(p1, p2, brush, 2);
                            Device.DrawLine(p3, p4, brush, 2);
                        }
                    }
                    
                    if (Settings.userSettings.VisualSettings.DrawAimbotFov)
                    {
                        var radAngle = Local.Fov * (3.14f / 180f);
                        var radHFov = 2 * Math.Atan(Math.Tan(radAngle / 2f) * gvar.AspectRatio);
                        var hFov = radHFov * (180f / 3.14f);

                        var perc = gvar.OverlaySize.Width / hFov;

                        var radius = Aimbot.AimbotSettings.Fov * (float)perc;

                        var math = Math.Sqrt((gvar.OverlaySize.Width / 2) * (gvar.OverlaySize.Width / 2) + 
                            (gvar.OverlaySize.Height / 2) * (gvar.OverlaySize.Height / 2));

                        if (radius < math)
                        {
                            var rcsPunchVec = Local.PunchAngle * .59999f;
                            rcsPunchVec *= -1;
                            rcsPunchVec *= (float)perc;
                            rcsPunchVec *= Local.ScopeScale;

                            RawVector2 center = new RawVector2(gvar.OverlaySize.Width / 2 + rcsPunchVec.x, gvar.OverlaySize.Height / 2 - rcsPunchVec.y);

                            using (SolidColorBrush brush = new SolidColorBrush(Device, Color.White.toRawColor4()))
                                Device.DrawEllipse(new Ellipse(center, radius, radius), brush);
                        }

                    }

                    

                    Device.EndDraw();
                } catch { Thread.Sleep(10); }

                
                var delayLength = frameLength - stopwatch.ElapsedMilliseconds;
                if (delayLength > 0 && !float.IsInfinity(frameLength))
                    Thread.Sleep((int)delayLength);

                stopwatch.Restart();

            }
            Device.Dispose();
        }

        private void wnd_overlay_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!gvar.isShuttingDown)
                e.Cancel = true;
        }
    }
}
