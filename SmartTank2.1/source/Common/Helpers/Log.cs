using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;

namespace Common.Helpers
{
    /// <summary>
    /// Log will create automatically a log file and write
    /// log/error/debug info for simple runtime error checking, very useful
    /// for minor errors, such as finding not files.
    /// The application can still continue working, but this log provides
    /// an easy support to find out what files are missing (in this example).
    /// </summary>
    public static class Log
    {
        #region Variables
        /// <summary>
        /// Writer
        /// </summary>
        private static StreamWriter writer = null;

        /// <summary>
        /// Log filename
        /// </summary>
        private const string LogFilename = "Log.txt";
        #endregion

        static bool initialized = false;

        #region Static constructor to create log file
        /// <summary>
        /// Static constructor
        /// </summary>
        public static void Initialize ()
        {
            if (initialized && writer != null)
                return;

            initialized = true;

            try
            {
                // Open file
                FileStream file = File.Open( LogFilename, FileMode.OpenOrCreate | FileMode.Append, FileAccess.Write );
                //old: new FileStream(
                //    LogFilename, FileMode.OpenOrCreate,
                //    FileAccess.Write, FileShare.ReadWrite);

                // Associate writer with that, when writing to a new file,
                // make sure UTF-8 sign is written, else don't write it again!
                if (file.Length == 0)
                    writer = new StreamWriter( file,
                        System.Text.Encoding.UTF8 );
                else
                    writer = new StreamWriter( file );

                // Go to end of file
                writer.BaseStream.Seek( 0, SeekOrigin.End );

                // Enable auto flush (always be up to date when reading!)
                writer.AutoFlush = true;

                // Add some info about this session
                writer.WriteLine( "" );
                writer.WriteLine( "/// Session started at: " + DateTime.Now.ToString() );
                writer.WriteLine( "/// Common" );
                writer.WriteLine( "" );
            }
            catch (IOException)
            {
                // Ignore any file exceptions, if file is not
                // createable (e.g. on a CD-Rom) it doesn't matter.
                initialized = false;
            }
            catch (UnauthorizedAccessException)
            {
                // Ignore any file exceptions, if file is not
                // createable (e.g. on a CD-Rom) it doesn't matter.
                initialized = false;
            }
        }
        #endregion

        #region Write log entry
        /// <summary>
        /// Writes a LogType and info/error message string to the Log file
        /// </summary>
        static public void Write ( string message )
        {
            // Can't continue without valid writer
            if (writer == null)
                return;

            try
            {
                DateTime ct = DateTime.Now;
                string s = "[" + ct.Hour.ToString( "00" ) + ":" +
                    ct.Minute.ToString( "00" ) + ":" +
                    ct.Second.ToString( "00" ) + "] " +
                    message;
                writer.WriteLine( s );

#if DEBUG
                // In debug mode write that message to the console as well!
                System.Console.WriteLine( s );
#endif
            }
            catch (IOException)
            {
                // Ignore any file exceptions, if file is not
                // createable (e.g. on a CD-Rom) it doesn't matter.
            }
            catch (UnauthorizedAccessException)
            {
                // Ignore any file exceptions, if file is not
                // createable (e.g. on a CD-Rom) it doesn't matter.
            }
        }
        #endregion
    }
}
