// This is installed in the Fetch User Profile script
// section of the Strava custom social connector.
function (accessToken, ctx, cb) {
    request({
        url: "https://www.strava.com/api/v3/athlete",
        method: "GET",
        headers: {
            "Authorization": "Bearer " + accessToken,
            "Content-Type": "application/json"
        }
    },
    function(e, r, b) {
        if (e) return cb(e);
        if (r.statusCode !== 200) return cb(new Error('StatusCode: ' + r.statusCode));
        
        var stravaProfile = JSON.parse(b);

        var profile = {
            user_id: stravaProfile.id,
            name: stravaProfile.username,
            given_name: stravaProfile.firstname,
            family_name: stravaProfile.lastname,
            picture: stravaProfile.profile_medium
        };
        cb(null, profile);
    });
}