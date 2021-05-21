using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using AutoMapper;

using Repo;
using Repo.Serializer;
using RepoAPI.Models;

namespace RepoAPI.Controllers
{ 
    /// <summary>
    /// Repo controller is used to controll the whole repository.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class RepoController : ControllerBase
    {

        /// <summary>
        /// Saves repository into file.
        /// </summary>
        [HttpPost("save")]
        public void SaveRepo([FromBody] SerializationRequest request)
        {
            lock (Locker.obj)
            {
                RepoContainer.CurrentRepo().Save("serialized/" + request.Filename);
            }
        }
        
        /// <summary>
        /// Loads repository from file.
        /// </summary>
        [HttpPost("load")]
        public void LoadRepo([FromBody] SerializationRequest request)
        {
            lock (Locker.obj)
            {
                RepoContainer.Load("serialized/" + request.Filename);
            }
        }
    }
}
