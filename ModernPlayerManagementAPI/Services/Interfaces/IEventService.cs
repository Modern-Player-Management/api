﻿using System;
using System.Collections.Generic;
using Ical.Net;
using ModernPlayerManagementAPI.Models;
using ModernPlayerManagementAPI.Models.DTOs;

namespace ModernPlayerManagementAPI.Services
{
    public interface IEventService
    {
        void ConfirmEvent(Guid eventId, Guid getCurrentUserId);
        void AddDiscrepancy(Guid eventId, UpsertDiscrepancyDTO dto, Guid getCurrentUserId);
        void UpdateEvent(UpsertEventDTO dto, Guid eventId);
        void DeleteEvent(Guid guid);
        bool IsUserTeamManager(Guid eventId, Guid userId);
        Calendar GetUserCalendar(Guid icalSecret);
    }
}