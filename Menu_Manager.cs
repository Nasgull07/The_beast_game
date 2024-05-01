using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Threading;
using UnityEditor.VersionControl;
using UnityEditor;
using System.Collections.Concurrent;

public class Menu_Manager : MonoBehaviour
{

    public IPAddress Direccion_Ip = IPAddress.Parse("10.4.119.5"); //192.168.1.105
    private int puerto = 50033;
    Socket server;
    [SerializeField] private InputField m_UserName = null;
    [SerializeField] private InputField m_Password = null;
    public TextMeshProUGUI m_Prefab = null;
    public TextMeshProUGUI m_conectados = null;
    public TextMeshProUGUI m_numero = null;

    public TextMeshProUGUI m_Usuario_Conectado = null;

    private ConcurrentQueue<string[]> mensajeQueue = new ConcurrentQueue<string[]>();

    string[] menssaje;

    Thread atender;

    IPEndPoint ipep;
    Socket Server;

    delegate void DelegadoParaPonerTexto(string texto);
    public void SaveDataBeforeSceneLoad()
    {
        // Guardar datos relevantes utilizando PlayerPrefs u otra forma de almacenamiento de datos
        PlayerPrefs.SetString("Conectados", m_conectados.text);
        PlayerPrefs.SetString("Usuario", m_Usuario_Conectado.text);
        PlayerPrefs.Save();
    }

    // Cargar datos después de que se haya cargado una nueva escena
    public void LoadDataAfterSceneLoad()
    {
        // Cargar datos guardados utilizando PlayerPrefs u otra forma de almacenamiento de datos
        m_conectados.text = PlayerPrefs.GetString("Conectados"); // 0 es el valor predeterminado si no se encuentra la clave
        m_Usuario_Conectado.text = PlayerPrefs.GetString("Usuario");
    }

    // Start is called before the first frame update
    void Start()
    {
        LoadDataAfterSceneLoad();
        m_Usuario_Conectado.text = null;
        ipep = new IPEndPoint(this.Direccion_Ip, puerto);

        //Creamos el socket para podernos conectar.
        server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        try
        {


            server.Connect(ipep);
            byte[] Mensaje_Cliente = System.Text.Encoding.ASCII.GetBytes("3/");
            server.Send(Mensaje_Cliente);

            atender = new Thread(AtenderServidor);
            atender.Start();



        }
        catch (SocketException)
        {
            m_Prefab.text = "No se pudo establecer conexion con el servidor";

            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Procesar mensajes en la cola de mensajes
        string[] mensaje;
        while (mensajeQueue.TryDequeue(out mensaje))
        {
            // Procesar el mensaje
            int codigo = Convert.ToInt32(mensaje[0]);
            string mensajeTexto = mensaje[1];
            int x = 2;
            int y = mensaje.Length;

            switch (codigo)
            {
                case 1:  // respuesta a longitud
                    m_Prefab.text = mensajeTexto;
                    string lista = null;
                    while (x < y)
                    {
                        lista = lista + "\n" + mensaje[x];
                        x++;
                    }

                    m_Usuario_Conectado.text = m_UserName.text;

                    break;
                case 2:      // respuesta a si mi nombre es bonito
                    m_Prefab.text = mensajeTexto;
                    break;
                case 3:
                    m_conectados.text = null;
                    int h = 1;
                    while (h < mensaje.Length)
                    {
                        m_conectados.text = m_conectados.text + mensaje[h] + "\n";
                        h++;
                    }
                    break;
                case 4:     // Recibimos notificacion
                    m_numero.text = mensajeTexto;
                    break;
                case 5:
                    m_conectados.text = mensaje[2];
                    break;
                case 0:

                    m_Usuario_Conectado.text = null;
                    break;
            }
        }

    }
    public void Game_Scene(string Scene)
    {

        SaveDataBeforeSceneLoad();
        SceneManager.LoadScene(Scene);
    }
    public void Exit()
    {

        string mensaje = "0/" + m_Usuario_Conectado.text;
        if (m_Usuario_Conectado.text != null)
        {

            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);

        }
    }

    private void AtenderServidor()
    {
        while (true)
        {
            //Recibimos mensaje del servidor
            byte[] msg2 = new byte[2000];
            server.Receive(msg2);
            string[] mensaje = Encoding.ASCII.GetString(msg2).Split('/');

            mensajeQueue.Enqueue(mensaje);


        }
    }
    public void Log_In()
    {

        if (m_Usuario_Conectado.text == null)
        {

            try
            {

                if (m_UserName.text != "" && m_Password.text != "")

                {
                    string NOMBRE = m_UserName.text; //Variable para almacenar el nombre q vamos a enviar
                    string CONTRASEÑA = m_Password.text;


                    byte[] Mensaje_Cliente = System.Text.Encoding.ASCII.GetBytes("1/" + NOMBRE + "/" + CONTRASEÑA);
                    server.Send(Mensaje_Cliente);



                }




            }
            catch (SocketException)
            {
                m_Prefab.text = "No se pudo establecer conexion con el servidor";

                return;
            }
        }
        else { m_Prefab.text = "Ya hay una sesion iniciada, si quiere cambiar de sesion, desconectese de la actual."; }
    }
    public void Sign_Up()
    {

        try
        {

            if (m_UserName.text != "" && m_Password.text != "")
            {


                string NOMBRE = m_UserName.text; //Variable para almacenar el nombre q vamos a enviar
                string CONTRASEÑA = m_Password.text;


                byte[] Mensaje_Cliente = System.Text.Encoding.ASCII.GetBytes("2/" + NOMBRE + "/" + CONTRASEÑA);
                server.Send(Mensaje_Cliente);


            }
            else
            {
 
                m_Prefab.text = "Porfavor, rellene los datos para ingresar";
            }

   
        }
        catch (SocketException ex)
        {
            m_Prefab.text = "No se ha podido establecer la conexion";
            return;
        }
    }

}