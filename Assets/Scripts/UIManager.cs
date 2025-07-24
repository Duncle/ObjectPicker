using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject itemPrefab;

    [Header("Panel")]
    [SerializeField] private Transform listRoot;
    [SerializeField] private Slider alphaSlider;
    [SerializeField] private Button hideAllBtn;
    [SerializeField] private Toggle toggleAll;

    private readonly HashSet<RenderableObject> _selection = new();

    void Start()
    {
        GenerateList();

        alphaSlider.onValueChanged.AddListener(a =>
        {
            foreach (var obj in _selection) obj.SetAlpha(a);
        });

        hideAllBtn.onClick.AddListener(() =>
        {
            foreach (var obj in _selection) obj.ToggleVisibility();
        });

        toggleAll.onValueChanged.AddListener(val =>
        {
            foreach (var t in listRoot.GetComponentsInChildren<Toggle>())
                t.isOn = val;
        });
    }

    void GenerateList()
    {
        foreach (var obj in GetObjectsInHierarchyOrder())
        {
            var go = Instantiate(itemPrefab, listRoot);
            var toggle = go.GetComponentInChildren<Toggle>();
            var eyeBtn = go.transform.Find("EyeButton").GetComponent<Button>();

            toggle.onValueChanged.AddListener(isOn =>
            {
                if (isOn) _selection.Add(obj);
                else _selection.Remove(obj);
            });

            eyeBtn.onClick.AddListener(obj.ToggleVisibility);
        }
    }

    IEnumerable<RenderableObject> GetObjectsInHierarchyOrder() =>
        ObjectRegistry.All.OrderBy(o => o.transform.GetSiblingIndex());
}