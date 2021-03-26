﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using AutoMapper;
using Repo;
using Repo.DeepMetamodel;
using RepoAPI.Models;

namespace RepoAPI.Controllers
{
    /// <summary>
    /// Element controller is used to create, get, change and remove elements.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ElementController : ControllerBase
    {
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:RepoAPI.Controllers.ElementController"/> class.
        /// </summary>
        /// <param name="mapper">Mapper is used to map object from Repo to Model classes.</param>
        public ElementController(IMapper mapper)
        {
            _mapper = mapper;
        }

        /// <summary>
        /// Returns the element in model specified by its unique number key.
        /// </summary>
        /// <returns>The element.</returns>
        /// <param name="modelName">Model name.</param>
        /// <param name="name">Element name</param>
        [HttpGet("{modelName}/{name}")]
        public ActionResult<Element> GetElement(string modelName, string name) =>
            _mapper.Map<Element>(GetElementFromRepo(modelName, name));

        /// <summary>
        /// Returns the node in model specified by its unique number key.
        /// </summary>
        /// <returns>The node.</returns>
        /// <param name="modelName">Model name.</param>
        /// <param name="name">Element name</param>
        [HttpGet("{modelName}/{name}/asNode")]
        public ActionResult<Node> GetNode(string modelName, string name) =>
            _mapper.Map<Node>((INode)GetElementFromRepo(modelName, name));


        /// <summary>
        /// Returns the edge in model specified by its unique number key.
        /// </summary>
        /// <returns>The edge.</returns>
        /// <param name="modelName">Model name.</param>
        /// <param name="name">Element name</param>
        [HttpGet("{modelName}/{name}/asEdge")]
        public ActionResult<Relationship> GetEdge(string modelName, string name) =>
            _mapper.Map<Relationship>((IDeepRelationship) GetElementFromRepo(modelName, name));


        /// <summary>
        /// Creates new element in model by its parent.
        /// </summary>
        /// <returns>The element.</returns>
        /// <param name="modelName">Model name.</param>
        /// <param name="parentName">Parent name.</param>
        /// <param name="name">New element name.</param>
        /// <param name="level">New element level.</param>
        /// <param name="potency">New element potency.</param>
        [HttpPost("{modelName}/{parentName}/{name}/{level}/{potency}")]
        public ActionResult<Element> CreateElement(string modelName, string parentName, string name, int level, int potency)
        {
            lock (Locker.obj)
            {
                IDeepModel meta = GetModelFromRepo(modelName).Metamodel;
                IDeepNode parentElement = (IDeepNode) GetElementFromRepo(meta.Name, parentName);
                IDeepElement result = GetModelFromRepo(modelName).InstantiateNode(name, parentElement, level, potency);
                return _mapper.Map<Element>(result);
            }
        }

        /// <summary>
        /// Returns all attributes for given element.
        /// </summary>
        /// <param name="modelName">Model name.</param>
        /// <param name="elementName">Element name.</param>
        [HttpGet("{modelName}/{name}/attributes")]
        public ActionResult<IEnumerable<Attribute>> GetAttributes(string modelName, string elementName)
        {
            lock (Locker.obj)
            {
                return _mapper.Map<List<Attribute>>(GetElementFromRepo(modelName, elementName).Attributes);
            }
        }
        
        
        /// <summary>
        /// Returns attribute for given element with specified name.
        /// </summary>
        /// <param name="modelName">Model name.</param>
        /// <param name="elementName">Element name.</param>
        /// <param name="attributeName">Attribute name.</param>
        [HttpGet("{modelName}/{name}/attribute/{attributeName}")]
        public ActionResult<Attribute> GetAttribute(string modelName, string elementName, string attributeName)
        {
            lock (Locker.obj)
            {
                var attributes = GetElementFromRepo(modelName, elementName).Attributes;
                var attribute = attributes.First(it => it.Name == attributeName);
                return _mapper.Map<Attribute>(attribute);
            }
        }
        

        /// <summary>
        /// Adds the attribute into element.
        /// </summary>
        /// <param name="modelName">Model name.</param>
        /// <param name="name">Element name.</param>
        /// <param name="attributeName">Attribute name.</param>
        /// <param name="type">Type element name.</param>
        /// <param name="level">New element level.</param>
        /// <param name="potency">New element potency.</param>
        [HttpPost("{modelName}/{name}/attribute/{attributeName}/{type}/{level}/{potency}")]
        public ActionResult<Attribute> AddAttribute(string modelName, string name,
            string attributeName, string type, int level, int potency)
        {
            lock (Locker.obj)
            {
                var typeElement = GetElementFromRepo(modelName, type);
                var attribute = GetElementFromRepo(modelName, name)
                    .AddAttribute(attributeName, typeElement, level, potency);
                return _mapper.Map<Attribute>(attribute);
            }
        }
        
        /// <summary>
        /// Set attribute single or dual.
        /// </summary>
        /// <param name="modelName">Model name.</param>
        /// <param name="elementName">Element name.</param>
        /// <param name="attributeName">Attribute name.</param>
        /// <param name="single">Is attribute single.</param>
        [HttpPut("{modelName}/{elementName}/attribute/{attributeName}/{single}")]
        public void AddAttributeSingle(string modelName, string elementName, string attributeName, bool single)
        {
            lock (Locker.obj)
            {
                var attributes = GetElementFromRepo(modelName, elementName).Attributes;
                var attribute = attributes.First(it => it.Name == attributeName);
                attribute.IsSingle = single;
            }
        }

        
        /// <summary>
        /// Returns element slots
        /// </summary>
        /// <param name="modelName">Model name.</param>
        /// <param name="elementName">Element name.</param>
        [HttpGet("{modelName}/{name}/slots")]
        public ActionResult<IEnumerable<Slot>> GetSlots(string modelName, string elementName)
        {
            lock (Locker.obj)
            {
                var element = GetElementFromRepo(modelName, elementName);
                return _mapper.Map<List<Slot>>(element.Slots);
            }
        }
        
        
        /// <summary>
        /// Returns slot for given element with specified attribute name.
        /// </summary>
        /// <param name="modelName">Model name.</param>
        /// <param name="elementName">Element name.</param>
        /// <param name="attributeName">Attribute name.</param>
        [HttpGet("{modelName}/{name}/slot/{attributeName}")]
        public ActionResult<Slot> GetSlot(string modelName, string elementName, string attributeName)
        {
            lock (Locker.obj)
            {
                var attributes = GetElementFromRepo(modelName, elementName).Slots;
                var attribute = attributes.First(it => it.Attribute.Name == attributeName);
                return _mapper.Map<Slot>(attribute);
            }
        }
        
        
        /// <summary>
        /// Adds the slot into element.
        /// </summary>
        /// <param name="modelName">Model name.</param>
        /// <param name="elementName">Element name.</param>
        /// <param name="attributeName">Attribute name.</param>
        /// <param name="value">Value element name.</param>
        /// <param name="level">New element level.</param>
        /// <param name="potency">New element potency.</param>
        [HttpPost("{modelName}/{elementName}/attribute/{attributeName}/{value}/{level}/{potency}")]
        public ActionResult<Slot> AddSlot(string modelName, string elementName,
            string attributeName, string value, int level, int potency)
        {
            lock (Locker.obj)
            {
                var valueElement = GetElementFromRepo(modelName, value);
                var element = GetElementFromRepo(modelName, elementName);
                var attribute = element.Attributes.First(it => it.Name == attributeName);
                var slot = element.AddSlot(attribute, valueElement, level, potency);
                return _mapper.Map<Slot>(slot);
            }
        }

        
        /// <summary>
        /// Removes the element from model.
        /// </summary>
        /// <param name="modelName">Model name.</param>
        /// <param name="name">Element identifier.</param>
        [HttpDelete("{modelName}/{name}")]
        public void DeleteElement(string modelName, string name)
        {
            lock (Locker.obj)
            {
                GetModelFromRepo(modelName).DeleteElement(
                    GetElementFromRepo(modelName, name));
            }
        }

        private IDeepElement GetElementFromRepo(string modelName, string name) =>
            GetModelFromRepo(modelName)
                .Elements
                .First(elem => (elem.Name == name));

        private IDeepModel GetModelFromRepo(string name) =>
            RepoContainer.CurrentRepo().Model(name);


    }
}
