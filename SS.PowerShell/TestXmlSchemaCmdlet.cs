using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace SS.PowerShell
{
    /// <summary>
    /// Determines whether the XML file is valid.
    /// <para type="synopsis">Determines whether the XML file is valid.</para>
    /// <para type="description">Determines whether the XML file is valid.</para>
    /// <example>
    ///   <title>Default usage. </title>
    ///   <code>Test-XmlSchema schema.xsd source.xml</code>
    /// </example>
    /// <example>
    ///   <title>Specifying the text encoding type for the files. </title>
    ///   <code>Test-XmlSchema schema.xsd source.xml UTF8</code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsDiagnostic.Test, "XmlSchema")]
    [OutputType(typeof(SchemaTestResult<Exception>))]
    public class TestXmlSchemaCmdlet : PSCmdlet
    {
        #region Parameters
        /// <summary>
        /// The XSD schema file to test against.
        /// <para type="description">The XSD schema file to test against.</para>
        /// </summary>
        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The XSD schema file to test against.")]
        [ValidateNotNullOrEmpty]
        public string SchemaFile { get; set; }

        /// <summary>
        /// The source XML file to test.
        /// <para type="description">The source XML file to test.</para>
        /// </summary>
        [Parameter(Position = 1, Mandatory = true, HelpMessage = "The source XML file to test.")]
        [ValidateNotNullOrEmpty]
        public string XmlFile { get; set; }

        /// <summary>
        /// Encoding type to use when reading schema and XML file.
        /// <para type="description">Encoding type to use when reading schema and XML file.</para>
        /// </summary>
        [Parameter(Position = 2, HelpMessage = "Encoding type to use when reading schema and XML file.")]
        public EncodingType Encoding { get; set; } = EncodingType.Default;

        //[Parameter(Position = 3, HelpMessage = "Return errors as detailed objects instead of strings.")]
        //public SwitchParameter ErrorAsObject { get; set; }
        #endregion

        private bool _Continue = true;
        private bool _valid = true;
        private string _SchemaFile;
        private string _XmlFile;
        private IList<Exception> _errors = new List<Exception>();

        protected override void BeginProcessing()
        {
            // Convert path to absolute path relative to calling script's/prompt's path
            this._SchemaFile = this.GetUnresolvedProviderPathFromPSPath(this.SchemaFile);
            WriteDebug(string.Format("SchemaFile: {0}", this._SchemaFile));
            // Convert path to absolute path relative to calling script's/prompt's path
            this._XmlFile = this.GetUnresolvedProviderPathFromPSPath(this.XmlFile);
            WriteDebug(string.Format("XmlFile: {0}", this._XmlFile));

            if (!File.Exists(this._SchemaFile))
            {
                WriteError(new ErrorRecord(new FileNotFoundException("Schema file not found!", this._SchemaFile), "SchemaExists,SS.PowerShell.TestXmlSchemaCmdlet", ErrorCategory.ObjectNotFound, this._SchemaFile));
                _Continue = false;
            }
            if (!File.Exists(this._XmlFile))
            {
                WriteError(new ErrorRecord(new FileNotFoundException("XML file not found!", this._XmlFile), "XmlExists,SS.PowerShell.TestXmlSchemaCmdlet", ErrorCategory.ObjectNotFound, this._XmlFile));
                _Continue = false;
            }
            base.BeginProcessing();
        }

        protected override void ProcessRecord()
        {
            WriteDebug(string.Format("Continue: {0}", _Continue));
            if (!_Continue)
            {
                WriteVerbose("Exiting process, previous error occurred.");
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
            using (StreamReader stmSchema = new StreamReader(this._SchemaFile, enc))
            {
                WriteVerbose("Reading XML File.");
                using (StreamReader stmXml = new StreamReader(this._XmlFile, enc))
                {

                    // load schema
                    WriteVerbose("Parsing Schema File.");
                    XmlSchemaSet schemas = new XmlSchemaSet();
                    schemas.Add("", XmlReader.Create(stmSchema));

                    WriteVerbose("Parsing XML File.");
                    XDocument xmlDoc = XDocument.Load(stmXml);

                    //if (this.ErrorAsObject) {
                    //    errors = new List<ValidationError>();
                    //} else {
                    //    errors = new List<string>();
                    //}

                    // validate XML
                    WriteVerbose("Testing XML file against schema.");
                    xmlDoc.Validate(schemas, ValidationXmlEvent);
                }
            }

            // return error messages and line info
            WriteObject(new SchemaTestResult<Exception>(){
                Valid = _valid,
                Errors = _valid ? null : _errors
            });

            base.ProcessRecord();
        }

        void ValidationXmlEvent(object sender, ValidationEventArgs e)
        {
            WriteVerbose(string.Format("{0}: {1}", e.Severity, e.Message));
            _errors.Add(e.Exception);
            _valid = false;
        }
    }
}
