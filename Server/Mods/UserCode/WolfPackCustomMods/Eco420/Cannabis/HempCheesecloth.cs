﻿// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.
// <auto-generated from ItemTemplate.tt/>

namespace Eco.Mods.TechTree
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Eco.Gameplay.Blocks;
    using Eco.Gameplay.Components;
    using Eco.Gameplay.DynamicValues;
    using Eco.Gameplay.Items;
    using Eco.Gameplay.Objects;
    using Eco.Gameplay.Players;
    using Eco.Gameplay.Skills;
    using Eco.Gameplay.Settlements;
    using Eco.Gameplay.Systems;
    using Eco.Gameplay.Systems.TextLinks;
    using Eco.Shared.Localization;
    using Eco.Shared.Serialization;
    using Eco.Shared.Utils;
    using Eco.Core.Items;
    using Eco.World;
    using Eco.World.Blocks;
    using Eco.Gameplay.Pipes;
    using Eco.Core.Controller;
    using Eco.Gameplay.Items.Recipes;





    //Item


    [Serialized]
    [LocDisplayName("Hemp Cheesecloth")]
    [Weight(25)]
    [MaxStackSize(420)]
    [Ecopedia("Items", "Products", createAsSubPage: true)]
    [LocDescription("Cheesecloth is a lightweight, cotton gauze fabric with an open texture, and it is primarily used for food preparation.")]
    public partial class HempCheeseclothItem : Item
    {
        

    }




    //Recipe


    [RequiresSkill(typeof(TailoringSkill), 3)]
    [Ecopedia("Items", "Products", subPageName: "Hemp Cheesecloth Item")]
    public partial class HempCheeseclothRecipe : RecipeFamily
    {
        public HempCheeseclothRecipe()
        {
            var recipe = new Recipe();
            recipe.Init(
                name: "HempCheesecloth",  //noloc
                displayName: Localizer.DoStr("Hemp Cheesecloth"),


                ingredients: new List<IngredientElement>
                {
                    new IngredientElement(typeof(HempYarnItem), 2, typeof(TailoringSkill), typeof(TailoringLavishResourcesTalent)),
                },


                items: new List<CraftingElement>
                {
                    new CraftingElement<HempCheeseclothItem>(1)
                });
            this.Recipes = new List<Recipe> { recipe };
            this.ExperienceOnCraft = 0.1f;


            this.LaborInCalories = CreateLaborInCaloriesValue(30, typeof(TailoringSkill));


            this.CraftMinutes = CreateCraftTimeValue(beneficiary: typeof(HempCheeseclothRecipe), start: 1, skillType: typeof(TailoringSkill), typeof(TailoringFocusedSpeedTalent), typeof(TailoringParallelSpeedTalent));


            this.ModsPreInitialize();
            this.Initialize(displayText: Localizer.DoStr("Hemp Cheesecloth"), recipeType: typeof(HempCheeseclothRecipe));
            this.ModsPostInitialize();


            CraftingComponent.AddRecipe(tableType: typeof(LoomObject), recipe: this);
        }

        partial void ModsPreInitialize();

        partial void ModsPostInitialize();
    }

}