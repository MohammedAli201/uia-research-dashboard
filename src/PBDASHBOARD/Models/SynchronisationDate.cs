using System;

namespace PBDASHBOARD.Models
{
    public class SynchronisationDate
    {
        public int id { get; set; }
        public bool runOnce { get; set; }
        public string startYear { get; set; }
        public string lasSyncYear { get; set; }
        public SynchronisationDate()
        {
            
        }        
        public SynchronisationDate(string synYear)
        {
            lasSyncYear = synYear;
        }

        public void reset()
        {
            runOnce = false;
        }
        
    }
}

