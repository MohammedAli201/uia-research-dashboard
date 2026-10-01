namespace PBDASHBOARD.Models.AssociationModels
{
    
    /**
     * this class represents the many-to-many relationship between the contributor class and
     * ResearchGroup class.
     */
    
    public class ContributorReGroup
    {
        public Contributor Contributor { get; set; }
        public int ContributorId { get; set; }

        public ResearchGroup ResearchGroup { get; set; }
        public int ResearchGroupId { get; set; }
        
    }
}