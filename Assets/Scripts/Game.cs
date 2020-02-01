using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{

    [SerializeField]
    private TextAsset csvFile;

    private List<string> Names;
    private List<string> Quotes;
    private List<string> Genders;
    private List<string> imagePaths;

    // Start is called before the first frame update
    void Start()
    {

        Names = new List<string>();
        Quotes = new List<string>();
        Genders = new List<string>();
        imagePaths = new List<string>();

        RetrieveFromCSV(csvFile);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void RetrieveFromCSV(TextAsset path)
    {
        List<string> lines = new List<string>(csvFile.text.Split("\n"[0]));
        
        // Ignore 2 first lines of csv (1st is blank, 2nd is field names)
        lines.RemoveRange(0, 2);

        foreach (string line in lines)
        {
            string[] lineData = line.Split(',');

            imagePaths.Add(lineData[1]);
            Names.Add(lineData[2]);
            Quotes.Add(lineData[3]);
            Genders.Add(lineData[4]);

            string debugLine = lineData[1] + ", " + lineData[2] + ", " + lineData[3] + ", " + lineData[4];
            Debug.Log(debugLine);

        }

    }
}
