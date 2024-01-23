using FootballPlayerStatWebApp.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using FootballPlayerStatWebApp.Models.ViewModels;
using System.Web.Security;
using System.Security.Cryptography;
using System.Text;

namespace FootballPlayerStatWebApp.Controllers
{
    public class MainController : Controller
    {
        // GET: Main
        #region Получение информации
        [AllowAnonymous]    
        public ActionResult ListOfPlayers()
        {
            List<PlayerDetailsVM> ListOfPlayers = new List<PlayerDetailsVM>();
            using (var db = new KovalevDO_IDZEntities())
            {
                ListOfPlayers = db.Players.Include(p => p.Person).Include(p => p.Person).Select(player => new PlayerDetailsVM
                {
                    PlayerId = player.PlayerId,
                    LastName = player.Person.LastName,
                    FirstName = player.Person.FirstName,
                    Number = player.PlayerCareers.OrderByDescending(pc => pc.StartDate).FirstOrDefault().Number,
                    Position = player.PlayerCareers.OrderByDescending(pc => pc.StartDate).FirstOrDefault().Position,
                    ClubName = player.PlayerCareers.OrderByDescending(pc => pc.StartDate).FirstOrDefault().Club.Name


                }).ToList();
            }
            return View(ListOfPlayers);
        }

        [HttpGet]
        [Authorize]
        public ActionResult PlayerDetails(int playerId)
        {
            using (var db = new KovalevDO_IDZEntities())
            {
                var playerDetails = db.Players.Where(p => p.PlayerId == playerId).Select(player => new PlayerDetailsVM
                {
                    FirstName = player.Person.FirstName,
                    LastName = player.Person.LastName,
                    MiddleName = player.Person.MiddleName,
                    Age = player.Person.Age,
                    Number = player.PlayerCareers.OrderByDescending(pc => pc.StartDate).FirstOrDefault().Number,
                    Position = player.PlayerCareers.OrderByDescending(pc => pc.StartDate).FirstOrDefault().Position,
                    ClubName = player.PlayerCareers.OrderByDescending(pc => pc.StartDate).FirstOrDefault().Club.Name,
                    StartDate = player.PlayerCareers.OrderBy(pc => pc.StartDate).FirstOrDefault().StartDate,
                    EndDate = player.PlayerCareers.OrderBy(pc => pc.StartDate).FirstOrDefault().EndDate
                }).FirstOrDefault();

                return View(playerDetails);
            }
        }

        #endregion

