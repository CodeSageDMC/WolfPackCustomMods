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




    //Item


    [Serialized]
    [LocDisplayName("Rolling Papers")]
    [Weight(42)]
    [MaxStackSize(420)]
    [LocDescription("A thin piece of paper with a glue for rolling joints.")]
    public partial class RollingPapersItem : Item
    {


    }



    //Recipe


    
    [RequiresSkill(typeof(PaperMillingSkill), 4)]
    public partial class RollingPapersRecipe : RecipeFamily
    {
        public RollingPapersRecipe()
        {
            var recipe = new Recipe();
            recipe.Init(
                name: "RollingPapers",  //noloc
                displayName: Localizer.DoStr("Rolling Papers"),


                ingredients: new List<IngredientElement>
                {
                    new IngredientElement(typeof(CelluloseFiberItem), 15, typeof(PaperMillingSkill), typeof(PaperMillingLavishResourcesTalent)),
                    new IngredientElement(typeof(WheatPasteGlueItem), 1, typeof(PaperMillingSkill), typeof(PaperMillingLavishResourcesTalent)),

                },

                items: new List<CraftingElement>
                {
                    new CraftingElement<RollingPapersItem>(10),
                });
            this.Recipes = new List<Recipe> { recipe };

            this.ExperienceOnCraft = 1f;

            this.LaborInCalories = CreateLaborInCaloriesValue(420);

            
            this.CraftMinutes = CreateCraftTimeValue(beneficiary: typeof(RollingPapersRecipe), start: 3f, skillType: typeof(PaperMillingSkill), typeof(PaperMillingFocusedSpeedTalent), typeof(PaperMillingParallelSpeedTalent));

            
            this.ModsPreInitialize();
            this.Initialize(displayText: Localizer.DoStr("Rolling Papers"), recipeType: typeof(RollingPapersRecipe));
            this.ModsPostInitialize();


    CraftingComponent.AddRecipe(tableType: typeof(SmallPaperMachineObject), recipe: this);
    }

    partial void ModsPreInitialize();

    partial void ModsPostInitialize();
}
}