using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SS.PowerShell
{
    public class JSchemaUrlResolverEvent : JSchemaUrlResolver
    {
        public delegate void SchemaResolvingEventHandler(object sender, SchemaResolvingEventArgs e);
        public event SchemaResolvingEventHandler SchemaResolving;

        public override SchemaReference ResolveSchemaReference(ResolveSchemaContext context)
        {
            if (SchemaResolving != null)
                SchemaResolving(this, new SchemaResolvingEventArgs() {
                    Context = context
                });

            return base.ResolveSchemaReference(context);
        }
    }

    public class SchemaResolvingEventArgs : EventArgs
    {
        public ResolveSchemaContext Context { get; internal set; }
    }
}
