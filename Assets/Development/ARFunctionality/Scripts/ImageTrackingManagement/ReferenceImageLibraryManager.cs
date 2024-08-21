using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ReferenceImageLibraryManager : MonoBehaviour
{
    public ARTrackedImageManager imageManager;
    public List<XRReferenceImageLibrary> staticReferenceImageLibraries = new List<XRReferenceImageLibrary>();
    private Dictionary<ReferenceLibrarySignature, RuntimeReferenceImageLibrary> runtimeReferenceImageLibraryLookup;

    private void Awake()
    {
        runtimeReferenceImageLibraryLookup = new Dictionary<ReferenceLibrarySignature, RuntimeReferenceImageLibrary>();
        foreach (var staticLibrary in staticReferenceImageLibraries)
        {
            Debug.Assert(staticLibrary.count > 0, "Tried to add a static XRReferenceImageLibrary with 0 reference images - name " + staticLibrary.name);
            XRReferenceImage referenceImage = staticLibrary[0];
            ReferenceLibrarySignature signature = new ReferenceLibrarySignature();
            signature.texture = referenceImage.texture;
            signature.textureName = referenceImage.name;
            signature.widthInMeters = referenceImage.width;
            RuntimeReferenceImageLibrary asRuntimeLibrary = imageManager.CreateRuntimeLibrary(staticLibrary);
            runtimeReferenceImageLibraryLookup.Add(signature, imageManager.CreateRuntimeLibrary(staticLibrary));
        }

        EdTechALLConfig.referenceLibraryChanged.AddListener(OnReferenceLibraryChanged);
        OnReferenceLibraryChanged();
    }

    private void OnReferenceLibraryChanged()
    {
        OnRuntimeReferenceImageLibraryRequest(EdTechALLConfig.referenceLibrary);
    }

    public void OnRuntimeReferenceImageLibraryRequest(ReferenceLibrarySignature signature)
    {
        if (runtimeReferenceImageLibraryLookup.ContainsKey(signature) && imageManager.referenceLibrary != runtimeReferenceImageLibraryLookup[signature])
        {
            SetActiveRuntimeLibrary(runtimeReferenceImageLibraryLookup[signature]);
        }
    }

    private void SetActiveRuntimeLibrary(RuntimeReferenceImageLibrary library)
    {
        imageManager.referenceLibrary = library;
        imageManager.enabled = true;
    }

    private void OnDestroy()
    {
        EdTechALLConfig.referenceLibraryChanged.RemoveListener(OnReferenceLibraryChanged);
    }
}

[System.Serializable]
public struct ReferenceLibrarySignature
{
    public Texture2D texture;
    public string textureName;
    public float widthInMeters;

    public override bool Equals(object obj)
    {
        if (obj is ReferenceLibrarySignature other)
        {
            return textureName == other.textureName;
        }
        return false;
    } 

    public override int GetHashCode()
    {
        return (textureName + widthInMeters).GetHashCode();
    }

    public static bool operator ==(ReferenceLibrarySignature left, ReferenceLibrarySignature right) => left.Equals(right);
    public static bool operator !=(ReferenceLibrarySignature left, ReferenceLibrarySignature right) => !left.Equals(right);

    public override string ToString()
    {
        return "Texture " + texture.name + " texture name " + textureName + " width (m) " + widthInMeters;
    }
}