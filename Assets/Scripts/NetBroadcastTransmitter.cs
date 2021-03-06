﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace ITDmProject
{
    public class NetBroadcastTransmitter : NetworkDiscovery //desktop part
    {
        private GlobalControllerDesktop Global;
        // Use this for initialization
        void Start()
        {
            Global = FindObjectOfType<GlobalControllerDesktop>();
			showGUI = false;
            Application.runInBackground = true;
            StartServer();
        }

        //Call to create a server
        public void StartServer()
        {
            int serverPort = CreateServer();
            if (serverPort != -1)
            {
                try
                {
                    Debug.Log("Server created on port : " + serverPort);
                    broadcastData = serverPort.ToString() + "#" + Global.ServerName;
                    Initialize();
                    StartAsServer();
                    Global.RunServerMessanger();
                }
                catch (Exception)
                {
					Debug.Log("Failed to run Server");
				}
            }
            else
            {
                Debug.Log("Failed to create Server");
            }
        }

        private const int minPort = 10000;
        private const int maxPort = 10010;
        private const int defaultPort = 10000;

        //Creates a server then returns the port the server is created with. Returns -1 if server is not created
        private int CreateServer()
        {
            int serverPort = -1;
            //Connect to default port
            bool serverCreated = NetworkServer.Listen(defaultPort);
            if (serverCreated)
            {
                serverPort = defaultPort;
                Debug.Log("Server Created with deafault port");
            }
            else
            {
                Debug.Log("Failed to create with the default port");
                //Try to create server with other port from min to max except the default port which we trid already
                for (int tempPort = minPort; tempPort <= maxPort; tempPort++)
                {
                    //Skip the default port since we have already tried it
                    if (tempPort != defaultPort)
                    {
                        //Exit loop if successfully create a server
                        if (NetworkServer.Listen(tempPort))
                        {
                            serverPort = tempPort;
                            break;
                        }

                        //If this is the max port and server is not still created, show, failed to create server error
                        if (tempPort == maxPort)
                        {
                            Debug.LogError("Failed to create server");
                        }
                    }
                }
            }
            return serverPort;
        }
        public void Down()
        {
            NetworkServer.Shutdown();
        }
    }
}
