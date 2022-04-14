using System;
using System.Collections.Generic;
using System.Linq;
using Repo;

namespace RepoConstraintsCheck
{
    public class NativeConstraintsCheckSystem
    {
        // nodeName and modelName
        private List<Constraint> constraints;

        private IModel repoModel;
        private NativeConstraintsMatcher matcher;

        public NativeConstraintsCheckSystem(IModel targetModel)
        {
            this.constraints = new List<Constraint>();
            this.repoModel = targetModel;
            this.matcher = new NativeConstraintsMatcher(repoModel);
        }

        public string AddConstraint(IModel constraintModel, string name)
        {
            var repoConstraintModel = constraintModel;
            if (!matcher.PreCheck(repoConstraintModel))
            {
                throw new Exception(matcher.ErrorMsg);
            }
            this.constraints.Add(new Constraint (name, matcher.root, repoConstraintModel));
            return matcher.root.Name;
        }

        public void DeleteConstraint(string constraintModelName, int unitHash)
        {
            // var constraintModel = this.Repo.Model(constraintModelName);
            this.constraints.RemoveAll(x => x.UnitHash == unitHash);
        }

        public bool Check()
        {
            var result = true;
            foreach (var node in this.repoModel.Nodes) //foreach (var element in this.graph.DataGraph.Vertices)
            {
                if (constraints.Count() == 0)
                {
                    //this.targetModel.SetElementAllowed(node, true);
                }
                else
                {
                    //var node = element.Node;
                    foreach (var constraint in this.constraints)
                    {
                        var isAllowed = true;
                        if (this.matcher.ElementsAreIdentic(constraint.Root, node) || constraint.Root.Class.Name == "AllNodes")
                        {
                            isAllowed = matcher.Check(node, constraint.Root, constraint.Tree);
                            //this.targetModel.SetElementAllowed(node, isAllowed);
                        }
                        else if ((node.Class.Name == constraint.Root.Class.Name) && (constraint.Tree.Nodes.Count() == 1))
                        {
                            isAllowed = matcher.AttrCheck(node, constraint.Root);
                            //this.targetModel.SetElementAllowed(node, isAllowed);
                        }
                        else
                        {
                            //this.targetModel.SetElementAllowed(node, true);
                        }
                        result = result && isAllowed;
                    }
                }
            }
            return result;
        }
    }
}
