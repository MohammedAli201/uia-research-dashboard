using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using PBDASHBOARD.Data;
using PBDASHBOARD.Models;
using PBDASHBOARD.Services;

namespace PBDASHBOARD.Controllers
{
    public class DataModifier : IDataModifier
    {
        private readonly ApplicationDbContext _dbContext;
        private IMemoryCache _cache;
        private Dictionary<string, int> groupBf;
        private Dictionary<string, int> groupProject;
        private Dictionary<string, int> _objects;
        private Dictionary<string, int> _objectsProject;
        private Dictionary<object, int> lastOne;
        List<object> obs = new List<object>();
        private Dictionary<string, Dictionary<string, int>> newD;
        private Dictionary<string, Dictionary<string, int>> newProject;


        public DataModifier()
        {
        }

        public DataModifier(ApplicationDbContext dbContext, IMemoryCache cache)
        {
            _dbContext = dbContext;
            _cache = cache;
        }

        //this method has implemented cache:
        public async Task<List<string>> GetFaculty()
        {
            List<string> facultiesName = new List<string>();

            var namesOfFaculties = await _dbContext.Faculties.Select(e => e.UnitName).ToListAsync();
            foreach (var VARIABLE in namesOfFaculties)
            {
                if (VARIABLE.En == null)
                {
                    if (VARIABLE.Nb == "Naturmuseum og botanisk hage")
                    {
                        facultiesName.Add("Nature Museum and Botanical Garden");
                    }
                    else if (VARIABLE.Nb == "Fellesadministrasjonen")
                    {
                        facultiesName.Add("The joint administration");
                    }
                }
                else
                    facultiesName.Add(VARIABLE.En);
            }


            return facultiesName;
        }

        public async Task<List<Institute>> GetInstituesByFacultName(string name)
        {
            Console.WriteLine("");
            return await _dbContext.Institutes.Where(e => e.UnitName.Nb == name).ToListAsync();
        }

        public async Task<Dictionary<string, int>> GetInstitutes()
        {
            var amountspub = 0;
            var queryLastNames =
                (from pp in _dbContext.Publications select pp).ToList();

            var gr = from queryLastName in queryLastNames.GroupBy(g => g.YearPublished) select queryLastName;

            Dictionary<string, int> vv = new Dictionary<string, int>();

            foreach (var VARIABLE in gr)
            {
                foreach (var vm in VARIABLE)
                {
                    if (!vv.ContainsKey(vm.YearPublished))
                    {
                        amountspub = 0;
                        vv.Add(vm.YearPublished, amountspub);
                    }
                    else
                    {
                        vv[vm.YearPublished]++;
                    }
                }
            }

            Console.WriteLine(vv);
            return vv;
        }

        // this api is avialable incase it needed.

        public Dictionary<string, int> GroupByFacultyProject()
        {
            var years = "";
            newProject = new Dictionary<string, Dictionary<string, int>>();
            _objectsProject = new Dictionary<string, int>();
            var numberOfFacutlies = 0;
            var facultProject = (from fc in _dbContext.InstituteResearchProjects select fc.Institute).ToList();

            foreach (var temp in facultProject)
            {
                var fc = (from institute in _dbContext.Faculties.Where(it => it.Id == temp.FacultyId)
                    select institute.UnitName).ToList();

                foreach (var namefc in fc)
                {
                    if (namefc.En != null)
                    {
                        if (!_objectsProject.ContainsKey(namefc.En))
                        {
                            _objectsProject.Add(namefc.En, numberOfFacutlies);
                        }
                        else
                        {
                            _objectsProject[namefc.En]++;
                        }
                    }
                }
            }


            return _objectsProject;
        }

        public Dictionary<string, Dictionary<string, int>> TopTenForGroupe()
        {
            Dictionary<string, Dictionary<string, int>> topTenDc = new Dictionary<string, Dictionary<string, int>>();
            Dictionary<string,int>vales=new Dictionary<string, int>();
            //This will be cache as soon as it rececved from listInstitute;
            if (!_cache.TryGetValue("topTen", out Dictionary<string, Dictionary<string, int>> cacheTemp))
            {
                var dem = PublicationBelongInstitutes();

                foreach (var value_list in dem)
                {
                    string key = value_list.Key;
                    int count = value_list.Value.Values.ToList()[0];
                    vales.Add(key,count );
                    var tm =
                        (from uNames in _dbContext.Institutes.Where(i => i.UnitName.Nb == value_list.Key)
                            select uNames.Faculty)
                        .ToList();
                    foreach (var l in tm)
                    {
                        var fcName =
                            (from faculty in _dbContext.Faculties.Where(it => it.CristinUnitId == l.CristinUnitId)
                                select faculty.UnitName).ToList();

                        var nb = fcName[0].Nb;


                        if (nb != null)
                        {
                            if (!topTenDc.ContainsKey(nb))
                            {
                                
                                topTenDc.Add(nb, value_list.Value);
                            }
                            else
                            {
                                int counts = 0;
                                Dictionary<string,int>dd= new Dictionary<string, int>();
                               var keys=  topTenDc[nb];
                               foreach (var ite in keys)
                               {
                                   int amounts= ite.Value + value_list.Value.Values.ToList()[counts];
                                   counts++;
                                   if (!dd.ContainsKey(ite.Key))
                                   { dd.Add(ite.Key, amounts); }
                                   else { dd[ite.Key] = amounts; } }
                               topTenDc[nb] = dd;
                            }
                        }
                    }
                }

                cacheTemp = topTenDc;

                _cache.Set("topTen", cacheTemp, TimeSpan.FromDays(1));
            }

            topTenDc = (Dictionary<string, Dictionary<string, int>>) _cache.Get("topTen");


            return topTenDc;
        }

