using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IIS;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PBDASHBOARD.Data;
using PBDASHBOARD.Models;
using PBDASHBOARD.Models.AssociationModels;
using PBDASHBOARD.Models.ModelMapping;
using PBDASHBOARD.Services;
using PBDASHBOARD.User;
using static PBDASHBOARD.BackgroundTaks.Utilities;

namespace PBDASHBOARD.BackgroundTaks
{
    public class ServiceTask
    {
        private readonly HttpClient _client;
        protected ApplicationDbContext DbContext;
        private string _datePublished;


        internal IServiceProvider ServiceProvider { get; set; }
        protected IConfiguration configuration { get; set; }

        protected ServiceTask(IHttpClientFactory clientFactory, IServiceProvider serviceProvider)
        {
            _client = clientFactory.CreateClient("Cristin");
            ServiceProvider = serviceProvider;
        }

        /// <summary>
        /// returns the
        /// </summary>
        /// <returns>true if _datePUblished is successfully set</returns>
        protected bool setDatePublished()
        {
            try
            {
                _datePublished = DbContext.syncDate.Find(1).lasSyncYear;
                if (_datePublished != null)
                {
                    return true;
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Message);
                
            }

            return false;
        }

        /// <summary>
        /// delagete that handeles generating the dateime.
        /// </summary>
        protected DelUpdateDatePublished getDateTimeNow = Utilities.getDateTimeNow;

        /// <summary>
        /// Update the lastSyncyear in database.
        /// </summary>
        /// <param name="newDAte">takes a delegate as param</param>
        protected void UpdatePublishedYear(DelUpdateDatePublished newDAte)
        {
            var lastSycYear = newDAte();

            try
            {
                DbContext.syncDate.Find(1).lasSyncYear = lastSycYear;
                DbContext.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                Console.WriteLine(e.Message);
                throw e;
            }

            Console.WriteLine(lastSycYear);
        }

        /// <summary>
        /// start the operation or reset the database to starting from 2017
        /// </summary>
        protected void reset()
        {
            _datePublished = configuration.GetSection("DatePublished").Value;
            if (!DbContext.syncDate.Any())
            {
                // create if does not exist 
                DbContext.syncDate.Add(new SynchronisationDate()
                {
                    startYear = _datePublished, lasSyncYear = _datePublished, runOnce = false
                });
                DbContext.SaveChanges();
            }
            else
            {
                // get the first row 
                var syncTable = DbContext.syncDate.Find(1);
                syncTable.startYear = _datePublished;
                syncTable.lasSyncYear = _datePublished;
                syncTable.runOnce = false;
                DbContext.SaveChanges();
            }
        }

