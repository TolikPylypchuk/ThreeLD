using System.Collections.Generic;

using ThreeLD.DB.Models;

namespace ThreeLD.Web.Models.ViewModels
{
    public class AdminUsersModel
    {
        public Dictionary<User, bool> Users { get; set; }
    }
}