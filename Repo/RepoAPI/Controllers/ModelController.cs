﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using AutoMapper;
using Repo.DeepMetamodel;
using RepoAPI.Models;

namespace RepoAPI.Controllers
{
    /// <summary>
    /// Model controller is used to create, get, change and remove models.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ModelController : ControllerBase
    {
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:RepoAPI.Controllers.ModelController"/> class.
        /// </summary>
        /// <param name="mapper">Mapper is used to map object from Repo to Model classes.</param>
        public ModelController(IMapper mapper)
        {
            _mapper = mapper;
        }

        /// <summary>
        /// Return all visible model names in repository.
        /// </summary>
        /// <returns>The sequence of models names.</returns>
        [HttpGet("all")]
        public ActionResult<IEnumerable<ModelInfo>> GetModels()
        {
            var result = RepoContainer.CurrentRepo()
                .Models
                .ToList();
            return _mapper.Map<List<ModelInfo>>(result);
        }

        /// <summary>
        /// Returns model by its name.
        /// </summary>
        /// <returns>Model by its name.</returns>
        /// <param name="modelName">Model name.</param>
        [HttpGet("{modelName}")]
        public ActionResult<Model> Model(string modelName) =>
            _mapper.Map<Model>(RepoContainer.CurrentRepo().Model(modelName));


        /// <summary>
        /// Return palette nodes for model.
        /// </summary>
        /// <param name="modelName">Model name.</param>
        [HttpGet("{modelName}/metanodes")]
        public ActionResult<IEnumerable<ElementInfo>> GetModelMetaNodes(string modelName)
        {
            IEnumerable<IDeepNode> result = new LinkedList<IDeepNode>();
            var currentModel = GetModelFromRepo(modelName).Metamodel;
            while (currentModel.Name != "LanguageMetamodel")
            {
                result = result.Concat(currentModel.Nodes);
                currentModel = currentModel.Metamodel;
            }
            return _mapper.Map<List<ElementInfo>>(result);
        }
        
        /// <summary>
        /// Return palette edges for model.
        /// </summary>
        /// <param name="modelName">Model name.</param>
        [HttpGet("{modelName}/metaedges")]
        public ActionResult<IEnumerable<ElementInfo>> GetModelMetaEdges(string modelName)
        {
            IEnumerable<IDeepAssociation> result = new LinkedList<IDeepAssociation>();
            var currentModel = GetModelFromRepo(modelName).Metamodel;
            while (currentModel.Name != "LanguageMetamodel")
            {
                result = result.Concat(currentModel.Relationships
                    .Where(it => it is IDeepAssociation)
                    .OfType<IDeepAssociation>()
                );
                currentModel = currentModel.Metamodel;
            }
            return _mapper.Map<List<ElementInfo>>(result);
        }

        /// <summary>
        /// Creates new model from metamodel.
        /// </summary>
        /// <param name="metamodel">Metamodel name.</param>
        /// <param name="name">Name from new model. Should be unique.</param>
        [HttpPost("{metamodel}/{name}")]
        public void CreateModelFromMetamodel(string metamodel, string name)
        {
            lock (Locker.obj)
            {
                var metamodelModel = RepoContainer.CurrentRepo().Model(metamodel);
                RepoContainer.CurrentRepo().InstantiateModel(name, metamodelModel);
                Console.Out.WriteLine("New model created");
            }
        }
        
        /// <summary>
        /// Creates new deep metamodel.
        /// </summary>
        /// <param name="name">Name from new model. Should be unique.</param>
        [HttpPost("{name}")]
        public void CreateDeepMetamodel(string name)
        {
            lock (Locker.obj)
            {
                RepoContainer.CurrentRepo().InstantiateDeepMetamodel(name);
                Console.Out.WriteLine("New model created");
            }
        }

        /// <summary>
        /// Changes the name of the model.
        /// </summary>
        /// <param name="modelName">Model name.</param>
        /// <param name="newName">New name.</param>
        [HttpPut("{modelName}/name/{newName}")]
        public void ChangeModelName(string modelName, string newName)
        {
            lock (Locker.obj)
            {
                GetModelFromRepo(modelName).Name = newName;
            }
        }

        /// <summary>
        /// Removes model from repository.
        /// </summary>
        /// <param name="modelName">Model name.</param>
        [HttpDelete("{modelName}")]
        public void DeleteModel(string modelName)
        {
            lock (Locker.obj)
            {
                IDeepModel model = GetModelFromRepo(modelName);
                RepoContainer.CurrentRepo().DeleteModel(model);
            }
        }

        private IDeepModel GetModelFromRepo(string name) =>
            RepoContainer.CurrentRepo().Model(name);

    }
}
