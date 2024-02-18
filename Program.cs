using Classwork_11._02._24_auth_authorization_role_.Interfaces;
using Classwork_11._02._24_auth_authorization_role_.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Classwork_11._02._24_auth_authorization_role_.Models;

using System.Security.Claims;
using System.Linq;
using System.Text;
using System.Drawing;
using System;

namespace Classwork_11._02._24_auth_authorization_role_
{
    public class Program
    {
        //Используя базу данных, создать приложение с возможностью авторизации и регистрации. Добавьте кнопку с возможностью просмотра профиля пользователя и кнопку для выхода
        //из учетной записи. С помощью Claim, хранить идентификатор пользователя, с помощью которого работать с базой данных.

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddTransient<IUserRepository, UserRepository>();
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options => options.LoginPath = "/login");
            builder.Services.AddAuthorization();

            var app = builder.Build();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseRouter(main => {
                main.MapGet("/", async context =>
                {
                    context.Response.Redirect("/login");
                });
            });

            var userRepo = app.Services.GetService<IUserRepository>();

            app.MapGet("/login", async context =>
            {
                await context.Response.WriteAsync("""
                        <!DOCTYPE html>
                        <html lang="en">
                          <head>
                            <meta charset="UTF-8">
                            <meta name="viewport" content="width=device-width, initial-scale=1.0">
                            <meta http-equiv="X-UA-Compatible" content="ie=edge">
                            <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@4.4.1/dist/css/bootstrap.min.css" integrity="sha384-Vkoo8x4CGsO3+Hhxv8T/Q5PaXtkKtu6ug5TOeNV6gBiFeWPGFN9MuhOf23Q9Ifjh" crossorigin="anonymous">
                            <title>User login</title>
                          </head>
                          <body>
                                <div class="container mt-2">
                                      <form method="post">
                                            <div class="form-group">
                                              <label for="login">Login:</label>
                                              <input type="text" class="form-control" id="login" name="login" placeholder="Enter login...">
                                            </div>
                                            <div class="form-group">
                                              <label for="email">Email:</label>
                                              <input type="text" class="form-control" id="email" name="email" placeholder="Enter email...">
                                            </div>
                                            <div>
                                                <p>Not authorized? Click <a href="/register">here</a> for register yourself!</p>
                                            </div>
                                            <button type="submit" class="btn btn-primary">Send</button>
                                      </form>
                                <div>
                          </body>
                        </html>
                    """);
            });

            app.MapPost("/login", async (string? returnURL, HttpContext context) =>
            {
                var form = context.Request.Form;
                string email = form["email"];
                string login = form["login"];

                var person = userRepo.GetUserByLoginAndEmail(login, email);
                if(person is null)
                {
                    return Results.Unauthorized();
                }
                else
                {
                    var claims = new List<Claim> { new Claim(ClaimTypes.Email, person.Email), new Claim(ClaimTypes.Role, person.Role.ToString()), new Claim(ClaimTypes.Name, person.Login), new Claim(ClaimTypes.SerialNumber, person.Id.ToString()) };
                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");
                    await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                    return Results.Redirect(returnURL ?? "/main");
                }
            });

            app.MapGet("/main", [Authorize] async (HttpContext context) =>
            {
                var currentUser = context.User.Identity;
                var id = context.User.FindFirstValue(ClaimTypes.SerialNumber);
                
                await context.Response.WriteAsync($"""
                     <!DOCTYPE html>
                        <html lang="en">
                          <head>
                            <meta charset="UTF-8">
                            <meta name="viewport" content="width=device-width, initial-scale=1.0">
                            <meta http-equiv="X-UA-Compatible" content="ie=edge">
                            <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@4.4.1/dist/css/bootstrap.min.css" integrity="sha384-Vkoo8x4CGsO3+Hhxv8T/Q5PaXtkKtu6ug5TOeNV6gBiFeWPGFN9MuhOf23Q9Ifjh" crossorigin="anonymous">
                            <title>User login</title>
                          </head>
                          <body>
                                <div class="container mt-2">
                                      <p> Hello, {currentUser!.Name}!</p>
                                      <div>
                                        <a class="btn btn-primary" href="/property/{id}" role="button">Properties</a>
                                        <a class="btn btn-primary" href="/logout" role="button">Log out</a>
                                      </div>
                                <div>
                          </body>
                        </html>
                    """);
            });

            app.MapGet("/logout", async (HttpContext context) =>
            {
                await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return Results.Redirect("/login");
            });

