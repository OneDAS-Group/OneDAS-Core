namespace OneDas.Hdf.VdsTool.Navigation
{
    abstract class NavigatorBase
    {
        protected int SelectedIndex { get; set; }
        protected int LastIndex { get; set; }

        protected void Start()
        {
            this.SelectedIndex = -1;
            this.LastIndex = -1;

            this.OnInitialize();
            this.OnRedraw();

            while (true)
            {
                if (this.OnWaitForUserInput())
                {
                    break;
                }
            }
        }

        protected abstract void OnInitialize();
        protected abstract void OnRedraw();
        protected abstract bool OnWaitForUserInput();
    }
}
