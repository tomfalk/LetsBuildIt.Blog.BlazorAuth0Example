using Cloudcrate.AspNetCore.Blazor.Browser.Storage;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LetsBuildIt.Blog.BlazorAuth0Example.Client
{
    public class Auth0Service
    {
        // Authentication State
        public bool IsLoggedIn { get; private set; }
        public string AccessToken { get; private set; }
        public string IdToken { get; private set; }
        private DateTime? ExpiresAt { get; set; }

        // Login notifications
        public event Action LoginSuccess;

        // DI Services
        private LocalStorage _storage { get; set; }

        public Auth0Service(LocalStorage storage)
        {
            _storage = storage;
            InitLock();
        }

        public async Task InitLock()
        {
            await JSRuntime.Current.InvokeAsync<object>("Auth0Service.init", new DotNetObjectRef(this));
            await LoadSession();

            //await ExampleJsInterop.Prompt("Hello");
        }

        public async Task Login()
        {
            await JSRuntime.Current.InvokeAsync<object>("Auth0Service.login");         
        }

        public async Task Logout()
        {
            await ClearSession();
            await JSRuntime.Current.InvokeAsync<object>("Auth0Service.logout");
        }

        private async Task RenewToken()
        {
            await JSRuntime.Current.InvokeAsync<object>("Auth0Service.renewToken", new DotNetObjectRef(this));
        }

        private async Task LoadSession()
        {
            ExpiresAt = _storage.GetItem<DateTime>("expires_at");

            if (ExpiresAt == null)
                return;

            if (ExpiresAt > DateTime.Now)
                await RenewToken();
        }

        private async Task SetSession(object authResultJSON)
        {
            if (authResultJSON == null)
                return;

            dynamic authResult = JsonConvert.DeserializeObject<dynamic>(authResultJSON.ToString());

            IdToken = authResult.idToken;
            AccessToken = authResult.accessToken;

            // Set expiry time
            double ticks = (authResult.expiresIn * 1000);
            TimeSpan time = TimeSpan.FromMilliseconds(ticks);
            ExpiresAt = DateTime.Now + time;
            _storage.SetItem("expires_at", ExpiresAt);

            IsLoggedIn = true;
            NotifyLoggedIn();
        }

        private async Task ClearSession()
        {
            IsLoggedIn = false;
            IdToken = null;
            AccessToken = null;
            ExpiresAt = null;
            _storage.RemoveItem("expires_at");
        }

        // JS Invokable Methods
        [JSInvokable]
        public async Task HandleLoginSuccess(object authResultJSON)
        {
            await SetSession(authResultJSON);

        }

        [JSInvokable]
        public async Task HandleTokenRenewedSuccess(object authResultJSON)
        {
            await SetSession(authResultJSON);
        }

        [JSInvokable]
        public async Task HandleLoginFail()
        {
            await ClearSession();
        }

        [JSInvokable]
        public async Task HandleTokenRenewedFail()
        {
            await ClearSession();
        }


        private void NotifyLoggedIn() => LoginSuccess?.Invoke();
    }
}
