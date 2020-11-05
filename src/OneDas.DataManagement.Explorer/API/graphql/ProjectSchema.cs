using GraphQL.Types;
using System;

namespace OneDas.DataManagement.Explorer.API
{
    public class ProjectSchema : Schema
    {
        public ProjectSchema(OneDasDatabaseManager databaseManager, IServiceProvider provider) : base(provider)
        {
            Query = new ProjectQuery(databaseManager);
        }
    }
}
