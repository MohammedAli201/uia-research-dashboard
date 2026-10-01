﻿namespace PBDASHBOARD.Models
{
    public class MemberRole
    {
        public MemberRole()
        {
        }

        public MemberRole(string roleName, string roleCode)
        {
            this.RoleCode = roleCode;
            this.RoleName = roleName;
        }
        public int id { get; set; }
        public string RoleName { get; set; }
        public string RoleCode { get; set; }
    }
}