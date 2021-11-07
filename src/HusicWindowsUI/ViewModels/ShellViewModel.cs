using Caliburn.Micro;
using Husic.Engine.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Husic.Windows.ViewModels
{
   internal class ShellViewModel : Conductor<object>
   {
      #region Private
      private readonly DashboardViewModel _Dashboard;
      #endregion
      public ShellViewModel(DashboardViewModel dashboard, IDataAccess da)
      {
         _Dashboard = dashboard;

         ActivateItemAsync(_Dashboard);

      }

   }
}
