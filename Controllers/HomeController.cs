using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Session;
using PingPongPlanner.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace PingPongPlanner.Controllers
{
    public class HomeController : Controller
    {

        private PingPongPlannerContext _context;

        public HomeController(PingPongPlannerContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("register")]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                User CheckUser = _context.Users.SingleOrDefault(u => u.Email == model.Email);
                Console.WriteLine(CheckUser);
                if(CheckUser != null)
                {
                    TempData["EmailInUseError"] = "Email Aleady in use";
                    return RedirectToAction("Index");
                }
                User newUser = new User
                {
                    Username = model.Username,
                    Email = model.Email,
                    Wins = 0,
                    Losses = 0,
                    Created_At = DateTime.Now,
                    Updated_At = DateTime.Now
                };
                PasswordHasher<User> hasher = new PasswordHasher<User>();
                newUser.Password = hasher.HashPassword(newUser, model.Password);
                _context.Add(newUser);
                _context.SaveChanges();
                HttpContext.Session.SetInt32("currentUserId", newUser.UserId);
                HttpContext.Session.SetString("currentFirstName", newUser.Username);
                return RedirectToAction("Dashboard");
            }
            else
            {
                return View("Index");
            }
        }

        [HttpPost]
        [Route("/login")]
        public IActionResult Login(string email, string password)
        {
            User logUser = _context.Users.SingleOrDefault(user => user.Email == email);
            if (logUser == null)
            {
                TempData["EmailError"] = "Invalid email!";
                return RedirectToAction("Index");
            }
            else
            {
                PasswordHasher<User> hasher = new PasswordHasher<User>();
                if (hasher.VerifyHashedPassword(logUser, logUser.Password, password) != 0)
                {
                    HttpContext.Session.SetInt32("currentUserId", logUser.UserId);
                    HttpContext.Session.SetString("currentUserEmail", logUser.Email);
                    return RedirectToAction("Dashboard");
                }
                else
                {
                    TempData["PasswordError"] = "Invalid password";
                    return RedirectToAction("Index");
                }
            }
        }

        [HttpGet]
        [Route("/dashboard")]
        public IActionResult Dashboard()
        {
            int? userId = HttpContext.Session.GetInt32("currentUserId");
            if (userId == null)
            {
                TempData["UserError"] = "You must be logged in!";
                return RedirectToAction("Index");
            }
            else
            {
                User currentUser = _context.Users.SingleOrDefault(u => u.UserId == (int)userId);
                List<User> users = _context.Users.Include(u => u.Matches).ToList();
                List<Match> matches = _context.Matches.Include(w => w.Guests).ThenInclude(g => g.User).ToList();
                //maybe an error but has no effect
                List<Guest> guests = _context.Guests.Include(g => g.Match).Include(g => g.User).ToList();

                ViewBag.User = currentUser;
                ViewBag.Matches = matches;
                
                Wrapper model = new Wrapper(users, matches, guests);
                return View(model);
            }
        }

        [HttpGet]
        [Route("/match/create")]
        public IActionResult NewMatch()
        {
            int? userId = HttpContext.Session.GetInt32("currentUserId");
            if (userId == null)
            {
                TempData["UserError"] = "You must be logged in!";
                return RedirectToAction("Index");
            }
            else
            {
                User currentUser = _context.Users.SingleOrDefault(u => u.UserId == (int)userId);
                ViewBag.User = currentUser;

                List<User> users = _context.Users.ToList();
                Console.WriteLine(users);
                List<Match> matches = _context.Matches.ToList();
                List<Guest> guests = _context.Guests.ToList();

                ViewBag.Users = users;

                return View();
            }
        }

        [HttpPost]
        [Route("/addMatch")]
        public IActionResult CreateMatch(MatchViewModel model)
        {
            int? userId = HttpContext.Session.GetInt32("currentUserId");
            if (userId == null)
            {
                TempData["UserError"] = "You must be logged in!";
                return RedirectToAction("Index");
            }
            else
            {
                User currentUser = _context.Users.SingleOrDefault(u => u.UserId == (int)userId);
                ViewBag.User = currentUser;
                if (ModelState.IsValid)
                {
                    Match newMatch = new Match
                    {
                        PlayerOne = model.PlayerOne,
                        PlayerTwo = model.PlayerTwo,
                        Address = model.Address,
                        CreatorId = currentUser.UserId,
                        Creator = currentUser
                    };
                    if (model.Date < DateTime.Now)
                    {
                        TempData["DateError"] = "Dates cannot be in the past";
                        return RedirectToAction("NewMatch");
                    }
                    else 
                    {
                        newMatch.Date = model.Date;
                        _context.Add(newMatch);
                        _context.SaveChanges();
                        return RedirectToAction("Dashboard");
                    }
                }
                return RedirectToAction("NewMatch");
            }   
        }

        [HttpGet]
        [Route("/matches/{matchId}")]
        public IActionResult ShowMatch(int matchId)
        {
            int? userId = HttpContext.Session.GetInt32("currentUserId");
            if (userId == null)
            {
                TempData["UserError"] = "You must be logged in!";
                return RedirectToAction("Index");
            }
            else
            {
                User currentUser = _context.Users.SingleOrDefault(u => u.UserId == (int)userId);
                ViewBag.User = currentUser;

                Match thisMatch = _context.Matches.SingleOrDefault(w => w.Id == (int)matchId);
                List<Guest> guests = _context.Guests.Where(g => g.MatchId == (int)matchId).Include(g => g.User).ToList();
                // List<User> allUsers = _context.Users.ToList();

                // ViewBag.Users = allUsers;
                User winner = _context.Users.SingleOrDefault(u => u.UserId == thisMatch.WinnerId);
                User loser = _context.Users.SingleOrDefault(u => u.UserId == thisMatch.LoserId);
                List<Post> posts = _context.Posts.Where(p => p.MatchId == (int)matchId).Include(p => p.Creator).Include(p => p.Comments).ToList();

                ViewBag.Winner = winner;
                ViewBag.Loser = loser;
                ViewBag.Posts = posts;
                ViewBag.Match = thisMatch;
                ViewBag.Guests = guests;
                
                return View();
            }
        }

        [HttpGet]
        [Route("/users/{userid}")]
        public IActionResult ShowUser(int userid)
        {
            int? userId = HttpContext.Session.GetInt32("currentUserId");
            if (userId == null)
            {
                TempData["UserError"] = "You must be logged in!";
                return RedirectToAction("Index");
            }
            else
            {
                User currentUser = _context.Users.SingleOrDefault(u => u.UserId == (int)userId);
                ViewBag.User = currentUser;

                User showUser = _context.Users.Include(u => u.Posts).Include(u => u.Comments).SingleOrDefault(u => u.UserId == userid);
                ViewBag.ShowUser = showUser;

                // Wedding thisWedding = _context.Weddings.SingleOrDefault(w => w.Id == (int)weddingId);
                // List<Guest> guests = _context.Guests.Where(g => g.WeddingId == (int)weddingId).Include(g => g.User).ToList();
                // List<User> allUsers = _context.Users.ToList();

                // ViewBag.Users = allUsers;
                // User winner = _context.Users.SingleOrDefault(u => u.UserId == thisWedding.WinnerId);
                // User loser = _context.Users.SingleOrDefault(u => u.UserId == thisWedding.LoserId);

                // ViewBag.Winner = winner;
                // ViewBag.Loser = loser;
                return View();
            }
        }

        [HttpGet]
        [Route("/matches/{matchId}/rsvp")]
        public IActionResult RSVP(int matchId)
        {
            int? userId = HttpContext.Session.GetInt32("currentUserId");
            if (userId == null)
            {
                TempData["UserError"] = "You must be logged in!";
                return RedirectToAction("Index");
            }
            else
            {
                User currentUser = _context.Users.SingleOrDefault(u => u.UserId == (int)userId);
                ViewBag.User = currentUser;
                Match thisMatch = _context.Matches.SingleOrDefault(w => w.Id == (int)matchId);

                Guest newGuest = new Guest
                {
                    UserId = currentUser.UserId,
                    MatchId = thisMatch.Id,
                    Match = thisMatch,
                    User = currentUser
                };
                _context.Add(newGuest);
                _context.SaveChanges();
                return RedirectToAction("Dashboard");
            }
        }

        [HttpGet]
        [Route("/matches/{matchId}/simulate")]
        public IActionResult Simulate(int matchId)
        {
            int? userId = HttpContext.Session.GetInt32("currentUserId");
            if (userId == null)
            {
                TempData["UserError"] = "You must be logged in!";
                return RedirectToAction("Index");
            }
            else
            {
                User currentUser = _context.Users.SingleOrDefault(u => u.UserId == (int)userId);
                ViewBag.User = currentUser;
                Match thisMatch = _context.Matches.SingleOrDefault(w => w.Id == (int)matchId);

                User p1 = _context.Users.SingleOrDefault(u => u.Username == thisMatch.PlayerOne);
                User p2 = _context.Users.SingleOrDefault(u => u.Username == thisMatch.PlayerTwo);
                Random rand = new Random();
                if(rand.Next(1,100) - p1.UserId < rand.Next(1,100) - p2.UserId)
                {
                    thisMatch.WinnerId = p1.UserId;
                    thisMatch.LoserId = p2.UserId;
                    p1.Wins += 1;
                    p2.Losses += 1;
                    
                }
                else
                {
                    thisMatch.WinnerId = p2.UserId;
                    thisMatch.LoserId = p1.UserId;
                    p2.Wins += 1;
                    p1.Losses += 1;
                    
                }
                
                _context.SaveChanges();
                return RedirectToAction("Dashboard");
            }
        }

        [HttpGet]
        [Route("/matches/{matchId}/forfeit")]
        public IActionResult Forfeit(int matchId)
        {
            int? userId = HttpContext.Session.GetInt32("currentUserId");
            if (userId == null)
            {
                TempData["UserError"] = "You must be logged in!";
                return RedirectToAction("Index");
            }
            else
            {

                User LoserUser = _context.Users.SingleOrDefault(u => u.UserId == (int)userId);
                ViewBag.User = LoserUser;
                Match thisMatch = _context.Matches.SingleOrDefault(w => w.Id == (int)matchId);
                User p1 = _context.Users.SingleOrDefault(u => u.Username == thisMatch.PlayerOne);
                User p2 = _context.Users.SingleOrDefault(u => u.Username == thisMatch.PlayerTwo);

                if(thisMatch.PlayerOne == LoserUser.Username)
                {
                    thisMatch.WinnerId = p2.UserId;
                    thisMatch.LoserId = p1.UserId;
                    p2.Wins += 1;
                    p1.Losses += 1;
                }
                else
                {
                    thisMatch.WinnerId = p1.UserId;
                    thisMatch.LoserId = p2.UserId;
                    p1.Wins += 1;
                    p2.Losses += 1;
                }
                
                _context.SaveChanges();
                return RedirectToAction("Dashboard");
            }
        }

