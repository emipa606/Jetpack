using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace JetPack
{
    // Token: 0x02000002 RID: 2
    [HarmonyPatch(typeof(GenDraw), "DrawMeshNowOrLater")]
    public class DrawMeshNowOrLater_JPPatch
    {
        // Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
        [HarmonyPrefix]
        [HarmonyPriority(800)]
        public static bool PreFix(Mesh mesh, Vector3 loc, Quaternion quat, Material mat, bool drawNow)
        {
            bool main = true;
            if (DrawMeshNowOrLater_JPPatch.isJetPack(mat))
            {
                loc = DrawMeshNowOrLater_JPPatch.convertJetPack(loc);
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

        // Token: 0x06000002 RID: 2 RVA: 0x00002094 File Offset: 0x00000294
        public static bool isJetPack(Material mat)
        {
            string material = mat?.name;
            if (material != null)
            {
                foreach (string JPName in DrawMeshNowOrLater_JPPatch.JPNames())
                {
                    if (material.EndsWith("_north") && mat.name.StartsWith("Custom/Cutout_" + JPName))
                    {
                        return true;
                    }
                }
                return false;
            }
            return false;
        }

        // Token: 0x06000003 RID: 3 RVA: 0x0000211C File Offset: 0x0000031C
        public static List<string> JPNames()
        {
            List<string> list = new List<string>();
            GenCollection.AddDistinct<string>(list, "SpacerJetPack");
            GenCollection.AddDistinct<string>(list, "BoosterJetPack");
            GenCollection.AddDistinct<string>(list, "JT-12_Jetpack");
            GenCollection.AddDistinct<string>(list, "PazVizla_Jetpack");
            return list;
        }

        // Token: 0x06000004 RID: 4 RVA: 0x00002150 File Offset: 0x00000350
        public static Vector3 convertJetPack(Vector3 loc)
        {
            Vector3 newloc = loc;
            newloc.y += 0.015625f;
            return newloc;
        }
    }
}
