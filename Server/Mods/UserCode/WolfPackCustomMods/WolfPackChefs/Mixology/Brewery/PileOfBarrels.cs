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
    using Eco.Gameplay.Items.Recipes;
    using Eco.Core.Plugins.Interfaces;
    using Eco.Mods.Organisms;
    using Eco.Mods.TechTree;
    using System.Diagnostics;
    using System.Linq;


    [Serialized]
    [RequireComponent(typeof(PropertyAuthComponent))]
    [RequireComponent(typeof(LinkComponent))]
    [RequireComponent(typeof(PublicStorageComponent))]
    [RequireComponent(typeof(OccupancyRequirementComponent))]
    [RequireComponent(typeof(ForSaleComponent))]
    [RequireComponent(typeof(RoomRequirementsComponent))]
    [RequireRoomContainment]
    [RequireRoomVolume(24)]
    [RequireRoomMaterialTier(2.8f)]
    [Tag("Usable")]
    [Ecopedia("Crafted Objects", "Storage", subPageName: "Pile Of Barrels")]
    public partial class PileOfBarrelsObject : WorldObject, IRepresentsItem
    {
        public virtual Type RepresentedItemType => typeof(PileOfBarrelsItem);
        public override LocString DisplayName => Localizer.DoStr("Pile Of Barrels");
        public override TableTextureMode TableTexture => TableTextureMode.Wood;

        protected override void Initialize()
        {
            this.ModsPreInitialize();
            this.GetComponent<LinkComponent>().Initialize(15);
            var storage = this.GetComponent<PublicStorageComponent>();
            storage.Initialize(20);
            storage.Storage.AddInvRestriction(new StackLimitRestriction(200));
            storage.ShelfLifeMultiplier = .5f;
            storage.Inventory.AddInvRestriction(new TagRestriction(new string[] // Les tags autorisées à être utilisées dans le stockage.
            {
                "Wooden Barrel",
            }));
            this.ModsPostInitialize();
        }

        public class InventoryMultiply : InventoryRestriction
        {
            public override LocString Message
            {
                get
                {
                    return Localizer.DoStr("Inventory Full");
                }
            }

            public override int MaxAccepted(Item item, int currentQuantity)
            {
                if (item.MaxStackSize > 1)
                {
                    return item.MaxStackSize * 10;
                }
                if (!TagUtils.Tags(item).Any((Tag x) => x.Name == "Wooden Barrel"))
                {
                    return 20;
                }
                return 1;
            }

            public override bool SurpassStackSize
            {
                get
                {
                    return true;
                }
            }
        }


        partial void ModsPreInitialize();
        partial void ModsPostInitialize();
    }




    [Serialized]
    [LocDisplayName("Pile Of Barrels")]
    [LocDescription("Stockpile your Barrels of Alcohol here.")]
    [Ecopedia("Crafted Objects", "Storage", createAsSubPage: true)]
    [Weight(2000)]
    [Tag(nameof(SurfaceTags.CanBeOnRug))]
    public partial class PileOfBarrelsItem : WorldObjectItem<PileOfBarrelsObject>
    {
        [NewTooltip(CacheAs.SubType, 50)] public static LocString UpdateTooltip() => Localizer.Do($"{Localizer.DoStr("Increases")} total shelf life by: {Text.InfoLight(Text.Percent(0.6f))}").Dash();
        protected override OccupancyContext GetOccupancyContext => new SideAttachedContext(0 | DirectionAxisFlags.Down, WorldObject.GetOccupancyInfo(this.WorldObjectType));
        

    }

    [RequiresSkill(typeof(LoggingSkill), 5)]
    [Ecopedia("Crafted Objects", "Storage", subPageName: "Pile Of Barrels")]
    public partial class PileOfBarrelsRecipe : RecipeFamily
    {
        public PileOfBarrelsRecipe()
        {
            var recipe = new Recipe();
            recipe.Init(
                name: "Pile Of Barrels",
                displayName: Localizer.DoStr("Pile Of Barrels"),

                ingredients: new List<IngredientElement>
                {
                    new IngredientElement("WoodBoard", 50, typeof(LoggingSkill)),
                    new IngredientElement(typeof(IronStripItem), 25, typeof(LoggingSkill)),
                },

                items: new List<CraftingElement>
                {
                    new CraftingElement<PileOfBarrelsItem>()
                });
            this.Recipes = new List<Recipe> { recipe };
            this.ExperienceOnCraft = 3;

            this.LaborInCalories = CreateLaborInCaloriesValue(500, typeof(LoggingSkill));

            this.CraftMinutes = CreateCraftTimeValue(beneficiary: typeof(PileOfBarrelsRecipe), start: 5, skillType: typeof(LoggingSkill), typeof(LoggingFocusedSpeedTalent), typeof(LoggingParallelSpeedTalent));

            this.ModsPreInitialize();
            this.Initialize(displayText: Localizer.DoStr("Pile Of Barrels"), recipeType: typeof(PileOfBarrelsRecipe));
            this.ModsPostInitialize();

            CraftingComponent.AddRecipe(tableType: typeof(CarpentryTableObject), recipe: this);
        }

        partial void ModsPreInitialize();
        partial void ModsPostInitialize();
    }
}