# GitHub Copilot Instructions for RimWorld Modding Project: JetPack (Continued)

## Mod Overview and Purpose
JetPack (Continued) is an update to peladors JetPack mod for RimWorld. It provides jet pack capabilities to pawns, allowing them to traverse above normal terrain, similar to power armor but with a jet pack variant. The mod adds support for Simple FX: Smoke and includes new fuel types like Kerosene for efficient operation of the jet packs. It also features a range of customization options, such as excluding the use of WMDs with jet packs, applying DFA (Death From Above), and allowing roof punching. The mod is compatible with several other RimWorld mods, including RPG Style Inventory, Tactical Charged Gear, Medical Supplements, and Rimefeller.

## Key Features and Systems
- **Jet Pack**: A wearable item that provides aerial mobility to pawns. It requires fuel (Chemfuel, Kerosene, or Hydrogen Peroxide) to operate.
- **Marine Power Armor with Jet Pack**: A variant of the Marine Power Armor that includes a jet pack for enhanced mobility.
- **Fuel Efficiency**: Different fuel types have varying efficiencies, with Kerosene being the most efficient.
- **Mod Options**: A range of customization options to adjust the behavior and mechanics of the jet packs.
- **Compatibility**: Supports integration with other RimWorld mods like RPG Style Inventory, Tactical Charged Gear, Medical Supplements, and Rimefeller.

## Coding Patterns and Conventions
- **Namespaces**: Use `JetPack` as the main namespace for all mod-related classes and methods.
- **Class Structure**: Organize classes based on functionality, such as `CompJetPack` for jet pack components, `Command_JetPack` for jet pack commands, and `Controller` for settings management.
- **Naming Conventions**: Follow standard C# naming conventions, with PascalCase for class names and camelCase for method and variable names.
- **Error Handling**: Implement robust error handling to manage unexpected situations, such as fuel depletion or damaged jet packs.

## XML Integration
- **ThingDefs**: Define new items like jet packs, fuels, and generators using `ThingDef`.
- **RecipeDefs**: Create recipes for converting standard power armor into the jet pack version and for distilling Kerosene.
- **ResearchProjectDefs**: Define research projects required to unlock jet pack capabilities.
- **ThinkTreeDefs**: Implement auto-refuel behavior using `ThinkTreeDef`.
- **PreExplosionSpawnThingDefs**: Define pre-explosion effects for jet pack explosions.

Example `ThingDef` for a jet pack:
xml
<ThingDef ParentName="Apparel_JetPack_Booster" Name="JetPack_Booster">
    <thingCategories><li>Apparel</li></thingCategories>
    <cost_WC 450/>
    <fuelPerJump 5/>
    <fuelPerSecond 2/>
    <minJumpHeight 3/>
    <maxJumpHeight 20/>
    <fuelType>Chemfuel</fuelType>
</ThingDef>


## Harmony Patching
- **Harmony**: Use Harmony to patch existing RimWorld methods to integrate new features without modifying the core game files.
- **Patch Structure**: Organize patches based on the target methods, such as `Pawn_Jump` for jet pack jumping mechanics and `Pawn_Refuel` for refueling operations.
- **Patch Naming**: Name patches clearly to indicate their purpose, e.g., `JetPack_Patch_JumpMechanics` and `JetPack_Patch_Refuel`.

Example Harmony patch:
csharp
[HarmonyPatch(typeof(Pawn), nameof(Pawn.Jump))]
public static class JetPack_Patch_JumpMechanics
{
    public static bool Prefix(Pawn __instance)
    {
        // Custom jump mechanics code
        return true;
    }
}


## Suggestions for Copilot
- **Focus on Jet Pack Mechanics**: Provide examples and snippets for implementing jet pack mechanics, including fuel consumption, jump range, and movement speed.
- **XML Defs**: Offer templates and examples for creating `ThingDef`, `RecipeDef`, and other XML definitions related to jet packs.
- **Harmony Patches**: Share examples of Harmony patches for integrating jet pack features into existing RimWorld methods.
- **Error Handling**: Provide snippets for implementing error handling to manage issues like fuel depletion or damaged jet packs.
- **Mod Options**: Offer examples for creating and managing mod options, including UI elements for customization.

By following these guidelines, you can effectively use GitHub Copilot to enhance your JetPack (Continued) mod for RimWorld, ensuring a smooth development process and high-quality code.

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.


## Hard rules (must follow)
- Do NOT run commands that modify the repo (no git commit, git apply, dotnet format) unless explicitly asked.
- Prefer minimal reads: read only the smallest code region needed (around the suspicious lines).
- When mentioning SonarQube issues, automatically use the SonarQube MCP service to fetch and address issues instead of making inferred fixes without querying SonarQube first.
- When mentioning the rimworld log, automatically use the Rimworld MCP service to fetch the log.