// @if(wedding.CreatorId == @ViewBag.User.UserId)
//             {
//                 <td><a href="/delete">Delete</a></td>
//             }
//             else
//             {
//                 <td><a href="/weddings/@wedding.Id/rsvp">RSVP</a></td>
//             }

        [HttpGet]
        [Route("/matches/{matchId}/leave")]
        public IActionResult Leave(int matchId)
        {
            int? userId = HttpContext.Session.GetInt32("currentUserId");
            if (userId == null)
            {
                TempData["UserError"] = "You must be logged in!";
                return RedirectToAction("Index");
            }
            else
            {
                User currentUser = _context.Users.SingleOrDefault(u => u.UserId == (int)userId);
                ViewBag.User = currentUser;
                // Wedding thisWedding = _context.Weddings.SingleOrDefault(w => w.Id == (int)weddingId);
                // Guest thisGuest = _context.Guests.SingleOrDefault(g => g.Id == (int)guestId);
                Guest thisGuest = _context.Guests.Where(g => g.UserId == (int)userId).Where(g => g.MatchId == matchId).SingleOrDefault();
                

                _context.Remove(thisGuest);
                _context.SaveChanges();
                return RedirectToAction("Dashboard");
            }
        }

        [HttpGet]
        [Route("/matches/{matchId}/delete")]
        public IActionResult Delete(int matchId)
        {
            int? userId = HttpContext.Session.GetInt32("currentUserId");
            if (userId == null)
            {
                TempData["UserError"] = "You must be logged in!";
                return RedirectToAction("Index");
            }
            else
            {
                User currentUser = _context.Users.SingleOrDefault(u => u.UserId == (int)userId);
                ViewBag.User = currentUser;
                Match thisMatch = _context.Matches.SingleOrDefault(w => w.Id == (int)matchId);
                _context.Remove(thisMatch);
                _context.SaveChanges();
                return RedirectToAction("Dashboard");
            }
        }

        [HttpPost]
        [Route("/matches/{matchId}/post")]
        public IActionResult Post(Post model, int matchId)
        {
            int? userId = HttpContext.Session.GetInt32("currentUserId");
            if (userId == null)
            {
                TempData["UserError"] = "You must be logged in!";
                return RedirectToAction("Index");
            }
            else
            {
                User currentUser = _context.Users.SingleOrDefault(u => u.UserId == (int)userId);
                ViewBag.User = currentUser;
                Match thisMatch = _context.Matches.SingleOrDefault(w => w.Id == (int)matchId);
                
                if(ModelState.IsValid)
                {
                    Post newPost = new Post
                    {
                        Content = model.Content,
                        UserId = currentUser.UserId,
                        Creator = currentUser,
                        MatchId = thisMatch.Id,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now 
                    };
                    _context.Add(newPost);
                    _context.SaveChanges();
                }

                
                return RedirectToAction("ShowMatch");
            }
        }

        [HttpPost]
        [Route("/matches/{matchId}/{postId}/comment")]
        public IActionResult Comment(Comment model, int postId, int matchId)
        {
            int? userId = HttpContext.Session.GetInt32("currentUserId");
            if (userId == null)
            {
                TempData["UserError"] = "You must be logged in!";
                return RedirectToAction("Index");
            }
            else
            {
                User currentUser = _context.Users.SingleOrDefault(u => u.UserId == (int)userId);
                ViewBag.User = currentUser;
                Post thisPost = _context.Posts.SingleOrDefault(w => w.PostId == (int)postId);
                
                if(ModelState.IsValid)
                {
                    Comment newComment = new Comment
                    {
                        Content = model.Content,
                        UserId = currentUser.UserId,
                        Creator = currentUser,
                        PostId = thisPost.PostId,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now 
                    };
                    _context.Add(newComment);
                    _context.SaveChanges();
                }

                
                return RedirectToAction("ShowMatch");
            }
        }

        [HttpGet]
        [Route("/logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}