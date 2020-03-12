using OneDas.Buffers;
using System;

namespace OneDas.Extensibility
{
    public class VariableContext
    {
        #region "Constructors"

        public VariableContext(VariableDescription variableDescription, IBuffer buffer)
        {
            this.VariableDescription = variableDescription;
            this.Buffer = buffer;

            if (variableDescription.BufferType != buffer.Type)
                throw new ArgumentException(ErrorMessage.VariableContext_BufferTypeInvalid);
        }

        #endregion

        #region "Properties"

        public VariableDescription VariableDescription { get; private set; }

        public IBuffer Buffer { get; private set; }

        #endregion
    }
}