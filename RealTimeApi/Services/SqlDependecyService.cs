using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using RealTimeApi.Hubs;
using System.Data.SqlClient;

namespace RealTimeApi.Services
{
    public interface IDataChangeNotificationService
    {
        void Config();
    }
    public class SqlDependecyService : IDataChangeNotificationService
    {
        private readonly IConfiguration configuration;
        private readonly IHubContext<HubNotificacion> hub;

        public SqlDependecyService(IConfiguration configuration, IHubContext<HubNotificacion> hub)
        {
            this.configuration = configuration;
            this.hub = hub;
        }

        public void Config()
        {
            Suscribir();
        }

        private void Suscribir()
        {
            string cns = configuration.GetConnectionString("DefaultConnection");
            using (var conn = new SqlConnection(cns))
            {
                conn.Open();
                using (var cmd = new SqlCommand(@"SELECT [EmployeeID], [FirstName], [LastName], [PhoneNumber] FROM [dbo].[EmployeeList]", conn))
                {
                    cmd.Notification = null;
                    SqlDependency dependency = new SqlDependency(cmd);
                    dependency.OnChange += Dependency_OnChange;
                    SqlDependency.Start(cns);
                    cmd.ExecuteReader();
                }
            }
        }

        private void Dependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            if (e.Type == SqlNotificationType.Change)
            {
                string msg = ObtenerMensaje(e);
                hub.Clients.All.SendAsync("ReceiveMessage", "Admin", msg);
            }
            Suscribir();
        }

        private string ObtenerMensaje(SqlNotificationEventArgs e)
        {
            string Mensaje = "";
            switch (e.Info)
            {
                case SqlNotificationInfo.Insert:
                    Mensaje = "Se ha insertado un registro";
                    break;
                case SqlNotificationInfo.Update:
                    Mensaje = "Se ha actualizado un registro";
                    break;
                case SqlNotificationInfo.Delete:
                    Mensaje = "Se ha Eliminado un registro";
                    break;
                default:
                    Mensaje = "Un cambio desconosido a ocurrido";
                    break;
            }

            return Mensaje;
        }
    }
}
