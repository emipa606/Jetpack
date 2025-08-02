using Mlie;
using UnityEngine;
using Verse;

namespace JetPack;

public class Controller : Mod
{
    private static Settings settings;
    public static string CurrentVersion;

    public Controller(ModContentPack content) : base(content)
    {
        settings = GetSettings<Settings>();
        CurrentVersion =
            VersionFromManifest.GetVersionFromModMetaData(content.ModMetaData);
    }

    public override string SettingsCategory()
    {
        return "JetPack.Name".Translate();
    }

    public override void DoSettingsWindowContents(Rect canvas)
    {
        Settings.DoWindowContents(canvas);
    }
}