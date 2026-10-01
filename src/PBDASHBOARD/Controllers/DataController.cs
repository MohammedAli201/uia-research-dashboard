using System;
using System.Collections.Generic;
 using System.ComponentModel.DataAnnotations;
 using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using PBDASHBOARD.Data;
using PBDASHBOARD.Models;
using PBDASHBOARD.Services;

namespace PBDASHBOARD.Controllers
{ 
    [ApiController]
    [Route("[controller]")]
    public class DataController : ControllerBase
    {
        private ApplicationDbContext _dbContext;
        private IMemoryCache _cache;
        private IDataModifier _api;
     


        public DataController(ApplicationDbContext dbContext, IMemoryCache memoryCache)
        {
            _dbContext = dbContext;
            _cache = memoryCache;
            _api= new DataModifier(_dbContext,_cache);


        }

      


        [HttpGet ("GetListOfAllFaculties")]
        public Task<List<string>> GetListOfAllFaculties()
        { 
            return _api.GetFaculty();
        }

        [HttpGet("GetInstituteByFacultyName")]
        public Task<List<Institute>> GetInstituteByFacultyName(string facultyName) =>
            _api.GetInstituesByFacultName(facultyName);
       

        [HttpGet("GroupByFaculty")]
        public IDictionary<string, Dictionary<string,int>> GroupByFaculty() => _api.TopTenForGroupe();

        [HttpPost("InstituteAndPublication")]
        [Produces("application/json")]
        [DataType("json")]

        public IDictionary<string, Dictionary<string,int>> InstAndPublication(JsonObjectsFraView name)

        {
            string n = name.name;
           
            
        return _api.GroupingInstituteByFaculty(n); }
        
        [HttpGet("ProjectGroupByFaculty")] public IDictionary<string, int> ProjectGroubByFaculty() => _api.GroupByFacultyProject();

        [HttpGet("listpublicationByDateOfPubLicationWithInsititute")]
        public IDictionary<string, Dictionary<string, int>> instListWithPub()
        {
            return _api.PublicationBelongInstitutes();
        }
        
        [HttpGet("")]
        public Task<Dictionary<string, int>> ResearchGroup()
        {
            return _api.GetInstitutes();
        }
        [HttpGet("GetGroupWithPublicationa")]

        public Dictionary<string,  int> GetGroupWithPublicationa()
        {
            return _api.GroupContributor_Publicationss();
        }
        
        [HttpGet("ListOfGroups")]

        public List<string> listOfGroups()
        {
        List<string>list= new List<string>();
            var det= _api.GroupContributor_Publicationss();
            foreach (var VARIABLE in det)
            {
              list.Add(VARIABLE.Key);  
                
            }

            return list;
        }

        [HttpGet("samarbied")]
        public Dictionary<string, int> Samarbeid()
        {
            return _api.Samarbeid();
        }














    }
}