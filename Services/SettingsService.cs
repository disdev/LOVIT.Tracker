using System;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using LOVIT.Tracker.Data;
using LOVIT.Tracker.Models;

namespace LOVIT.Tracker.Services;

public interface ISettingsService
{
    Task<bool> IsTwilioEnabled();
}

public class SettingsService : ISettingsService
{
    private readonly TrackerContext _context;

    public SettingsService(TrackerContext context)
    {
        _context = context;
    }

    public async Task<bool> IsTwilioEnabled()
    {
        var setting = await GetSetting("TwilioEnabled");
        
        if (String.IsNullOrEmpty(setting.Key)) {
            return false;
        }

        return Convert.ToBoolean(setting.Value, CultureInfo.InvariantCulture);
    }

    private async Task<Setting> GetSetting(string key)
    {
        try
        {
            return await _context.Settings.Where(x => x.Key == key).FirstAsync();
        }
        catch
        {
            return new Setting();
        }
    }
}