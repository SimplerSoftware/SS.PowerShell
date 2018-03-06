using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Schema;

namespace SS.PowerShell
{
    /// <summary>
    /// <para type="description">Result object that is returned from schema test.</para>
    /// </summary>
    public class SchemaTestResult
    {
        public bool Valid { get; internal set; }
        public IList<ValidationError> Errors { get; internal set; }
    }
}
