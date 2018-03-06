using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace SS.PowerShell
{
    /// <summary>
    /// Determines whether the JSON file is valid.
    /// <para type="synopsis">Determines whether the JSON file is valid.</para>
    /// <para type="description">Determines whether the JSON file is valid.</para>
    /// <example>
    ///   <title>Default usage. </title>
    ///   <code>Test-JsonSchema schema.json source.json</code>
    /// </example>
    /// <example>
    ///   <title>Specifying the text encoding type for the files. </title>
    ///   <code>Test-JsonSchema schema.json source.json UTF8</code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsDiagnostic.Test, "JsonSchema")]
    [OutputType(typeof(SchemaTestResult))]
    public class TestJsonSchemaCmdlet : Cmdlet
    {
        /// <summary>
        /// The JSON schema file to test against.
        /// <para type="description">The JSON schema file to test against.</para>
        /// </summary>
        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The JSON schema file to test against.")]
        [ValidateNotNullOrEmpty]
        public string SchemaFile { get; set; }

        /// <summary>
        /// The source JSON file to test.
        /// <para type="description">The source JSON file to test.</para>
        /// </summary>
        [Parameter(Position = 1, Mandatory = true, HelpMessage = "The source JSON file to test.")]
        [ValidateNotNullOrEmpty]
        public string JsonFile { get; set; }

        /// <summary>
        /// Encoding type to use when reading schema and JSON file.
        /// <para type="description">Encoding type to use when reading schema and JSON file.</para>
        /// </summary>
        [Parameter(Position = 2, HelpMessage = "Encoding type to use when reading schema and JSON file.")]
        public EncodingType Encoding { get; set; } = EncodingType.Default;

        //[Parameter(Position = 3, HelpMessage = "Return errors as detailed objects instead of strings.")]
        //public SwitchParameter ErrorAsObject { get; set; }

        private bool _Continue = true;

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            if (!File.Exists(this.SchemaFile))
            {
                WriteError(new ErrorRecord(new FileNotFoundException("Schema file not found!", this.SchemaFile), "SchemaExists,SS.PowerShell.TestJsonSchemaCmdlet", ErrorCategory.ObjectNotFound, this.SchemaFile));
                _Continue = false;
            }
            if (!File.Exists(this.JsonFile))
            {
                WriteError(new ErrorRecord(new FileNotFoundException("JSON file not found!", this.JsonFile), "JsonExists,SS.PowerShell.TestJsonSchemaCmdlet", ErrorCategory.ObjectNotFound, this.JsonFile));
                _Continue = false;
            }
        }

        protected override void ProcessRecord()
        {
            WriteDebug(string.Format("Continue: {0}", _Continue));
            if (!_Continue)
            {
                return;
            }

            Encoding enc = null;
            switch (this.Encoding)
            {
                case EncodingType.ASCII:
                    WriteVerbose("Using ASCII encoding.");
                    enc = System.Text.Encoding.ASCII;
                    break;
                case EncodingType.BigEndianUnicode:
                    WriteVerbose("Using Big Endian Unicode encoding.");
                    enc = System.Text.Encoding.BigEndianUnicode;
                    break;
                case EncodingType.Unicode:
                    WriteVerbose("Using Unicode encoding.");
                    enc = System.Text.Encoding.Unicode;
                    break;
                case EncodingType.UTF32:
                    WriteVerbose("Using UTF32 encoding.");
                    enc = System.Text.Encoding.UTF32;
                    break;
                case EncodingType.UTF7:
                    WriteVerbose("Using UTF7 encoding.");
                    enc = System.Text.Encoding.UTF7;
                    break;
                case EncodingType.UTF8:
                    WriteVerbose("Using UTF8 encoding.");
                    enc = System.Text.Encoding.UTF8;
                    break;
                case EncodingType.Default:
                default:
                    enc = System.Text.Encoding.Default;
                    WriteVerbose(string.Format("Using System's Default encoding. ({0})", enc.EncodingName));
                    break;
            }

            WriteVerbose("Reading Schema File.");
            string sSchema = File.ReadAllText(this.SchemaFile, enc);
            WriteVerbose("Reading JSON File.");
            string sJson = File.ReadAllText(this.JsonFile, enc);

            // load schema
            WriteVerbose("Parsing Schema File.");
            var schema = JSchema.Parse(sSchema);
            WriteVerbose("Parsing JSON File.");
            var json = JObject.Parse(sJson);

            IList<ValidationError>  errors = new List<ValidationError>();
            //if (this.ErrorAsObject) {
            //    errors = new List<ValidationError>();
            //} else {
            //    errors = new List<string>();
            //}

            // validate JSON
            WriteVerbose("Testing JSON file against schema.");
            var valid = SchemaExtensions.IsValid(json, schema, out errors);
            
            // return error messages and line info
            WriteObject(new SchemaTestResult(){
                Valid = valid,
                Errors = valid ? null : errors
            });

            base.ProcessRecord();
        }
    }
}
