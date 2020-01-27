using OneDas.DataManagement;

namespace OneDas.Hdf.VdsTool.Commands
{
    public class UpdateCommand
    {
        public UpdateCommand()
        {
            //   
        }
            
        public void Run()
        {
            DatabaseUtilities.UpdateDatabase();
        }
    }
}
