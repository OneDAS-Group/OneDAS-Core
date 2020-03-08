using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OneDas.DataManagement.BlazorExplorer.Hubs
{
    public class DataHub : Hub
    {
        public Task<double[]> GetData()
        {
            return Task.Run(() =>
            {
                var random = new Random();
                var data = Enumerable.Range(0, 100).Select(value => (double)random.NextDouble() * 100).ToArray();
                data[5] = double.NaN;
                data[6] = double.NaN;
                data[7] = double.NaN;
                data[5] = double.NaN;
                data[20] = double.NaN;
                return data;
            });
        }
    }
}
