using Cloudcrate.AspNetCore.Blazor.Browser.Storage;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LetsBuildIt.Blog.BlazorAuth0Example.Client.Services
{
    public class AppState
    {
        // State
        public bool IsLoggedIn => _auth0Service.IsLoggedIn;

        // Lets components receive change notifications
        // Could have whatever granularity you want (more events, hierarchy...)
        public event Action OnChange;

        // DI Services
        private Auth0Service _auth0Service { get; set; }


        public AppState(Auth0Service auth0Service)
        {
            _auth0Service = auth0Service;
            _auth0Service.LoginSuccess += LoggedIn;
        }

        public async Task Login()
        {
            await _auth0Service.Login();        
        }

        public async Task Logout()
        {
            await _auth0Service.Logout();
        }
        
        //public bool IsLoggedIn()
        //{
        //    return _auth0Service.IsLoggedIn;
        //}

        private void LoggedIn()
        {
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
