using NUnit.Framework;
using Repo;
using RepoConstraintsCheck;
using System.Linq;

namespace RepoConstraintsCheckTests
{
    public class Tests
    {
        private IRepo repo;
        private string modelName;

        [SetUp]
        public void Setup()
        {
            repo = RepoFactory.Create();
            modelName = "QueryTestModel";
        }

        [Test]
        public void CheckTest()
        {
            var queryModel = repo.Model(modelName);
            var queryStrategy = new QueryConstraintsCheckStrategy(queryModel);
            var checkSystem = new ConstraintsCheckSystem(queryModel, queryStrategy);
            Assert.IsTrue(checkSystem.Check());
        }

        [Test]
        public void CheckWithErrorInfoHaveNoErrorsTest()
        {
            var queryModel = repo.Model(modelName);
            var queryStrategy = new QueryConstraintsCheckStrategy(queryModel);
            var checkSystem = new ConstraintsCheckSystem(queryModel, queryStrategy);
            var result = checkSystem.CheckWithErrorInfo();
            Assert.IsTrue(result.Item1);
            Assert.IsEmpty(result.Item2);
        }

        [Test]
        public void CheckWithErrorInfoPositionalOperatorsHaveReadersTest()
        {
            var queryModel = repo.Model(modelName);
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
        public void CheckWithErrorInfoJoinOperatorsHaveAtLeastTwoReadersTest()
        {
            var queryModel = repo.Model(modelName);
            var queryStrategy = new QueryConstraintsCheckStrategy(queryModel);
            var checkSystem = new ConstraintsCheckSystem(queryModel, queryStrategy);
            var join = queryModel.Nodes.Where(x => x.Name == "aJoin").FirstOrDefault();
            var edges = queryModel.Edges.Where(x => x.From.Name == "aJoin" && x.To.Name == "aRead");
            queryModel.DeleteElement(edges.First());
            var result = checkSystem.CheckWithErrorInfo();
            Assert.IsFalse(result.Item1);
            Assert.AreEqual(5, result.Item2.First().Item1);
            Assert.AreEqual(join.Id, result.Item2.First().Item2.First());
        }

        [Test]
        public void CheckWithErrorInfoPositionalOperatorsHaveReadersMultipleTest()
        {
            var queryModel = repo.Model(modelName);
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

        [Test]
        public void CheckWithErrorInfoTupleOperatorsHaveNoReadersTest()
        {
            var queryModel = repo.Model(modelName);
            var queryStrategy = new QueryConstraintsCheckStrategy(queryModel);
            var checkSystem = new ConstraintsCheckSystem(queryModel, queryStrategy);
            var sort = queryModel.Nodes.Where(x => x.Name == "aSort").FirstOrDefault();
            var read = queryModel.Nodes.Where(x => x.Name == "aRead").FirstOrDefault();
            var metamodel = repo.Model(modelName).Metamodel;
            var edge = metamodel.Edges.Where(x => x.Name == "link").FirstOrDefault();
            var newEdge = queryModel.CreateElement(edge);
            var createdEdge = queryModel.Edges.Where(x => x.Id == newEdge.Id).FirstOrDefault();
            createdEdge.From = sort;
            createdEdge.To = read;
            var result = checkSystem.CheckWithErrorInfo();
            Assert.IsFalse(result.Item1);
            Assert.AreEqual(3, result.Item2.First().Item1);
            Assert.AreEqual(2, result.Item2.First().Item2.Count());
            Assert.IsTrue(result.Item2.First().Item2.Contains(sort.Id));
            Assert.IsTrue(result.Item2.First().Item2.Contains(read.Id));
        }

        [Test]
        public void CheckWithErrorInfoMultipleErrorsTest()
        {
            var queryModel = repo.Model(modelName);
            var queryStrategy = new QueryConstraintsCheckStrategy(queryModel);
            var checkSystem = new ConstraintsCheckSystem(queryModel, queryStrategy);

            // Positional operators have readers
            var join = queryModel.Nodes.Where(x => x.Name == "aJoin").FirstOrDefault();
            var edges = queryModel.Edges.Where(x => x.From.Name == "aJoin" && x.To.Name == "aRead");
            foreach (var edge in edges)
            {
                queryModel.DeleteElement(edge);
            }
            var filter = queryModel.Nodes.Where(x => x.Name == "aFilter").FirstOrDefault();
            var filterEdge = queryModel.Edges.Where(x => x.From.Name == "aFilter").FirstOrDefault();
            queryModel.DeleteElement(filterEdge);

            // Tuple operators have no readers
            var sort = queryModel.Nodes.Where(x => x.Name == "aSort").FirstOrDefault();
            var read = queryModel.Nodes.Where(x => x.Name == "aRead").FirstOrDefault();
            var metamodel = repo.Model(modelName).Metamodel;
            var metamodelEdge = metamodel.Edges.Where(x => x.Name == "link").FirstOrDefault();
            var newEdge = queryModel.CreateElement(metamodelEdge);
            var createdEdge = queryModel.Edges.Where(x => x.Id == newEdge.Id).FirstOrDefault();
            createdEdge.From = sort;
            createdEdge.To = read;

            var result = checkSystem.CheckWithErrorInfo();
            Assert.IsFalse(result.Item1);
            Assert.AreEqual(2, result.Item2.Count());
            Assert.AreEqual(2, result.Item2.First().Item1);
            Assert.AreEqual(3, result.Item2.ElementAt(1).Item1);
            Assert.IsTrue(result.Item2.First().Item2.Contains(join.Id));
            Assert.IsTrue(result.Item2.First().Item2.Contains(filter.Id));
            Assert.IsTrue(result.Item2.ElementAt(1).Item2.Contains(sort.Id));
            Assert.IsTrue(result.Item2.ElementAt(1).Item2.Contains(read.Id));
        }

        [Test]
        public void CheckWithErrorInfoLeavesAreDSTest()
        {
            var queryModel = repo.Model(modelName);
            var queryStrategy = new QueryConstraintsCheckStrategy(queryModel);
            var checkSystem = new ConstraintsCheckSystem(queryModel, queryStrategy);
            var edge = queryModel.Edges.Where(x => x.To.Name == "aDS").FirstOrDefault();
            queryModel.DeleteElement(edge);
            var join = queryModel.Nodes.Where(x => x.Name == "aJoin").FirstOrDefault();
            var result = checkSystem.CheckWithErrorInfo();
            Assert.IsFalse(result.Item1);
            Assert.AreEqual(4, result.Item2.First().Item1);
            Assert.AreEqual(join.Id, result.Item2.First().Item2.First());
        }

        [Test]
        public void CheckWithErrorInfoLastOperatorIsTupleTest()
        {
            var queryModel = repo.Model(modelName);
            var queryStrategy = new QueryConstraintsCheckStrategy(queryModel);
            var checkSystem = new ConstraintsCheckSystem(queryModel, queryStrategy);
            var sort = queryModel.Nodes.Where(x => x.Name == "aSort").FirstOrDefault();
            sort.Attributes.Where(x => x.Name == "type").First().StringValue = "";
            var result = checkSystem.CheckWithErrorInfo();
            Assert.IsFalse(result.Item1);
            Assert.AreEqual(6, result.Item2.First().Item1);
            Assert.AreEqual(sort.Id, result.Item2.First().Item2.First());
        }

        [Test]
        public void CheckWithErrorInfoMaterializingOperatorsHaveTupleParentTest()
        {
            var queryModel = repo.Model(modelName);
            var queryStrategy = new QueryConstraintsCheckStrategy(queryModel);
            var checkSystem = new ConstraintsCheckSystem(queryModel, queryStrategy);
            var sort = queryModel.Nodes.Where(x => x.Name == "aSort").FirstOrDefault();
            sort.Attributes.Where(x => x.Name == "type").First().StringValue = "";
            var result = checkSystem.CheckWithErrorInfo();
            Assert.IsFalse(result.Item1);
            Assert.AreEqual(2, result.Item2.Count());
            Assert.AreEqual(10, result.Item2.ElementAt(1).Item1);
            Assert.AreEqual(sort.Id, result.Item2.ElementAt(1).Item2.FirstOrDefault());
        }

        [Test]
        public void CheckWithErrorInfoChildrenTypesAreCorrectFirstTest()
        {
            var queryModel = repo.Model(modelName);
            var queryStrategy = new QueryConstraintsCheckStrategy(queryModel);
            var checkSystem = new ConstraintsCheckSystem(queryModel, queryStrategy);
            var aggregate = queryModel.Nodes.Where(x => x.Name == "aAggregate").FirstOrDefault();
            aggregate.Attributes.Where(x => x.Name == "type").First().StringValue = "";
            var result = checkSystem.CheckWithErrorInfo();
            Assert.IsFalse(result.Item1);
            Assert.AreEqual(7, result.Item2.First().Item1);
            Assert.AreEqual(aggregate.Id, result.Item2.First().Item2.First());
        }

        [Test]
        public void CheckWithErrorInfoChildrenTypesAreCorrectSecondTest()
        {
            var queryModel = repo.Model(modelName);
            var queryStrategy = new QueryConstraintsCheckStrategy(queryModel);
            var checkSystem = new ConstraintsCheckSystem(queryModel, queryStrategy);
            var edges = queryModel.Edges.Where(x => x.From.Name == "aJoin" && x.To.Name == "aRead");
            foreach (var edge in edges)
            {
                queryModel.DeleteElement(edge);
            }
            var join = queryModel.Nodes.Where(x => x.Name == "aJoin").FirstOrDefault();
            join.Attributes.Where(x => x.Name == "type").First().StringValue = "tuple";
            var ds = queryModel.Nodes.Where(x => x.Name == "aDS").FirstOrDefault();
            var result = checkSystem.CheckWithErrorInfo();
            Assert.IsFalse(result.Item1);
            Assert.AreEqual(2, result.Item2.Count());
            Assert.AreEqual(7, result.Item2.First().Item1);
            Assert.AreEqual(9, result.Item2.ElementAt(1).Item1);
            Assert.IsTrue(result.Item2.First().Item2.Contains(ds.Id));
            Assert.IsTrue(result.Item2.ElementAt(1).Item2.Contains(join.Id));
        }
    }
}