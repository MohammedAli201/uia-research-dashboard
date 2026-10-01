using System.Collections.Generic;
using PBDASHBOARD.Models;

namespace PBDASHBOARD.Controllers
{
    public class JsonObjectsFraView
    {
        public string name { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string groupName { get; set; }
        public List<string>List { get; set; }
    }
}