using System;
using OneDas.Infrastructure;

namespace OneDas.Main.Core
{
    public class OneDasStateChangedEventArgs : EventArgs
    {
        public OneDasStateChangedEventArgs(OneDasState oldState, OneDasState newState)
        {
            this.OldState = oldState;
            this.NewState = newState;
        }

        public OneDasState OldState { get; set; }
        public OneDasState NewState { get; set; }
    }
}