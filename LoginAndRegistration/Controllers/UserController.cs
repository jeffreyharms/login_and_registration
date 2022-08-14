using Microsoft.AspNetCore.Mvc;
using LoginAndRegistration.Models;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace LoginAndRegistration.Controllers;

public class UserController : Controller
{
    private LoginRegistrationContext db;
    public UserController(LoginRegistrationContext context)
    {
        db = context;
    }
    private int? UUID
    {
        get
        {
            return HttpContext.Session.GetInt32("UUID");
        }
    }

    private bool loggedIn
    {
        get
        {
            return UUID != null;
        }
    }

    [HttpGet("/")]
    public IActionResult Index()
    {
        if(loggedIn)
        {
            return Success();
        }
        return View("Index");
    }


    [HttpPost("/register")]
    public IActionResult Register(User newUser)
    {
        if(ModelState.IsValid)
        {
            if(db.users.Any(user => user.email == newUser.email))
            {
                ModelState.AddModelError("email", "is taken");
            }
        }

        if(ModelState.IsValid == false)
        {
            return Index();
        }

        PasswordHasher<User> hashKetchum = new PasswordHasher<User>();
        newUser.password = hashKetchum.HashPassword(newUser, newUser.password);
        db.users.Add(newUser);
        db.SaveChanges();
        HttpContext.Session.SetInt32("UUID", newUser.id);
        return Success();
    }


    [HttpGet("/login")]
    public IActionResult Enter()
    {
        if(loggedIn)
        {
            return Success();
        }
        return View("Index");
    }


    [HttpPost("/loggingin")]
    public IActionResult Login(LoginUser loginUser)
    {
        if(ModelState.IsValid == false)
        {
            return Enter();
        }

        User? dbUser = db.users.FirstOrDefault(loggedUser => loggedUser.email == loginUser.LoginEmail);

        if(dbUser == null)
        {
            ModelState.AddModelError("LoginEmail", "not found");
            return Enter();
        }

        PasswordHasher<LoginUser> hashKetchum = new PasswordHasher<LoginUser>();
        PasswordVerificationResult pwCompare = hashKetchum.VerifyHashedPassword(loginUser, dbUser.password, loginUser.LoginPassword);

        if(pwCompare == 0)
        {
            ModelState.AddModelError("LoginPassword", "is not correct");
            return Enter();
        }

        HttpContext.Session.SetInt32("UUID", dbUser.id);
        return Success();
    }


    [HttpGet("/success")]
    public IActionResult Success()
    {
        if(!loggedIn)
        {
            return Enter();
        }
        return View("Success");
    }

    [HttpPost("/logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return Enter();
    }
}