var Auth0Service = function() {
    var lock = new Auth0Lock('9gwHtz1odGVlikcgSG3GkxmwHsVyfKg9', 'letsbuildit.eu.auth0.com', {
        autoclose: true,
        allowAutocomplete: true,
        avatar: null,
        configurationBaseUrl: 'https://cdn.eu.auth0.com',
        auth: {
            // Set redirect to 'false' to use Lock without redirection after login
            redirect: false,
            redirectUrl: 'http://localhost:56407/login-callback',
            responseType: 'token id_token',
            audience: 'https://letsbuildit.com/api',
            params: {
                scope: 'openid profile email read:all'
            }
        }
    });

    var handleAuth = function (dotnetHelper) {
        lock.on('authenticated', (authResult) => {
            if (authResult && authResult.accessToken && authResult.idToken) {
                console.log("Authentication successful");
                dotnetHelper.invokeMethodAsync('HandleLoginSuccess', authResult);
            }
            else {
                dotnetHelper.invokeMethodAsync('HandleLoginFail');
                console.error('Error authenticating');
            }
        });
    };

    var renewToken = function (dotnetHelper) {
        lock.checkSession({
            audience: 'https://letsbuildit.com/api',
            scope: 'openid profile email read:all'
        }, (err, authResult) => {
            if (authResult && authResult.accessToken) {
                console.log("Token renewed successfully");
                dotnetHelper.invokeMethodAsync('HandleTokenRenewedSuccess', authResult);
            } else {
                console.log("Token renew failed");
                dotnetHelper.invokeMethodAsync('HandleTokenRenewedFail');
            }
        });
    };

    return {
        //== Init lock
        init: function (dotnetHelper) {
            console.log("Initing lock");
            handleAuth(dotnetHelper);    
        },
        login: function () {
            lock.show();
        },
        logout: function () {
            lock.logout({
                returnTo: 'http://localhost:56407'
            });
        },
        renewToken: function (dotnetHelper) {
            console.log("Renewing token");
            renewToken(dotnetHelper);
        }
    };
}();