using System;
using Repo.DeepMetamodel;

namespace RepoAPI.Models
{
    public static class RepoContainer
    {
        private static IDeepRepository repo;

        public static void Create()
        {
            repo = DeepMetamodelRepoFactory.Create();
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
