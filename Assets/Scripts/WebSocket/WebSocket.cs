using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class WebSocket : MonoBehaviour
{
    static ClientWebSocket cws;
    public async Task<string> Reqest(string msg)
    {
        string response = "";
        try
        {
            using (ClientWebSocket ws = new ClientWebSocket())
            {
                cws = ws;
                Uri serverUri = new Uri("ws://95.181.230.220:7777");   //или ws://95.181.230.220:8090/sock

                //Implementation of timeout of 5000 ms
                var source = new CancellationTokenSource();
                source.CancelAfter(10000);

                await ws.ConnectAsync(serverUri, source.Token);
                //await ws.ConnectAsync(serverUri, CancellationToken.None);
                // restricted to 5 iteration only
                if (ws.State == WebSocketState.Open)
                {
                    ArraySegment<byte> bytesToSend =
                                new ArraySegment<byte>(Encoding.UTF8.GetBytes(msg));
                    await ws.SendAsync(bytesToSend, WebSocketMessageType.Text,
                                         true, source.Token);

                    //Receive buffer
                    var receiveBuffer = new byte[5000];
                    //Multipacket response
                    var offset = 0;
                    var dataPerPacket = 1; //Just for example
                    WebSocketReceiveResult result;
                    do
                    {
                        ArraySegment<byte> bytesReceived =
                                  new ArraySegment<byte>(receiveBuffer, offset, dataPerPacket);
                        result = await ws.ReceiveAsync(bytesReceived, source.Token);
                        //Partial data received
                        Debug.Log("Data:" +
                                         Encoding.UTF8.GetString(receiveBuffer, offset,
                                                                      result.Count));
                        offset += result.Count;
                        Debug.Log(offset);
                        if (result.EndOfMessage)
                            break;
                    } while (!result.EndOfMessage);
                    response = Encoding.UTF8.GetString(receiveBuffer, 0, offset);
                    Debug.Log("Result response:" + response);
                }
            }
        }
        catch (Exception e)
        {

            Debug.Log(e.Message);
        }
        
        
        return response;
    }
}
