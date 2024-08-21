using UnityEngine;

namespace InteractionSystem
{
    public class Sensable : MonoBehaviour
    {
        public Sensable root { get { return _root; } }
        // highest sensable in the scenegraph object hierarchy
        protected Sensable _root;

        protected virtual void Awake()
        {
            if (!_root)
            {
                FindRoot();
            }
        }

        protected void FindRoot()
        {
            Transform current = transform;
            Sensable currentSensable;
            while (current && current.TryGetComponent(out currentSensable))
            {
                current = current.parent;
                _root = currentSensable;
            }
        }
    }

    public interface IInteractable
    {
        public void Interact();
    }
}