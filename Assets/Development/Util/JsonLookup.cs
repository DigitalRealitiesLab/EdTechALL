using System.Collections.Generic;
using UnityEngine;

public class JsonLookup<K, V, S> : MonoBehaviour where S: IEnumerable<V>
{
    public TextAsset sourceData;
    protected string keyName;
    protected Dictionary<K, V> lookup = new Dictionary<K, V>();
    private static JsonLookup<K, V, S> instance;

    public static JsonLookup<K, V, S> Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (JsonLookup<K, V, S>)FindObjectOfType(typeof(JsonLookup<K, V, S>));

                if (instance == null)
                {
                    var singletonObject = new GameObject();
                    instance = singletonObject.AddComponent<JsonLookup<K, V, S>>();
                    singletonObject.name = typeof(JsonLookup<K, V, S>).ToString() + " (Singleton)";
                }
            }

            return instance;
        }
    }

    protected virtual void Awake()
    {
        Debug.Assert(sourceData, "JsonLookup is missing a source data file");
        foreach (V data in JsonUtility.FromJson<S>(sourceData.text))
        {
            lookup.Add((K)data.GetType().GetField(keyName).GetValue(data), data);
        }
    }

    public V GetAt(K key)
    {
        if (lookup.ContainsKey(key))
        {
            return lookup[key];
        }
        return default(V);
    }
}