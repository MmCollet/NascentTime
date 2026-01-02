using System;
using System.Collections.Generic;
using RedBjorn.ProtoTiles;
using RedBjorn.ProtoTiles.Example;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public MapSettings Map;
    public KeyCode GridToggle = KeyCode.G;
    public MapView MapView;
    public UnitController Unit;
    public GameObject TileHighlighterPrefab;
    public MapEntity MapEntity { get; private set; }
    public Leader Player;

    GameObject TileHighlighter;
    TileEntity SelectedTile = null;

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

        FindFirstObjectByType<CameraController>().Init(Map);

        UnitController[] allUnits = new UnitController[0];

#if UNITY_2023_1_OR_NEWER
        allUnits = FindObjectsByType<UnitController>(FindObjectsSortMode.None);
#else
        allUnits = FindObjectsOfType<UnitController>();
#endif

        Array.ForEach(allUnits, unit => unit.Init(MapEntity, () => { Unit = null; }));

        if (TileHighlighterPrefab)
        {
            TileHighlighter = Instantiate(TileHighlighterPrefab);
            TileHighlighter.SetActive(false);
        } else
        {
            Debug.Log("No tile highlighter assigned");
        }
        
        Player = new Leader(Color.violet, "My Country");
        Player.Units = new List<UnitController> (allUnits);
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
            if (Unit != null)
            {
                // handling click when a unit is selected is UnitController's job
            } else if (tile.Empty)
            {
                // select tile
                if (SelectedTile == null)
                {
                    SelectTile(tile);
                } else // unselect tile
                {
                    SelectTile(null);
                }
            } else
            {
                // unselect tile
                SelectTile(null);

                // select unit
                Unit = tile.Unit;
                Unit.Select();
            }
        } else
        {
            // unselect tile
            SelectTile(null);
        }
    }

    void SelectTile(TileEntity tile)
    {
        SelectedTile = tile;

        if (tile != null)
        {
            TileHighlighter?.SetActive(true);
            Vector3 position = MapEntity.WorldPosition(tile.Position);
            Vector3 positionWorld = new (position.x, 0.1f, position.z);
            TileHighlighter.transform.position = positionWorld;
        } else
        {
            TileHighlighter?.SetActive(false);
        }
    }

    public void NewTurn()
    {
        Unit = null;
        SelectedTile = null;
        Player.NewTurn();
    }
}
