using System.Collections.Generic;
using UnityEngine;

public class Leader : Duke
{
    public List<Duke> DukeVassals { get; set; }
    Color LeaderColor;
    string CountryName = "Default";

    public Leader(Color color, string name)
    {
        LeaderColor = color;
        CountryName = name;
    }

    public override void NewTurn()
    {
        DukeVassals?.ForEach(duke => duke.NewTurn());
        base.NewTurn();
    }

    public Color Color()
    {
        return LeaderColor!=null ? LeaderColor : UnityEngine.Color.blue;
    }

    public string Name()
    {
        return CountryName;
    }
}