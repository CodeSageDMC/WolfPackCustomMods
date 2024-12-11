// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.


// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.
// <do-not-localize />

namespace Eco.Animation
{
    using System.Collections.Generic;

    /// <summary>The Class to convert the Avatar States to the Animation States, where animations states can have properties, used in the <see cref="AvatarAnimationManager"/> for set up animations with different conditions.</summary>
    public static class AnimationStateManager
    {

        /// <summary>Names for different possible Avatar States. Other classes can use these to decide on a state <see cref="MountSpot"/> ///</summary>

        public enum AvatarState
        {
            Grounded = 0,
            Jumping = 1,
            Swimming = 2,
            Sitting = 3,
            Flying = 4,
            Pulling = 5,
            ClimbingLadder = 6,
            SummittingLadder = 7,
            EmoteLooping = 8,
            Sleeping = 9,
            Paddling = 10,
            Drowning = 11,
            Respawn  = 12,

            // avatar editing states
            NewBeard = 104,
            NewShirt = 105,
            NewBelt = 106,
            NewShoes = 107,
            NewHair = 108,
            NewPants = 109,
            NewSkin = 110,

            None = 255
        }

        /// <summary>Animation State that should be used in the main manager, which provide more flexible way to work with logic. Just add Avatar state alongside with it's behavior pattern (properties).</summary>
        public class AvatarAnimationState
        {
            public string AnimationStateName { get; }
            public int    AnimationStateNum  { get; } //For animator to properly set values.

            //Specifics Action Parameters, will help to switch animations, decide on rotations of an Avatar.
            public bool AllowRotation  { get; } //Is avatar rotation with a camera rotation allowed, in general?
            public bool UseInAir       { get; } //Is avatar will be in the air?
            public bool NeedQuickBlend { get; } //Is the animation need a quick blend, for smoother transitions?
            public bool UsedOnLadder   { get; } //Is this one will be used on ladder?

            public AvatarAnimationState(AvatarState avatarState, bool allowRotation, bool useInAir, bool isSpecialState, bool usedOnLadder)
            {
                this.AnimationStateName = avatarState.ToString();
                this.AnimationStateNum  = (int)avatarState;
                this.AllowRotation      = allowRotation;
                this.UseInAir           = useInAir;
                this.NeedQuickBlend     = isSpecialState;
                this.UsedOnLadder       = usedOnLadder;
            }
        }

        //Collect all trnsfered animation states in one place
        public static Dictionary<AvatarState, AvatarAnimationState> AnimationStates { get; private set; }

        /// <summary>Translate all Avatar States into Animation States.</summary>
        public static void InitAnimationStates()
        {
            AnimationStates = new Dictionary<AvatarState, AvatarAnimationState>();

            AnimationStates[AvatarState.Grounded]         = new AvatarAnimationState(AvatarState.Grounded,         allowRotation: true,  useInAir: false, isSpecialState: false, usedOnLadder: false);
            AnimationStates[AvatarState.Jumping]          = new AvatarAnimationState(AvatarState.Jumping,          allowRotation: true,  useInAir: true,  isSpecialState: false, usedOnLadder: false);
            AnimationStates[AvatarState.Swimming]         = new AvatarAnimationState(AvatarState.Swimming,         allowRotation: true,  useInAir: false, isSpecialState: true,  usedOnLadder: false);
            AnimationStates[AvatarState.Sitting]          = new AvatarAnimationState(AvatarState.Sitting,          allowRotation: false, useInAir: false, isSpecialState: true,  usedOnLadder: false);
            AnimationStates[AvatarState.Flying]           = new AvatarAnimationState(AvatarState.Flying,           allowRotation: true,  useInAir: true,  isSpecialState: true,  usedOnLadder: false);
            AnimationStates[AvatarState.Pulling]          = new AvatarAnimationState(AvatarState.Pulling,          allowRotation: true,  useInAir: false, isSpecialState: false, usedOnLadder: false);
            AnimationStates[AvatarState.ClimbingLadder]   = new AvatarAnimationState(AvatarState.ClimbingLadder,   allowRotation: false, useInAir: false, isSpecialState: true,  usedOnLadder: true);
            AnimationStates[AvatarState.SummittingLadder] = new AvatarAnimationState(AvatarState.SummittingLadder, allowRotation: false, useInAir: false, isSpecialState: false, usedOnLadder: true);
            AnimationStates[AvatarState.EmoteLooping]     = new AvatarAnimationState(AvatarState.EmoteLooping,     allowRotation: false, useInAir: false, isSpecialState: false, usedOnLadder: false);
            AnimationStates[AvatarState.Sleeping]         = new AvatarAnimationState(AvatarState.Sleeping,         allowRotation: false, useInAir: false, isSpecialState: false, usedOnLadder: false);
            AnimationStates[AvatarState.Paddling]         = new AvatarAnimationState(AvatarState.Paddling,         allowRotation: false, useInAir: false, isSpecialState: false, usedOnLadder: false);
            AnimationStates[AvatarState.Drowning]         = new AvatarAnimationState(AvatarState.Drowning,         allowRotation: false, useInAir: false, isSpecialState: false, usedOnLadder: false);
            AnimationStates[AvatarState.Respawn]          = new AvatarAnimationState(AvatarState.Respawn,          allowRotation: false, useInAir: false, isSpecialState: false, usedOnLadder: false);

            AnimationStates[AvatarState.NewBeard]         = new AvatarAnimationState(AvatarState.NewBeard,         allowRotation: false, useInAir: false, isSpecialState: false, usedOnLadder: false);
            AnimationStates[AvatarState.NewShirt]         = new AvatarAnimationState(AvatarState.NewShirt,         allowRotation: false, useInAir: false, isSpecialState: false, usedOnLadder: false);
            AnimationStates[AvatarState.NewBelt]          = new AvatarAnimationState(AvatarState.NewBelt,          allowRotation: false, useInAir: false, isSpecialState: false, usedOnLadder: false);
            AnimationStates[AvatarState.NewShoes]         = new AvatarAnimationState(AvatarState.NewShoes,         allowRotation: false, useInAir: false, isSpecialState: false, usedOnLadder: false);
            AnimationStates[AvatarState.NewHair]          = new AvatarAnimationState(AvatarState.NewHair,          allowRotation: false, useInAir: false, isSpecialState: false, usedOnLadder: false);
            AnimationStates[AvatarState.NewPants]         = new AvatarAnimationState(AvatarState.NewPants,         allowRotation: false, useInAir: false, isSpecialState: false, usedOnLadder: false);
            AnimationStates[AvatarState.NewSkin]          = new AvatarAnimationState(AvatarState.NewSkin,          allowRotation: false, useInAir: false, isSpecialState: false, usedOnLadder: false);

            AnimationStates[AvatarState.None]             = new AvatarAnimationState(AvatarState.None,             allowRotation: true,  useInAir: false, isSpecialState: false, usedOnLadder: false);
        }
    }
}
