using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Darc_Euphoria.Euphoric.BspParsing.Enums;
using static Darc_Euphoria.Euphoric.Structs;

namespace Darc_Euphoria.Euphoric.BspParsing
{
    public class Structs
    {
        public struct Header
        {
            public int ident;
            public int version;
            public Lump[] lumps;
            public int mapRevision;
        }

        public struct Brush
        {
            public int firstside, numsides;
            public ContentsFlag contents;
        }

        public struct Brushside
        {
            public ushort planenum;
            public short texinfo, dispinfo, bevel;
        }

        public struct Plane
        {
            public Vector3 normal;
            public float distance;
            public int type;
        }

        public struct Lump
        {
            public int offset, length, version, fourCC;
            public LumpType type;
        }

        public struct Node
        {
            public int planenum;     
            public int[] children;  
            public short[] mins;      
            public short[] maxs;     
            public ushort firstface;     
            public ushort numfaces;    
            public short area;     
            public short paddding;  
        };

        public struct Leaf
        {
            public ContentsFlag contents;  
            public short cluster;      
            public short area;          
            public short flags;    
            public short[] mins;    
            public short[] maxs;   
            public ushort firstleafface;   
            public ushort numleaffaces;
            public ushort firstleafbrush;      
            public ushort numleafbrushes;
            public short leafWaterDataID;
        }

        public struct Face
        {
            public ushort planeNumber;
            public byte side;
            public byte onNode;
            public int firstEdge;
            public short numEdges;
            public short texinfo;
            public short dispinfo;
            public short surfaceFogVolumeID;
            public byte[] styles;
            public int lightOffset;
            public float area;
            public int[] LightmapTextureMinsInLuxels;
            public int[] LightmapTextureSizeInLuxels;
            public int originalFace;
            public ushort numPrims;
            public ushort firstPrimID;
            public uint smoothingGroups;
        }
    }
}
