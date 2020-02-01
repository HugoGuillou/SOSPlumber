using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{

    [SerializeField]
    private string quote;

    [SerializeField]
    private string name;

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

    public void SetAllData(string n, string q, int s, int r, Texture2D i)
    {
        name = n;
        quote = q;
        sexyStat = s;
        repairStat = r;
        image = i;
    }
}
