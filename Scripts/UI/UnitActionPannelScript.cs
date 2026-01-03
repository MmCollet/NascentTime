using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitActionPannelScript : MonoBehaviour
{
    UnitController Unit;
    GameController GameController;
    Image BorderBackground;
    TextMeshProUGUI UnitNameText;
    TextMeshProUGUI UnitHealthText;
    TextMeshProUGUI UnitRangeText;
    TextMeshProUGUI UnitAttackText;

    void Start()
    {
        GameController = FindFirstObjectByType<GameController>();
        Unit = GameController.Unit;

        BorderBackground = transform.Find("Background/Border").GetComponent<Image>();
        UnitNameText = transform.Find("Details/Name").GetComponent<TextMeshProUGUI>();
        UnitHealthText = transform.Find("Details/Health/Text").GetComponent<TextMeshProUGUI>();
        UnitRangeText = transform.Find("Details/Mobility/Text").GetComponent<TextMeshProUGUI>();
        UnitAttackText = transform.Find("Details/Attack/Text").GetComponent<TextMeshProUGUI>();

        UnitNameText.overflowMode = TextOverflowModes.Ellipsis;
    }

    void Update()
    {
        Unit = GameController.Unit;
        // Show the pannel if a unit is asigned, hide it otherwise
        if (Unit)
        {
            var leader = Unit.Leader;
            var color = leader.Color();
            BorderBackground.color = color;
            UnitNameText.text = Unit.Name;
            UnitHealthText.text = Unit.Health.ToString();
            UnitRangeText.text = Unit.Range.ToString();
            UnitAttackText.text = Unit.Attack.ToString();
        }
    }

    public void Show(bool active)
    {
        gameObject.SetActive(active);
    }
}
