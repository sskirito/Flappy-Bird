using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Security.Cryptography;

public static class ClientSocket {

    //设定服务器IP地址  
    public static IPAddress ip = IPAddress.Parse("127.0.0.1");
    static Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    public static string publicKey = "";
    private static byte[] result = new byte[1024];

    public static bool connnetionSuccess;

    public static bool isLoading;
    public static string receivedMessage;

    public static void Connect() {
        Thread todo;
        isLoading = true;
        Thread oThread = new Thread(delegate () {
            try {
                clientSocket.Connect(new IPEndPoint(ip, 8885)); //配置服务器IP与端口  
                todo = new Thread(ToDo);
                todo.Start("连接服务器成功");
                isLoading = false;
                connnetionSuccess = true;
                Send("hello");
                Receive();
            } catch {
                UnityEngine.Debug.Log("连接服务器失败，请按回车键退出！");
                isLoading = false;
                connnetionSuccess = false;

                return;
            }
        });
        oThread.Start();
    }
    public static void Send(string sendMessage) {
        if (!connnetionSuccess) {
            return;
        }

        Thread todo;
        Thread oThread = new Thread(delegate () {
            try {
                clientSocket.Send(Encoding.ASCII.GetBytes(sendMessage));
                todo = new Thread(ToDo);
                todo.Start("向服务器发送消息：" + sendMessage);
            } catch {
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
            }
        });
        //通过 clientSocket 发送数据  
        oThread.Start();
    }

    public static void EncryptSend(string msg) {
        RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
        rsa.FromXmlString(publicKey);
        byte[] ciphertext =
            rsa.Encrypt(System.Text.Encoding.UTF8.GetBytes(msg), false);

        if (!connnetionSuccess) {
            return;
        }

        Thread todo;
        Thread oThread = new Thread(delegate () {
            try {
                clientSocket.Send(ciphertext);
                todo = new Thread(ToDo);
                todo.Start("向服务器发送消息：" + Convert.ToBase64String(ciphertext));
            } catch {
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
            }
        });
        //通过 clientSocket 发送数据  
        oThread.Start();
    }

    public static void Receive() {
        if (!connnetionSuccess) {
            return;
        }
        isLoading = true;
        string message = "";
        Thread todo;
        Thread oThread = new Thread(delegate () {
            string msg = "";
            do {
                msg = Encoding.ASCII.GetString(result, 0, clientSocket.Receive(result));
                UnityEngine.Debug.Log("接收服务器消息：" + msg);
                todo = new Thread(ToDo);
                todo.Start(msg);
                message += msg;
            } while (msg == "");
            if (message.Split(' ')[0] == "RSA")
                publicKey = message.Split(' ')[1];
            isLoading = false;
            receivedMessage = message;

        });
        oThread.Start();
    }
    public static void ToDo(System.Object obj) {
        UnityEngine.Debug.Log(obj);
    }

    public static void Close() {
        connnetionSuccess = false;
        clientSocket.Close();
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    }

}
