using System.Collections.Generic;
using UnityEngine;

public class Duke : Lord
{
    public List<Lord> LordVassals { get; set; }

    public override void NewTurn()
    {
        LordVassals?.ForEach(lord => lord.NewTurn());
        base.NewTurn();
    }
}