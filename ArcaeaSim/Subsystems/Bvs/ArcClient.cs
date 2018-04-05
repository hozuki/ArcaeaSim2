using System.Threading.Tasks;
using JetBrains.Annotations;
using Moe.Mottomo.ArcaeaSim.Subsystems.Bvs.Models;
using OpenMLTD.Piyopiyo;
using OpenMLTD.Piyopiyo.Net.JsonRpc;

namespace Moe.Mottomo.ArcaeaSim.Subsystems.Bvs {
    public sealed class ArcClient : JsonRpcClient {

        internal ArcClient([NotNull] ArcCommunication communication) {
            _communication = communication;
        }

        public Task SendPlayingNotification() {
            return SendNotificationWithEmptyBody(CommonProtocolMethodNames.Preview_Playing);
        }

        public Task SendPausedNotification() {
            return SendNotificationWithEmptyBody(CommonProtocolMethodNames.Preview_Paused);
        }

        public Task SendStoppedNotification() {
            return SendNotificationWithEmptyBody(CommonProtocolMethodNames.Preview_Stopped);
        }

        public Task SendReloadedNotification() {
            return SendNotificationWithEmptyBody(CommonProtocolMethodNames.Edit_Reloaded);
        }

        public Task SendLaunchedNotification() {
            var serverUri = _communication.EditorServerUri;

            if (serverUri == null) {
                return Task.FromResult(0);
            }

            var endPoint = _communication.Server.EndPoint;

            var param0Object = new GeneralSimLaunchedNotificationParams {
                SimulatorServerUri = $"http://{endPoint.Address}:{endPoint.Port}/"
            };

            return SendNotificationAsync(serverUri, CommonProtocolMethodNames.General_SimLaunched, new[] { param0Object });
        }

        public Task SendSimExitedNotification() {
            return SendNotificationWithEmptyBody(CommonProtocolMethodNames.General_SimExited);
        }

        private Task SendNotificationWithEmptyBody([NotNull] string method) {
            if (_communication.EditorServerUri == null) {
                return Task.FromResult(0);
            }

            return SendNotificationAsync(_communication.EditorServerUri, method);
        }

        [NotNull]
        private readonly ArcCommunication _communication;

    }
}
