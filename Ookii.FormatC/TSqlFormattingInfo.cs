// Copyright © Sven Groot (Ookii.org)
// BSD license; see license.txt for details.
using System;
using System.Collections.Generic;
using System.Text;

namespace Ookii.FormatC
{
    /// <summary>
    /// Provides formatting information for Transact-SQL scripts.
    /// </summary>
    /// <threadsafety static="true" instance="true" />
    public class TSqlFormattingInfo : IFormattingInfo
    {
        private static readonly CodeElement[] _patterns = new CodeElement[] {
                                  new CodeElement("comment",  @"(/\*(.|\n)*?\*/)|(--.*?$)"),
                                  new CodeElement("sqlString", @"N?'.*?'"),
                                  new CodeElement("escapedIdentifier", @"\[.*?\]|"".*"""),
                                  new CodeElement("keyword", new string[] {"absolute", "action", "ada", "add", "admin", "after", "aggregate",
				                        "alias", "allocate", "alter", "are", "array", "as", "asc",
				                        "assertion", "at", "authorization", "avg", "backup", "before", "begin",
				                        "binary", "bit", "bit_length", "blob", "boolean", "both", "breadth",
				                        "break", "browse", "bulk", "by", "call", "cascade", "cascaded", "case", "cast",
				                        "catalog", "char", "char_length", "character", "character_length",
				                        "check", "checkpoint", "class", "clob", "close", "clustered", "coalesce",
				                        "collate", "collation", "column", "commit", "completion", "compute",
				                        "connect", "connection", "constraint", "constraints", "constructor",
				                        "contains", "containstable", "continue", "convert", "corresponding",
				                        "count", "create", "cross", "cube", "current", "current_date", "current_path",
				                        "current_role", "current_time", "current_timestamp", "current_user",
				                        "cursor", "cycle", "data", "database", "date", "day", "dbcc", "deallocate", "dec",
				                        "decimal", "declare", "default", "deferrable", "deferred", "delete", "deny",
				                        "depth", "deref", "desc", "describe", "descriptor", "destroy", "destructor",
				                        "deterministic", "diagnostics", "dictionary", "disconnect", "disk",
				                        "distinct", "distributed", "domain", "double", "drop", "dummy", "dump",
				                        "dynamic", "each", "else", "end", "end-exec", "equals", "errlvl", "escape",
				                        "every", "except", "exception", "exec", "execute", "exit", "external",
				                        "extract", "false", "fetch", "file", "fillfactor", "first", "float", "for",
				                        "foreign", "fortran", "found", "free", "freetext", "freetexttable", "from",
				                        "full", "function", "general", "get", "global", "go", "goto", "grant", "group",
				                        "grouping", "having", "holdlock", "host", "hour", "identity", "identity_insert",
				                        "identitycol", "if", "ignore", "immediate", "include", "index", "indicator",
				                        "initialize", "initially", "inner", "inout", "input", "insensitive", "insert",
				                        "int", "integer", "intersect", "interval", "into", "is", "isolation", "iterate",
				                        "join", "key", "kill", "language", "large", "last", "lateral", "leading", "left",
				                        "less", "level", "limit", "lineno", "load", "local", "localtime", "localtimestamp",
				                        "locator", "lower", "map", "match", "max", "min", "minute", "modifies", "modify",
				                        "module", "month", "names", "national", "natural", "nchar", "nclob", "new", "next",
				                        "no", "nocheck", "nonclustered", "none", "null", "nullif", "numeric", "object",
				                        "octet_length", "of", "off", "offsets", "old", "on", "only", "open", "opendatasource",
				                        "openquery", "openrowset", "openxml", "operation", "option", "order",
				                        "ordinality", "out", "outer", "output", "over", "overlaps", "pad", "parameter",
				                        "parameters", "partial", "pascal", "path", "percent", "plan", "position",
				                        "postfix", "precision", "prefix", "preorder", "prepare", "preserve",
				                        "primary", "print", "prior", "privileges", "proc", "procedure",
				                        "public", "raiserror", "read", "reads", "readtext", "real", "reconfigure",
				                        "recursive", "ref", "references", "referencing", "relative", "replication",
				                        "restore", "restrict", "result", "return", "returns", "revoke", "right", "role",
				                        "rollback", "rollup", "routine", "row", "rowcount", "rowguidcol", "rows", "rule",
				                        "save", "savepoint", "schema", "scope", "scroll", "search", "second", "section",
				                        "select", "sequence", "session", "session_user", "set", "sets", "setuser",
				                        "shutdown", "size", "smallint", "space", "specific", "specifictype",
				                        "sql", "sqlca", "sqlcode", "sqlerror", "sqlexception", "sqlstate", "sqlwarning",
				                        "start", "state", "statement", "static", "statistics", "structure", "substring",
				                        "sum", "system_user", "table", "temporary", "terminate", "textsize", "than", "then",
				                        "time", "timestamp", "timezone_hour", "timezone_minute", "to", "top", "trailing",
				                        "tran", "transaction", "translate", "translation", "treat", "trigger", "trim",
				                        "true", "truncate", "tsequal", "under", "union", "unique", "unknown", "unnest",
				                        "update", "updatetext", "upper", "usage", "use", "user", "using", "value", "values",
				                        "varchar", "variable", "varying", "view", "waitfor", "when", "whenever", "where",
				                        "while", "with", "without", "work", "write", "writetext", "year", "zone"}),
                                    new CodeElement("sqlSystemFunction", new string[] { "@@CONNECTIONS", "@@CPU_BUSY", "@@CURSOR_ROWS", 
                                        "@@DATEFIRST", "@@DBTS", "@@ERROR", "@@FETCH_STATUS", "@@IDENTITY", "@@IDLE", "@@IO_BUSY", 
                                        "@@LANGID", "@@LANGUAGE", "@@LOCK_TIMEOUT", "@@MAX_CONNECTIONS", "@@MAX_PRECISION", 
                                        "@@NESTLEVEL", "@@OPTIONS", "@@PACK_RECEIVED", "@@PACK_SENT", "@@PACKET_ERRORS", "@@PROCID", 
                                        "@@REMSERVER", "@@ROWCOUNT", "@@SERVERNAME", "@@SERVICENAME", "@@SPID", "@@TEXTSIZE", 
                                        "@@TIMETICKS", "@@TOTAL_ERRORS", "@@TOTAL_READ", "@@TOTAL_WRITE", "@@TRANCOUNT", "@@VERSION" }),
                                    new CodeElement("sqlOperator", new string[] { @"\+", "-", "/", @"\*", "%", "=", "&", @"\|", @"\^", "<=",
                                        ">=", "<>", "<", ">", "!=", "!<", "!>", "all", "and", "any", "between", "exists", "in", "like",
                                        "not", "or", "some", "~", @"\(", @"\)", ";", @"\.", "," })
                              };

        /// <summary>
        /// Initializes a new instance of the <see cref="TSqlFormattingInfo"/> class.
        /// </summary>
        public TSqlFormattingInfo()
        {
        }


        #region IFormattingInfo Members

        /// <summary>
        /// Gets a list of regular expression patterns used to identify elements of the code.
        /// </summary>
        /// <value>
        /// A list of <see cref="CodeElement"/> classes that provide regular expressions for identifying elements of the code.
        /// </value>
        public IEnumerable<CodeElement> Patterns
        {
            get { return _patterns; }
        }

        /// <summary>
        /// Gets a value that indicates whether the language to be formatted is case sensitive.
        /// </summary>
        /// <value>
        /// Returns <see langword="false" />.
        /// </value>
        public bool CaseSensitive
        {
            get { return false; }
        }

        #endregion
    }
}
