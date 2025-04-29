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
        ObjectRegistry.Register(this);   // глобальный список
    }

    public void SetAlpha(float a)
    {
        // получаем экземпляр материала (если хотите избегать клонирования, берите sharedMaterial + MPB)
        var mat = _rend.material;

        /* --- 1. Переводим в Transparent, если ещё Opaque --- */
        if (mat.HasFloat("_Surface") && mat.GetFloat("_Surface") == 0f)
        {
            mat.SetFloat("_Surface", 1f);                       // 0 = Opaque, 1 = Transparent
            mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        }

        /* --- 2. Меняем альфу базового цвета --- */
        if (mat.HasColor("_BaseColor"))
        {
            var c = mat.GetColor("_BaseColor");
            c.a = a;
            mat.SetColor("_BaseColor", c);
        }
        else                                  // fallback на Standard-shader, если вдруг
        {
            var c = mat.color;
            c.a = a;
            mat.color = c;
        }
    }


    public void SetColor(Color col) => _rend.material.color = col;

    public void ToggleVisibility()
    {
        IsHidden = !IsHidden;
        _rend.enabled = !IsHidden;
    }

    private void OnDestroy() => ObjectRegistry.Unregister(this);
}
