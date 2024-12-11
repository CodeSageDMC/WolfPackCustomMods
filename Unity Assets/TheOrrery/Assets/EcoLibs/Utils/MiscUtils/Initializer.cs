// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

//Client version of the same thing in server, except uses an Action instead of a ThreadSafeAction.  
//Kinda ugly its duplicated, duplication could be removed through fanciness
namespace Eco.Shared.Utils
{
    using System;

    //For classes that want to allow subscribers to perform actions upon initialization
    public interface IInitializationSubscribable { Initializer Initializer { get; }}

    //Handles initializing and subscribing to initialization for the containing class.
    public class Initializer
    {
        //Pass in the initialiation function for the containing class.  This passed initalization function should only be called by Initializer!
        public Initializer() { }
        public Initializer(Action onInitialized)
        {
            this.OnInitialized += onInitialized;
        }

        public Action OnInitialized { get; set; }
        public bool Initialized { get; private set; }

        public void Initialize()
        {
            DebugUtils.Assert(this.Initialized == false, "Double initialization");
            this.Initialized = true;
            this.OnInitialized?.Invoke();
            this.OnInitialized = null;
        }

        //Call now if the object is already initialized, and if not then queue it for when that happens.
        public void RunIfOrWhenInitialized(Action action)
        {
            if (this.Initialized) action.Invoke();
            else this.OnInitialized += action;
        }
    }
}
