using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace FMLeadRouter
{
    [RunInstaller(true)]
    public class ProgramInstaller : Installer
    {
        private ServiceProcessInstaller process;
        private ServiceInstaller service;
        public ProgramInstaller()
        {
            process = new ServiceProcessInstaller();
            process.Account = ServiceAccount.LocalService;

            service = new ServiceInstaller();
            service.ServiceName = Program.ThisServiceName;

            service.StartType = ServiceStartMode.Automatic;
            service.Description = "FM Routing of Leads";


            Installers.Add(process);
            Installers.Add(service);
        }
    }
}
