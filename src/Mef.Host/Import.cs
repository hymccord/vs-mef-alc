using System.ComponentModel.Composition;

using Mef.Contracts;

using Microsoft.VisualStudio.Composition;

namespace Mef.Host
{
    [Export(typeof(IImport))]
    public class Import : IImport { }

}
