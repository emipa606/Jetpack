# GitHub Copilot Instructions for JetPack (Continued) Mod

## Mod Overview and Purpose

**JetPack (Continued)** is a mod for RimWorld that extends the capabilities of pawns by providing them with jet packs. This mod is an update of the original mod by peladors and enhances gameplay by allowing pawns to traverse difficult terrain with ease. The mod also integrates new fuel types and additional functionalities to enhance the strategic possibilities within the game.

## Key Features and Systems

1. **Jet Pack Crafting and Use**:
   - Jet packs can be crafted once researched.
   - Marine power armor can be upgraded to include a jet pack.
   - Pawns can traverse over normal terrains using jet packs with a fuel requirement (default: Chemfuel).

2. **Fuel Management**:
   - Various fuels available: Chemfuel, Kerosene, and Hydrogen Peroxide, each with different efficiencies.
   - Refueling tasks can be automated or manually initiated.
   - Fuel types can be switched, and excess fuel can be dumped.

3. **Movement Mechanics**:
   - Jet packs allow pawns to jump within a defined range that is fuel-limited.
   - Booster packs provide enhanced lateral movement and speed.
   - Jet pack functionality is restricted under roofs and in melee combat.

4. **Additional Equipment**:
   - "Jump Suit" offers fire protection and is worn as an under layer.

5. **Mod Options**:
   - Weapon restrictions, explosion risks, roof punch abilities, and ground movement enhancements.
   - Auto-refueling settings and carrying capacity adjustments affect jump range and fuel use.

## Coding Patterns and Conventions

- **Class Naming**: Classes are named in PascalCase, e.g., `CompJetPack`, `JobDriver_JPRefuel`.
- **Methods**: Method names are in camelCase and clearly describe their actions, e.g., `UseJetPack`, `DoJumpJet`.
- **Encapsulation**: Classes and methods utilize appropriate levels of accessibility (public, internal).

## XML Integration

The mod uses XML to define:
- Research projects: New projects for crafting jet packs.
- Recipes: Convert power armor to jet pack variants.
- Fuel definitions and efficiencies.
- Apparel layering restrictions.

XML files should be well-structured, following RimWorld's schema and conventions for apparel, items, and game mechanics.

## Harmony Patching

- Harmony is used to patch existing RimWorld methods to integrate the jet pack functionality seamlessly.
- Files involved in Harmony patching include `HarmonyPatching.cs` and `ResolveApparelGraphics_JPPostPatch.cs`.
- Ensure that patches are properly annotated and do not cause conflicts with other mods by using unique naming prefixes.

## Suggestions for Copilot

- **Automate Routine Tasks**: Use Copilot to generate boilerplate code for repetitive tasks like method signatures, property definitions, and basic logic.
- **Create XML**: Leverage Copilot to help generate and format XML files for new items, recipes, and research projects using provided templates.
- **Utilize Snippets for Harmony**: Define snippets for standard Harmony patching procedures like `Prefix` and `Postfix` methods.
- **Error Handling**: Use Copilot to suggest exception handling patterns, especially in methods that deal with inventory and fuel logic.
- **Performance**: Implement caching and efficient search logic for AI tasks, benefiting from Copilot’s suggestions on iterating over large datasets efficiently.

By following these instructions and utilizing the capabilities of GitHub Copilot, mod developers can efficiently maintain and enhance the JetPack (Continued) mod.
