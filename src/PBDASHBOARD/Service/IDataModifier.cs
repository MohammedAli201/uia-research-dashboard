using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PBDASHBOARD.Models;
using Polly.Caching;

namespace PBDASHBOARD.Services
{
    public interface   IDataModifier: IDisposable
    {
        // get all list of faculties= list of faculties Names
        Task<List<string>> GetFaculty();
        // take name of faculties Name and then returnes its institutes.
        Task<List<Institute>> GetInstituesByFacultName(string name);

        // return list of all institue's name
        Task<Dictionary<string, int>> GetInstitutes();
        public Dictionary<string, int> GroupByFacultyProject();
        public Dictionary<string, Dictionary<string,int>> TopTenForGroupe();
        public Dictionary<string,int> GroupContributor_Publicationss();
        public Dictionary<string, Dictionary<string, int>> GroupingInstituteByFaculty(string nameOfFc);
        public Dictionary<string, Dictionary<string, int>> PublicationBelongInstitutes();
        public Dictionary<string, int> Samarbeid();








    }
}