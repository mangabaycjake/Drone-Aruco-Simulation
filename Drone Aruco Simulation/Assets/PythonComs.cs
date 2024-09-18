using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Unity.VisualScripting;
using TMPro;

public class UnityServer : MonoBehaviour
{
    public int serverPort = 12345;

    private TcpListener listener;
    private TcpClient client;
    private NetworkStream stream;
    private byte[] receiveBuffer = new byte[1024];

    private bool connectedToPython;

    public Rigidbody rbDrone;
    TextMeshProUGUI txtConnected;

    private void Start()
    {
        StartServer();
        txtConnected = GameObject.Find("TxConnected").GetComponent<TextMeshProUGUI>();
    }

    private void StartServer()
    {
        listener = new TcpListener(IPAddress.Any, serverPort);
        listener.Start();
        Debug.Log("Unity server is listening on port " + serverPort);

        // Start listening for incoming connections in a separate thread
        System.Threading.Thread serverThread = new System.Threading.Thread(ListenForClients);
        serverThread.Start();
    }

    private void ListenForClients()
    {
        while (true)
        {
            client = listener.AcceptTcpClient();
            Debug.Log("Client connected: " + ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());

            stream = client.GetStream();

            // Start listening for incoming data in a separate thread
            System.Threading.Thread receiveThread = new System.Threading.Thread(ReceiveData);
            receiveThread.Start();
        }
    }

    private void ReceiveData()
    {
        while (true)
        {
            int bytesRead = stream.Read(receiveBuffer, 0, receiveBuffer.Length);
            if (bytesRead > 0)
            {
                string receivedData = Encoding.UTF8.GetString(receiveBuffer, 0, bytesRead);
                Debug.Log("Received data from client: " + receivedData);
            }
        }
    }

    private void SendData(string data)
    {
        byte[] dataBytes = Encoding.UTF8.GetBytes(data);
        stream.Write(dataBytes, 0, dataBytes.Length);
        Debug.Log("Sent data to client: " + data);
    }

    
    private void Update()
    {
        try
        {
            SendData(rbDrone.transform.position.y.ToString());
            connectedToPython = true;
        }
        catch
        {
            connectedToPython = false;
        }
        if (connectedToPython )
        {
            txtConnected.text = "Python Status: Connected";
            txtConnected.color = Color.green;
        }
        else
        {
            txtConnected.text = "Python Status: Not Connected";
            txtConnected.color = Color.blue;
        }
        
        
    }
}
