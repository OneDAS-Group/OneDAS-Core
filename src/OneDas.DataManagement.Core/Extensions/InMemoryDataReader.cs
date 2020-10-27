using Microsoft.Extensions.Logging;
using OneDas.DataManagement.Database;
using OneDas.DataManagement.Extensibility;
using OneDas.Extensibility;
using OneDas.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OneDas.DataManagement.Extensions
{
    [ExtensionIdentification("OneDas.InMemory", "OneDAS in-memory", "Provides an in-memory database.", "", "")]
    public class InMemoryDataReader : DataReaderExtensionBase
    {
        #region Fields

        private Random _random;

        #endregion

        #region Constructors

        public InMemoryDataReader(string rootPath, ILogger logger) : base(rootPath, logger)
        {
            _random = new Random();
        }

        #endregion

        #region Methods

        public override (T[] Dataset, byte[] StatusSet) ReadSingle<T>(DatasetInfo dataset, DateTime begin, DateTime end)
        {
            double[] dataDouble;

            var beginTime = begin.ToUnixTimeStamp();
            var endTime = end.ToUnixTimeStamp();

            var length = (int)((end - begin).TotalSeconds * (double)dataset.GetSampleRate().SamplesPerSecond);
            var dt = (double)(1 / dataset.GetSampleRate().SamplesPerSecond);

            if (dataset.Parent.Id.Contains("unix_time"))
            {
                dataDouble = Enumerable.Range(0, length).Select(i => i * dt + beginTime).ToArray();
            }
            else // temperature or wind speed
            {
                var kernelSize = 1000;
                var movingAverage = new double[kernelSize];
                var random = new Random();
                var mean = 15;

                dataDouble = new double[length];

                for (int i = 0; i < length; i++)
                {
                    movingAverage[i % kernelSize] = (random.NextDouble() - 0.5) * mean * 10 + mean;

                    if (movingAverage[kernelSize - 1] == 0)
                        dataDouble[i] = mean;
                    else
                        dataDouble[i] = movingAverage.Sum() / kernelSize;
                }
            }

            var data = dataDouble.Select(value => (T)Convert.ChangeType(value, typeof(T))).ToArray();
            var statusSet = Enumerable.Range(0, length).Select(value => (byte)1).ToArray();

            return (data, statusSet);
        }

        protected override List<ProjectInfo> LoadProjects()
        {
            var id11 = "7dec6d79-b92e-4af2-9358-21be1f3626c9";
            var id12 = "cf50190b-fd2a-477b-9655-48f4f41ba7bf";
            var id13 = "f01b6a96-1de6-4caa-9205-184d8a3eb2f8";
            var id14 = "d549a4dd-e003-4d24-98de-4d5bc8c72aca";
            var project_allowed = this.LoadProject("/IN_MEMORY/ALLOWED/TEST", id11, id12, id13, id14);

            var id21 = "511d6e9c-9075-41ee-bac7-891d359f0dda";
            var id22 = "99b85689-5373-4a9a-8fd7-be04a89c9da8";
            var id23 = "50d38fe5-a7a8-49e8-8bd4-3e98a48a951f";
            var id24 = "d47d1adc6-7c38-4b75-9459-742fa570ef9d";
            var project_restricted = this.LoadProject("/IN_MEMORY/RESTRICTED/TEST", id21, id22, id23, id24);

            return new List<ProjectInfo>() { project_allowed, project_restricted };
        }

        protected override double GetDataAvailability(string projectId, DateTime Day)
        {
            if (!this.Projects.Any(project => project.Id == projectId))
                throw new Exception($"The requested project with name '{projectId}' could not be found.");

            return _random.NextDouble() / 10 + 0.9;
        }

        private ProjectInfo LoadProject(string projectId, string id1, string id2, string id3, string id4)
        {
            var project = new ProjectInfo(projectId);

            var variableA = new VariableInfo(id1, project);
            var variableB = new VariableInfo(id2, project);
            var variableC = new VariableInfo(id3, project);
            var variableD = new VariableInfo(id4, project);

            var dataset1 = new DatasetInfo("25 Hz", variableA) { DataType = OneDasDataType.INT32 };
            var dataset2 = new DatasetInfo("1 s_max", variableB) { DataType = OneDasDataType.FLOAT64 };
            var dataset3 = new DatasetInfo("1 s_mean", variableB) { DataType = OneDasDataType.FLOAT64 };
            var dataset4 = new DatasetInfo("1 s_mean", variableC) { DataType = OneDasDataType.FLOAT64 };
            var dataset5 = new DatasetInfo("1 s_mean", variableD) { DataType = OneDasDataType.FLOAT64 };

            // variable A
            variableA.Name = "unix_time1";
            variableA.Group = "Group 1";
            variableA.Unit = "";

            variableA.Datasets.Add(dataset1);

            // variable B
            variableB.Name = "unix_time2";
            variableB.Group = "Group 1";
            variableB.Unit = string.Empty;

            variableB.Datasets.Add(dataset2);
            variableB.Datasets.Add(dataset3);

            // variable C
            variableC.Name = "T1";
            variableC.Group = "Group 2";
            variableC.Unit = "°C";

            variableC.Datasets.Add(dataset4);

            // variable D
            variableD.Name = "V1";
            variableD.Group = "Group 2";
            variableD.Unit = "m/s";

            variableD.Datasets.Add(dataset5);

            // project
            project.Variables = new List<VariableInfo>()
            {
                variableA,
                variableB,
                variableC,
                variableD
            };

            return project;
        }

        #endregion
    }
}
