﻿using ModernPlayerManagementAPI.Models;

namespace ModernPlayerManagementAPI.Services
{
    public interface IUserService
    {
        bool IsUniqueUser(string username);
        User Authenticate(string username, string password);
        User Register(string username, string email, string password);
    }
}