using System.Collections.Generic;
using UnityEngine;

public class Lord : Knight
{
    public List<Knight> KnightVassals { get; set; }

    public override void NewTurn()
    {
        KnightVassals?.ForEach(knight => knight.NewTurn());
        base.NewTurn();
    }
}