        //this method will return all contributors that belongs institutes
        public Dictionary<string, int> GroupContributor_Publicationss()
        {
            // grouping 
            var groupingContributors = (from ob in _dbContext.PublicationContributors
                join o in _dbContext.ContributorReGroups on ob.ContributorId equals o.ContributorId
                group o by o.ResearchGroup.GroupName
                into gry
                select new {key = gry.Key, cnt = gry.Count()}).ToList();
            return groupingContributors.ToDictionary(eachContributors => eachContributors.key,
                eachContributors => eachContributors.cnt);
        }


        public Dictionary<string, Dictionary<string, int>> GroupingInstituteByFaculty(string nameOfFc)
        {
            Dictionary<string, Dictionary<string, int>> instituteDictionary =
                new Dictionary<string, Dictionary<string, int>>();

            Dictionary<string, int> tes = new Dictionary<string, int>();
            var dem = PublicationBelongInstitutes();
            var sameName = (from eachInstitute in _dbContext.Institutes.Where(it => it.Faculty.UnitName.En == nameOfFc)
                select eachInstitute.UnitName).ToList();

            foreach (var eachNameBelongInstitute in sameName)
            {
                if (!instituteDictionary.ContainsKey(eachNameBelongInstitute.Nb))
                {
                    var vl = dem[eachNameBelongInstitute.Nb];
                    instituteDictionary.Add(eachNameBelongInstitute.Nb, vl);
                }
            }


            return instituteDictionary;
        }

        public Dictionary<string, Dictionary<string, int>> PublicationBelongInstitutes()
        {
            // This is our Dictionary will will hold institutes

            Dictionary<string, Dictionary<string, int>> performance = new Dictionary<string, Dictionary<string, int>>();
            Dictionary<string, int> instAndPro = new Dictionary<string, int>();
            var amounts = 0;
            if (!_cache.TryGetValue("list", out Dictionary<string, Dictionary<string, int>> cacheTemp))
            {
                var groupingPublicationWithInstitute
                    = (from o in _dbContext.Publications
                        join ob2 in _dbContext.PublicationInstitutes on o.Id equals ob2.PublicationId
                        join ob in _dbContext.Institutes on ob2.InstituteId equals ob.Id
                        select new
                        {
                            yearPublished = o.YearPublished,
                            InstituteName = ob.UnitName.Nb,
                            total = ob.TotalCount,
                        }).ToList();
                foreach (var u in groupingPublicationWithInstitute)
                {
                    if (!performance.ContainsKey(u.InstituteName))
                    {
                        amounts = 0;
                        amounts++;
                        if (instAndPro.ContainsKey(u.yearPublished))
                        {
                            instAndPro = new Dictionary<string, int>();
                            instAndPro.Add(u.yearPublished, amounts);
                        }
                        else
                        {
                            instAndPro.Add(u.yearPublished, amounts);
                        }

                        performance.Add(u.InstituteName, instAndPro);
                    }
                    else if (performance.ContainsKey(u.InstituteName))
                    {
                        if (!instAndPro.ContainsKey(u.yearPublished))
                        {
                            instAndPro.Add(u.yearPublished, amounts);
                            performance[u.InstituteName] = instAndPro;
                        }
                        else
                        {
                            instAndPro[u.yearPublished]++;
                            performance[u.InstituteName] = instAndPro;
                        }
                    }
                }

                cacheTemp = performance;
                _cache.Set("list", cacheTemp, TimeSpan.FromDays(1));
            }

            performance = (Dictionary<string, Dictionary<string, int>>) _cache.Get("list");

            return performance;
        }

        public Dictionary<string, int> Samarbeid()
        {
            var tem = (from o in _dbContext.Affiliations
                join ob2 in _dbContext.Institutions on o.ContributorId equals ob2.Id
                join o2 in _dbContext.Contributors on o.ContributorId equals o2.Id
                select new
                {
                    cont = o.Contributor,
                    instit = o.Institution,
                    cnt = o.Contributor,
                    pub = o.Contributor.ContributorPublications
                }).ToList();

            var t = (from VAR in tem group VAR by VAR.instit.CristinUnitId).ToList();
            Console.WriteLine(t);
            int cnt = 0;

            Dictionary<string, int> tD = new Dictionary<string, int>();
            foreach (var VARIABLE in t)
            {
                tD.Add(VARIABLE.Key, VARIABLE.Count(
                ));
            }

            return tD;
        }


        //Implementing the Dispose method is primarily for releasing unmanaged resources
        public void Dispose() => _dbContext.Dispose();
        
    }
}