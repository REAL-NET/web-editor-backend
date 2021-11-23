using System;
using Repo.DeepMetamodel;
using Repo.DeepMetamodels;

namespace RepoAPI.Models
{
    public static class RepoContainer
    {
        private static IDeepRepository repo;

        public static void Create()
        {
            repo = DeepMetamodelRepoFactory.Create();
            var metamodel = repo.InstantiateDeepMetamodel("TestMetamodel");
            var model1 = repo.InstantiateModel("TestModel1", metamodel);
            repo.InstantiateModel("TestModel2", model1);
            IDeepModelBuilder airSimMetamodelBuilder = new AirSimMetamodelBuilder();
            airSimMetamodelBuilder.Build(repo);
            IDeepModelBuilder atkinsonModelBuilder = new AtkinsonModelBuilder();
            atkinsonModelBuilder.Build(repo);
            IDeepModelBuilder robots = new RobotsSubroutineModelsBuilder();
            robots.Build(repo);
            IDeepModelBuilder qRealRobots = new RobotsQRealModelsBuilder();
            qRealRobots.Build(repo);
            IDeepModelBuilder query = new QueryModelBuilder();
            query.Build(repo);
            var airSimMetamodel = repo.Model("AirSimMetamodel");
            repo.InstantiateModel("AirSimModel", airSimMetamodel);
        }

        public static void Load(string path)
        {
            //repo = DeepMetamodelRepoFactory.Load(path);
        }

        public static IDeepRepository CurrentRepo()
        {
            if (repo is null)
            {
                Create();
                Console.Out.Write("New repo created");
            }
            return repo;
        }

    }
}
