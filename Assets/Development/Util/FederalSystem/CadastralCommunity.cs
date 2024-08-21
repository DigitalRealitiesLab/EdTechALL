using UnityEngine;

public class CadastralCommunity : FederalUnit
{
    [SerializeField]
    protected Renderer unitRenderer;
    public override bool requireHighlightAreaText { get { return true; } }
    public override Bounds bounds { get { return unitRenderer.bounds; } }

    protected void Awake()
    {
        if (!unitRenderer && !TryGetComponent(out unitRenderer))
        {
            Debug.LogError("CadastralCommunity does not have a reference to its renderer assigned");
        }
    }

    public override void HighlightTarget(bool enabled)
    {
        unitRenderer.enabled = enabled;
    }
}
