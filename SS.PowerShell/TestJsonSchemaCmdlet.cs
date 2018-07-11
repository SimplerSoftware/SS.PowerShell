using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
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
    [Cmdlet(VerbsDiagnostic.Test, "JsonSchema", DefaultParameterSetName = "SchemaFile")]
    [OutputType(typeof(SchemaTestResult<ValidationError>))]
    public class TestJsonSchemaCmdlet : PSCmdlet
    {
        #region Parameters

        /// <summary>
        /// The JSON schema file should be read from the '$schema' parameter in the JSON file.
        /// <para type="description">The JSON schema file should be read from the '$schema' parameter in the JSON file.</para>
        /// </summary>
        [Parameter(HelpMessage = "The JSON schema file should be read from the '$schema' parameter in the JSON file.", ParameterSetName = "SchemaInJsonFile")]
        public SwitchParameter SchemaInJsonFile { get; set; }

        /// <summary>
        /// The JSON schema file to test against.
        /// <para type="description">The JSON schema file to test against.</para>
        /// </summary>
        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The JSON schema file to test against.", ParameterSetName = "SchemaFile")]
        [Parameter(Mandatory = false, DontShow = true, HelpMessage = "The source JSON file to test.", ParameterSetName = "SchemaInJsonFile")]
        //[Parameter(ParameterSetName = "One")]
        public string SchemaFile { get; set; }

        /// <summary>
        /// The source JSON file to test.
        /// <para type="description">The source JSON file to test.</para>
        /// </summary>
        [Parameter(Position = 1, Mandatory = true, HelpMessage = "The source JSON file to test.", ParameterSetName = "SchemaFile")]
        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The source JSON file to test.", ParameterSetName = "SchemaInJsonFile")]
        public string JsonFile { get; set; }

        /// <summary>
        /// Do not write warning messages when schema is resolving external referenced schema's.
        /// <para type="description">Do not write warning messages when schema is resolving external referenced schema's.</para>
        /// </summary>
        [Parameter(HelpMessage = "Do not write warning messages when schema is resolving external referenced schema's.")]
        public SwitchParameter IgnoreReferenceResolvingWarnings { get; set; }

        /// <summary>
        /// Encoding type to use when reading schema and JSON file.
        /// <para type="description">Encoding type to use when reading schema and JSON file.</para>
        /// </summary>
        [Parameter(Position = 2, HelpMessage = "Encoding type to use when reading schema and JSON file.")]
        public EncodingType Encoding { get; set; } = EncodingType.Default;

        //[Parameter(Position = 3, HelpMessage = "Return errors as detailed objects instead of strings.")]
        //public SwitchParameter ErrorAsObject { get; set; }
        #endregion

        private bool _Continue = true;
        private string _SchemaFile;
        private string _JsonFile;
        Regex pathType = new Regex(@"^(?<protocol>\w{3,5})|(?<drive>\w{1})\:(?:\\)?(?:/{2})?", RegexOptions.IgnoreCase);
        //Regex localType = new Regex(@"^(?:(?<drive>\w{1})\:(?:\\))|^(?<relativedir>\.{1,2}\\)|^(?<rootdir>\\)", RegexOptions.IgnoreCase);
        Regex remoteType = new Regex(@"^(?<protocol>\w{3,5})\:/{2}", RegexOptions.IgnoreCase);
        
        protected override void BeginProcessing()
        {
            WriteDebug(string.Format("JsonFile: {0}", this.JsonFile));
            if (!remoteType.IsMatch(this.JsonFile))
            {
                // Convert path to absolute path relative to calling script's/prompt's path
                this._JsonFile = this.GetUnresolvedProviderPathFromPSPath(this.JsonFile);
                if (!File.Exists(this._JsonFile))
                {
                    WriteError(new ErrorRecord(new FileNotFoundException("JSON file not found!", this._JsonFile), "JsonExists,SS.PowerShell.TestJsonSchemaCmdlet", ErrorCategory.ObjectNotFound, this._JsonFile));
                    _Continue = false;
                }
            }
            if (!this.SchemaInJsonFile)
            {
                WriteDebug(string.Format("SchemaFile: {0}", this.SchemaFile));
                if (!remoteType.IsMatch(this.SchemaFile))
                {
                    // Convert path to absolute path relative to calling script's/prompt's path
                    this._SchemaFile = this.GetUnresolvedProviderPathFromPSPath(this.SchemaFile);
                    if (!File.Exists(this._SchemaFile))
                    {
                        WriteError(new ErrorRecord(new FileNotFoundException("Schema file not found!", this._SchemaFile), "SchemaExists,SS.PowerShell.TestJsonSchemaCmdlet", ErrorCategory.ObjectNotFound, this._SchemaFile));
                        _Continue = false;
                    }
                }
                else if (remoteType.IsMatch(this.SchemaFile))
                {
                    this._SchemaFile = this.SchemaFile;
                }
                else
                {
                    WriteError(new ErrorRecord(new FileNotFoundException("Schema file not valid local or remote path!", this._SchemaFile), "SchemaValidPath,SS.PowerShell.TestJsonSchemaCmdlet", ErrorCategory.InvalidArgument, this._SchemaFile));
                    _Continue = false;
                }
            }
            base.BeginProcessing();
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

            JSchemaUrlResolverEvent resolver = new JSchemaUrlResolverEvent();
            resolver.SchemaResolving += Resolver_SchemaResolving;

            this._JsonFile = TransformPath(this._JsonFile);
            WriteVerbose("Reading JSON File.");
            string sJson = File.ReadAllText(this._JsonFile, enc);
            WriteVerbose("Parsing JSON File.");
            var json = JObject.Parse(sJson);

            string sSchema = string.Empty;
            if (this.SchemaInJsonFile)
            {
                WriteVerbose("Read schema file path from JSON file.");
                if (json["$schema"] == null || string.IsNullOrWhiteSpace(json["$schema"].Value<string>()))
                    throw new PSInvalidOperationException("No $schema property defined in JSON file.");

                this._SchemaFile = json["$schema"].Value<string>();
                WriteVerbose($"Found schema file path [{this._SchemaFile}] in JSON data file [{this._JsonFile}].");
            }

            this._SchemaFile = TransformPath(this._SchemaFile);
            WriteVerbose("Reading Schema File.");
            sSchema = File.ReadAllText(this._SchemaFile, enc);

            // load schema
            WriteVerbose("Parsing Schema File.");
            var schema = JSchema.Parse(sSchema, resolver);

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
            WriteObject(new SchemaTestResult<ValidationError>(){
                Valid = valid,
                Errors = valid ? null : errors
            });

            base.ProcessRecord();
        }

        private void Resolver_SchemaResolving(object sender, SchemaResolvingEventArgs e)
        {
            if (!IgnoreReferenceResolvingWarnings)
                WriteWarning($"Resolving external referenced schema [{e.Context.SchemaId}]");
        }

        private string TransformPath(string filePath)
        {
            filePath = filePath.Trim(' ');
            var mtch = pathType.Match(filePath);
            if (mtch.Success)
            {
                if (mtch.Groups["drive"].Success)
                    return filePath;
                else if (mtch.Groups["protocol"].Success)
                {
                    switch (mtch.Groups["protocol"].Value.ToLower())
                    {
                        case "http":
                        case "https":
                            var tmpHttpFile =Path.GetTempFileName();
                            WriteVerbose($"Downloading [{filePath}]");
                            using (var client = new WebClient())
                            {
                                client.DownloadFile(filePath, tmpHttpFile);
                            }
                            WriteVerbose($"Downloading of [{filePath}] completed");
                            return tmpHttpFile;
                        case "ftp":
                            var tmpFtpFile = Path.GetTempFileName();
                            WriteVerbose($"Downloading [{filePath}]");
                            // Get the object used to communicate with the server.
                            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(filePath);
                            request.Method = WebRequestMethods.Ftp.DownloadFile;
                            //request.Credentials = new NetworkCredential("anonymous", "janeDoe@contoso.com");
                            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                            {
                                using (Stream responseStream = response.GetResponseStream())
                                using (Stream fileStream = File.Create(tmpFtpFile))
                                {
                                    responseStream.CopyTo(fileStream);
                                }
                                WriteVerbose($"Downloading [{filePath}] completed");
                            }
                            return tmpFtpFile;
                    }
                }
            }
            throw new PSInvalidOperationException("Invalid file path. Only local drive and web locations are supported.");
        }
    }
}
