using System;
using UnityEngine;

namespace RedBjorn.ProtoTiles
{
    [Serializable]
    public partial class TileEntity : INode
    {
        int CachedMovabeArea;
        int ObstacleCount;
        public TileData Data { get; private set; }
        public TilePreset Preset { get; private set; }
        MapRules Rules;

        public int MovableArea { get { return CachedMovabeArea; } set { CachedMovabeArea = value; } }
        public bool Vacant
        {
            get
            {
                if (Data == null || Rules == null || Rules.IsMovable == null)
                {
                    return true;
                }
                return Rules.IsMovable.IsMet(this) && ObstacleCount == 0;
            }
        }

        public bool Empty
        {
            get
            {
                if (Data == null) return true;
                return Data.Unit == null;
            }
        }

        public UnitController Unit
        {
            get
            {
                if (Data == null) return null;
                return Data.Unit;
            }
            set
            {
                if (Data != null)
                {
                    Data.Unit = value;
                }
            }
        }

        public bool Visited { get; set; }
        public bool Considered { get; set; }
        public float Depth { get; set; }
        public float[] NeighbourMovable { get { return Data == null ? null : Data.SideHeight; } }
        public Vector3Int Position { get { return Data == null ? Vector3Int.zero : Data.TilePos; } }

        public float Weight
        { 
            get 
            {
                if (Rules == null || Rules.IsWaterTile == null) return 1;
                if (Rules.IsWaterTile.IsMet(this)) return float.MaxValue;
                if (Rules.IsForrestTile.IsMet(this)) return 2;
                if (Rules.IsHillTile.IsMet(this)) return 2;
                // Other types...
                
                return 1;
            } 
        }

        TileEntity() { }

        public TileEntity(TileData preset, TilePreset type, MapRules rules)
        {
            Data = preset;
            Rules = rules;
            Preset = type;
            MovableArea = Data.MovableArea;
        }

        public override string ToString()
        {
            return string.Format("Position: {0}. Vacant = {1}", Position, Vacant);
        }

        public void ChangeMovableAreaPreset(int area)
        {
            Data.MovableArea = area;
        }
    }
}

