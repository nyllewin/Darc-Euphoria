using Darc_Euphoria.Euphoric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darc_Euphoria.Hacks.Injection
{
    public static class RankRevealer
    {
        public static byte[] Shellcode = {
            0x68,0x00,0x00,0x00,0x00,
            0x55,0x89,0xE5,             
            0xB8,0x00,0x00,0x00,0x00,
            0xFF,0xD0,
            0x83,0xC4,0x04,
            0x5D,
            0xC3,
            0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
        };

        public static int Size = Shellcode.Length;
        public static IntPtr Address;

        public static void Show()
        {
            if (gvar.isPanorama) return;


            if (Address == IntPtr.Zero)
            {
                Address = gvar.Alloc.Alloc(Size);
                if (Address == IntPtr.Zero)
                    return;

                Buffer.BlockCopy(BitConverter.GetBytes((int)Address + 20), 0, Shellcode, 1, 4);
                Buffer.BlockCopy(BitConverter.GetBytes(Memory.client + Offsets.dw_RevealRankFn), 0, Shellcode, 9, 4);
                WinAPI.WriteProcessMemory(Memory.pHandle, Address, Shellcode, Shellcode.Length, 0);
            }

            CreateThread.Execute(Address);
        }
    }
}
