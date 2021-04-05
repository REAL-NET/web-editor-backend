using System.Collections.Generic;
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
        [HttpGet("node/{modelName}/{name}/asNode")]
        public ActionResult<Node> GetNode(string modelName, string name) =>
            _mapper.Map<Node>((INode)GetElementFromRepo(modelName, name));


        /// <summary>
        /// Returns the relationship in model specified by its name.
        /// </summary>
        /// <returns>The relationship.</returns>
        /// <param name="modelName">Model name.</param>
        /// <param name="name">Element name</param>
        [HttpGet("relationship/{modelName}/{name}/asRelationship")]
        public ActionResult<Relationship> GetRelationship(string modelName, string name) =>
            _mapper.Map<Relationship>((IDeepRelationship) GetElementFromRepo(modelName, name));

        
        /// <summary>
        /// Returns the association in model specified by its name.
        /// </summary>
        /// <returns>The association.</returns>
        /// <param name="modelName">Model name.</param>
        /// <param name="name">Element name</param>
        [HttpGet("association/{modelName}/{name}/")]
        public ActionResult<Association> GetAssociation(string modelName, string name) =>
            _mapper.Map<Association>((IDeepAssociation) GetElementFromRepo(modelName, name));
        
        
        /// <summary>
        /// Creates new element in model.
        /// </summary>
        /// <returns>The element.</returns>
        /// <param name="modelName">Model name.</param>
        /// <param name="name">New element name.</param>
        /// <param name="level">New element level.</param>
        /// <param name="potency">New element potency.</param>
        [HttpPost("node/{modelName}/{name}/{level}/{potency}")]
        public ActionResult<Node> CreateNode(string modelName, string name, int level, int potency)
        {
            lock (Locker.obj)
            {
                IDeepNode result = GetModelFromRepo(modelName).CreateNode(name, level, potency);
                return _mapper.Map<Node>(result);
            }
        }
        
        /// <summary>
        /// Creates new generalization in model by its parent.
        /// </summary>
        /// <returns>The element.</returns>
        /// <param name="modelName">Model name.</param>
        /// <param name="targetName">Target name.</param>
        /// <param name="level">New element level.</param>
        /// <param name="potency">New element potency.</param>
        /// <param name="sourceName">Source name.</param>
        [HttpPost("generalization/{modelName}/{sourceName}/{targetName}/{level}/{potency}")]
        public ActionResult<Generalization> CreateGeneralization(string modelName, string sourceName, string targetName, int level, int potency)
        {
            lock (Locker.obj)
            {
                IDeepElement source = GetElementFromRepo(modelName, sourceName);
                IDeepElement target = GetElementFromRepo(modelName, targetName);
                var result = GetModelFromRepo(modelName).CreateGeneralization(source, target, level, potency);
                return _mapper.Map<Generalization>(result);
            }
        }
        
        /// <summary>
        /// Creates new relationship in model.
        /// </summary>
        /// <returns>The element.</returns>
        /// <param name="modelName">Model name.</param>
        /// <param name="name">Name.</param>
        /// <param name="targetName">Target name.</param>
        /// <param name="level">New element level.</param>
        /// <param name="potency">New element potency.</param>
        /// <param name="sourceName">Source name.</param>
        /// <param name="minSource">Min level for source.</param>
        /// <param name="maxSource">Max level for source.</param>
        /// <param name="minTarget">Min level for target.</param>
        /// <param name="maxTarget">Max level for target.</param>
        [HttpPost("association/{modelName}/{name}/{sourceName}/{targetName}/{level}/{potency}/{minSource}/{maxSource}/{minTarget}/{maxTarget}")]
        public ActionResult<Association> CreateAssociation(
            string modelName, string name, string sourceName, string targetName, 
            int level, int potency, int minSource, int maxSource, int minTarget, int maxTarget)
        {
            lock (Locker.obj)
            {
                IDeepElement source = GetElementFromRepo(modelName, sourceName);
                IDeepElement target = GetElementFromRepo(modelName, targetName);
                var result = GetModelFromRepo(modelName).CreateAssociation(source, target, name, level, potency, 
                    minSource, maxSource, minTarget, maxTarget);
                return _mapper.Map<Association>(result);
            }
        }
        
        /// <summary>
        /// Creates new element in model by its parent.
        /// </summary>
        /// <returns>The element.</returns>
        /// <param name="modelName">Model name.</param>
        /// <param name="parentName">Parent name.</param>
        /// <param name="name">New element name.</param>
        /// <param name="level">New element level.</param>
        /// <param name="potency">New element potency.</param>
        [HttpPost("node/{modelName}/{parentName}/{name}/{level}/{potency}")]
        public ActionResult<Node> InstantiateNode(string modelName, string parentName, string name, int level, int potency)
        {
            lock (Locker.obj)
            {
                IDeepModel meta = GetModelFromRepo(modelName).Metamodel;
                IDeepNode parentElement = (IDeepNode) GetElementFromRepo(meta.Name, parentName);
                IDeepNode result = GetModelFromRepo(modelName).InstantiateNode(name, parentElement, level, potency);
                return _mapper.Map<Node>(result);
            }
        }

        /// <summary>
        /// Creates new relationship in model by its parent.
        /// </summary>
        /// <returns>The element.</returns>
        /// <param name="modelName">Model name.</param>
        /// <param name="parentName">Parent name.</param>
        /// <param name="targetName">Target name.</param>
        /// <param name="level">New element level.</param>
        /// <param name="potency">New element potency.</param>
        /// <param name="sourceName">Source name.</param>
        /// <param name="minSource">Min level for source.</param>
        /// <param name="maxSource">Max level for source.</param>
        /// <param name="minTarget">Min level for target.</param>
        /// <param name="maxTarget">Max level for target.</param>
        [HttpPost("association/{modelName}/{parentName}/{sourceName}/{targetName}/{level}/{potency}/{minSource}/{maxSource}/{minTarget}/{maxTarget}")]
        public ActionResult<Association> InstantiateAssociation(
            string modelName, string parentName, string sourceName, string targetName, 
            int level, int potency, int minSource, int maxSource, int minTarget, int maxTarget)
        {
            lock (Locker.obj)
            {
                IDeepModel meta = GetModelFromRepo(modelName).Metamodel;
                IDeepAssociation parentElement = (IDeepAssociation) GetElementFromRepo(meta.Name, parentName);
                IDeepElement source = (IDeepElement) GetElementFromRepo(meta.Name, sourceName);
                IDeepElement target = (IDeepElement) GetElementFromRepo(meta.Name, targetName);
                var result = GetModelFromRepo(modelName).InstantiateAssociation(source, target, parentElement, level, potency, 
                    minSource, maxSource, minTarget, maxTarget);
                return _mapper.Map<Association>(result);
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
        /// Set slot value
        /// </summary>
        /// <param name="modelName">Model name.</param>
        /// <param name="elementName">Element name.</param>
        /// <param name="attributeName">Attribute name.</param>
        /// <param name="newValue">Value element name.</param>
        [HttpPut("{modelName}/{elementName}/attribute/{attributeName}/{value}")]
        public ActionResult<Slot> SetSlotValue(string modelName, string elementName, string attributeName, string newValue)
        {
            lock (Locker.obj)
            {
                var valueElement = GetElementFromRepo(modelName, newValue);
                var element = GetElementFromRepo(modelName, elementName);
                var slot = element.Slots.First(it => it.Attribute.Name == attributeName);
                slot.Value = valueElement;
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
