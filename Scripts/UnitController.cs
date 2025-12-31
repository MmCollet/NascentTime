using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RedBjorn.ProtoTiles;
using RedBjorn.ProtoTiles.Example;
using RedBjorn.Utils;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    public float Speed = 5;
    public float Range = 10f;
    public Transform RotationNode;
    public AreaOutline AreaPrefab;
    public PathDrawer PathPrefab;


    float mobility = 10f;
    bool isSelected = false;
    MapEntity Map;
    AreaOutline Area;
    PathDrawer Path;
    Coroutine MovingCoroutine;
    Action Unselect;

    void Update()
    {
        if (isSelected)
        {
            if (MyInput.GetOnWorldUp(Map.Settings.Plane()))
            {
                HandleWorldClick();
            }
            PathUpdate();
        }
    }

    public void Init(MapEntity map, Action unselectHandler)
    {
        Unselect = unselectHandler;
        Map = map;
        map.Tile(transform.position).Unit = this;
        Area = Spawner.Spawn(AreaPrefab, Vector3.zero, Quaternion.identity);
        PathCreate();
        Path.IsEnabled = false;
        PathHide();
    }

    void HandleWorldClick()
    {
        var clickPos = MyInput.GroundPosition(Map.Settings.Plane());
        var tile = Map.Tile(clickPos);
        if (tile != null && tile.Vacant && tile.Empty && tile.Weight != float.MaxValue)
        {
            AreaHide();
            Path.IsEnabled = false;
            PathHide();
            var path = Map.PathTiles(transform.position, clickPos, Range);

            if (path.Any())
            {
                path.Last().Unit = this;
                Map.Tile(transform.position).Unit = null;
            }

            Move(path, () =>
            {
                Path.IsEnabled = true;
                AreaShow();
            });
        } else
        {
            // Makes sure the unit is unselected at the end of the turn, so that the rest of the code doesn't consider
            // that no unit was selected
            StartCoroutine(RunAtEndOfFrame(() =>
            {
                isSelected = false;
                AreaHide();
                Path.IsEnabled = false;
                PathHide();
                Unselect();
            }));
        }
    }

    public void Move(List<TileEntity> path, Action onCompleted)
    {
        if (path != null && path.Any())
        {
            if (MovingCoroutine != null)
            {
                StopCoroutine(MovingCoroutine);
            }
            MovingCoroutine = StartCoroutine(Moving(path, onCompleted));
        }
        else
        {
            onCompleted.SafeInvoke();
        }
    }

    IEnumerator Moving(List<TileEntity> path, Action onCompleted)
    {
        var nextIndex = 0;
        transform.position = Map.Settings.Projection(transform.position);

        while (nextIndex < path.Count)
        {
            if (nextIndex != 0) Range-=path[nextIndex].Weight;
            var targetPoint = Map.WorldPosition(path[nextIndex]);
            var stepDir = (targetPoint - transform.position) * Speed;
            if (Map.RotationType == RotationType.LookAt)
            {
                RotationNode.rotation = Quaternion.LookRotation(stepDir, Vector3.up);
            }
            else if (Map.RotationType == RotationType.Flip)
            {
                RotationNode.rotation = Map.Settings.Flip(stepDir);
            }
            var reached = stepDir.sqrMagnitude < 0.01f;
            while (!reached)
            {

                transform.position += stepDir * Time.deltaTime;
                reached = Vector3.Dot(stepDir, (targetPoint - transform.position)) < 0f;
                yield return null;
            }
            transform.position = targetPoint;
            nextIndex++;
        }
        onCompleted.SafeInvoke();
    }

    void AreaShow()
    {
        AreaHide();
        Area.Show(Map.WalkableBorder(transform.position, Range), Map);
    }

    void AreaHide()
    {
        Area.Hide();
    }

    void PathCreate()
    {
        if (!Path)
        {
            Path = Spawner.Spawn(PathPrefab, Vector3.zero, Quaternion.identity);
            Path.Show(new List<Vector3>() { }, Map);
            Path.InactiveState();
            Path.IsEnabled = true;
        }
    }

    void PathHide()
    {
        if (Path)
        {
            Path.Hide();
        }
    }

    void PathUpdate()
    {
        if (Path && Path.IsEnabled)
        {
            var tile = Map.Tile(MyInput.GroundPosition(Map.Settings.Plane()));
            if (tile != null && tile.Vacant && tile.Weight != float.MaxValue)
            {
                var path = Map.PathPoints(transform.position, Map.WorldPosition(tile.Position), Range);
                Path.Show(path, Map);
                Path.ActiveState();
                Area.ActiveState();
            }
            else
            {
                Path.InactiveState();
                Area.InactiveState();
            }
        }
    }

    public void Select()
    {
        // Make sure we don't update in the frame the unit get selected
        StartCoroutine(RunAtEndOfFrame(() =>
        {
            isSelected = true;
            Path.IsEnabled = true;
            PathUpdate();
            AreaShow();
        }));
        
    }

    IEnumerator RunAtEndOfFrame(Action toRun)
    {
        yield return new WaitForEndOfFrame();
        toRun();
    }
}
