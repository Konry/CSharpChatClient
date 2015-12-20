using CSharpChatClient.controller;
using CSharpChatClient.Controller;
using System;
using System.IO;

namespace CSharpChatClient
{
    /// <summary>
    /// The FileSerice handles all file operations instead of the Logger. 
    /// 
    /// </summary>
    public class FileService
    {
        private ProgramController programController;

        public FileService(ProgramController programController)
        {
            this.programController = programController;
        }

        private void InitializeUserID()
        {
            if (!HasUserCfgFile())
            {
                WriteUserIDFile(true);
            }
        }

        /// <summary>
        /// Writes the user in the file, if it is a new user, than create a new id.
        /// </summary>
        /// <param name="newUser">if this is a new user -> generate a new ID</param>
        private void WriteUserIDFile(bool newUser = false)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter("user.cfg"))
                {
                    sw.WriteLine(Configuration.localUser.Name);
                    if (newUser)
                    {
                        long id = User.GenerateUserID();
                        Configuration.localUser.Id = id;
                        sw.WriteLine(id);
                    }
                    else
                    {
                        sw.WriteLine(Configuration.localUser.Id);
                    }
                    sw.Close();
                }
            }
            catch (IOException ex)
            {
                Logger.LogException("Writing is not possible, please check your permissions.", ex);
            }
            catch (Exception ex)
            {
                Logger.LogException("File could not be written by the application.", ex);
            }
        }

        /// <summary>
        /// Reads the configuration file, catches all errors and returns a null user
        /// </summary>
        /// <returns>The user which read out the file. Occuring error returning a null Object.</returns>
        internal User ReadUserCfgFile()
        {
            try
            {
                using (StreamReader sr = new StreamReader("user.cfg"))
                {
                    // Read the stream to a string, and write the string to the console.
                    String line = sr.ReadToEnd();
                    if (line.Length > 0)
                    {
                        String[] split = line.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

                        long id = long.Parse(split[1]);
                        User user = new User(split[0]);
                        user.Id = id;
                        sr.Close();
                        return user;
                    }
                    sr.Close();
                }
            }
            catch (FileNotFoundException ex)
            {
                Logger.LogException("File could not be found by the application.", ex);
            }
            catch (IOException ex)
            {
                Logger.LogException("File could not be read by the application.", ex);
            }
            catch (Exception ex)
            {
                Logger.LogException("File could not be read by the application.", ex);
            }
            return null;
        }

        /// <summary>
        /// Function checks if there is a usernamefile existing.
        /// </summary>
        /// <returns></returns>
        internal bool HasCurrentUserFile()
        {
            return HasUserCfgFile();
        }

        /// <summary>
        /// Checks if there is an existing and readable user.cfg file
        /// </summary>
        /// <returns></returns>
        private bool HasUserCfgFile()
        {
            if (ReadUserCfgFile() != null)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Update the user in the file.
        /// </summary>
        internal void UpdateUserName()
        {
            WriteUserIDFile(false);
        }

    }
}