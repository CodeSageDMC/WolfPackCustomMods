﻿// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.
// <auto-generated from FoodTemplate.tt/>

namespace Eco.Mods.TechTree
{
    using Eco.Core.Items;
    using Eco.Gameplay.Items;
    using Eco.Gameplay.Players;
    using Eco.Shared.Localization;
    using Eco.Shared.Serialization;
    using Eco.Shared.Utils;
    using Eco.Shared.Time;
    using System;
    using System.Collections.Generic;
    using Eco.Gameplay.Components;
    using Eco.Gameplay.DynamicValues;
    using Eco.Gameplay.Skills;
    using Eco.Shared.Items;
    using Eco.World;
    using Eco.World.Blocks;
    using Gameplay.Systems.TextLinks;
    using Eco.Core.Controller;
    using Eco.Gameplay.Settlements.ClaimStakes;
    using Eco.Gameplay.Items.Recipes;




    //FoodItem


    [Serialized]
    [LocDisplayName("Decarboxylated Cannabis")]
    [Tag("Cannabis")]
    [Weight(72)]
    [MaxStackSize(420)]
    [LocDescription("Ground cannabis buds that have been decarbed at 245ºF in an oven.")]
    public partial class DecarboxylatedCannabisItem : FoodItem
    {

        public override float Calories => 0;

        public override Nutrients Nutrition => new Nutrients()

        {
            Carbs = 0,
            Fat = 3,
            Protein = 0,
            Vitamins = 2
        };

        protected override float BaseShelfLife => (float)TimeUtil.HoursToSeconds(64);

    }




    //Recipe


    [RequiresSkill(typeof(BakingSkill), 4)]
   
    public partial class DecarboxylatedCannabisRecipe : RecipeFamily
    {
        public DecarboxylatedCannabisRecipe()
        {
            var recipe = new Recipe();
            recipe.Init(
                name: "DecarboxylatedCannabis",  //noloc
                displayName: Localizer.DoStr("Decarboxylate Ground Cannabis"),

                ingredients: new List<IngredientElement>
                {
                    new IngredientElement(typeof(GroundCannabisItem), 42, true),
                    
                },

                items: new List<CraftingElement>
                {
                    new CraftingElement<DecarboxylatedCannabisItem>(42),
                });

            this.Recipes = new List<Recipe> { recipe };

            this.ExperienceOnCraft = 1;

            this.LaborInCalories = CreateLaborInCaloriesValue(420);

            this.CraftMinutes = CreateCraftTimeValue(
                beneficiary: typeof(DecarboxylatedCannabisRecipe),
                start: 3f,
                skillType: typeof(BakingSkill), typeof(BakingFocusedSpeedTalent), typeof(BakingParallelSpeedTalent));

            this.ModsPreInitialize();

            this.Initialize(displayText: Localizer.DoStr("Decarboxylate Ground Cannabis"), recipeType: typeof(DecarboxylatedCannabisRecipe));

            this.ModsPostInitialize();

            CraftingComponent.AddRecipe(tableType: typeof(BakeryOvenObject), recipe: this);

        }
        partial void ModsPreInitialize();
        partial void ModsPostInitialize();
    }
}
