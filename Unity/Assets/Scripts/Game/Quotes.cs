using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using JsonFx.Json;

public class QuoteCollection
{
    public string topic;
    public List<QuoteData> quotes;
}

public class QuoteData
{
    public string quote;
    public string author;
}

public class Quotes : MonoBehaviour
{
    private List<QuoteCollection> allQuotes;

    private static Quotes instance = null;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        if (instance != null && instance != this)
            Destroy(gameObject);
    }

    private void Start()
    {
        allQuotes = new List<QuoteCollection>();

        Object[] objs = Resources.LoadAll("quotes/", typeof(TextAsset));

        for (int i = 0; i < objs.Length; ++i)
        {
            TextAsset jsonText = objs[i] as TextAsset;

            JsonReaderSettings settings = new JsonReaderSettings();
            JsonReader json = new JsonReader(jsonText.text as string, settings);

            QuoteCollection qc = json.Deserialize<QuoteCollection>();

            allQuotes.Add(qc);
        }
    }

    public static QuoteData GetQuote()
    {
        return instance.allQuotes.GetRandom().quotes.GetRandom();
    }
}
