using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HouseUI : MonoBehaviour
{
    [SerializeField] private Image Chimney;
    [SerializeField] private Image Plumbing;
    [SerializeField] private Image Kitchen;
    [SerializeField] private Image Boiler;

    private void OnEnable()
    {
        Player.OnStatChanged += StatsUpdated;
    }

    private void OnDisable()
    {
        Player.OnStatChanged -= StatsUpdated;
    }

    private void Start()
    {
        Chimney.fillAmount = Player.ChimneyStat.Value;
        Plumbing.fillAmount = Player.PlumbingStat.Value;
        Kitchen.fillAmount = Player.KitchenStat.Value;
        Boiler.fillAmount = Player.BoilerStat.Value;
    }

    void StatsUpdated(Player.StatType type, float newValue, float oldValue)
    {
        if (type == Player.StatType.Hotness)
            return;

        Image image = null;
        switch (type)
        {
            case Player.StatType.Chimney:
                image = Chimney;
                break;
            case Player.StatType.Plumbing:
                image = Plumbing;
                break;
            case Player.StatType.Kitchen:
                image = Kitchen;
                break;
            case Player.StatType.Boiler:
                image = Boiler;
                break;
        }
        if (image == null)
            return;

        Debug.LogFormat("{0} : {1}", type, newValue);
        image.fillAmount = newValue;
    }
}
