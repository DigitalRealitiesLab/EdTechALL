using UnityEngine;
using UnityEngine.Events;

public static class EdTechALLConfig
{
    public static UnityEvent referenceLibraryChanged = new UnityEvent();
    public static bool hasReferenceLibrary => _referenceLibrary != default(ReferenceLibrarySignature);
    private static ReferenceLibrarySignature _referenceLibrary = default(ReferenceLibrarySignature);
    public static ReferenceLibrarySignature referenceLibrary
    {
        get
        {
            Debug.Assert(hasReferenceLibrary, "EdTechALLConfig has no referenceLibrary set");
            return _referenceLibrary;
        }
        set
        {
            if (_referenceLibrary == value)
            {
                return;
            }
            _referenceLibrary = value;
            referenceLibraryChanged.Invoke();
        }
    }

    public static bool isReferenceImageMarkerBig => _referenceLibrary.widthInMeters > 1.0f;

    public static UnityEvent prefabMapperChanged = new UnityEvent();
    public static bool hasPrefabMapper => _prefabMapper != null;
    private static TextureToPrefabMapper _prefabMapper = null;
    public static TextureToPrefabMapper prefabMapper
    {
        get
        {
            Debug.Assert(_prefabMapper, "EdTechALLConfig has no prefabMapper set");
            return _prefabMapper;
        }
        set
        {
            if (_prefabMapper == value)
            {
                return;
            }
            _prefabMapper = value;
            if (_prefabMapper & !_prefabMapper.initialized)
            {
                _prefabMapper.Initialize();
            }
            prefabMapperChanged.Invoke();
        }
    }

    public static float mapPositionLerpFraction = 0.05f;
    public static float mapRotationLerpFraction = 0.1f;
}