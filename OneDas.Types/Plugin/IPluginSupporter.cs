using OneDas.Plugin;
using System;

namespace OneDas.Plugin
{
    public interface IPluginSupporter
    {
        ActionResponse HandleActionRequest(ActionRequest actionRequest, Func<object, Type, object> deserializeFunction);
    }
}
