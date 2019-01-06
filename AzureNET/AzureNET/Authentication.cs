﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AzureNET
{
    public class Authentication
    {
        private string subscriptionKey;
        private string token;
        private Timer accessTokenRenewer;

        //Access token expires every 10 minutes. Renew it every 9 minutes.
        private const int RefreshTokenDuration = 9;

        public Authentication( string subscriptionKey )
        {
            this.subscriptionKey = subscriptionKey;
            this.token = FetchToken( AzurePrivateData.FETCH_TOKEN_URI, subscriptionKey ).Result;

            // renew the token on set duration.
            accessTokenRenewer = new Timer( new TimerCallback(OnTokenExpiredCallback),
                                            this,
                                            TimeSpan.FromMinutes(RefreshTokenDuration),
                                            TimeSpan.FromMilliseconds(-1) );
        }

        public string GetAccessToken()
        {
            return this.token;
        }

        private void RenewAccessToken()
        {
            this.token = FetchToken( AzurePrivateData.FETCH_TOKEN_URI, this.subscriptionKey ).Result;
            Console.WriteLine("Renewed token.");
        }

        private void OnTokenExpiredCallback( object stateInfo )
        {
            try
            {
                RenewAccessToken();
            }
            catch( Exception ex )
            {
                Console.WriteLine( string.Format( "Failed renewing access token. Details: {0}", ex.Message ) );
            }
            finally
            {
                try
                {
                    accessTokenRenewer.Change( TimeSpan.FromMinutes( RefreshTokenDuration ), 
                        TimeSpan.FromMilliseconds(-1) );
                }
                catch( Exception ex )
                {
                    Console.WriteLine( string.Format( "Failed to reschedule the timer to renew access token. Details: {0}", ex.Message ) );
                }
            }
        }

        private async Task<string> FetchToken( string fetchUri, string subscriptionKey )
        {
            using( var client = new HttpClient() )
            {
                client.DefaultRequestHeaders.Add( "Ocp-Apim-Subscription-Key", subscriptionKey );
                UriBuilder uriBuilder = new UriBuilder( fetchUri );

                var result = await client.PostAsync( uriBuilder.Uri.AbsoluteUri, null );
                Console.WriteLine( "Token Uri: {0}", uriBuilder.Uri.AbsoluteUri );
                return await result.Content.ReadAsStringAsync();
            }
        }
    }
}