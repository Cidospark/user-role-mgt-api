using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using UserRoleMgtApi.Data.EFCore;
using UserRoleMgtApi.Models;

namespace UserRoleMgtApi.Data
{
    public class Seeder
    {
        private readonly AppDbContext _ctx;
        private readonly UserManager<User> _userMgr;
        private readonly RoleManager<IdentityRole> _roleMgr;

        public Seeder(AppDbContext ctx,
            UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _ctx = ctx;
            _userMgr = userManager;
            _roleMgr = roleManager;
        }

        public async Task SeedIt()
        {
            _ctx.Database.EnsureCreated();

            try
            {
                var roles = new string[] { "Regular", "Admin" };
                if (!_roleMgr.Roles.Any())
                {
                    foreach (var role in roles)
                    {
                        await _roleMgr.CreateAsync(new IdentityRole(role));
                    }
                }

                //var path = "";

                //if()

                //var data = System.IO.File.ReadAllText("../UserRoleMgtApi.Data/SeedData.json");
                var data = System.IO.File.ReadAllText("/app/SeedData.json");
                var ListOfAppUsers = JsonConvert.DeserializeObject<List<User>>(data);

                if (!_userMgr.Users.Any())
                {
                    var counter = 0;
                    var role = "";
                    foreach (var user in ListOfAppUsers)
                    {
                        user.UserName = user.Email;
                        if(counter < ListOfAppUsers.Count)
                            user.Address[0].Id = Guid.NewGuid().ToString();
                        role = counter < 1 ? roles[1] : roles[0]; // tenary operator

                        var res = await _userMgr.CreateAsync(user, "P@ssw0rd");
                        if (res.Succeeded)
                            await _userMgr.AddToRoleAsync(user, role);

                        counter++;
                    }
                }
            }
            catch (DbException)
            {
                //log err
            }

        }
    }
}
