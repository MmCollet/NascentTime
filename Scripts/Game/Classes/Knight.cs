using System.Collections.Generic;
using UnityEngine;

public class Knight
{
    public List<UnitController> Units { get; set; }

    public virtual void NewTurn()
    {
        Units?.ForEach(unit => unit.NewTurn());
    }
}
