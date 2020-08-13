using System;
using System.Reflection;
using HarmonyLib;
using Verse;

namespace JetPack
{
    // Token: 0x02000010 RID: 16
    [StaticConstructorOnStartup]
    internal static class HarmonyPatching
    {
        // Token: 0x06000041 RID: 65 RVA: 0x00003AB1 File Offset: 0x00001CB1
        static HarmonyPatching()
        {
            new Harmony("Pelador.Rimworld.JetPack").PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
