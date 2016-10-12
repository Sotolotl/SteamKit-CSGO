using System;
using System.Collections.Generic;
using System.Threading;
using SteamKit2;
using SteamKit2.GC;
using SteamKit2.GC.CSGO.Internal;
using SteamKit2.Internal;

namespace CSGO
{
    public class CsgoClient
    {
        private const int CsgoAppid = 730;
        private readonly bool _debug;
        private readonly SteamGameCoordinator _gameCoordinator;

        private readonly SteamClient _steamClient;

        private Dictionary<uint, Action<IPacketGCMsg>> _gcMap = new Dictionary<uint, Action<IPacketGCMsg>>();
        private SteamUser _user;

        #region contructor
        public CsgoClient(SteamClient steamClient, CallbackManager callbackManager, bool debug = false)
        {
            _steamClient = steamClient;
            _user = steamClient.GetHandler<SteamUser>();
            _gameCoordinator = steamClient.GetHandler<SteamGameCoordinator>();

            _debug = debug;

            callbackManager.Subscribe<SteamGameCoordinator.MessageCallback>(OnGcMessage);
        }
        #endregion

        #region events

        public event Action<ClientGCMsgProtobuf<CMsgClientWelcome>> OnLaunchComplete;
        public event Action<ClientGCMsgProtobuf<CMsgGCCStrike15_v2_PlayersProfile>> OnPlayersProfile;

        #endregion

        private void OnGcMessage(SteamGameCoordinator.MessageCallback obj)
        {
            Console.WriteLine("Received GC message: " + (EGCBaseClientMsg) obj.EMsg);
            var messageMap = new Dictionary<uint, Action<IPacketGCMsg>>
            {
                {(uint) EGCBaseClientMsg.k_EMsgGCClientWelcome, msg => OnLaunchComplete?.Invoke(new ClientGCMsgProtobuf<CMsgClientWelcome>(msg))},
                {(uint)ECsgoGCMsg.k_EMsgGCCStrike15_v2_PlayersProfile, msg => OnPlayersProfile?.Invoke(new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_PlayersProfile>(msg)) }
        
            };

            Action<IPacketGCMsg> func;
            if (!messageMap.TryGetValue(obj.EMsg, out func))
                return;

            func(obj.Message);
        }

        public void Launch()
        {
            Console.WriteLine("Launching CSGO");
            var playGame = new ClientMsgProtobuf<CMsgClientGamesPlayed>(EMsg.ClientGamesPlayed);

            playGame.Body.games_played.Add(new CMsgClientGamesPlayed.GamePlayed
            {
                game_id = CsgoAppid
            });

            _steamClient.Send(playGame);

            Thread.Sleep(3000);

            var clientHello = new ClientGCMsgProtobuf<CMsgClientHello>((uint) EGCBaseClientMsg.k_EMsgGCClientHello);
            _gameCoordinator.Send(clientHello, CsgoAppid);
        }

        public void PlayerProfileRequest(uint accountId)
        {
            PlayerProfileRequest(accountId, 32);
        }
        public void PlayerProfileRequest(uint accountId, uint reqLevel)
        {
            var clientMsgProtobuf =
                new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_ClientRequestPlayersProfile>(
                    (uint)ECsgoGCMsg.k_EMsgGCCStrike15_v2_ClientRequestPlayersProfile)
                {
                    Body =
                    {
                        account_id = accountId,
                        request_level = reqLevel
                    }
                };

            _gameCoordinator.Send(clientMsgProtobuf, CsgoAppid);
        }
    }
}