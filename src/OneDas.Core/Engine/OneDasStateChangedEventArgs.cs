using System;

namespace OneDas.Core.Engine
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