namespace PBDASHBOARD.Models.AssociationModels
{
    public class PublicationInstitute
    {
        public Institute Institute { get; set; }
        public int InstituteId { get; set; }

        public Publication Publication { get; set; }
        public int PublicationId { get; set; }
    }
}