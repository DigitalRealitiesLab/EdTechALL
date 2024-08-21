using System.Collections;
using System.Collections.Generic;

public class MunicipalityLookup : JsonLookup<string, Municipality, Municipalities>
{
    protected override void Awake()
    {
        keyName = "MunicipalCode";
        base.Awake();
    }
}

[System.Serializable]
public class Municipality
{
    public string MunicipalCode;
    public string MunicipalityName;
    public string Status;
    public string Population;
}

[System.Serializable]
public class Municipalities : IEnumerable<Municipality>
{
    public Municipality[] SalzburgMunicipalities;

    IEnumerator IEnumerable.GetEnumerator()
    {
        return (IEnumerator)GetEnumerator();
    }

    IEnumerator<Municipality> IEnumerable<Municipality>.GetEnumerator()
    {
        return (IEnumerator<Municipality>)GetEnumerator();
    }

    public MunicipalityEnumerator GetEnumerator()
    {
        return new MunicipalityEnumerator(SalzburgMunicipalities);
    }
}

public class MunicipalityEnumerator : IEnumerator<Municipality>
{
    public Municipality[] EnumeratorMunicipalities;
    int position = -1;

    public MunicipalityEnumerator(Municipality[] list)
    {
        EnumeratorMunicipalities = list;
    }

    public bool MoveNext()
    {
        position++;
        return (position < EnumeratorMunicipalities.Length);
    }

    public void Reset()
    {
        position = -1;
    }

    public void Dispose()
    {

    }

    object IEnumerator.Current
    {
        get
        {
            return Current;
        }
    }

    public Municipality Current
    {
        get
        {
            try
            {
                return EnumeratorMunicipalities[position];
            }
            catch (System.IndexOutOfRangeException)
            {
                throw new System.InvalidOperationException();
            }
        }
    }
}