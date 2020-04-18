using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Storage.Models;
using Storage.Services;

namespace Storage.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StorageController : ControllerBase
    {
        private readonly StorageService _storageService;

        public StorageController(StorageService storageService)
        {
            _storageService = storageService;
        }
        
        /// <summary>
        /// Returns all saves from storage database
        /// </summary>
        [HttpGet("all")]
        public ActionResult<List<Record>> GetListOfSaves()
        {
            return _storageService.GetRecords();
        }

        /// <summary>
        /// Returns all saves for given user
        /// </summary>
        /// <param name="user">Username for saves to be returned</param>
        [HttpGet("{user}")]
        public ActionResult<List<Record>> GetListOfSaves(string user)
        {
            return _storageService.GetRecords(user);
        }
        
        /// <summary>
        /// Deletes a save information from storage database.
        /// </summary>
        /// <param name="record">Record object should contain save_id and user, others are unnecessary</param>
        [HttpDelete("")]
        public ActionResult GetListOfSaves([FromBody] Record record)
        {
            _storageService.DeleteRecord(record);
            return Ok();
        }

        /// <summary>
        /// Loads repo with specified save_id.
        /// </summary>
        /// <param name="record">Record object with specifies save_id field</param>
        [HttpPost("load")]
        public ActionResult LoadRepo([FromBody] Record record)
        {
            var token = Request.Headers.Keys.Contains("authorization") 
                ? Request.Headers["authorization"].ToString() 
                : null;
            _storageService.LoadRepo(record.SaveId, token);
            return Ok();
        }
        
        /// <summary>
        /// Loads last repo for given user.
        /// </summary>
        /// <param name="user">Username for last repo load</param>
        [HttpGet("load/last/{user}")]
        public ActionResult LoadLastRepo(string user)
        {
            var token = Request.Headers.Keys.Contains("authorization") 
                ? Request.Headers["authorization"].ToString() 
                : null;
            _storageService.LoadLastRepo(user, token);
            return Ok();
        }

        /// <summary>
        /// Saves repo into storage.
        /// SaveRequest object to be passed should have user,
        /// additional info if needed, last modification date if specified.
        /// </summary>
        /// <param name="saveRequest">SaveRequest object with info</param>
        [HttpPost("save")]
        public ActionResult SaveRepo([FromBody] SaveRequest saveRequest)
        {
            var token = Request.Headers.Keys.Contains("authorization") 
                ? Request.Headers["authorization"].ToString() 
                : null;
            _storageService.Save(
                saveRequest.User,
                saveRequest.Info, 
                saveRequest.LastModified,
                token);
            return Ok();
        }
    }
}