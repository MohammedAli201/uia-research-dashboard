using System.Collections.Generic;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;

namespace PBDASHBOARD.Models
{
    /// <summary>
    ///Current members 
    /// </summary>
    public class CurrentReGroupMember : IdentityUser<int>
    {
  
        public CurrentReGroupMember(){}
        
        /// <summary>
        /// Full name of Current member
        /// </summary>
        public string FullName { get; set; }
        public string Role { get; set; }
        public List<ResearchGroupCurrentMembers> CurrentGroupMembers { get; set; } = new List<ResearchGroupCurrentMembers>();
        
    }
}