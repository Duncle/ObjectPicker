using System.Collections.Generic;
using UnityEngine;

public static class ObjectRegistry
{
    static readonly List<RenderableObject> _all = new();
    public  static IReadOnlyList<RenderableObject> All => _all;

    public static void Register(RenderableObject obj)   => _all.Add(obj);
    public static void Unregister(RenderableObject obj) => _all.Remove(obj);
}
