﻿// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.
// <auto-generated from FoodTemplate.tt/>

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
    using Eco.Core.Controller;
    using Eco.Gameplay.Items.Recipes;
    using Eco.Shared.Time;




    //Item


    

    [Serialized]
    [LocDisplayName("Wheat Paste Glue")]
    [Weight(100)]
    [MaxStackSize(420)]
    [LocDescription("A glue made from wheat flour.  Please do not eat the glue.")]
    public partial class WheatPasteGlueItem : FoodItem
    {

        public override float Calories => -100;

        public override Nutrients Nutrition => new Nutrients()

        {
            Carbs = 0,
            Fat = 0,
            Protein = 0,
            Vitamins = 0
        };

        protected override float BaseShelfLife => (float)TimeUtil.HoursToSeconds(120);

    }


    //Recipe


    [RequiresSkill(typeof(CookingSkill), 3)]
    public partial class WheatPasteGlueRecipe : RecipeFamily
    {
        public WheatPasteGlueRecipe()
        {
            var recipe = new Recipe();
            recipe.Init(
                name: "WheatPasteGlue",  //noloc
                displayName: Localizer.DoStr("Wheat Paste Glue"),


                ingredients: new List<IngredientElement>
                {
                    new IngredientElement(typeof(FlourItem), 15, typeof(CookingSkill), typeof(CookingLavishResourcesTalent)),
                    new IngredientElement(typeof(SugarItem), 3, typeof(CookingSkill), typeof(CookingLavishResourcesTalent)),

                },

                items: new List<CraftingElement>
                {
                    new CraftingElement<WheatPasteGlueItem>(5)
                });
            this.Recipes = new List<Recipe> { recipe };

            this.ExperienceOnCraft = 1f;

            this.LaborInCalories = CreateLaborInCaloriesValue(420);

            
            this.CraftMinutes = CreateCraftTimeValue(beneficiary: typeof(WheatPasteGlueRecipe), start: 3f, skillType: typeof(CookingSkill), typeof(CookingFocusedSpeedTalent), typeof(CookingParallelSpeedTalent));

            
            this.ModsPreInitialize();
            this.Initialize(displayText: Localizer.DoStr("Wheat Paste Glue"), recipeType: typeof(WheatPasteGlueRecipe));
            this.ModsPostInitialize();


    CraftingComponent.AddRecipe(tableType: typeof(CastIronStoveObject), recipe: this);
    }

    partial void ModsPreInitialize();

    partial void ModsPostInitialize();
}
}