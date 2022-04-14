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

        public bool Check()
        {
            return checkStrategy.Check(targetModel);
        }

        public int Count()
        {
            return targetModel.Nodes.ToArray().Length;
        }
    }
}
