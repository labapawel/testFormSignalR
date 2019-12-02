using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.Hosting;
using Microsoft.Owin.StaticFiles;
using Owin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;
using System.Windows.Forms;

namespace TestFormSignalR
{
    public partial class Form1 : Form
    {
        private IDisposable SignalR { get; set; }
        const string ServerURI = "http://localhost:8080";


        public Form1()
        {
            InitializeComponent();
        }

        internal void WriteToConsole(String message)
        {
        }

        private void StartServer()
        {
            try
            {
                SignalR = WebApp.Start(ServerURI);
            }
            catch (TargetInvocationException)
            {
                WriteToConsole("Server failed to start. A server is already running on " + ServerURI);
                //Re-enable button to let user try to start server again 
           //     this.Invoke((Action)(() => ButtonStart.Enabled = true));
                return;
            }
         //   this.Invoke((Action)(() => ButtonStop.Enabled = true));
            WriteToConsole("Server started at " + ServerURI);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            webBrowser1.Navigate(ServerURI);
            Task.Run(() => StartServer());

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (SignalR != null)
            {
                SignalR.Dispose();
            }
        }
    }

    /// <summary> 
    /// Used by OWIN's startup process.  
    /// </summary> 
    class Startup
    {

        private FileServerOptions ConfigureFileSystem(IAppBuilder appBuilder)
        {
            PhysicalFileSystem physicalFileSystem = new PhysicalFileSystem(@".\wwwroot");
            FileServerOptions fileOptions = new FileServerOptions();

            fileOptions.EnableDefaultFiles = true;
            fileOptions.RequestPath = PathString.Empty;
            fileOptions.FileSystem = physicalFileSystem;
            fileOptions.DefaultFilesOptions.DefaultFileNames = new[] { "index.html" };
            fileOptions.StaticFileOptions.FileSystem = fileOptions.FileSystem = physicalFileSystem;
            fileOptions.StaticFileOptions.ServeUnknownFileTypes = true;

            return fileOptions;
        }

        public void Configuration(IAppBuilder app)
        {
            FileServerOptions fileServerOptions = this.ConfigureFileSystem(app);
            app.UseCors(CorsOptions.AllowAll);
            app.UseFileServer(fileServerOptions);
            app.MapSignalR();
        }
    }
    /// <summary> 
    /// Echoes messages sent using the Send message by calling the 
    /// addMessage method on the client. Also reports to the console 
    /// when clients connect and disconnect. 
    /// </summary> 
    public class MyHub : Hub
    {
        public void Send(string name, string message)
        {
            Clients.All.addMessage(name, message);
        }
        public override Task OnConnected()
        {
      //      Program.MainForm.WriteToConsole("Client connected: " + Context.ConnectionId);
            return base.OnConnected();
        }
    /*    public override Task OnDisconnected()
        {
            Program.MainForm.WriteToConsole("Client disconnected: " + Context.ConnectionId);
            return base.OnDisconnected();
        }*/
    }

}
