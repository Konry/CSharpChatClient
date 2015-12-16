﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSharpChatClient.controller;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;

namespace CSharpChatClient
{
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
                WriteUserIDFile();
            }
        }


        //private void RenameUserHistory()
        //{
        //    for()
        //}

        private void WriteUserIDFile()
        {
            try {
                long id = GenerateUserID();
                Configuration.localUser.id = id;
                using (StreamWriter sw = new StreamWriter("user.cfg"))
                {
                    sw.WriteLine(Configuration.localUser.name);
                    sw.WriteLine(id);
                    sw.Close();
                }
            }catch(IOException ex)
            {
                Debug.WriteLine("Schreiben nicht möglich, bitte prüfen Sie ihre Rechte! " + ex.StackTrace);
            } catch (ObjectDisposedException ex)
            {
                Debug.WriteLine("Datei konnte aufgrund eines internen Fehlers nicht geschrieben werden! " + ex.StackTrace);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unbekannte Exeption wurde gefangen! "+ex.StackTrace);
            }
        }

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
                        Debug.WriteLine(id);
                        User user = new User(split[0]);
                        user.id = id;
                        sr.Close();
                        return user;
                    }
                    sr.Close();
                }
            }
            catch (FileNotFoundException ex)
            {
                Debug.WriteLine("Datei konnte nicht gelesen werden! " + ex.StackTrace);
            }
            catch (IOException ex)
            {
                Debug.WriteLine("Schreiben nicht möglich, bitte prüfen Sie ihre Rechte! " + ex.StackTrace);
            }
            catch (ObjectDisposedException ex)
            {
                Debug.WriteLine("Datei konnte aufgrund eines internen Fehlers nicht geschrieben werden! " + ex.StackTrace);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unbekannte Exeption wurde gefangen! " + ex.StackTrace);
            }
            return null;
        }

        internal bool HasCurrentUserName()
        {
            return HasUserCfgFile();
        }

        private bool HasUserCfgFile()
        {
            if(ReadUserCfgFile() != null)
            {
                return true;
            }
            return false;
        }

        internal void UpdateUserName()
        {
            WriteUserIDFile();
        }

        private static long GenerateUserID()
        {
            Random random = new Random();
            byte[] buffer = new byte[8];
            random.NextBytes(buffer);
            return BitConverter.ToInt64(buffer, 0);
        }


        //private bool CreateHistoryFilesDirectory()
        //{
        //    string path = Application.ExecutablePath()+"history";
        //    if (Directory.Exists(path))
        //    {
        //        return true;
        //    } else
        //    {
        //        DirectoryInfo di = Directory.CreateDirectory(path);
        //        Debug.WriteLine("The directory was created successfully at {0}.", Directory.GetCreationTime(path));
        //    }
        //}

        //internal Message[] ReadHistoryFile()
        //{
        //    throw new NotImplementedException();
        //}

        //internal Message[] WriteHistoryFile()
        //{
        //    try
        //    {
        //        long id = GenerateUserID();
        //        Configuration.localUser.id = id;
        //        using (StreamWriter sw = new StreamWriter("history"Configuration.localUser.name + ".cfg"))
        //        {
        //            sw.WriteLine(Configuration.localUser.name);
        //            sw.WriteLine(id);
        //            sw.Close();
        //        }
        //    }
        //    catch (IOException ex)
        //    {
        //        Debug.WriteLine("Schreiben nicht möglich, bitte prüfen Sie ihre Rechte! " + ex.StackTrace);
        //    }
        //    catch (ObjectDisposedException ex)
        //    {
        //        Debug.WriteLine("Datei konnte aufgrund eines internen Fehlers nicht geschrieben werden! " + ex.StackTrace);
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine("Unbekannte Exeption wurde gefangen! " + ex.StackTrace);
        //    }
        //}
    }
}