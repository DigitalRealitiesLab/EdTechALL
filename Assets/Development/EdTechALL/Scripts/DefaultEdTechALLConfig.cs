using UnityEngine;
using UnityEngine.XR.ARSubsystems;

public class DefaultEdTechALLConfig : MonoBehaviour
{
    public XRReferenceImageLibrary defaultImageLibrary;
    public TextureToPrefabMapper defaultTextureToPrefabMapper;

    private void Awake()
    {
        if(EdTechALLConfig.hasReferenceLibrary)
        {
            XRReferenceImage referenceImage = defaultImageLibrary[0];
            ReferenceLibrarySignature signature = new ReferenceLibrarySignature();
            signature.texture = referenceImage.texture;
            signature.textureName = referenceImage.name;
            signature.widthInMeters = referenceImage.width;
        }

        if (!EdTechALLConfig.hasPrefabMapper)
        {
            EdTechALLConfig.prefabMapper = defaultTextureToPrefabMapper;
        }
    }
}