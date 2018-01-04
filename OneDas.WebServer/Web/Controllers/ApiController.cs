using Microsoft.AspNetCore.Mvc;

namespace OneDas.WebServer.Web
{
    public class ApiController : Controller
    {
        //[ActionName("getpluginview")]
        //public string GetPluginView([Bind(Prefix = "type")] string pluginType, [Bind(Prefix = "path")] string relativeViewPath)
        //{
        //    try
        //    {
        //        string basePath = Path.Combine(LocalSettings.BaseDirectoryPath, "plugin");
        //        string filePath = Path.GetFullPath(Path.Combine(basePath, pluginType, relativeViewPath));

        //        if (filePath.StartsWith(Path.Combine(LocalSettings.BaseDirectoryPath, "plugin")))
        //        {
        //            if (System.IO.File.Exists(filePath))
        //            {
        //                return System.IO.File.ReadAllText(filePath);
        //            }
        //            else
        //            {
        //                return $"The requested view with type = {pluginType} and path = '{relativeViewPath}' could not be found.";
        //            }
        //        }
        //        else
        //        {
        //            return $"The requested path '{relativeViewPath}' is not within the plugin folder.";
        //        }

        //    }
        //    catch (Exception)
        //    {
        //        return $"An unknown error occured.";
        //    }           
        //}

        //[ActionName("getview")]
        //public string GetView([Bind(Prefix = "filename")] string fileName)
        //{
        //    try
        //    {
        //        Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"OneDas.WebServer.Web.Views.Home.{fileName}");

        //        if (stream != null)
        //        {
        //            return new StreamReader(stream).ReadToEnd();
        //        }
        //        else
        //        {
        //            return $"The requested resource with name = '{fileName}' could not be found.";
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        return $"An unknown error occured.";
        //    }
        //}
    }
}
