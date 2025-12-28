using System;
using RedBjorn.ProtoTiles;
using RedBjorn.ProtoTiles.Example;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public MapSettings Map;
    public KeyCode GridToggle = KeyCode.G;
    public MapView MapView;
    public UnitController Unit;

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

        UnitController[] allUnits = new UnitController[0];

#if UNITY_2023_1_OR_NEWER
        allUnits = FindObjectsByType<UnitController>(FindObjectsSortMode.None);
#else
        allUnits = FindObjectsOfType<UnitController>();
#endif

        Array.ForEach(allUnits, unit => unit.Init(MapEntity));
    }

    void Update()
    {
        if (InputActionsProvider.GridToggleReleased)
        {
            MapEntity.GridToggle();
        }

        if (MyInput.GetOnWorldUp(Map.Plane()))
        {
            HandleWorldClick();
        }
    }

    void HandleWorldClick()
    {
        var clickPos = MyInput.GroundPosition(Map.Plane());
        var tile = MapEntity.Tile(clickPos);
        if (tile != null && tile.Vacant)
        {
            if (tile.Empty && Unit == null)
            {
                // select tile
            } else if (!tile.Empty)
            {
                Unit = tile.Unit;
                Unit.Select();
            }
        }
    }
}
