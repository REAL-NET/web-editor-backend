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
        public void CheckWithErrorInfoPositionalOperatorsHaveReadersThroughAttributeTest()
        {
            var queryModel = repo.Model("QueryTestModel");
            var queryStrategy = new QueryConstraintsCheckStrategy(queryModel);
            var checkSystem = new ConstraintsCheckSystem(queryModel, queryStrategy);
            var join = queryModel.Nodes.Where(x => x.Name == "join").FirstOrDefault();
            var joinChildren = join.Attributes.Where(x => x.Name == "children").FirstOrDefault();
            joinChildren.StringValue = "";
            var result = checkSystem.CheckWithErrorInfo();
            Assert.IsFalse(result.Item1);
            Assert.AreEqual(2, result.Item2.First().Item1);
            Assert.AreEqual(join.Id, result.Item2.First().Item2.First());
        }

        [Test]
        public void CheckWithErrorInfoPositionalOperatorsHaveReadersThroughEdgesTest()
        {
            var queryModel = repo.Model("QueryTestModel");
            var queryStrategy = new QueryConstraintsCheckStrategy(queryModel);
            var checkSystem = new ConstraintsCheckSystem(queryModel, queryStrategy);
            var join = queryModel.Nodes.Where(x => x.Name == "join").FirstOrDefault();
            var edge1 = queryModel.Edges.Where(x => x.To.Name == "read1").FirstOrDefault();
            var edge2 = queryModel.Edges.Where(x => x.To.Name == "read2").FirstOrDefault();
            queryModel.DeleteElement(edge1);
            queryModel.DeleteElement(edge2);
            var result = checkSystem.CheckWithErrorInfo();
            Assert.IsFalse(result.Item1);
            Assert.AreEqual(2, result.Item2.First().Item1);
            Assert.AreEqual(join.Id, result.Item2.First().Item2.First());
        }

        [Test]
        public void CheckWithErrorInfoPositionalOperatorsHaveReadersMultipleMultipleTest()
        {
            var queryModel = repo.Model("QueryTestModel");
            var queryStrategy = new QueryConstraintsCheckStrategy(queryModel);
            var checkSystem = new ConstraintsCheckSystem(queryModel, queryStrategy);
            var join = queryModel.Nodes.Where(x => x.Name == "join").FirstOrDefault();
            var edge1 = queryModel.Edges.Where(x => x.To.Name == "read1").FirstOrDefault();
            var edge2 = queryModel.Edges.Where(x => x.To.Name == "read2").FirstOrDefault();
            queryModel.DeleteElement(edge1);
            queryModel.DeleteElement(edge2);
            var filter = queryModel.Nodes.Where(x => x.Name == "filter").FirstOrDefault();
            var filterChildren = filter.Attributes.Where(x => x.Name == "children").FirstOrDefault();
            filterChildren.StringValue = "";
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