using System.Reflection;
using HarmonyLib;
using Verse;

namespace JetPack;

[StaticConstructorOnStartup]
internal static class HarmonyPatching
{
    static HarmonyPatching()
    {
        new Harmony("Pelador.Rimworld.JetPack").PatchAll(Assembly.GetExecutingAssembly());
    }
}