using NUnit.Framework;
using Repo;
using RepoConstraintsCheck;
using System.Linq;

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
        public void CheckTest()
        {
            var queryModel = repo.Model("QueryTestModel");
            var queryStrategy = new QueryConstraintsCheckStrategy(queryModel);
            var checkSystem = new ConstraintsCheckSystem(queryModel, queryStrategy);
            Assert.IsTrue(checkSystem.Check());
        }

        [Test]
        public void CheckWithErrorInfoHaveNoErrorsTest()
        {
            var queryModel = repo.Model("QueryTestModel");
            var queryStrategy = new QueryConstraintsCheckStrategy(queryModel);
            var checkSystem = new ConstraintsCheckSystem(queryModel, queryStrategy);
            var result = checkSystem.CheckWithErrorInfo();
            Assert.IsTrue(result.Item1);
            Assert.IsEmpty(result.Item2);
        }

        [Test]
        public void CheckWithErrorInfoPositionalOperatorsHaveReadersTest()
        {
            var queryModel = repo.Model("QueryTestModel");
            var queryStrategy = new QueryConstraintsCheckStrategy(queryModel);
            var checkSystem = new ConstraintsCheckSystem(queryModel, queryStrategy);
            var join = queryModel.Nodes.Where(x => x.Name == "aJoin").FirstOrDefault();
            var edges = queryModel.Edges.Where(x => x.From.Name == "aJoin" && x.To.Name == "aRead");
            foreach (var edge in edges)
            {
                queryModel.DeleteElement(edge);
            }
            var result = checkSystem.CheckWithErrorInfo();
            Assert.IsFalse(result.Item1);
            Assert.AreEqual(2, result.Item2.First().Item1);
            Assert.AreEqual(join.Id, result.Item2.First().Item2.First());
        }

        [Test]
        public void CheckWithErrorInfoPositionalOperatorsHaveReadersMultipleTest()
        {
            var queryModel = repo.Model("QueryTestModel");
            var queryStrategy = new QueryConstraintsCheckStrategy(queryModel);
            var checkSystem = new ConstraintsCheckSystem(queryModel, queryStrategy);
            var join = queryModel.Nodes.Where(x => x.Name == "aJoin").FirstOrDefault();
            var edges = queryModel.Edges.Where(x => x.From.Name == "aJoin" && x.To.Name == "aRead");
            foreach (var edge in edges)
            {
                queryModel.DeleteElement(edge);
            }
            var filter = queryModel.Nodes.Where(x => x.Name == "aFilter").FirstOrDefault();
            var filterEdge = queryModel.Edges.Where(x => x.From.Name == "aFilter").FirstOrDefault();
            queryModel.DeleteElement(filterEdge);
            var result = checkSystem.CheckWithErrorInfo();
            Assert.IsFalse(result.Item1);
            Assert.AreEqual(2, result.Item2.First().Item1);
            Assert.AreEqual(2, result.Item2.First().Item2.Count());
            Assert.IsTrue(result.Item2.First().Item2.Contains(join.Id));
            Assert.IsTrue(result.Item2.First().Item2.Contains(filter.Id));
        }

        //[Test]
        //public void CheckWithErrorInfoTupleOperatorsHaveNoReadersTest()
        //{
        //    var queryModel = repo.Model("QueryTestModel");
        //    var queryStrategy = new QueryConstraintsCheckStrategy(queryModel);
        //    var checkSystem = new ConstraintsCheckSystem(queryModel, queryStrategy);
        //    var sort = queryModel.Nodes.Where(x => x.Name == "sort").FirstOrDefault();
        //    var sortChildren = sort.Attributes.Where(x => x.Name == "children").FirstOrDefault();
        //    sortChildren.StringValue = sortChildren.StringValue + ", read1";
        //    var result = checkSystem.CheckWithErrorInfo();
        //    Assert.IsFalse(result.Item1);
        //    Assert.Equals(2, result.Item2.First().Item1);
        //    Assert.Equals(sort.Id, result.Item2.First().Item2.First());
        //}
    }
}