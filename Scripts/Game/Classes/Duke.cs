using System.Collections.Generic;
using UnityEngine;

public class Duke : Lord
{
    public List<Lord> LordVassals { get; set; }

    public override void NewTurn()
    {
        base.NewTurn();
        Debug.Log("New Turn Duke");
    }
}