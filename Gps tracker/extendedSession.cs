using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.ExtendedExecution;

namespace Gps_tracker
{
    public class extendedSession
    {
        ExtendedExecutionSession session;
        public async void StartLocationExtensionSession()
        {
            session = new ExtendedExecutionSession();
            session.Description = "Location Tracker";
            session.Reason = ExtendedExecutionReason.LocationTracking;
            session.Revoked += ExtendedExecutionSession_Revoked;

            var result = await session.RequestExtensionAsync();
            if (result == ExtendedExecutionResult.Denied)
            {
                MainPage.messageBox("error on creating the extended session");
                Console.WriteLine("Error on creationg the extended session");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Extended session succesfuly created");
                Console.WriteLine("Extended session succesfuly created");
            }

        }

        void ExtendedExecutionSession_Revoked(object sender, ExtendedExecutionRevokedEventArgs args)
        //ExtendedExecutionSession sender, ExtensionRevokedEventArgs args)
        {
            MainPage.messageBox("Extended session revoked");
            Console.WriteLine("Extended session revoked");
            //TODO: clean up session data
            StopLocationExtensionSession();

        }

        void StopLocationExtensionSession()
        {
            MainPage.messageBox("Extended session stoped");
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
