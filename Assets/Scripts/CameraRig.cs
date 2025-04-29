using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraRig : MonoBehaviour
{
    [Header("Speeds")]
    [SerializeField] float moveSpeed = 10f;   // pan
    [SerializeField] float zoomSpeed = 5f;    // wheel
    [SerializeField] float rotSpeed  = 3f;    // orbit
    [SerializeField] float focusTime = .25f;  // сек. на плавное подлётывание

    Camera   _cam;
    Vector3  _pivot   = Vector3.zero;
    float    _minDist = 1f;                  // камера не войдёт внутрь объекта
    bool     _isFocusing;

    void Awake() => _cam = GetComponent<Camera>();

    void Update()
    {
        /* ---------- 1. ЛКМ — выбор и фокус ---------- */
        if (Input.GetMouseButtonDown(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            if (Physics.Raycast(_cam.ScreenPointToRay(Input.mousePosition), out var hit))
                StartCoroutine(FocusOn(hit.collider.transform));
        }

        /* ---------- 2. Колёсико — приближение ---------- */
        var scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0 && !_isFocusing)
            _cam.transform.Translate(Vector3.forward * scroll * zoomSpeed, Space.Self);

        /* ---------- 3. MMB — перемещение (pan) ---------- */
        if (Input.GetMouseButton(2) && !_isFocusing)
        {
            var dx = -Input.GetAxisRaw("Mouse X") * moveSpeed * Time.deltaTime;
            var dy = -Input.GetAxisRaw("Mouse Y") * moveSpeed * Time.deltaTime;
            _cam.transform.Translate(new Vector3(dx, dy, 0), Space.Self);
            _pivot += _cam.transform.right * -dx + _cam.transform.up * -dy; // чтобы орбита «держалась»
        }

        /* ---------- 4. ПКМ — орбита вокруг pivot ---------- */
        if (Input.GetMouseButton(1) && !_isFocusing)
        {
            var rotX = Input.GetAxis("Mouse X") * rotSpeed;
            var rotY = -Input.GetAxis("Mouse Y") * rotSpeed;
            _cam.transform.RotateAround(_pivot, Vector3.up,   rotX);
            _cam.transform.RotateAround(_pivot, _cam.transform.right, rotY);
        }
    }

    /* =================  ФОКУС ================= */

    System.Collections.IEnumerator FocusOn(Transform target)
    {
        _isFocusing = true;

        // 1. Новый pivot — центр коллайдера (Bounds)
        var rend = target.GetComponent<Renderer>();
        _pivot   = rend ? rend.bounds.center : target.position;

        // 2. Вычисляем оптимальную дистанцию, чтобы объект целиком влез
        float radius = rend ? rend.bounds.extents.magnitude : 1f;
        float dist   = Mathf.Max(radius / Mathf.Tan(_cam.fieldOfView * 0.5f * Mathf.Deg2Rad), _minDist);

        Vector3 finalPos = _pivot - _cam.transform.forward * dist;

        // 3. Плавное движение камеры
        Vector3 startPos = _cam.transform.position;
        float   t        = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / focusTime;
            _cam.transform.position = Vector3.Slerp(startPos, finalPos, t);
            yield return null;
        }

        _isFocusing = false;
    }
}
