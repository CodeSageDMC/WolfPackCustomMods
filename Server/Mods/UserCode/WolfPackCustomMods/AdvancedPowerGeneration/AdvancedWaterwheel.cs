﻿// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.
// <auto-generated from WorldObjectTemplate.tt />

namespace Eco.Mods.TechTree
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Eco.Core.Items;
    using Eco.Gameplay.Blocks;
    using Eco.Gameplay.Components;
    using Eco.Gameplay.Components.Auth;
    using Eco.Gameplay.DynamicValues;
    using Eco.Gameplay.Economy;
    using Eco.Gameplay.Housing;
    using Eco.Gameplay.Interactions;
    using Eco.Gameplay.Items;
    using Eco.Gameplay.Modules;
    using Eco.Gameplay.Minimap;
    using Eco.Gameplay.Objects;
    using Eco.Gameplay.Occupancy;
    using Eco.Gameplay.Players;
    using Eco.Gameplay.Property;
    using Eco.Gameplay.Skills;
    using Eco.Gameplay.Systems;
    using Eco.Gameplay.Systems.TextLinks;
    using Eco.Gameplay.Pipes.LiquidComponents;
    using Eco.Gameplay.Pipes.Gases;
    using Eco.Shared;
    using Eco.Shared.Math;
    using Eco.Shared.Localization;
    using Eco.Shared.Serialization;
    using Eco.Shared.Utils;
    using Eco.Shared.View;
    using Eco.Shared.Items;
    using Eco.Shared.Networking;
    using Eco.Gameplay.Pipes;
    using Eco.World.Blocks;
    using Eco.Gameplay.Housing.PropertyValues;
    using Eco.Gameplay.Civics.Objects;
    using Eco.Gameplay.Settlements;
    using Eco.Gameplay.Systems.NewTooltip;
    using Eco.Core.Controller;
    using Eco.Core.Utils;
    using Eco.Gameplay.Components.Storage;
    using static Eco.Gameplay.Housing.PropertyValues.HomeFurnishingValue;
    using static Eco.Gameplay.Components.PartsComponent;
    using Eco.Gameplay.Items.Recipes;

    [Serialized]
    [RequireComponent(typeof(OnOffComponent))]
    [RequireComponent(typeof(PropertyAuthComponent))]
    [RequireComponent(typeof(MinimapComponent))]
    [RequireComponent(typeof(PartsComponent))]
    [RequireComponent(typeof(PowerGridComponent))]
    [RequireComponent(typeof(PowerGeneratorComponent))]
    [RequireComponent(typeof(HousingComponent))]
    [RequireComponent(typeof(WaterRiverGeneratorComponent))]
    [RequireComponent(typeof(OccupancyRequirementComponent))]
    [RequireComponent(typeof(ForSaleComponent))]
    [PowerGenerator(typeof(MechanicalPower))]
    [Tag("Usable")]
    [Ecopedia("Crafted Objects", "Power Generation", subPageName: "Advanced Waterwheel Item")]
    [RepairRequiresSkill(typeof(BasicEngineeringSkill), 1)]
    [RepairRequiresSkill(typeof(SelfImprovementSkill), 4)]
    public partial class AdvancedWaterwheelObject : WorldObject, IRepresentsItem
    {
        public virtual Type RepresentedItemType => typeof(AdvancedWaterwheelItem);
        public override LocString DisplayName => Localizer.DoStr("AdvancedWaterwheel");
        public override TableTextureMode TableTexture => TableTextureMode.Wood;

        protected override void Initialize()
        {
            this.ModsPreInitialize();
            this.GetComponent<MinimapComponent>().SetCategory(Localizer.DoStr("Power"));
            this.GetComponent<PowerGridComponent>().Initialize(30, new MechanicalPower(), 10, true);
            this.GetComponent<PowerGeneratorComponent>().Initialize(1000);
            this.GetComponent<HousingComponent>().HomeValue = AdvancedWaterwheelItem.homeValue;
            this.ModsPostInitialize();
            {
                this.GetComponent<PartsComponent>().Config(() => LocString.Empty, new PartInfo[]
                {
                                        new() { TypeName = nameof(IronGearItem), Quantity = 1},
                                        new() { TypeName = nameof(LubricantItem), Quantity = 1},
                                    });
                this.GetComponent<PowerGridComponent>().DurabilityUsedPerHourOfUse = 1.4f;
            }
        }

      
        partial void ModsPreInitialize();
    
        partial void ModsPostInitialize();
    }

    [Serialized]
    [LocDisplayName("Advanced Waterwheel")]
    [LocDescription("Uses the power of flowing water to produce mechanical power. Must be placed in fresh water and produces double power when placed in both a river and waterfall.")]
    [IconGroup("World Object Minimap")]
    [Ecopedia("Crafted Objects", "Power Generation", createAsSubPage: true)]
    [Weight(5000)] // Defines how heavy AdvancedWaterwheel is.
    public partial class AdvancedWaterwheelItem : WorldObjectItem<AdvancedWaterwheelObject>, IPersistentData
    {
        protected override OccupancyContext GetOccupancyContext => new SideAttachedContext(0 | DirectionAxisFlags.Left | DirectionAxisFlags.Right, WorldObject.GetOccupancyInfo(this.WorldObjectType));
        public override HomeFurnishingValue HomeValue => homeValue;
        public static readonly HomeFurnishingValue homeValue = new HomeFurnishingValue()
        {
            ObjectName = typeof(AdvancedWaterwheelObject).UILink(),
            Category = HousingConfig.GetRoomCategory("Industrial"),
            TypeForRoomLimit = Localizer.DoStr(""),

        };

        [NewTooltip(CacheAs.SubType, 8)] public static LocString PowerProductionTooltip() => Localizer.Do($"Produces: {Text.Info(1000)}w of {new MechanicalPower().Name} power.");
        [Serialized, SyncToView, NewTooltipChildren(CacheAs.Instance, flags: TTFlags.AllowNonControllerTypeForChildren)] public object PersistentData { get; set; }
    }

   
    [RequiresSkill(typeof(BasicEngineeringSkill), 4)]
    [Ecopedia("Crafted Objects", "Power Generation", subPageName: "Advanced Waterwheel Item")]
    public partial class AdvancedWaterwheelRecipe : RecipeFamily
    {
        public AdvancedWaterwheelRecipe()
        {
            var recipe = new Recipe();
            recipe.Init(
                name: "AdvancedWaterwheel",  //noloc
                displayName: Localizer.DoStr("Advanced Waterwheel"),

                
                ingredients: new List<IngredientElement>
                {
                    new IngredientElement(typeof(CopperBarItem), 10, typeof(BasicEngineeringSkill), typeof(BasicEngineeringLavishResourcesTalent)),
                    new IngredientElement(typeof(IronGearItem), 10, typeof(BasicEngineeringSkill), typeof(BasicEngineeringLavishResourcesTalent)),
                    new IngredientElement(typeof(GearboxItem), 1, typeof(BasicEngineeringSkill), typeof(BasicEngineeringLavishResourcesTalent)),
                    new IngredientElement(typeof(LubricantItem), 2, typeof(BasicEngineeringSkill), typeof(BasicEngineeringLavishResourcesTalent)),
                    new IngredientElement(typeof(CopperPlateItem), 25, typeof(BasicEngineeringSkill), typeof(BasicEngineeringLavishResourcesTalent)),
                    new IngredientElement(typeof(ScrewsItem), 40, typeof(BasicEngineeringSkill), typeof(BasicEngineeringLavishResourcesTalent)),
                    
                    
                },

               
                items: new List<CraftingElement>
                {
                    new CraftingElement<AdvancedWaterwheelItem>()
                });
            this.Recipes = new List<Recipe> { recipe };

            this.ExperienceOnCraft = 8; 

            this.LaborInCalories = CreateLaborInCaloriesValue(180, typeof(BasicEngineeringSkill));

            
            this.CraftMinutes = CreateCraftTimeValue(beneficiary: typeof(AdvancedWaterwheelRecipe), start: 5, skillType: typeof(BasicEngineeringSkill), typeof(BasicEngineeringFocusedSpeedTalent), typeof(BasicEngineeringParallelSpeedTalent));

            
            this.ModsPreInitialize();
            this.Initialize(displayText: Localizer.DoStr("Advanced Waterwheel"), recipeType: typeof(AdvancedWaterwheelRecipe));
            this.ModsPostInitialize();

            
            CraftingComponent.AddRecipe(tableType: typeof(WainwrightTableObject), recipe: this);
        }

        
        partial void ModsPreInitialize();

      
        partial void ModsPostInitialize();
    }
}
