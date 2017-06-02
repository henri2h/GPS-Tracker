using System;
using System.AppCore;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.ExtendedExecution;

namespace Gps_tracker
{
    public class ExtendedSession
    {
        public static bool ExtendedSessionActive
        {
            get
            {
                if (session != null)
                {
                    return true;
                }
                return false;
            }
        }
        static ExtendedExecutionSession session;
        public static async void StartLocationExtensionSession()
        {
            if (session == null)
            {
                session = new ExtendedExecutionSession()
                {
                    Description = "Location Tracker",
                    Reason = ExtendedExecutionReason.LocationTracking
                };
                session.Revoked += ExtendedExecutionSession_Revoked;

                var result = await session.RequestExtensionAsync();
                if (result == ExtendedExecutionResult.Denied)
                {
                    MainPage.MessageBox("error on creating the extended session");
                    Console.WriteLine("Error on creationg the extended session");
                }
                else
                {

                    System.Diagnostics.Debug.WriteLine("Extended session succesfuly created");
                    Console.WriteLine("Extended session succesfuly created");
                }
            }

        }

        static void ExtendedExecutionSession_Revoked(object sender, ExtendedExecutionRevokedEventArgs args)
        //ExtendedExecutionSession sender, ExtensionRevokedEventArgs args)
        {
            MainPage.MessageBox("Extended session revoked");
            Console.WriteLine("Extended session revoked");
            //TODO: clean up session data
            StopLocationExtensionSession();
            Console.WriteLine("to relaunch the extended session, type exRelaunch");
        }

        static void StopLocationExtensionSession()
        {
            MainPage.MessageBox("Extended session stoped");
            //reinitialisze the session
            if (session != null)
            {
                session.Dispose();
                session = null;
            }
            System.Diagnostics.Debug.WriteLine("Extended session stoped");
            Console.WriteLine("Extended session stoped");
        }

    }
}
