using Husic.Standard.DataAccess.Scripts;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;

namespace Husic.DataAccess.Scripts
{
   public class AssemblyScriptContainer : IScriptContainer
   {
      #region Fields
      public readonly Assembly _Assembly;
      public readonly string? _AssemblyName;
      public readonly string _NameFormat;
      private readonly string _Prefix;
      #endregion
      public AssemblyScriptContainer([DisallowNull] Assembly assembly, [DisallowNull] string nameFormat)
      {
         _Assembly = assembly;
         _NameFormat = nameFormat;
         _AssemblyName = _Assembly.GetName().Name;
         _Prefix = _AssemblyName == null ? string.Empty : _AssemblyName + '.';
      }

      #region Methods
      private string GetFullName([DisallowNull] string key) => string.Concat(_Prefix, string.Format(_NameFormat, key));
      public string GetScript([DisallowNull] string key)
      {
         string full = GetFullName(key);
         using Stream resource = _Assembly.GetManifestResourceStream(full) ?? throw new ArgumentException($"Could not find a script with the full name {full}", nameof(key));
         using StreamReader sr = new StreamReader(resource);

         string script = sr.ReadToEnd();

         return script;
      }
      #endregion
   }
}
