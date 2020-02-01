using System;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public delegate void CardEvent(Card card);

    public static CardEvent OnCurrentCardChanged;
    public static CardEvent OnCardDiscarded;
    public static CardEvent OnCardAccepted;

    private static Card _current;
    public static Card current
    {
        get { return _current; }
        private set
        {
            if (_current == value)
                return;
            _current = value;
            if (value != null)
                OnCurrentCardChanged(_current);
        }
    }

    [SerializeField]
    private string quote;

    [SerializeField]
    private new string name;

    [SerializeField]
    private int sexyStat;

    [SerializeField]
    private int repairStat;

    [SerializeField]
    private Texture2D image;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AcceptCard()
    {
        OnCardAccepted(this);
    }

    public void DisCard()
    {
        OnCardDiscarded(this);
    }

    public void SetAllData(string n, string q, int s, int r, Texture2D i)
    {
        name = n;
        quote = q;
        sexyStat = s;
        repairStat = r;
        image = i;
    }
}
