using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using AutoMapper;

using Repo;
using RepoAPI.Models;
using CheckingConstraints;

namespace RepoAPI.Controllers
{
  /// <summary>
  /// Constraints controller is used to create, get, change constraint and also to check model.
  /// </summary>
  [Route("api/[controller]")]
  [ApiController]
  public class ConstraintsCheckingSystemController : ControllerBase
  {
    private IModel targetModel;
    private readonly IMapper _mapper;
    private CheckingConstraints.ConstraintsCheckSystem checkingSystem;
    /// <summary>
    /// Initializes a new instance of the <see cref="T:RepoAPI.Controllers.ContraintsController"/> class.
    /// </summary>
    /// <param name="mapper"></param>
    public ConstraintsCheckingSystemController(IMapper mapper)
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
            .FindAll(name=> name.Contains("Constraint"));

    [HttpGet("check/{targetModel}")]
    public ActionResult<bool> CheckModel(string targetModel)
    {
      lock (Locker.obj)
      {
        this.targetModel = RepoContainer.CurrentRepo().Model(targetModel);
        var constraint1 = RepoContainer.CurrentRepo().Model("SimpleConstraintsModel");
        checkingSystem = new CheckingConstraints.ConstraintsCheckSystem(this.targetModel);
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
        var secondCheckingSystem = new CheckingConstraints.ConstraintsCheckSystem(this.targetModel);
        secondCheckingSystem.AddConstraint(constraint1, "SimpleConstraintsModel");
        secondCheckingSystem.AddConstraint(constraint2, "SecondConstraintsModel");
        result = secondCheckingSystem.Check();
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
