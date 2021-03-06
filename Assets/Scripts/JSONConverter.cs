using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using static ClientLogic;
using static GameManager;
public class JSONConverter : MonoBehaviour
{
    //public HeadClass configJson;
    [TextArea]
    public string jsonString;
    public byte[] arrayBytes;

    static ClientWebSocket cws;
    public void Send()
    {
        try
        {
            TestPlugin.SetMessage(arrayBytes);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
        
    }
    private async Task DoClientWebSocket()
    {

        using (ClientWebSocket ws = new ClientWebSocket())
        {
            cws = ws;
            Uri serverUri = new Uri("ws://95.181.230.220:7777");   //или ws://95.181.230.220:8090/sock

            //Implementation of timeout of 5000 ms
            var source = new CancellationTokenSource();
            source.CancelAfter(5000);

            //await ws.ConnectAsync(serverUri, source.Token);
            await ws.ConnectAsync(serverUri, CancellationToken.None);
            // restricted to 5 iteration only
            if (ws.State == WebSocketState.Open)
            {
                string msg = "{\"head\":\"AUTH\",\"body\":\"A\"}"; //"socket.emit('AUTH')"
                                                                   // Debug.Log(msg);
                ArraySegment<byte> bytesToSend =
                            new ArraySegment<byte>(Encoding.UTF8.GetBytes(msg));
                await ws.SendAsync(bytesToSend, WebSocketMessageType.Text,
                                     true, source.Token);

                // Debug.Log("SUCCESS");
                //Receive buffer
                var receiveBuffer = new byte[2000];
                //Multipacket response
                var offset = 0;
                var dataPerPacket = 10; //Just for example
                WebSocketReceiveResult result;
                do
                {
                    ArraySegment<byte> bytesReceived =
                              new ArraySegment<byte>(receiveBuffer, offset, dataPerPacket);
                    result = await ws.ReceiveAsync(bytesReceived,
                                                                  source.Token);
                    //Partial data received
                    Debug.Log("Data:" +
                                     Encoding.UTF8.GetString(receiveBuffer, offset,
                                                                  result.Count));
                    offset += result.Count;
                    if (result.EndOfMessage)
                        break;
                } while (!result.EndOfMessage);
                string response = Encoding.UTF8.GetString(receiveBuffer, 0, offset);
                Debug.Log("Complete response: " + response);
                try
                {
                    //configJson = JsonUtility.FromJson<HeadClass>(response);
                    //arrayBytes = GetFromConfigJsonByteList().ToArray();
                    

                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                    throw;
                }
            }
        }
       
    }
    public void SetMessageFromConfigJsonToMC()
    {
        Debug.Log("Start reqest");
        var taskWebConnect = Task.Run(() => DoClientWebSocket());
        taskWebConnect.Wait();

        StartCoroutine(Wait(taskWebConnect));

    }

    public IEnumerator Wait(Task task)
    {
        while(true)
        {
            if(task.IsCompleted)
            {
                Send();
                yield break;
            }
            yield return new WaitForEndOfFrame();

        }
    }
    //public List<byte> GetFromConfigJsonByteList()
    //{
    //    //return configJson.body.GetByteArray();
    //}
}
