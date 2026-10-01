namespace PBDASHBOARD.Models.AssociationModels
{
    public class InstituteResearchProject
    {
        public ResearchProject ResearchProject { get; set; }
        public int ResearchProjectId { get; set; }

        public Institute Institute { get; set; }
        public int InstituteId { get; set; }
    }
}