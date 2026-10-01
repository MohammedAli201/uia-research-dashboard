namespace PBDASHBOARD.Models.AssociationModels
{
    /**
         * Association class between contributor and publication
         */
    public class PublicationContributor
    {
        public Contributor Contributor { get; set; }
        public int ContributorId { get; set; }

        public Publication Publication { get; set; }
        public int PublicationId { get; set; }
        
        public Affiliation Affiliation { get; set; }
       
    }
}