            app.MapGet("/register", async context =>
            {
                await context.Response.WriteAsync("""
                        <!DOCTYPE html>
                        <html lang="en">
                          <head>
                            <meta charset="UTF-8">
                            <meta name="viewport" content="width=device-width, initial-scale=1.0">
                            <meta http-equiv="X-UA-Compatible" content="ie=edge">
                            <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@4.4.1/dist/css/bootstrap.min.css" integrity="sha384-Vkoo8x4CGsO3+Hhxv8T/Q5PaXtkKtu6ug5TOeNV6gBiFeWPGFN9MuhOf23Q9Ifjh" crossorigin="anonymous">
                            <title>User login</title>
                          </head>
                          <body>
                                <div class="container mt-2">
                                      <form method="post">
                                            <p>Let's register yourself!</p>
                                            <div class="form-group">
                                              <label for="login">Login:</label>
                                              <input type="text" class="form-control" id="login" name="login" placeholder="Enter login...">
                                            </div>
                                            <div class="form-group">
                                              <label for="email">Email:</label>
                                              <input type="text" class="form-control" id="email" name="email" placeholder="Enter email...">
                                            </div>
                                            <button type="submit" class="btn btn-primary">Register</button>
                                      </form>
                                <div>
                          </body>
                        </html>
                    """);
            });

            app.MapPost("/register", async context =>
            {
                var form = context.Request.Form;
                var email = form["email"];
                var login = form["login"];

                var current = userRepo.GetUserByLoginAndEmail(login, email);

                if(current == null)
                {
                    userRepo.AddUser(new User { Id = Guid.NewGuid(), Email = email, Login = login});
                    await context.Response.WriteAsync("""
                            <!DOCTYPE html>
                            <html lang="en">
                              <head>
                                <meta charset="UTF-8">
                                <meta name="viewport" content="width=device-width, initial-scale=1.0">
                                <meta http-equiv="X-UA-Compatible" content="ie=edge">
                                <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@4.4.1/dist/css/bootstrap.min.css" integrity="sha384-Vkoo8x4CGsO3+Hhxv8T/Q5PaXtkKtu6ug5TOeNV6gBiFeWPGFN9MuhOf23Q9Ifjh" crossorigin="anonymous">
                                <title>User login</title>
                              </head>
                              <body>
                                    <div class="container mt-2">
                                          <p>You have registered yourself!</p>
                                          <a class="btn btn-primary" href="/login" role="button">Back to login</a>
                                    <div>
                              </body>
                            </html>
                        """);
                }
                else
                {
                    Results.Redirect("/login");
                }
            });

            app.MapGet("/property/{id}", [Authorize] async (string id, HttpContext context) =>
            {
                var currentUser = userRepo!.GetUserById(Guid.Parse(id));
                var roles = Enum.GetValues(typeof(Role)).Cast<Role>().ToList();
                var availableRoles = roles.Where(e => !e.Equals(currentUser.Role)).ToList();

            StringBuilder str = new StringBuilder();

            str.Append($"""
                    <!DOCTYPE html>
                    <html lang="en">
                      <head>
                        <meta charset="UTF-8">
                        <meta name="viewport" content="width=device-width, initial-scale=1.0">
                        <meta http-equiv="X-UA-Compatible" content="ie=edge">
                        <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@4.4.1/dist/css/bootstrap.min.css" integrity="sha384-Vkoo8x4CGsO3+Hhxv8T/Q5PaXtkKtu6ug5TOeNV6gBiFeWPGFN9MuhOf23Q9Ifjh" crossorigin="anonymous">
                        <title>User login</title>
                      </head>
                      <body>
                            <div class="container mt-2">
                                  <form method="post" action="/property">
                                        <input type="hidden" value = {currentUser.Id} name = "id" />
                                        <div class="form-group">
                                          <label for="login">Login:</label>
                                          <input type="text" class="form-control" id="login" name="login" value={currentUser.Login}>
                                        </div>
                                        <div class="form-group">
                                          <label for="email">Email:</label>
                                          <input type="text" class="form-control" id="email" name="email" value={currentUser.Email}>
                                        </div>
                                        <div>
                                            <label for="role">Role:</label>
                                            <select class="form-control" name="role">
                                                <option selected>{currentUser.Role}</option>
                    """);

            foreach (var item in availableRoles)
            {
                    str.Append($"""
                        <option value="{item}">{item}</option>
                    """);
                }

            str.Append("""
                    </select>
                    </div>
                                    <br><button type="submit" class="btn btn-primary mt-2">Change data</button>
                              </form>
                        </div>
                  </body>
                </html>
                """);

                await context.Response.WriteAsync(str.ToString());
            });

            app.MapPost("/property", [Authorize] async (HttpContext context) =>
            {
                var form = context.Request.Form;
                var currentLogin = form["login"];
                var currentEmail = form["email"];
                var currentRole = form["role"];
                var currentId = form["Id"];

                var userFromDB = userRepo.GetUserById(Guid.Parse(currentId));
                userFromDB.Login = currentLogin;
                userFromDB.Email = currentEmail;
                userFromDB.Role = (Role)Enum.Parse(typeof(Role), currentRole);

                userRepo.UpdateUser(userFromDB);

                var claims = new List<Claim> { new Claim(ClaimTypes.Email, userFromDB.Email), new Claim(ClaimTypes.Role, userFromDB.Role.ToString()), new Claim(ClaimTypes.Name, userFromDB.Login), new Claim(ClaimTypes.SerialNumber, userFromDB.Id.ToString()) };
                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");
                await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                return Results.Redirect("main");
            });

            app.Run();
        }
    }
}