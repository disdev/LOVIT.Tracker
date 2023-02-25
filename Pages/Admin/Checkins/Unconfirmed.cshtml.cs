#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using LOVIT.Tracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using LOVIT.Tracker.Services;

namespace LOVIT.Tracker.Pages.Admin.Checkins
{
    [Authorize(Roles="Administrator")]
    public class UnconfirmedModel : PageModel
    {
        private readonly ICheckinService _checkinService;
        private readonly ISegmentService _segmentService;
        private readonly IMonitorService _monitorService;

        public UnconfirmedModel(ICheckinService checkinService, ISegmentService segmentService, IMonitorService monitorService)
        {
            _checkinService = checkinService;
            _segmentService = segmentService;
            _monitorService = monitorService;
        }

        public IList<Checkin> Checkins { get; set; }
        public List<LOVIT.Tracker.Models.Monitor> Monitors { get; set; }

        public async Task OnGetAsync()
        {
            await LoadData();
        }

        public async Task<IActionResult> OnPostAsync(Guid checkinId, DateTime checkinWhen, Guid checkinSegmentId)
        {
            await _checkinService.ConfirmCheckinAsync(checkinId, checkinWhen.ToUniversalTime(), checkinSegmentId);
            await LoadData();
            return RedirectToPage("/admin/checkins/unconfirmed");
        }

        private async Task LoadData()
        {
            Checkins = await _checkinService.GetUnconfirmedCheckinsAsync();
            Monitors = await _monitorService.GetMonitorsAsync();
            ViewData["Segments"] = new SelectList(await _segmentService.GetSegmentsAsync(), "Id", "Name");
        }
    }
}
