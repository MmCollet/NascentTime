using UnityEngine;

public class TerrainTile : MonoBehaviour
{
    protected TerrainType type;


    TerrainTile(TerrainType type)
    {
        this.type = type;
    }
}
