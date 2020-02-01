using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HotnessUI : MonoBehaviour
{

    [SerializeField] private Image[] Hotness;

    private void OnEnable()
    {
        Player.OnStatChanged += HotnessUpdated;
    }

    private void OnDisable()
    {
        Player.OnStatChanged -= HotnessUpdated;
    }

    private void Start()
    {
        for (int i = 0; i < Hotness.Length; ++i)
            SetHotnessLevel(i, 0.5f);
    }

    void HotnessUpdated(Player.StatType type, float hotness, float oldHotness)
    {
        if (type != Player.StatType.Hotness)
            return;
        int newStep = Mathf.Clamp((int)(hotness * 4f), 0, 3);
        int oldStep = Mathf.Clamp((int)(oldHotness * 4f), 0, 3);
        if (newStep != oldStep)
        {
            SetHotnessLevel(oldStep, hotness);
            if (newStep > oldStep)
            {
                Hotness[oldStep].transform.DOPunchScale(Vector3.one * 0.2f, 1f, 10, 1f);
            }
        }
        SetHotnessLevel(newStep, hotness);
    }

    void SetHotnessLevel(int step, float value)
    {
        Hotness[step].fillAmount = (value * 4f) - step;
    }
}
