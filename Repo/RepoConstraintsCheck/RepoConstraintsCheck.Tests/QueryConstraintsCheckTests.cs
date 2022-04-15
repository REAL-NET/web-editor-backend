using NUnit.Framework;
using Repo;
using RepoConstraintsCheck;

namespace RepoConstraintsCheckTests
{
    public class Tests
    {
        private IRepo repo;

        [SetUp]
        public void Setup()
        {
            repo = RepoFactory.Create();
        }

        [Test]
        public void QueryConstraintsCheckStrategyTest()
        {
            var queryModel = repo.Model("QueryModel");
            var queryStrategy = new QueryConstraintsCheckStrategy(queryModel);
            var checkingSystem = new ConstraintsCheckSystem(queryModel, queryStrategy);
            Assert.IsTrue(checkingSystem.Check());
        }
    }
}