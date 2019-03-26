using OneDas.DataStorage;
using System;

namespace OneDas.Extensibility
{
    public class VariableContext
    {
        #region "Constructors"

        public VariableContext(VariableDescription variableDescription, IDataStorage dataStorage)
        {
            this.VariableDescription = variableDescription;
            this.DataStorage = dataStorage;

            if (!variableDescription.DataStorageType.IsAssignableFrom(dataStorage.GetType()))
            {
                throw new ArgumentException(ErrorMessage.VariableContext_DataStorageTypeInvalid);
            }
        }

        #endregion

        #region "Properties"

        public VariableDescription VariableDescription { get; private set; }
        public IDataStorage DataStorage { get; private set; }

        #endregion
    }
}