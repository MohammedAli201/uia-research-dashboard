using System.Collections.Generic;
using PBDASHBOARD.Models.AssociationModels;

namespace PBDASHBOARD.Models
{
    /**
     * This class represents the research groups in UiA
     */
    public class ResearchGroup
    {
        public int Id { get; set; }

        /** name of the group**/
        public string GroupName { get; set; }
        public CurrentReGroupMember Admin { get; set; }
        
        /// <summary>
        /// status: Active or inactive group
        /// </summary>
        public bool Status { get; set; } 
        public string GroupUrl { get; set; }
        /** Members list through the class ContributorGroup**/
        public List<ContributorReGroup> GroupMembers { get; set; } = new List<ContributorReGroup>();
        public List<ResearchGroupCurrentMembers> CurrentGroupMembers { get; set; } = new List<ResearchGroupCurrentMembers>();
    }
}