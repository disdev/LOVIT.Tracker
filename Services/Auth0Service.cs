using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;
using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using LOVIT.Tracker.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace LOVIT.Tracker.Services;

public interface IAuth0Service
{
    Task<User> GetUserAsync(string userId);
    Task<User> UpdateUserAsync(string userId, string firstName, string lastName, string phoneNumber);
    Task<string> GetUserPhoneNumber(string userId);
}

public class Auth0Service : IAuth0Service
{
    private readonly Auth0Config _auth0Config;
    private AuthenticationApiClient _auth0Client; 
    private ManagementApiClient _auth0ManagementClient;
    private readonly ITextService _TextService;

    public Auth0Service(IOptionsMonitor<Auth0Config> optionsMonitor, ITextService TextService)
    {
        _auth0Config = optionsMonitor.CurrentValue;
        _TextService = TextService;

        _auth0Client = new AuthenticationApiClient(_auth0Config.Domain);
        var adminToken = _auth0Client.GetTokenAsync(new ClientCredentialsTokenRequest() 
        {
            ClientId = _auth0Config.ManagementClientId,
            ClientSecret = _auth0Config.ManagementClientSecret,
            Audience = _auth0Config.ManagementAudience
        }).Result;

        _auth0ManagementClient = new ManagementApiClient(adminToken.AccessToken, _auth0Config.Domain);
    }

    public async Task<User> GetUserAsync(string userId)
    {
        return await _auth0ManagementClient.Users.GetAsync(userId);
    }

    public async Task<string> GetUserPhoneNumber(string userId)
    {
        var user = await GetUserAsync(userId);
        if (user.UserMetadata == null)
        {
            return "";    
        }

        return user.UserMetadata["PhoneNumber"].Value;
    }

    public async Task<User> UpdateUserAsync(string userId, string firstName, string lastName, string phoneNumber)
    {
        var user = await GetUserAsync(userId);
        var userUpdateRequest = new UserUpdateRequest();
        
        if (user.UserId.StartsWith("auth0"))
        {
            userUpdateRequest.FirstName = firstName;
            userUpdateRequest.LastName = lastName;
        };        
        
        dynamic metadata = new JObject();
        metadata.PhoneNumber = phoneNumber;

        userUpdateRequest.UserMetadata = metadata;

        return await _auth0ManagementClient.Users.UpdateAsync(userId, userUpdateRequest);
    }
}