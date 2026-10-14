using CivicConnect.App.Forms;
using CivicConnect.Auth;
using CivicConnect.Requests;
using CivicConnect.Notifications;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CivicConnect.AuditLog;
namespace CivicConnect.App;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();

        // Config sources, later ones win: appsettings.json (gitignored) -> environment variables
        // -> user-secrets. Keep the real DB password in user-secrets or an environment variable
        // (ConnectionStrings__CivicConnect), never in a committed file.
        // For a non-local database add "SSL Mode=Require" to the connection string.
        var builder = Host.CreateApplicationBuilder();
        builder.Configuration.AddUserSecrets(typeof(Program).Assembly, optional: true);

        var connectionString = builder.Configuration.GetConnectionString("CivicConnect");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            MessageBox.Show(
                "No database connection string found.\n\nCopy appsettings.example.json to appsettings.json " +
                "(or use user-secrets) and set ConnectionStrings:CivicConnect.",
                "CivicConnect", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        builder.Services.AddNotificationsModule();
        builder.Services.AddMessageBoxNotification();
        builder.Services.AddAuthModule(connectionString);
        builder.Services.AddRequestsModule(connectionString);   // needs the Auth module registered first
        builder.Services.AddAuditLogModule(connectionString);
        builder.Services.AddSingleton<IFormFactory, FormFactory>();
        // Other modules register here later, e.g. builder.Services.AddRequestsModule(connectionString);

        // Forms are created on demand with ActivatorUtilities, so the container does not keep
        // every closed form alive. Nobody should resolve them as plain services.
        builder.Services.AddSingleton<Func<RegisterForm>>(
            sp => () => ActivatorUtilities.CreateInstance<RegisterForm>(sp));

        using var host = builder.Build();

        using var login = ActivatorUtilities.CreateInstance<LoginForm>(host.Services);
        if (login.ShowDialog() != DialogResult.OK) return;

        // PLACEHOLDER until the main window exists: replace with the role-based main form.
        //var user = host.Services.GetRequiredService<ISessionContext>().CurrentUser!;
        //MessageBox.Show($"Signed in as {user.Name} ({user.Role}).\n\nThe main window has not been built yet.",
        //"CivicConnect", MessageBoxButtons.OK, MessageBoxIcon.Information);
        using var main = ActivatorUtilities.CreateInstance<MainForm>(host.Services);
        Application.Run(main);
    }
}
