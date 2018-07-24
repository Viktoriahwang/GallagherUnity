using SocketIO;
using UnityEngine;

public class main : MonoBehaviour {

    public PowerUI.Manager htmlView;
    private static SocketIOComponent socket;

    void Start () {
        var htmlGameObject = GameObject.Find("main-ui");
        if (htmlGameObject != null)
        {
            htmlView = htmlGameObject.GetComponent<PowerUI.Manager>();
            if (htmlView)
            {

                GameObject go = GameObject.Find("SocketIO");
                socket = go.GetComponent<SocketIOComponent>();

                socket.On("open", (SocketIOEvent e) => {
                    Debug.Log("CONNECTED");

                    socket.Emit("init");

                });

                socket.On("ui", (SocketIOEvent e) => {

                    Debug.Log("[SocketIO] UI received: " + e.name + " " + e.data);

                    if (e.data == null) { return; }

                 
                    if (htmlView)
                    {
                        htmlView.Document.innerHTML = e.data["data"].ToString().Replace(@"\", string.Empty);
                    }
                });
                socket.On("error", (SocketIOEvent e) => {

                    Debug.Log("[SocketIO] Error received: " + e.ToString() + " " + e.name + " " + e.data);
                });
                socket.On("close", (SocketIOEvent e) => {
                    Debug.Log("CLOSE");
                });
            }
        }
	}
	
	void Update () {
		
	}

    public static void onClick(PowerUI.MouseEvent e) {

        Debug.Log("ONCLICK! " + e.htmlTarget.value);
        if(!string.IsNullOrEmpty(e.htmlTarget.value) && socket != null && socket.IsConnected) {
            var payload = JSONObject.Create();
            payload.AddField("data", e.htmlTarget.value);
            Debug.Log("PAYLOAD: "+ payload.ToString());
            socket.Emit("action", payload);
        }
    }
}
