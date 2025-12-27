using RedBjorn.ProtoTiles;
using RedBjorn.ProtoTiles.Example;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public MapSettings Map;
    public KeyCode GridToggle = KeyCode.G;
    public MapView MapView;
    public UnitMove Unit;

    public MapEntity MapEntity { get; private set; }

    void Start()
    {
        if (!MapView)
        {
#if UNITY_2023_1_OR_NEWER
            MapView = FindFirstObjectByType<MapView>();
#else
            MapView = FindObjectOfType<MapView>();
#endif
        }
        MapEntity = new MapEntity(Map, MapView);
        if (MapView)
        {
            MapView.Init(MapEntity);
        }
        else
        {
            Debug.Log("Can't find MapView. Random errors can occur");
        }
    }

    void Update()
    {
        if (InputActionsProvider.GridToggleReleased)
        {
            MapEntity.GridToggle();
        }
    }
}
