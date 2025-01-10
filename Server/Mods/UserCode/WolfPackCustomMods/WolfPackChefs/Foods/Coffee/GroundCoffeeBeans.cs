
namespace Eco.Mods.TechTree
{
    using System.Collections.Generic;
    using Eco.Core.Items;
    using Eco.Gameplay.Components;
    using Eco.Gameplay.Items;
    using Eco.Gameplay.Players;
    using Eco.Gameplay.Skills;
    using Eco.Gameplay.Systems.TextLinks;
    using Eco.Shared.Localization;
    using Eco.Shared.Serialization;
    using Eco.Shared.Utils;
    using Eco.Shared.Time;
    using Eco.Core.Controller;
    using Eco.Gameplay.Items.Recipes;

    [Serialized] // Tells the save/load system this object needs to be serialized. 
    [LocDisplayName("Ground Coffee Beans")] // Defines the localized name of the item.
    [Weight(350)] // Defines how heavy the GroundCoffeeBeans is.
    [Ecopedia("Food", "Mixology", createAsSubPage: true)]
    [LocDescription("Coffee beans grounded with a blender.  oh the anticipation.")] //The tooltip description for the food item.
    public partial class GroundCoffeeBeansItem : FoodItem
    {


        /// <summary>The amount of calories awarded for eating the food item.</summary>
        public override float Calories => 100;
        /// <summary>The nutritional value of the food item.</summary>
        public override Nutrients Nutrition => new Nutrients() { Carbs = 0, Fat = 0, Protein = 1, Vitamins = 0 };

        /// <summary>Defines the default time it takes for this item to spoil. This value can be modified by the inventory this item currently resides in.</summary>
        protected override float BaseShelfLife => (float)TimeUtil.HoursToSeconds(96);
    }


    /// <summary>
    /// <para>Server side recipe definition for "GroundCoffeeBeans".</para>
    /// <para>More information about RecipeFamily objects can be found at https://docs.play.eco/api/server/eco.gameplay/Eco.Gameplay.Items.RecipeFamily.html</para>
    /// </summary>
    /// <remarks>
    /// This is an auto-generated class. Don't modify it! All your changes will be wiped with next update! Use Mods* partial methods instead for customization. 
    /// If you wish to modify this class, please create a new partial class or follow the instructions in the "UserCode" folder to override the entire file.
    /// </remarks>
    [RequiresSkill(typeof(MixologySkill), 2)]
    [Ecopedia("Food", "Mixology", subPageName: "Ground Coffee Beans Item")]
    public partial class GroundCoffeeBeansRecipe : RecipeFamily
    {
        public GroundCoffeeBeansRecipe()
        {
            var recipe = new Recipe();
            recipe.Init(
                name: "GroundCoffeeBeans",  //noloc
                displayName: Localizer.DoStr("Ground Coffee Beans"),

            // Defines the ingredients needed to craft this recipe. An ingredient items takes the following inputs
            // type of the item, the amount of the item, the skill required, and the talent used.
                ingredients: new List<IngredientElement>
                {
                         new IngredientElement(typeof(RoastedCoffeeBeansItem), 3, typeof(MixologySkill), typeof(MixologyLavishResourcesTalent)),
                },

                // Define our recipe output items.
                // For every output item there needs to be one CraftingElement entry with the type of the final item and the amount
                // to create.
                items: new List<CraftingElement>
                {
                    new CraftingElement<GroundCoffeeBeansItem>(1)
                });
            this.Recipes = new List<Recipe> { recipe };
            this.ExperienceOnCraft = 1; // Defines how much experience is gained when crafted.

            // Defines the amount of labor required and the required skill to add labor
            this.LaborInCalories = CreateLaborInCaloriesValue(15, typeof(MixologySkill));

            // Defines our crafting time for the recipe
            this.CraftMinutes = CreateCraftTimeValue(beneficiary: typeof(GroundCoffeeBeansRecipe), start: 3f, skillType: typeof(MixologySkill), typeof(MixologyFocusedSpeedTalent), typeof(MixologyParallelSpeedTalent));

            // Perform pre/post initialization for user mods and initialize our recipe instance with the display name "Ground Coffee Beans"
            this.ModsPreInitialize();
            this.Initialize(displayText: Localizer.DoStr("Ground Coffee Beans"), recipeType: typeof(GroundCoffeeBeansRecipe));
            this.ModsPostInitialize();

            // Register our RecipeFamily instance with the crafting system so it can be crafted.
            CraftingComponent.AddRecipe(tableType: typeof(BlenderObject), recipe: this);
        }

        /// <summary>Hook for mods to customize RecipeFamily before initialization. You can change recipes, xp, labor, time here.</summary>
        partial void ModsPreInitialize();

        /// <summary>Hook for mods to customize RecipeFamily after initialization, but before registration. You can change skill requirements here.</summary>
        partial void ModsPostInitialize();
    }


}
