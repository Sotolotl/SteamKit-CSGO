﻿using System;
using System.Threading;
using System.Timers;
using SteamKit2;
using SteamKit2.GC;
using SteamKit2.GC.CSGO.Internal;
using SteamKit2.Internal;

namespace SteamKit.CSGO
{
    /// <summary>
    ///     Client for CSGO, allows basic operations such as requesting ranks
    /// </summary>
    public partial class CsgoClient
    {
        //APP ID for csgo
        private const int CsgoAppid = 730;
        private readonly bool _debug;
        private readonly SteamGameCoordinator _gameCoordinator;

        //Contains the callbacks
        private readonly CallbackStore _gcMap = new CallbackStore();

        private readonly SteamClient _steamClient;
        private readonly SteamUser _steamUser;

        private readonly System.Timers.Timer HelloTimer;

        #region contructor

        /// <summary>
        ///     Creates the client
        /// </summary>
        /// <param name="steamClient">A logged in SteamKit2 SteamClient</param>
        /// <param name="callbackManager">The callback manager you used in your log in code</param>
        /// <param name="debug">Wether or not we want to have debug output</param>
        public CsgoClient(SteamClient steamClient, CallbackManager callbackManager, bool debug = false)
        {
            _steamClient = steamClient;
            _steamUser = steamClient.GetHandler<SteamUser>();
            _gameCoordinator = steamClient.GetHandler<SteamGameCoordinator>();

            _debug = debug;

            callbackManager.Subscribe<SteamGameCoordinator.MessageCallback>(OnGcMessage);

            HelloTimer = new System.Timers.Timer(1000);
            HelloTimer.AutoReset = true;
            HelloTimer.Elapsed += Knock;
        }

        private void Knock(object state, ElapsedEventArgs elapsedEventArgs)
        {
            Console.WriteLine("Knocking");
            var clientmsg = new ClientGCMsgProtobuf<CMsgClientHello>((uint)EGCBaseClientMsg.k_EMsgGCClientHello);
            _gameCoordinator.Send(clientmsg, CsgoAppid);
        }

        #endregion

        private void OnGcMessage(SteamGameCoordinator.MessageCallback obj)
        {
            if (_debug)
                Console.WriteLine(
                    $"GC Message: {Enum.GetName(typeof(ECsgoGCMsg), obj.EMsg) ?? Enum.GetName(typeof(EMsg), obj.EMsg)}");

            if (obj.EMsg == (uint) EGCBaseClientMsg.k_EMsgGCClientWelcome)
                HelloTimer.Stop();

            Action<IPacketGCMsg> func;
            if (!_gcMap.TryGetValue(obj.EMsg, out func))
                return;

            func(obj.Message);
        }

        /// <summary>
        ///     Launches the game
        /// </summary>
        /// <param name="callback">The callback to be executed when the operation finishes</param>
        public void Launch(Action<CMsgClientWelcome> callback)
        {
            _gcMap.Add((uint) EGCBaseClientMsg.k_EMsgGCClientWelcome,
                msg => callback(new ClientGCMsgProtobuf<CMsgClientWelcome>(msg).Body));

            if (_debug)
                Console.WriteLine("Launching CSGO");

            var playGame = new ClientMsgProtobuf<CMsgClientGamesPlayed>(EMsg.ClientGamesPlayed);

            playGame.Body.games_played.Add(new CMsgClientGamesPlayed.GamePlayed
            {
                game_id = CsgoAppid
            });

            _steamClient.Send(playGame);

            HelloTimer.Start();

            
        }
    }
}