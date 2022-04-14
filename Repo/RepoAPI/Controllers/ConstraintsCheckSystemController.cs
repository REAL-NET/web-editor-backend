﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using AutoMapper;
using Repo;
using RepoAPI.Models;
using RepoConstraintsCheck;

namespace RepoAPI.Controllers
{
    /// <summary>
    /// Constraints controller is used to create, get, change constraint and also to check model.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ConstraintsCheckSystemController : ControllerBase
    {
        private IModel targetModel;
        private readonly IMapper _mapper;
        private NativeConstraintsCheckSystem checkingSystem;
        /// <summary>
        /// Initializes a new instance of the <see cref="T:RepoAPI.Controllers.ContraintsController"/> class.
        /// </summary>
        /// <param name="mapper"></param>
        public ConstraintsCheckSystemController(IMapper mapper)
        {
            _mapper = mapper;
        }

        /// <summary>
        /// Return all visible constraint names in repository.
        /// </summary>
        /// <returns>The sequence of constraints names.</returns>
        [HttpGet("all")]
        public ActionResult<IEnumerable<string>> GetModels() =>
                RepoContainer.CurrentRepo()
                .Models
                .ToList()
                .ConvertAll<string>(model => model.Name)
                .FindAll(name => name.Contains("Constraint"));

        [HttpGet("check/{targetModel}")]
        public ActionResult<bool> CheckModel(string targetModel)
        {
            lock (Locker.obj)
            {
                this.targetModel = RepoContainer.CurrentRepo().Model(targetModel);
                var constraint1 = RepoContainer.CurrentRepo().Model("SimpleConstraintsModel");
                checkingSystem = new NativeConstraintsCheckSystem(this.targetModel);
                checkingSystem.AddConstraint(constraint1, "SimpleConstraintsModel");
            }
            return checkingSystem.Check();
        }

        [HttpGet("secondcheck/{targetModel}")]
        public ActionResult<bool> SecondCheckModel(string targetModel)
        {
            var result = false;
            lock (Locker.obj)
            {
                this.targetModel = RepoContainer.CurrentRepo().Model(targetModel);
                var constraint1 = RepoContainer.CurrentRepo().Model("SimpleConstraintsModel");
                var constraint2 = RepoContainer.CurrentRepo().Model("SecondConstraintsModel");
                var secondCheckingSystem = new NativeConstraintsCheckSystem(this.targetModel);
                secondCheckingSystem.AddConstraint(constraint1, "SimpleConstraintsModel");
                secondCheckingSystem.AddConstraint(constraint2, "SecondConstraintsModel");
                result = secondCheckingSystem.Check();
            }
            return result;
        }

        [HttpGet("queryCheck/{targetModel}")]
        public ActionResult<bool> QueryCheckModel(string targetModel)
        {
            var result = false;
            lock (Locker.obj)
            {
                this.targetModel = RepoContainer.CurrentRepo().Model(targetModel);
                var queryStrategy = new QueryConstraintsCheckStrategy(this.targetModel);
                var checkingSystem = new ConstraintsCheckSystem(this.targetModel, queryStrategy);
                result = checkingSystem.Check();
            }
            return result;
        }

        [HttpGet("count/{targetModel}")]
        public ActionResult<int> QueryCount(string targetModel)
        {
            var result = 0;
            lock (Locker.obj)
            {
                this.targetModel = RepoContainer.CurrentRepo().Model(targetModel);
                var queryStrategy = new QueryConstraintsCheckStrategy(this.targetModel);
                var checkingSystem = new ConstraintsCheckSystem(this.targetModel, queryStrategy);
                result = checkingSystem.Count();
            }
            return result;
        }

        //[HttpPost("{targetModel}/{name}")]
        //public void AddConstraint(string targetModel, string name)
        //{
        //  lock (Locker.obj)
        //  {
        //    checkingSystem.AddConstraint
        //  }
        //}
    }
}
