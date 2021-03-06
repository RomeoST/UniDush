﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DashBoard.BLL.Infrastructure;
using DashBoard.Model.Models;

namespace DashBoard.BLL.Interfaces
{
    public interface IUserService
    {
        Task<OperationDetails> Create(DutUser userDto, string password);
        Task<ClaimsIdentity> Authenticate(string login, string password);
        Task<DutUser> FindByName(string name);
        Task<IEnumerable<ClientProfile>> GetAll();
        Task SetInitialData(DutUser adminDto, List<string> roles);

        Task<bool> HasPermission(string userName,string requiredPermission);

        Task<OperationDetails> EditProfile(DutUser user);

        Task SaveUser();
    }
}
