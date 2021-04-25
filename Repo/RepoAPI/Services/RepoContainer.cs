﻿using System;
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
            IModelBuilder airSimMetamodelBuilder = new AirSimMetamodelBuilder();
            airSimMetamodelBuilder.Build(repo);
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
