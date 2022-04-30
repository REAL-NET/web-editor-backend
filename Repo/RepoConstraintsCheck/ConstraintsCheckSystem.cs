using System.Collections.Generic;
using System.Linq;
using Repo;

namespace RepoConstraintsCheck
{
    public class ConstraintsCheckSystem : IConstraintsCheckSystem
    {
        private IConstraintsCheckStrategy checkStrategy;
        private IModel targetModel;

        public ConstraintsCheckSystem(IModel model, IConstraintsCheckStrategy strategy)
        {
            checkStrategy = strategy;
            targetModel = model;
        }

        public (bool, IEnumerable<(int, IEnumerable<int>)>) Check()
        {
            return checkStrategy.Check(targetModel);
        }

        public int Count()
        {
            return targetModel.Nodes.ToArray().Length;
        }
    }
}
