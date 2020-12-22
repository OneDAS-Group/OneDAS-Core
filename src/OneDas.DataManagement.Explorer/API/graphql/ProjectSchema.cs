using GraphQL.Types;
using OneDas.DataManagement.Explorer.Services;
using System;

namespace OneDas.DataManagement.Explorer.API
{
    public class ProjectSchema : Schema
    {
        public ProjectSchema(DatabaseManager databaseManager, IServiceProvider provider) : base(provider)
        {
            Query = new ProjectQuery(databaseManager);
        }
    }
}