        #region Обработка информации
        public List<SelectListItem> GetClubList()
        {
            using (var context = new KovalevDO_IDZEntities())
            {
                return context.Clubs
                    .Select(c => new SelectListItem
                    {
                        Value = c.ClubId.ToString(),
                        Text = c.Name
                    })
                    .ToList();
            }
        }
        public List<SelectListItem> GetUserList()
        {
            using (var context = new KovalevDO_IDZEntities())
            {
                return context.Users
                    .Select(u => new SelectListItem
                    {
                        Value = u.Id.ToString(),
                        Text = u.Login
                    })
                    .ToList();
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult CreatePlayer()
        {
            ViewBag.Clubs = GetClubList();
            ViewBag.Users = GetUserList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]

        public ActionResult CreatePlayer(PlayerDetailsVM newPlayer)
        {
            if (ModelState.IsValid)
            {
                using (var context = new KovalevDO_IDZEntities())
                {
                    // Создаем объект Person
                    var person = new Person
                    {
                        FirstName = newPlayer.FirstName,
                        LastName = newPlayer.LastName,
                        MiddleName = newPlayer.MiddleName,
                        Age = newPlayer.Age,
                        UserId = newPlayer.UserId
                    };
                    context.People.Add(person);
                    context.SaveChanges();

                    // Создаем объект Player
                    var player = new Player
                    {
                         PersonId = person.PersonId,
                        PlayerCareers = new List<PlayerCareer>
                        {
                            new PlayerCareer
                            {
                                Number = newPlayer.Number,
                                Position = newPlayer.Position,
                                ClubId = newPlayer.ClubId,
                                StartDate = DateTime.Now,
                                EndDate = newPlayer.EndDate
                            }
                        }
                    };

                    context.Players.Add(player);
                    context.SaveChanges();

                    return RedirectToAction("ListOfPlayers");
                }
            }
            ViewBag.Clubs = GetClubList();
            ViewBag.Users = GetUserList();
            return View(newPlayer);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult EditPlayer(int playerId)
        {
            PlayerDetailsVM model;

            using (var context = new KovalevDO_IDZEntities())
            {
                var player = context.Players
                    .Include(p => p.Person)
                    .Include(p => p.PlayerCareers)
                    .FirstOrDefault(p => p.PlayerId == playerId);

                if (player == null)// Обработка случая, когда игрок с указанным ID не найден
                {
                    return HttpNotFound();
                }

                model = new PlayerDetailsVM
                {
                    PlayerId = player.PlayerId,
                    FirstName = player.Person.FirstName,
                    LastName = player.Person.LastName,
                    MiddleName = player.Person.MiddleName,
                    Age = player.Person.Age,
                    Number = player.PlayerCareers.OrderByDescending(pc => pc.StartDate).FirstOrDefault().Number,
                    Position = player.PlayerCareers.OrderByDescending(pc => pc.StartDate).FirstOrDefault().Position,
                    ClubName = player.PlayerCareers.OrderByDescending(pc => pc.StartDate).FirstOrDefault().Club.Name,
                    StartDate = player.PlayerCareers.OrderBy(pc => pc.StartDate).FirstOrDefault().StartDate,
                    EndDate = player.PlayerCareers.OrderBy(pc => pc.StartDate).FirstOrDefault().EndDate
                };
            }
            ViewBag.SelectedClub = model.ClubId;
            ViewBag.SelectedUser = model.UserId;
            ViewBag.Clubs = GetClubList();
            ViewBag.Users = GetUserList();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult EditPlayer(PlayerDetailsVM updatedPlayer)
        {
            if (ModelState.IsValid)
            {
                using (var context = new KovalevDO_IDZEntities())
                {
                    var playerToUpdate = context.Players
                        .Include(p => p.Person)
                        .Include(p => p.PlayerCareers)
                        .FirstOrDefault(p => p.PlayerId == updatedPlayer.PlayerId);

                    if (playerToUpdate == null)// Обработка случая, когда игрок с указанным ID не найден
                        return HttpNotFound();
                    // Обновление свойств игрока 
                    playerToUpdate.Person.FirstName = updatedPlayer.FirstName;
                    playerToUpdate.Person.LastName = updatedPlayer.LastName;
                    playerToUpdate.Person.MiddleName = updatedPlayer.MiddleName;
                    playerToUpdate.Person.Age = updatedPlayer.Age;
                    // Обновление данных карьеры игрока
                    var careerToUpdate = playerToUpdate.PlayerCareers.OrderByDescending(pc => pc.StartDate).FirstOrDefault();
                    if (careerToUpdate != null)
                    {
                        careerToUpdate.Number = updatedPlayer.Number;
                        careerToUpdate.Position = updatedPlayer.Position;
                        careerToUpdate.ClubId = updatedPlayer.ClubId;
                        careerToUpdate.StartDate = DateTime.Now;

                        if (careerToUpdate.EndDate.HasValue)
                        {
                            careerToUpdate.EndDate = updatedPlayer.EndDate;
                        }
                    }
                    playerToUpdate.Person.UserId = updatedPlayer.UserId;

                    context.Entry(playerToUpdate).State = EntityState.Modified;
                    context.Entry(careerToUpdate).State = EntityState.Modified;
                    context.SaveChanges();
                    return RedirectToAction("ListOfPlayers");
                }
            }

            ViewBag.Clubs = GetClubList();
            ViewBag.Users = GetUserList();

            return View(updatedPlayer);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult DeletePlayer(int playerId)
        {
            Player playerToDelete;
            PlayerDetailsVM model;

            using (var context = new KovalevDO_IDZEntities())
            {
                playerToDelete = context.Players.Include("Person").SingleOrDefault(p => p.PlayerId == playerId);
                model = new PlayerDetailsVM
                {
                    FirstName = playerToDelete.Person.FirstName,
                    LastName = playerToDelete.Person.LastName,
                    MiddleName = playerToDelete.Person.MiddleName,
                    Age = playerToDelete.Person.Age,
                    Number = playerToDelete.PlayerCareers.OrderByDescending(pc => pc.StartDate).FirstOrDefault().Number,
                    Position = playerToDelete.PlayerCareers.OrderByDescending(pc => pc.StartDate).FirstOrDefault().Position,
                    ClubName = playerToDelete.PlayerCareers.OrderByDescending(pc => pc.StartDate).FirstOrDefault().Club.Name,
                    StartDate = playerToDelete.PlayerCareers.OrderBy(pc => pc.StartDate).FirstOrDefault().StartDate,
                    EndDate = playerToDelete.PlayerCareers.OrderBy(pc => pc.StartDate).FirstOrDefault().EndDate
                };
            }

            return View(model);
        }

        [HttpPost, ActionName("DeletePlayer")]
        [Authorize(Roles = "Admin")]
        public ActionResult DeletePlayerConfirmed(int playerId)
        {
            using (var context = new KovalevDO_IDZEntities())
            {
                Player playerToDelete = context.Players.Include("PlayerCareers").Include("Person").SingleOrDefault(p => p.PlayerId == playerId);

                if (playerToDelete != null)
                {
                    context.Entry(playerToDelete.Person).State = EntityState.Deleted;
                    context.PlayerCareers.RemoveRange(playerToDelete.PlayerCareers); // RemoveRange для удаления всех связанных записей

                    context.Entry(playerToDelete).State = EntityState.Deleted;


                    context.SaveChanges();
                }
            }
            return RedirectToAction("ListOfPlayers");
        }
        #endregion

        #region Авторизация
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(UserVM webUser)
        {
            if (ModelState.IsValid)
            {
                using (KovalevDO_IDZEntities context = new KovalevDO_IDZEntities())
                { // Идентификация

                    User user = null;
                    user = context.Users.Where(u => u.Login == webUser.Login).FirstOrDefault();
                    if(user != null)
                    {
                        //Аутентификация
                        string passwordHash = ReturnHashCode(webUser.Password + user.Salt.ToString().ToUpper());
                        if(passwordHash == user.PasswordHash)
                        {
                            string userRole = "";
                            switch (user.UserRole)
                            {
                                case 1:
                                    userRole = "Participant";
                                    break;
                                case 2:
                                    userRole = "Admin";
                                    break;
                            }
                            FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                               1,                         // Version
                               user.Login,                // User name
                               DateTime.Now,              // Created
                               DateTime.Now.AddDays(1),   // Expires
                               false,                     // Persistent
                               userRole                   // User data => Roles
                               );
                            string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                            HttpContext.Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket));
                            return RedirectToAction("ListOfPlayers", "Main");
                        }
                    }
                }
            }
            ViewBag.Error = "Пользователя с таким логином или паролем не существует, попробуйте еще раз.";
            return View(webUser);
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        string ReturnHashCode(string loginAndSalt)
        {
            string hash = "";
            using (SHA1 sha1Hash = SHA1.Create())
            {
                byte[] data = sha1Hash.ComputeHash(Encoding.UTF8.GetBytes(loginAndSalt));
                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }
                hash = sBuilder.ToString().ToUpper();
            }
            return hash;
        }

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("ListOfPlayers", "Main");
        }
        #endregion
    }
}