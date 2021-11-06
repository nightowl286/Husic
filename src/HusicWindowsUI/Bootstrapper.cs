using Caliburn.Micro;
using Husic.Windows.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Husic.Windows
{
   internal class Bootstrapper : BootstrapperBase
   {
      public Bootstrapper()
      {
         Initialize();
      }

      #region Overrides
      protected override void OnStartup(object sender, StartupEventArgs e)
      {
         DisplayRootViewFor<ShellViewModel>();
      }
      #endregion
   }
}
