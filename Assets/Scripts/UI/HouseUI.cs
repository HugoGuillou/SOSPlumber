using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HouseUI : MonoBehaviour
{
    [SerializeField]
    [Range(0f, 1f)]
    private float _LowerThreshold = 0.2f;

    [SerializeField]
    private Color _LowValueColor = Color.red;
    [SerializeField]
    private Color _NormalValueColor = Color.white;
    [SerializeField]
    private Color _MaxValueColor = Color.green;

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
        SetValue(Chimney, Player.ChimneyStat.Value);
        SetValue(Plumbing, Player.PlumbingStat.Value);
        SetValue(Kitchen, Player.KitchenStat.Value);
        SetValue(Boiler, Player.BoilerStat.Value);
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

        SetValue(image, newValue);
    }

    private void SetValue(Image image, float value)
    {
        image.fillAmount = value;
        if (Mathf.Approximately(value, 1f))
            image.color = _MaxValueColor;
        else
            image.color = value < _LowerThreshold ? _LowValueColor : _NormalValueColor;
    }
}
