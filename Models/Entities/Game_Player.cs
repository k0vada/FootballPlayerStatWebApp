//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FootballPlayerStatWebApp.Models.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class Game_Player
    {
        public int GameId { get; set; }
        public int PlayerId { get; set; }
        public int GoalsScored { get; set; }
        public int GoalsConceded { get; set; }
    
        public virtual Player Player { get; set; }
    }
}