        /// <summary>
        /// updates the first row of SynchronisationDate table first row. Turns the variable runOnce = true,
        /// </summary>
        /// <returns>Return true if table update, other wise false</returns>
        protected bool isRunOnce()
        {
            // try to save true since isRunonce is completed or return false becasue something happend.
            try
            {
                // if reset went well so the sysdate is updated correctly, there is run once must be updated
                var isRunonce = DbContext.syncDate.Find(1);
                isRunonce.runOnce = true;
                DbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        /// <summary>
        /// Returns true if the database had succesfully update completed synchronisation and sat the
        /// runOnce= true. Other wise returns false
        /// </summary>
        /// <returns></returns>
        protected bool getIsrunOnce()
        {
            if (DbContext.syncDate.Any())
            {
                var confirm = DbContext.syncDate.Find(1).runOnce;
                if (confirm)
                {
                    return true;
                }
            }

            return false;
        }

        private readonly List<string> facultyUrls = new List<string>();
        private readonly List<string> InstituteUrls = new List<string>();

        /*This list holds list of urls*/


        /// <summary>
        /// request sending method.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns>HttpResponse.content as string</returns>
        [HttpGet]
        private async Task<string> HttpRequestTask(string uri)
        {
            _client.DefaultRequestHeaders.Accept.Clear();

            var responseString = string.Empty;

            try
            {
                
                var response = await _client.GetAsync(uri);
                
                if (response.IsSuccessStatusCode)
                {
                    responseString = await response.Content.ReadAsStringAsync();
                  
                }
                else if (response.StatusCode == HttpStatusCode.InternalServerError)
                {
                    Console.WriteLine(response.StatusCode);
                }
                
            } catch (TaskCanceledException canceledException)
            {
                Console.WriteLine(canceledException.Message);
                Console.WriteLine(canceledException.StackTrace);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
            catch (InvalidOperationException Iexception)
            {
                Console.WriteLine(Iexception.Message);
                Console.WriteLine(Iexception.TargetSite);
                
            }
           

            return responseString;
        }
        
        /// <summary>
        /// Parse Uia.no webpage and collect research groups 
        /// </summary>
        /// <param name="htmlString"></param>
        /// <param name="groupNames"></param>
        /// <returns></returns>
        protected Dictionary<string, string> HmtlParse(string htmlString, Dictionary<string, string> groupNames)
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(htmlString);
            var searchGroupResult = htmlDocument.DocumentNode.Descendants("div").Where(node => node
                .GetAttributeValue("class", "")
                .Contains("searchresults")).ToList(); // get div element than holds the list of groups.

            var groupsHtmlNodes = searchGroupResult[0].Descendants("a").ToList();
            foreach (var node in groupsHtmlNodes)
            {
                var key = node.Elements("span")
                    .First(htmlNode => htmlNode.HasClass("attribute-title")).InnerText;
                //=>node.GetAttributeValue("class", "").Contains("attribute-title"))
                var val = node.Attributes.First(attribute => attribute.Name.Equals("href")).Value;
                //todo: find "amp" in the string and delete it from the url , it is causing problems 
                var origin = "https://www.uia.no";

                groupNames[key] = origin + val;
            } //registrering of groupname,groupUrl in dictionary as key value.

            return groupNames;
        }

        /// <summary>
        /// returns dictionary that holds groupName and list of members from Html string.
        /// </summary>
        /// <param name="htmlString"></param>
        /// <returns></returns>
        protected List<string> GetMembersList(KeyValuePair<string, string> str)
        {
            var htmlString = str.Value;
            var id = str.Key;
            var membersList = new List<string>();

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(htmlString);
            Console.WriteLine(id);
            List<HtmlNode> members = new List<HtmlNode>();
            List<HtmlNode> leaders = new List<HtmlNode>();
            List<HtmlNode> phdMembers = new List<HtmlNode>();

            try
            {
                members = htmlDocument.DocumentNode.Descendants("div").Where(node => node
                        .GetAttributeValue("class", "")
                        .Contains("openclose-content attribute-participants"))
                    .ToList(); // get the div element that holds the member lists.
                leaders = htmlDocument.DocumentNode.Descendants("div").Where(node => node
                    .GetAttributeValue("class", "").Contains("infobox")).ToList();
                /*phdMembers = htmlDocument.DocumentNode.Descendants("div").Where(node => node
                    .GetAttributeValue("class", "").Contains("associateMembers")).ToList();#1#*/
                if (members.Count == 0)
                {
                    Console.WriteLine("NULLL alert");
                    return new List<string>(); // return empty list.
                }

                var membersHtmlNodes = members[0].Descendants("li").ToList();
                foreach (var li in membersHtmlNodes)
                {
                    membersList.Add(li.InnerText); // add the names to the list
                }

                return membersList;
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return new List<string>();
            }
        }

        /// <summary>
        ///     web scraping uia.no for research groups.
        /// </summary>
        /// <returns></returns>
        protected async Task GetResearchGroupFromUia()
        {
            var baseUrl = "https://www.uia.no/researchgroups/search"; // first request to get webfragments links
            var jsonBaseUrl = "https://www.uia.no";
            var ResearGroupsResultUrls = new List<string>
            {
                jsonBaseUrl + "/researchgroups/search?Search=&SearchText=&Format=json"
            }; //url to current and next webpages. The first is added to the list.

            var responseStrings = new List<string>(); // list Of http Response strings
            var groupNames = new Dictionary<string, string>(); //Dictionary of key=groupNames value= group url.
            var htmlString = await _client.GetStringAsync(baseUrl);

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(htmlString);
            var searchGroupResult = htmlDocument.DocumentNode.Descendants("div").Where(node => node
                .GetAttributeValue("id", "")
                .Contains("searchresults-container")).ToList(); // list that holds divs that hold groupnames
            //and navigationlinks
            var links = searchGroupResult[0].Descendants("span")
                .Where(node => node.GetAttributeValue("class", "").Contains("other")).ToList(); // the the html
            // element that holds the navigationlinks. 

            foreach (var a in links)
            {
                //fetching navigation links start here
                var aaa = a.Elements("a").First().Attributes;
                foreach (var v in aaa)
                {
                    var href = v.Value;
                    var subString = "amp;";
                    var indexOfSubsting = href.IndexOf(subString, StringComparison.Ordinal);
                    while (indexOfSubsting != -1)
                    {
                        href = href.Remove(indexOfSubsting, subString.Length);
                        indexOfSubsting = href.IndexOf(subString, StringComparison.Ordinal);
                    }

                    ResearGroupsResultUrls.Add(jsonBaseUrl + href); // save href:values here. They are url
                }
            } // fetching navigation link from href urls ends her


            foreach (var url in ResearGroupsResultUrls)
            {
                /* Send request to the urls and get all research groups and their urls.
                               returning response as list of string.*/
                var tes = await HttpRequestTask(url);
                var test = JsonConvert.DeserializeObject<ResearchGroupMapper>(tes);
                var testSting = test.searchResults;
                responseStrings.Add(testSting);


                Console.WriteLine(
                    " Http Request to fetch uia.no/researchGroups fragments successful, the string response are saved in list responseStrings ");
            } // End of sending Request to each Uri and returning response as list.

            foreach (var s in responseStrings)
            {
                // Start of parsing each hmlstring, to fetch groupnames and url.
                groupNames = HmtlParse(s, groupNames);

                Console.WriteLine("Info: Dictionary <GroupName, Url > successfully populated ");
            } //end of fetching groupNames.

            var fetchMembersStrings = new Dictionary<string, string>(); // key= groupName value = httpresponseString
            foreach (var group in groupNames)
            {
                // send request to groups private Urls to parse the htmlstring and fetch members.
                var groupUrl = group.Value;
                fetchMembersStrings.Add(group.Key, await HttpRequestTask(groupUrl));
            } // end of sending request of groupUrls and saving responses as key value in a dictionary.

            var membersList = new Dictionary<string, List<string>>(); // key = groupname value = members list

            foreach (var str in fetchMembersStrings)
            {
                // parsing html, fetching all members and saving to dictionary. 
                var list = GetMembersList(str);
                membersList[str.Key] = list; // key= researchgroupName and value= researchGroup members.
            } // end parsing htnldocuments for fetching members fo a specific group.

            //todo: Roles are commented starting her.
            /*
            MemberRole Participants, ASSOCIATES, PHD;

            if (!DbContext.MemberRoles.Any())
            {
                // if the role table does not have any entry.
                Participants = new MemberRole("Participant", "PARTICIPANT");
                ASSOCIATES = new MemberRole("Associate member", "ASSOCIATES");
                PHD = new MemberRole("Student", "PHD");
                await DbContext.MemberRoles.AddRangeAsync(Participants, ASSOCIATES, PHD);
                await DbContext.SaveChangesAsync();
            }
            else
            {
                Participants = await DbContext.MemberRoles.FirstOrDefaultAsync(part => part.RoleName == "Participant");
                ASSOCIATES =
                    await DbContext.MemberRoles.FirstOrDefaultAsync(part => part.RoleName == "Associate member");
                PHD = await DbContext.MemberRoles.FirstOrDefaultAsync(part => part.RoleName == "Student");
            }
            */
             
      
         
            //todo: roles end her
            foreach (var group in groupNames)
            {
                // start of adding research groups and their members into database.
                var gr = new ResearchGroup()
                {
                    GroupName = group.Key, GroupUrl = group.Value,
                };
                DbContext.ResearchGroups.Add(gr);
                await DbContext.SaveChangesAsync();

                var members = membersList[group.Key];

                foreach (var name in members)
                {
                    // adding members to the database
                    var member = new CurrentReGroupMember()
                    {
                        FullName = name
                    };
                    
                    await DbContext.CurrentReGroupMembers.AddAsync(member);
                    await DbContext.SaveChangesAsync();
                   
                    var rg = new ResearchGroupCurrentMembers()
                    {
                        _member= member, ResearchGroup = gr, RoleStatus = true, MemberRole = UserRole.User
                    };
                    var CurrentGroupMembers = DbContext.CurrentGroupMembers.Any(cr
                        => cr._member == member && cr.ResearchGroupId == rg.ResearchGroupId);
                    if (!CurrentGroupMembers)
                    {
                        await DbContext.CurrentGroupMembers.AddAsync(rg);
                        await DbContext.SaveChangesAsync();
                    }
                   
                } // end of adding member to database 
            } // End of adding Research groups into database.

            await DbContext.SaveChangesAsync();
            Console.WriteLine("Info: All Group Members successfully added to database ");
        }

        /// <summary>
        ///     map categories into models
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        protected async Task MapCategories()
        {
            var reqResponse =
                await HttpRequestTask("https://api.cristin.no/v2/results/categories?per_page=1000&lang=en,nb");
            var categories = JsonConvert.DeserializeObject<List<Category>>(reqResponse);
            foreach (var cat in categories)
            {
                DbContext.Names.Add(cat.Name);
                DbContext.Categories.Add(cat);
                await DbContext.SaveChangesAsync();
            }
        }

        /// <summary>
        ///     mapping institutions, faculties , institutes(departments) into their respective models
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        protected async Task MapUnits()
        {
            var unitsRequestResponse = await HttpRequestTask("https://api.cristin.no/v2/units/201.0.0.0?lang=en,nb");
            var unitsString = JsonConvert.DeserializeObject<units>(unitsRequestResponse);
            var institution = unitsString.institution;
            var faculties = unitsString.subunits; /*saves the faculty here*/

            var facultiesResponseStringList = new List<string>();

            var institutionORgString = await HttpRequestTask(institution.Url);

            var instORg = JsonConvert.DeserializeObject<institutionMap>(institutionORgString);
            institution.Country = instORg.Country;
            institution.InstitutionName = instORg.InstitutionName;

            await DbContext.Institutions.AddAsync(institution);
            await DbContext.SaveChangesAsync();


            foreach (var fac in faculties)
            {
                facultyUrls.Add(fac.Url); //save the urls of the faculties to this array for later use.
                await DbContext.Faculties.AddAsync(fac);
                await DbContext.SaveChangesAsync();

                Console.WriteLine(fac.UnitName.En);
            }


            // send request and wait until all request are handled and returned
            var urlRequest = facultyUrls.Select(furl => _client.GetAsync(furl)).ToList();
            await Task.WhenAll(urlRequest);
            var responses = urlRequest.Select(task => task.Result);

            foreach (var res in responses)
            {
                var rs = await res.Content.ReadAsStringAsync();

                facultiesResponseStringList.Add(rs);
            }


            foreach (var responseString in facultiesResponseStringList)
            {
                var facUnit = JsonConvert.DeserializeObject<FacultyUnitsRoot>(responseString);
                var institutes = facUnit.subunits;

                if (institutes != null)

                    foreach (var subunit in institutes)
                    {
                        InstituteUrls.Add(subunit.url);
                        var crstnId = facUnit.cristin_unit_id;
                        subunit.Faculty = DbContext.Faculties.First(fac => fac.CristinUnitId == crstnId);
                        await DbContext.Institutes.AddAsync(subunit);
                        await DbContext.SaveChangesAsync();
                    }
            }
        }

        /// <summary>
        ///     This maps  fetches projects and save them to database.
        /// </summary>
        /// <returns></returns>
        protected async Task MapProjects()
        {
            var reProjectsUrl = new Dictionary<string, string>(); // institute , researchproject dictionary
            var institutCrstnIds = from institute in DbContext.Institutes select institute.CristinUnitId;
            foreach (var crstnId in institutCrstnIds)
            {
                /* sending request as a loop and saving  as the response as key=InstituteId value= response to the
                 dictionary publicationUrl */
                //todo: change this to create a url.
                var projectInstituteUri = new UrlHelper().AddQueryString("https://api.cristin.no/v2/projects",
                    new Dictionary<string, string>()
                    {
                        {"lang", "en,nb"}, {"per_page", "1000"}, {"unit", crstnId}
                    });
                //var projectInstituteUri = "https://api.cristin.no/v2/projects?per_page=1000&lang=en,nb&unit=" + crstnId;
                reProjectsUrl[crstnId] = projectInstituteUri;
            } // end of foreach

            // sending request for each publication Url starts here
            foreach (var url in reProjectsUrl)
            {
                //todo: try first sending all the requests and then reading the content instead of sending one by one.
                var reqUrl = url.Value; // Inst_url 
                var crstnId = url.Key; // Current Institute critin Id
                var currentInst = DbContext.Institutes.First(inst => inst.CristinUnitId == crstnId);


                //todo: refer parsingUrlLink.txt for sudoCode
                var paginationUrl = await PagingUrls(reqUrl, _client);
                /*PaginationUrl.Add(reqUrl);
                ** save all pagination url for a given Institute,
                ** if more data, from the query, is available.*/

                foreach (var page in paginationUrl)
                {
                    /*var req = await _client.GetAsync(page);
                    if (req.IsSuccessStatusCode)
                    {
                        var insituteProj = await req.Content.ReadAsStringAsync()*/;
                    /*}
                    else if (req.StatusCode == HttpStatusCode.InternalServerError)
                    {
                        
                        Console.WriteLine("Error:{0}",req.StatusCode);
                    }*/
                        var insituteProj = await HttpRequestTask(page);
                        var projects = JsonConvert.DeserializeObject<List<ResearchProject>>(insituteProj);
                        foreach (var prj in projects)
                        {
                     
                            // todo: test the effectiveness this block of code that checks if a
                            // todo: publication is already registered in database. It works right now.
                            /* steps: if found in database fetch the Id and register it the Institute and publication table
                             if not create new publication and register it in database.
                             then create a new entry in the publicationInsititute table. 
                             then fetch all the contributors and register them in database
                             then create new entry in ContributorsPublication Table */

                            /* check is of type publication. is null publication does not exist, otherwise returns the pb*/
                            var check = await DbContext.ResearchProjects
                                .FirstOrDefaultAsync(o => o.CristinProjectId == prj.CristinProjectId);

                            if (check != null)
                            {
                                /* if publication exists in data base no need to create again. Just create new entry in 
                            the association table between publication and institute*/
                                var projIns = await DbContext.InstituteResearchProjects
                                    .FirstOrDefaultAsync(prIst=>
                                        prIst.ResearchProjectId==check.Id && prIst.InstituteId == currentInst.Id );
                                if (projIns == null)
                                {
                                    await DbContext.InstituteResearchProjects.AddAsync(new InstituteResearchProject
                                    {
                                        ResearchProject = check, Institute = currentInst
                                    });
                                    await DbContext.SaveChangesAsync(); 
                                }
                               
                            }
                            else
                            {
                                await DbContext.ResearchProjects.AddAsync(prj);
                                /*(project = new ResearchProject
                                {
                                    CristinProjectId = prj.cristin_project_id,
                                    StartDateTime = prj.start_date,
                                    Url = prj.url,
                                    EndDate = prj.end_date,
                                });*/
                                await DbContext.SaveChangesAsync();


                                //publicationInstitute is many to many table. 
                                await DbContext.InstituteResearchProjects.AddAsync(new InstituteResearchProject
                                {
                                    ResearchProject = prj, Institute = currentInst
                                }); // pub, Inst entry in PublicationInstitutes association table. 
                                await DbContext.SaveChangesAsync();

                                /* contributors are being fetched and saved in database in the following code*/
                                // todo: uncomment uncommentthis, after fixing the bug.
                               
                            }
                        }
                    
                }
            }
        }

        /// <summary>
        ///     Fetches contributors from each publication. Check their existence in database,
        ///     and assign affiliation institution and institute, enter new entry to PublicationContributor
        ///     association table. save changes to database.
        /// </summary>
        /// <param name="PublicationContributorsUrl"></param>
        /// <param name="pb"></param>
        /// <returns></returns>
        protected async Task MapContributors(string PublicationContributorsUrl, Publication pb)
        {
            //var contributorsRes = await _client.GetAsync(PublicationContributorsUrl);
            var contributorsRes = await HttpRequestTask(PublicationContributorsUrl);
            //todo: replaced contributorsRes.IsSuccessStatusCode by HttpRequestTask
            /*if (contributorsRes.IsSuccessStatusCode)
            {*/
            //var cntrString = await contributorsRes.Content.ReadAsStringAsync();
            var contributors = JsonConvert.DeserializeObject<List<person>>(contributorsRes);

            foreach (var ctrbtr in contributors)
            {
                bool status = DbContext.Contributors
                    .Any(o => o.CristinPersonId == ctrbtr.CristinPersonId);
                if (status)
                {
                    await ctrbtrExistInDataBase(ctrbtr, pb);
                }
                else
                {
                    await ctrbtrNotInDataBase(ctrbtr, pb);
                }
            }

            //}
        }

        /// <summary>
        /// Helping method. query the database.
        /// </summary>
        /// <param name="contributorName"></param>
        /// <returns></returns>
        protected List<List<ResearchGroupCurrentMembers>> GetMember(string contributorName)
        {
            //var member = await DbContext.CurrentReGroupMembers.FirstOrDefaultAsync(c => c.FullName == contributorName);
            var member = (from i in DbContext.CurrentReGroupMembers.Where(c => c.FullName == contributorName)
                select i.CurrentGroupMembers).ToList();

            return member ?? null;
        }

        /// <summary>
        ///  maps publication to publication model and saves in database
        /// </summary>
        /// <returns></returns>
        /// ///
        protected async Task MapPublications()
        {
            var publicationUrl = new Dictionary<string, string>(); // institute, publication dictionary
            var institutCrstnIds = from institute in DbContext.Institutes select institute.CristinUnitId;

            foreach (var crstnId in institutCrstnIds)
            {
                /* sending request as a loop and saving  as the response as key=FacultyId value= response to the
                 dictionary publicationUrl */

                var pbInstituteUri = new UrlHelper().AddQueryString("https://api.cristin.no/v2/results",
                    new Dictionary<string, string>()
                    {
                        {"lang", "en,nb"}, {"per_page", "1000"}, {"published_since", _datePublished}, {"unit", crstnId}
                    });
                // "https://api.cristin.no/v2/results?per_page=1000&lang=en,nb&published_since=2020&unit="+crstnId;

                publicationUrl[crstnId] = pbInstituteUri; // dictionary of instcrst_id = key and Url = value
            } //end of foreach(institute id) create a url

            // sending request for each publication Url starts here
            foreach (var url in publicationUrl)
            {
                //todo: try first sending all the requests and then reading the content instead of sending one by one.
                var reqUrl = url.Value; // Inst_url 
                var crstnId = url.Key; // Current Institute critin Id
                var currentInst = DbContext.Institutes.First(inst => inst.CristinUnitId == crstnId);


                /*save all pagination url for a given Institute,
                if more data, from the query, is available.*/
                var paginationUrl = await PagingUrls(reqUrl, _client); //PaginationUrl.Add(reqUrl); 

                foreach (var page in paginationUrl)
                {
                    //todo: replacing the req.IsSuccessStatusCoce part by httprequestTask
                    //todo: to be able to catch exceptions code here.

                    // req = await _client.GetAsync(page);
                    /*var req = await _client.GetAsync(page);
                    
                    if (req.IsSuccessStatusCode)
                    {
                        var insitutePb = await req.Content.ReadAsStringAsync();*/
                    var insitutePb = await HttpRequestTask(page);
                    var pub = JsonConvert.DeserializeObject<List<pbMapper>>(insitutePb);

                    await LogXtotalCount(pub.Count, page, "PublicationLog\\PulblicationTotalCount.txt");

                    foreach (var p in pub)
                    {
                        var categoryCode = p.category.Code;
                        Publication pb;
                        // todo: test the effectiveness this block of code that checks if a
                        // todo: publication is already registered in database. It works right now.
                        /* steps: if found in database fetch the Id and register it the Institute and publication table
                         if not create new publication and register it in database.
                         then create a new entry in the publicationInsititute table. 
                         then fetch all the contributors and register them in database
                         then create new entry in ContributorsPublication Table */

                        /* check is of type publication. if null publication does not exist, otherwise returns the pb*/
                        var check = await DbContext.Publications
                            .FirstOrDefaultAsync(o => o.CristinId == p.cristin_result_id);

                        if (check != null)
                        {
                            /* if publication exists in database no need to create again. Just create new entry in 
                        the association table between publication and institute*/
                            var checkInst = await
                                DbContext.PublicationInstitutes.FirstOrDefaultAsync(o =>
                                    o.PublicationId == check.Id && o.InstituteId == currentInst.Id);
                            if (checkInst == null)
                            {
                                await DbContext.PublicationInstitutes.AddAsync(new PublicationInstitute
                                {
                                    Publication = check, Institute = currentInst
                                });
                                await DbContext.SaveChangesAsync();
                                /* increment count by 1 and save changes*/
                                currentInst.incrementTotalCount();
                                await DbContext.SaveChangesAsync();
                                var cntrlUrl = p.contributors.url;
                                await MapContributors(cntrlUrl, check);
                            }
                        }
                        else
                        {
                            await DbContext.Publications.AddAsync(pb = new Publication
                            {
                                Category = DbContext.Categories.First(cat => cat.Code == categoryCode),
                                CristinId = p.cristin_result_id,
                                YearPublished = p.year_published,
                                Url = p.url,
                                DatePublished = p.date_published
                            });
                            await DbContext.SaveChangesAsync();


                            //publicationInstitute is many to many table. 
                            await DbContext.PublicationInstitutes.AddAsync(new PublicationInstitute
                            {
                                Publication = pb, Institute = currentInst
                            }); // pub, Inst entry in PublicationInstitutes association table. 
                            await DbContext.SaveChangesAsync();
                            /*Increment total count by 1 */
                            currentInst.incrementTotalCount();
                            await DbContext.SaveChangesAsync();

                            /* contributors are being fetched and saved in database in the following code*/
                            /* continue to feching contributors */
                            var cntrlUrl = p.contributors.url;
                            await MapContributors(cntrlUrl, pb);
                        }
                    }

                    // }
                }
            } //end of looping over urls for a given Institute.
        }

        /// <summary>
        ///     getting all pages url for a requested Query.
        /// </summary>
        /// <param name="firstUrl"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        private async Task<IList<string>> PagingUrls(string firstUrl, HttpClient client)
        {
            //todo: try recursive code here
            var pages = new List<string> {firstUrl}; //add the first page
            var req = await _client.GetAsync(firstUrl); // send the first request
            HttpHeaders headers = req.Headers; // save headers values and extract link if it exists
            var numberpage = getXCount(headers);//gets number of pages from the first page
            IEnumerable<string> values;
            var nexturl = " ";
            if (headers.TryGetValues("Link", out values))
            {
                var Link = values.FirstOrDefault(); // save the extracted link here. 
                var subsStrings = Link.Split(';');
                var length = subsStrings[0].Length - 2;
                nexturl = subsStrings[0].Substring(1, length);
                pages.Add(nexturl);
                var next = true;
                /* while the link has next link continue extracting and saving to the list*/
                while (next)
                {
                    req = await _client.GetAsync(nexturl);
                    headers = req.Headers;

                    if (headers.TryGetValues("Link", out values))
                    {
                        Link = values.FirstOrDefault(); // save the extracted link here. 
                        next = Link.Contains("rel=\"next\""); // check if the link have next link
                        if (next)
                        {
                            subsStrings = Link.Split(';');
                            length = subsStrings[0].Length - 2;
                            nexturl = subsStrings[0].Substring(1, length);
                            pages.Add(nexturl);
                        }
                    } //end of if (headers has link) condition
                } //end of while loop (link has rel=next).
            } //end of first if(headers has link) condition. 

            //todo: continue from here validating the total amount of pages.
            bool confirm = numberpage == pages.Count;

            await LogToLogCofirm(confirm, firstUrl, "PublicationLog\\LogConfirm.txt",pages.Count,numberpage); // writes the url and confimr to log file

            Console.WriteLine(" the calculated number of pages is equal to registered pages:" + confirm);
            return pages;
        } //end of pagingUrls


        /// <summary>
        /// Helping block in mapping Institution from contributors url in publication
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        protected async Task<Institution> affiliation(string url)
        {
            // create new institute 
            try
            {
                var institutionORgString = await HttpRequestTask(url);
                var instORg = JsonConvert.DeserializeObject<institutionMap>(institutionORgString);
                Institution insta;
                return new Institution()
                {
                    CristinUnitId = instORg.CristinUnitId,
                    Url = instORg.corresponding_unit.url,
                    InstitutionName = instORg.InstitutionName,
                    Country = instORg.Country,
                    Acronym = instORg.Acronym
                }; // create new institute ;
                return insta;
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine("This contributor has no institution registered and generated: {0}", e.Message);
                return null;
            }
            catch (HttpRequestException)
            {
                Console.WriteLine("This institution responded error");
                return null;
            }
        } // end of sending request and mapping an Institution

        /// <summary>
        /// this code is helping code for contributor mapping if person already exist
        /// in database
        /// </summary>
        /// <param name="ctrbtr"></param>
        /// <param name="pb"></param>
        /// <returns></returns>
        private async Task ctrbtrExistInDataBase(person ctrbtr, Publication pb)
        {
            {
                /* This reseacher exists, check affiliation, check institution
                 and if all exist update PublicationContributors,*/
                var contrb = DbContext.Contributors
                    .First(c => c.CristinPersonId == ctrbtr.CristinPersonId);
                var PC = await DbContext.PublicationContributors.FirstOrDefaultAsync(
                    pc => pc.PublicationId == pb.Id && pc.ContributorId == contrb.Id);
                if (PC != null)
                {
                    Console.WriteLine(" returning from ctrbtrExistInDataBase because PublicationContributor exists,");
                    return;
                }
                /* steps:  check if an affiliation is registered for this contributor and institution
                 * at the affiliations table if so, fetch the id and add to publicationContributors.
                 */

                //var af = ctrbtr.Affiliations[0].Institution;
                Institution af;
                try
                {
                    af = ctrbtr.Affiliations[0].Institution;
                }
                catch (NullReferenceException e)
                {
                    await DbContext.PublicationContributors.AddAsync(new PublicationContributor()
                    {
                        Publication = pb, Contributor = contrb
                    });
                    await DbContext.SaveChangesAsync();
                    await CountributorGroups(ctrbtr, contrb);
                    Console.WriteLine(e.Message);
                    return;
                }

                var inst = await DbContext.Institutions.FirstOrDefaultAsync(o
                    => o.CristinUnitId == af.CristinUnitId);
                if (inst != null)
                {
                    // instituion exist thus check affiliation
                    var afll = await DbContext.Affiliations
                        .FirstOrDefaultAsync(o => o.ContributorId == contrb.Id
                                                  && o.InstitutionId == inst.Id);
                    if (afll != null)
                    {
                        /* affilition exist thus all three exist.
                                               *then check if PublicationContributors of this three exist
                                              * if exist: do nothing
                                              * else : create new entry*/
                        var PCAFfl = await DbContext.PublicationContributors.FirstOrDefaultAsync(
                            pc => pc.PublicationId == pb.Id && pc.Affiliation == afll);
                        if (PCAFfl == null)
                        {
                            await DbContext.PublicationContributors.AddAsync(
                                new PublicationContributor
                                {
                                    Publication = pb, Contributor = contrb, Affiliation = afll
                                });
                            await DbContext.SaveChangesAsync();
                        }
                    }
                    else
                    {
                        // create new affiliation with contrb and inst
                        var afll2 = new Affiliation()
                        {
                            Contributor = contrb, Institution = inst
                        }; // creating new affiliate
                        await DbContext.PublicationContributors.AddAsync(
                            new PublicationContributor
                            {
                                Publication = pb, Contributor = contrb, Affiliation = afll2
                            }); // add to database
                        await DbContext.SaveChangesAsync();
                    } // ending of checking affiliation. 
                }
                else
                {
                    // institute does not exist, and affiliation can not exist, thus create both.
                    bool afId = af.CristinUnitId == "0"; // since some have 0 id.

                    if (afId)
                    {
                        await DbContext.PublicationContributors.AddAsync(new PublicationContributor()
                        {
                            Publication = pb, Contributor = contrb
                        });
                        await DbContext.SaveChangesAsync();
                    }
                    else
                    {
                        var institutionMap = await affiliation(af.Url);
                        if (institutionMap == null)
                        {
                            await DbContext.PublicationContributors.AddAsync(new PublicationContributor()
                            {
                                Publication = pb, Contributor = contrb
                            });
                            await DbContext.SaveChangesAsync();
                        }
                        else
                        {
                            // send request and map to InstituionMapper

                            //Add and save institution
                            await DbContext.Institutions.AddAsync(institutionMap);
                            await DbContext.SaveChangesAsync();

                            var afiliation = new Affiliation()
                            {
                                Contributor = contrb, Institution = institutionMap
                            }; //create new affiliation 
                            //Todo: Aler Alert found DEbugging error may be 
                            await DbContext.Affiliations.AddAsync(afiliation);
                            await DbContext.SaveChangesAsync();
                            await DbContext.PublicationContributors.AddAsync(new PublicationContributor()
                            {
                                Publication = pb, Contributor = contrb, Affiliation = afiliation
                            });
                            await DbContext.SaveChangesAsync();
                        }
                    }
                } //end of if-else checking institution. adding incase of a contributor that exists in a database.
            } //end if a contributor exist
        }

        /// <summary>
        /// This  block is helping block for contributor mapping if person does not exist at
        /// the database
        /// </summary>
        /// <param name="ctrbtr"></param>
        /// <param name="pb"></param>
        /// <returns></returns>
        private async Task ctrbtrNotInDataBase(person ctrbtr, Publication pb)
        {
            {
                /* This researcher does not exist in the database.
                 so no affiliation can exist but institution can exist. 
                 there fore : Target PublicationContributors, But since 
                 it needs Contributor, publication, and afilation { contributor, institution }
                 
                 1. create new contributor 2. check if an institution exist then 
                 3. if institution exist create an affiliation between contributor and institution
                 4. if institution does not exist create institution then affiliation */

                /* start of contributor creating and adding to database*/
                var contributor = new Contributor()
                {
                    CristinPersonId = ctrbtr.CristinPersonId,
                    FirstName = ctrbtr.FirstName,
                    SurName = ctrbtr.SurName,
                    Url = ctrbtr.Url
                }; // creat new contributor 
                await DbContext.Contributors.AddAsync(contributor);
                await DbContext.SaveChangesAsync();
                /* end of contributor adding to database*/


                Institution af;
                try
                {
                    af = ctrbtr.Affiliations[0].Institution;
                }
                catch (NullReferenceException e)
                {
                       Console.WriteLine(e.Message);
                    await DbContext.PublicationContributors.AddAsync(new PublicationContributor()
                    {
                        Publication = pb, Contributor = contributor
                    });
                    await DbContext.SaveChangesAsync();
                    await CountributorGroups(ctrbtr, contributor);

                    return;
                }


                bool afId = af.CristinUnitId == "0"; // since some have 0 id.
                //  check if the institution is already registered here. if not hoppover to else
                var instStatus = DbContext.Institutions.Any(o
                    => o.CristinUnitId == af.CristinUnitId); // check if institution exist

                if (instStatus)
                {
                    //first fetch the institution from the database
                    var institution = await DbContext.Institutions
                        .FirstAsync(uni => uni.CristinUnitId == af.CristinUnitId);
                    //second create a new affliation instance, add to database.
                    var afliation = new Affiliation()
                    {
                        Contributor = contributor, Institution = institution
                    }; //create new affiliation
                    await DbContext.Affiliations.AddAsync(afliation);
                    await DbContext.SaveChangesAsync();

                    // then the affiliation to PublicationContributor association table
                    await DbContext.PublicationContributors.AddAsync(
                        new PublicationContributor
                        {
                            Publication = pb, Contributor = contributor, Affiliation = afliation
                        }); //create the association target of PublicationContributor
                    await DbContext.SaveChangesAsync();
                    //Todo Status: if block Debugging gikk bra til hit 
                }
                else
                {
                    // if not in database create new 
                    /*steps: 
                     * create a new institution
                     * send request to the url and fetch and parse as in MapUnits. see this 
                     * * add to database
                     * add it to the contributor.affiliation list
                     */

                    // todo Alert: institutionMap or Instttt are returning NullREferenceException and handle this. 
                    // institute does not exist, and affiliation can not exist, thus create both.

                    if (afId)
                    {
                        await DbContext.PublicationContributors.AddAsync(new PublicationContributor()
                        {
                            Publication = pb, Contributor = contributor
                        });
                        await DbContext.SaveChangesAsync();
                    }
                    else
                    {
                        var institutionMap = await affiliation(af.Url);

                        // send request and map to InstituionMapper
                        if (institutionMap == null)
                        {
                            await DbContext.PublicationContributors.AddAsync(
                                new PublicationContributor
                                {
                                    Publication = pb, Contributor = contributor
                                }); //create the association target of PublicationContributor
                            await DbContext.SaveChangesAsync();
                        }
                        else
                        {
                            // create new institute 
                            await DbContext.Institutions.AddAsync(institutionMap);
                            await DbContext.SaveChangesAsync();

                            //contributor.Affiliations.Add(institution);
                            var afliation = new Affiliation()
                            {
                                Contributor = contributor, Institution = institutionMap
                            }; //create new affiliation
                            await DbContext.Affiliations.AddAsync(afliation);
                            await DbContext.SaveChangesAsync();
                            await DbContext.PublicationContributors.AddAsync(
                                new PublicationContributor
                                {
                                    Publication = pb, Contributor = contributor, Affiliation = afliation
                                }); //create the association target of PublicationContributor
                            await DbContext.SaveChangesAsync();
                        }
                    }
                }
                /* Todo:Affiliations finish here */

                await DbContext.SaveChangesAsync();


                await CountributorGroups(ctrbtr, contributor); /* start checking for groups*/
                await DbContext.SaveChangesAsync();
                Console.WriteLine("Info: contributrs map Contributors reached here safly.");
            }
        } //End of contributor not in database

        /// <summary>
        /// Will check if a contributor matches with already registered fullName and ensure to assign
        /// the contributors to Researchgroups
        /// </summary>
        /// <param name="ctrbtr"></param>
        /// <param name="contributor"></param>
        /// <returns></returns>
        private async Task CountributorGroups(person ctrbtr, Contributor contributor)
        {
            // from here checks if the name is available at the database
            var fullname = ctrbtr.FirstName + " " + ctrbtr.SurName;
            var member = GetMember(fullname); /* check if there is similar name
                         in CurrentReGroupMembers, case(exist): fetch all groupIds this researcher is a member*/
            if (member.Count != 0)
            {
                // if member is not empty, assign them to this contributor.

                foreach (var re in member)
                {
                    var gr = re[0].ResearchGroupId;
                    await DbContext.ContributorReGroups.AddAsync(new ContributorReGroup()
                    {
                        ResearchGroupId = gr, Contributor = contributor
                    });
                }

                Console.WriteLine("inside member loop");
                await DbContext.SaveChangesAsync();
            } /*end of registering contributors and research groups fetched from uia.no 
                        */
        }

        public async Task InovkeAPIs()
        {
            List<string>localApi= new List<string>();
            localApi.Add("/Data/listOfPubLicationWithInsititute");
            localApi.Add("/Data/publicationByDate");
            var urlRequest = localApi.Select(furl => _client.GetAsync(furl)).ToList();
            await Task.WhenAll(urlRequest);
            var responses = urlRequest.Select(task => task.Result);
           
        }
    } //end of class scope
    
    
} //end of namespace scope
