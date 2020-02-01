using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HotnessUI : MonoBehaviour
{

    [SerializeField] private Image Hotness0;
    [SerializeField] private Image Hotness1;
    [SerializeField] private Image Hotness2;
    [SerializeField] private Image Hotness3;

    private void OnEnable()
    {
        Card.OnCardAccepted += CardAccepted;
        Card.OnCardDiscarded += CardDiscarded;
    }

    private void OnDisable()
    {
        Card.OnCardAccepted -= CardAccepted;
        Card.OnCardDiscarded -= CardDiscarded;
    }

    void CardAccepted(Card card)
    {

    }

    void CardDiscarded(Card card)
    {

    }
}
