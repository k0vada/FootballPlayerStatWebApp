using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FootballPlayerStatWebApp.Models.ViewModels
{
    public class PlayerVM
    {
        public int PlayerId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public int Number { get; set; }
        public string Position { get; set; }
        public string ClubName { get; set; }
    }
}