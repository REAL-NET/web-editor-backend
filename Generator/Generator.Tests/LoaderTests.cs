using System.Linq;
using Generator.Requests;
using Generator.Services;
using NUnit.Framework;

namespace Generator.Tests
{
    public class LoaderTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void LoadModelTest()
        {
            RepoRequest.SetClient("localhost", 8000);
            var model = new RepoLoader().LoadModel("AirSimModel");
            
            Assert.AreEqual("AirSimModel", model.Name);
            Assert.AreEqual(19, model.Elements.Count());
        }
        
    }
}