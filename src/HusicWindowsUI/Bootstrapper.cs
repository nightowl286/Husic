using Caliburn.Micro;
using Husic.Standard.DataAccess;
using Husic.Standard.Playback;
using Husic.Windows.Playback;
using Husic.Windows.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Husic.Windows
{
   internal class Bootstrapper : BootstrapperBase
   {
      #region Private
      private SimpleContainer _Container;
      #endregion

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
      public Bootstrapper()
      {
         Initialize();
      }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

      #region Overrides
      protected override void Configure()
      {
         _Container = new SimpleContainer();

         _Container.Singleton<IWindowManager, WindowManager>()
            .Singleton<IEventAggregator, EventAggregator>()
            .Singleton<SimpleContainer>()
            .Singleton<IHusicPlayer, HusicPlayer>()
            .Singleton<IDataAccess, DataAccess.DataAccess>();

         _Container.PerRequest<IPlayer, WindowsPlayer>()
            .PerRequest<ShellViewModel>()
            .PerRequest<DashboardViewModel>();
      }
      protected override void OnStartup(object sender, StartupEventArgs e)
      {
         IDataAccess data = _Container.GetInstance<IDataAccess>();
         data.EnsureTables();

         DisplayRootViewFor<ShellViewModel>();
      }
      protected override IEnumerable<Assembly> SelectAssemblies() => new[] { Assembly.GetExecutingAssembly() };
      #endregion

      #region Overrides for container
      protected override object GetInstance(Type service, string key) => _Container.GetInstance(service, key);
      protected override IEnumerable<object> GetAllInstances(Type service) => _Container.GetAllInstances(service);
      protected override void BuildUp(object instance) => _Container.BuildUp(instance);
      #endregion
   }
}
