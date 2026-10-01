using System;
using PBDASHBOARD.User;

namespace PBDASHBOARD.Models
{
    public class ResearchGroupCurrentMembers
    {
        /// <summary>
        /// represents the research Group and members fetched from uia.no
        /// </summary>
        public CurrentReGroupMember _member { get; set; }
        public int CurrentReGroupMemberId { get; set; }

        public ResearchGroup ResearchGroup { get; set; }
        public int ResearchGroupId { get; set; }

        /// <summary>
        /// role of the member. Access is controlled based on role
        /// </summary>

        public string MemberRole { get; set; }
        /// <summary>
        /// if Active memebr value is true else false.
        /// </summary>
        public bool RoleStatus { get; set; }

        public void setRoleStatus(bool status)
        {
            RoleStatus = status;
            CreatedDateTime = DateTime.Now;
        }

        
        private DateTime CreatedDateTime { get; set; }// try to implmente after the event of RoleStatus 
        
        
    }
}