using UnityEngine;

[RequireComponent(typeof(Renderer), typeof(Collider))]
public class RenderableObject : MonoBehaviour
{
    Renderer _rend;
    Color _original;
    public bool IsHidden { get; private set; }

    void Awake()
    {
        _rend = GetComponent<Renderer>();
        _original = _rend.material.color;
        ObjectRegistry.Register(this);
    }

    public void SetAlpha(float a)
    {
        var mat = _rend.material;
        
        if (mat.HasFloat("_Surface") && mat.GetFloat("_Surface") == 0f)
        {
            mat.SetFloat("_Surface", 1f);
            mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        }
        
        if (mat.HasColor("_BaseColor"))
        {
            var c = mat.GetColor("_BaseColor");
            c.a = a;
            mat.SetColor("_BaseColor", c);
        }
        else
        {
            var c = mat.color;
            c.a = a;
            mat.color = c;
        }
    }

    public void ToggleVisibility()
    {
        IsHidden = !IsHidden;
        _rend.enabled = !IsHidden;
    }

    private void OnDestroy() => ObjectRegistry.Unregister(this);
}
