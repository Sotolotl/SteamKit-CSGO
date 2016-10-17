using System;
using System.IO;
using System.Threading;
using System.Security.Cryptography;
using SteamKit2;
using CsgoClient = SteamKit.CSGO.CsgoClient;

namespace Basic
{
    class Program
    {
        static SteamClient _steamClient;
        static CallbackManager _manager;

        static SteamUser _steamUser;

        static bool _isRunning;

        static string _authCode, _twoFactorAuth;


        static void Main()
        {

            // save our logon details

            // create our steamclient instance
            _steamClient = new SteamClient();
            // create the callback manager which will route callbacks to function calls
            _manager = new CallbackManager(_steamClient);

            // get the steamuser handler, which is used for logging on after successfully connecting
            _steamUser = _steamClient.GetHandler<SteamUser>();
            _steamClient.GetHandler<SteamGameCoordinator>();

            // register a few callbacks we're interested in
            // these are registered upon creation to a callback manager, which will then route the callbacks
            // to the functions specified
            _manager.Subscribe<SteamClient.ConnectedCallback>(OnConnected);
            _manager.Subscribe<SteamClient.DisconnectedCallback>(OnDisconnected);

            _manager.Subscribe<SteamUser.LoggedOnCallback>(OnLoggedOn);
            _manager.Subscribe<SteamUser.LoggedOffCallback>(OnLoggedOff);

            // this callback is triggered when the steam servers wish for the client to store the sentry file
            _manager.Subscribe<SteamUser.UpdateMachineAuthCallback>(OnMachineAuth);

            _isRunning = true;

            Console.WriteLine("Connecting to Steam...");

            // initiate the connection
            _steamClient.Connect();

            // create our callback handling loop
            while (_isRunning)
            {
                // in order for the callbacks to get routed, they need to be handled by the manager
                _manager.RunWaitCallbacks(TimeSpan.FromSeconds(1));
            }
        }

        static void OnConnected(SteamClient.ConnectedCallback callback)
        {
            if (callback.Result != EResult.OK)
            {
                Console.WriteLine("Unable to connect to Steam: {0}", callback.Result);

                _isRunning = false;
                return;
            }

            Console.WriteLine("Connected to Steam! Logging in '{0}'...", Settings.Username);

            byte[] sentryHash = null;
            if (File.Exists("sentry.bin"))
            {
                // if we have a saved sentry file, read and sha-1 hash it
                byte[] sentryFile = File.ReadAllBytes("sentry.bin");
                sentryHash = CryptoHelper.SHAHash(sentryFile);
            }

            _steamUser.LogOn(new SteamUser.LogOnDetails
            {
                Username = Settings.Username,
                Password = Settings.Password,

                // in this sample, we pass in an additional authcode
                // this value will be null (which is the default) for our first logon attempt
                AuthCode = _authCode,

                // if the account is using 2-factor auth, we'll provide the two factor code instead
                // this will also be null on our first logon attempt
                TwoFactorCode = _twoFactorAuth,

                // our subsequent logons use the hash of the sentry file as proof of ownership of the file
                // this will also be null for our first (no authcode) and second (authcode only) logon attempts
                SentryFileHash = sentryHash,
            });
        }

        static void OnDisconnected(SteamClient.DisconnectedCallback callback)
        {
            // after recieving an AccountLogonDenied, we'll be disconnected from steam
            // so after we read an authcode from the user, we need to reconnect to begin the logon flow again

            Console.WriteLine("Disconnected from Steam, reconnecting in 5...");

            Thread.Sleep(TimeSpan.FromSeconds(5));

            _steamClient.Connect();
        }

        static void OnLoggedOn(SteamUser.LoggedOnCallback callback)
        {
            bool isSteamGuard = callback.Result == EResult.AccountLogonDenied;
            bool is2Fa = callback.Result == EResult.AccountLoginDeniedNeedTwoFactor;

            if (isSteamGuard || is2Fa)
            {
                Console.WriteLine("This account is SteamGuard protected!");

                if (is2Fa)
                {
                    Console.Write("Please enter your 2 factor auth code from your authenticator app: ");
                    _twoFactorAuth = Console.ReadLine();
                }
                else
                {
                    Console.Write("Please enter the auth code sent to the email at {0}: ", callback.EmailDomain);
                    _authCode = Console.ReadLine();
                }

                return;
            }

            if (callback.Result != EResult.OK)
            {
                Console.WriteLine("Unable to logon to Steam: {0} / {1}", callback.Result, callback.ExtendedResult);

                _isRunning = false;
                return;
            }

            Console.WriteLine("Successfully logged on!");

            // at this point, we'd be able to perform actions on Steam

            //TODO: More CSGO Stuff!
            CsgoClient csgo = new CsgoClient(_steamClient, _manager, true);
            csgo.Launch(protobuf =>
            {
                Thread.Sleep(1000);
                csgo.PlayerProfileRequest(51455204, msgProtobuf =>
                {
                    if(msgProtobuf.account_profiles.Count > 0)
                        Console.WriteLine($"Player has {msgProtobuf.account_profiles[0].ranking.wins} wins");
                });

                csgo.MatchmakingStatsRequest(msgProtobuf =>
                {
                    Console.WriteLine($"{msgProtobuf.global_stats.players_online} players searching");
                });

                csgo.RequestCurrentLiveGames(list => { Console.WriteLine(list.matches.Count); });

                csgo.RequestRecentGames(list => { Console.WriteLine(list.accountid); });
            });
        }

        static void OnLoggedOff(SteamUser.LoggedOffCallback callback)
        {
            Console.WriteLine("Logged off of Steam: {0}", callback.Result);
        }

        static void OnMachineAuth(SteamUser.UpdateMachineAuthCallback callback)
        {
            Console.WriteLine("Updating sentryfile...");

            // write out our sentry file
            // ideally we'd want to write to the filename specified in the callback
            // but then this sample would require more code to find the correct sentry file to read during logon
            // for the sake of simplicity, we'll just use "sentry.bin"

            int fileSize;
            byte[] sentryHash;
            using (var fs = File.Open("sentry.bin", FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                fs.Seek(callback.Offset, SeekOrigin.Begin);
                fs.Write(callback.Data, 0, callback.BytesToWrite);
                fileSize = (int)fs.Length;

                fs.Seek(0, SeekOrigin.Begin);
                using (var sha = new SHA1CryptoServiceProvider())
                {
                    sentryHash = sha.ComputeHash(fs);
                }
            }

            // inform the steam servers that we're accepting this sentry file
            _steamUser.SendMachineAuthResponse(new SteamUser.MachineAuthDetails
            {
                JobID = callback.JobID,

                FileName = callback.FileName,

                BytesWritten = callback.BytesToWrite,
                FileSize = fileSize,
                Offset = callback.Offset,

                Result = EResult.OK,
                LastError = 0,

                OneTimePassword = callback.OneTimePassword,

                SentryFileHash = sentryHash,
            });

            Console.WriteLine("Done!");
        }
    }
}
