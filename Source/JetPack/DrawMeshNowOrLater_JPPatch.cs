using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace JetPack;

[HarmonyPatch(typeof(GenDraw), nameof(GenDraw.DrawMeshNowOrLater), typeof(Mesh), typeof(Vector3), typeof(Quaternion),
    typeof(Material), typeof(bool))]
public class DrawMeshNowOrLater_JPPatch
{
    [HarmonyPrefix]
    [HarmonyPriority(800)]
    public static bool PreFix(Mesh mesh, Vector3 loc, Quaternion quat, Material mat, bool drawNow)
    {
        var main = true;
        if (isJetPack(mat))
        {
            loc = convertJetPack(loc);
            main = false;
        }

        if (drawNow)
        {
            mat.SetPass(0);
            Graphics.DrawMeshNow(mesh, loc, quat);
        }
        else
        {
            Graphics.DrawMesh(mesh, loc, quat, mat, 0);
        }

        return main;
    }

    public static bool isJetPack(Material mat)
    {
        var material = mat?.name;
        if (material == null)
        {
            return false;
        }

        foreach (var JPName in JPNames())
        {
            if (material.EndsWith("_north") && mat.name.StartsWith($"Custom/Cutout_{JPName}"))
            {
                return true;
            }
        }

        return false;
    }

    public static List<string> JPNames()
    {
        var list = new List<string>();
        list.AddDistinct("SpacerJetPack");
        list.AddDistinct("BoosterJetPack");
        list.AddDistinct("JT-12_Jetpack");
        list.AddDistinct("PazVizla_Jetpack");
        return list;
    }

    public static Vector3 convertJetPack(Vector3 loc)
    {
        var newloc = loc;
        newloc.y += 0.015625f;
        return newloc;
    }
}