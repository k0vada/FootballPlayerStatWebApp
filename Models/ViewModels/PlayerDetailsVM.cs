using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FootballPlayerStatWebApp.Models.ViewModels
{
    public class PlayerDetailsVM
    {
        public int PlayerId { get; set; }

        [Required]
        [DisplayName("Имя")]
        [StringLength(50)]
        [RegularExpression("^[A-Za-zА-Яа-я]+$", ErrorMessage = "Поле может содержать только буквы.")]
        public string FirstName { get; set; }
        [Required]
        [DisplayName("Фамилия")]
        [StringLength(50)]
        [RegularExpression("^[A-Za-zА-Яа-я]+$", ErrorMessage = "Поле может содержать только буквы.")]
        public string LastName { get; set; }
        [DisplayName("Отчество")]
        [StringLength(50)]
        [RegularExpression("^[A-Za-zА-Яа-я]+$", ErrorMessage = "Поле может содержать только буквы.")]
        public string MiddleName { get; set; }
        [Required]
        [DisplayName("Возраст")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Поле может содержать только цифры.")]
        public int Age { get; set; }
        [Required]
        [DisplayName("Номер")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Поле может содержать только цифры.")]
        public int Number { get; set; }
        [Required]
        [DisplayName("Позиция")]
        [StringLength(50)]
        [RegularExpression("^[A-Za-zА-Яа-я]+$", ErrorMessage = "Поле может содержать только буквы.")]
        public string Position { get; set; }
        //[Required]
        [DisplayName("Клуб")]
        [StringLength(50)]
        public string ClubName { get; set; }
        [Required]

        [DisplayName("Клуб")]
        public int ClubId { get; set; }
        [Required]
        [DisplayName("Дата начала карьеры")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? StartDate { get; set; }
        [DisplayName("Дата окончания карьеры")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? EndDate { get; set; }
        [Required]
        [DisplayName("Пользователь")]
        public System.Guid UserId { get; set; }

    }
}