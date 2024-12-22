namespace Eco.Mods.TechTree
{

    using Eco.Core.Plugins.Interfaces;

    public class Eco420 : IModInit
    {
        public static ModRegistration Register() => new()
        {
            ModName = "Eco420",
            ModDescription = "A mod that adds a new crafting tables, items, and recipes for Cannabis production.",
            ModDisplayName = "Eco420",
        };
    }
}
