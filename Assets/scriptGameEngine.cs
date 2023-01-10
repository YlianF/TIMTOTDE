using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class scriptGameEngine : MonoBehaviour
{

    public GameObject PJjoueur;
    public GameObject PJDistant;
    private Transform folderGame;

    //objet joueur creer
    //!\ faire une classe joueur par pitier
    private GameObject joueur;
    private String idJoueur;
    private GameObject joueur2;
    private String idJoueur2;
    private float x2;
    private float y2;
    private Boolean ifJ2;
    private GameObject joueur3;
    private String idJoueur3;
    private GameObject joueur4;
    private String idJoueur4;

    //socket pour la reception et l'envoie
    private UDPSocket socketReceive;
    private UDPSocket socketSend;
    //port local
    private int portLocal;

    private int nbDePassage;//pour pas trop surchager l'envoie de requete



    // Start is called before the first frame update
    void Start()
    {
        //------------------------------------------------
        //generation du personnage
        //------------------------------------------------
        folderGame = new GameObject("Game").transform;

        joueur = Instantiate(PJjoueur, folderGame);
        //joueur.transform.SetParent(folderGame);
        joueur2 = Instantiate(PJDistant, folderGame);
        ifJ2 = false;

        //set temporaire pour l'idJoueur
        idJoueur = UnityEngine.Random.Range(0, 1000).ToString();

        //set des autres idJoueur a null
        idJoueur2 = null;
        //idJoueur3 = null;
        //idJoueur4 = null;


        //------------------------------------------------
        //gestion du multijoueur
        //------------------------------------------------

        //pas upnp flem se soir


        Boolean local;
        const int portServeurDeJeu = 52051;
        string IPLocal; //IP local qui à acces a internet

        //a modifier en fonction de si le jouer joue en local
        local = true;
        IPLocal = "127.0.0.1";
        


        string ipDistante;
        socketReceive = new UDPSocket();
        socketSend = new UDPSocket();
        //UPnP upnp;//obj pour l'UPnP qui s'active pas si c'est en local

        //UDP serveur local
        portLocal = 52051;//pour de vrai ce sera au minimum 52052

        if (local)
        {
            ipDistante = "127.0.0.1";
            //upnp = new UPnP("local");
        }
        else
        {
            ipDistante = "gamama.dynbug.ovh";
            //upnp = new UPnP();
        }

        int i = 0;

        try
        {
            socketSend.Client(ipDistante, portServeurDeJeu);
            //Console.WriteLine("\nConnect in " + ipDistante);
        }
        catch(FormatException)
        {/*
            while (true)
            {
                try //resolution de nom DNS
                {
                    string ipDistanteResolu = Dns.GetHostEntry(ipDistante).AddressList[i].ToString();
                    socketSend.Client(ipDistanteResolu, portServeurDeJeu);
                    Console.WriteLine("\nConnect in " + ipDistanteResolu);
                    break;
                }
                catch (SocketException)//Pas d'enregistrement trouve pour le nom de domaine donnée
                {
                    Console.WriteLine("\nUne erreur s'est produite lors de la résolution d'adresse");
                    Environment.Exit(0);
                }
                catch (NotSupportedException)//IPv6 tester, normal
                {
                    i++;
                }
                catch (IndexOutOfRangeException)//Pas d'address IPv4 pour le nom de domaine donnée
                {
                    Console.WriteLine("No address IPv4 found");
                    //exception plus tard
                    Environment.Exit(0);
                }
            }*/
        }


        //code UPnP a metre

        i = 0;
        int tentativeMax = 16;//tentative maximal pour générer un UPnP et ou un port

        do
        {
            try
            {
                i++;
                portLocal++;
                socketReceive.Server(IPLocal, portLocal);//1er catch
                //await upnp.CreationUPnP(portLocal);//2eme catch
                i = tentativeMax;
            }
            catch (SocketException)
            {
                if (i > tentativeMax)
                {
                    //Console.WriteLine("Imposibilité dobtenir un port");
                    //exception plus tard
                    //Environment.Exit(0);
                }
            }//port deja occuper sur l'ordinateur
            /*
            catch (MappingException)//port deja occuper sur le routeur UPnP
            {
                socketReceive._socket.Close();
                if (i > tentativeMax)
                {
                    Console.WriteLine("Imposibilité de créer un UPnP");
                    //exception plus tard
                    Environment.Exit(0);
                }
            }*/
        }
        while (i < tentativeMax);

        //declancheur pour quand on recois un message du serveur
        socketReceive.OnReceived += OnMsgRecieve;

        socketSend.Send(idJoueur + ":" + portLocal.ToString() + ":-1:-1");

        nbDePassage = 0;


    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        //if(nbDePassage>=20)
        //{
            //nbDePassage = 1;

            //cette ligne la a commenter
            //socketSend.Send(idJoueur + ":" + joueur.transform.position.x + ":" + joueur.transform.position.y);
            if(ifJ2)
            {   
                joueur2.transform.position = new Vector2(x2, y2);
            }
        //}
        //nbDePassage+=1;
        
    }


    void Quitter()
    {
        socketSend.Send("exit");
        socketReceive._socket.Close();
    }


    private void OnMsgRecieve(byte[] dataBytes, int bytesRead, string IPaddress)
    {
        try
            {
            string data = Encoding.ASCII.GetString(dataBytes, 0, bytesRead);
            string[] dataDecoupe = data.Split(':');

            for(int i = 1; i<dataDecoupe.Length; i+=3)
            {
                if(!dataDecoupe[i].Equals(idJoueur))
                {
                    //Debug.Log(i + "|" + dataDecoupe[i]);
                    if(dataDecoupe[i].Equals(idJoueur2))
                    {
                        x2 = float.Parse(dataDecoupe[i+1]);
                        y2 = float.Parse(dataDecoupe[i+2]);
                        //joueur2.transform.position = new Vector2(x, y);
                        //Debug.Log(x + ";" + y);
                    }
                    else
                    {
                        idJoueur2 = dataDecoupe[i];
                        ifJ2 = true;
                        x2 = float.Parse(dataDecoupe[i+1]);
                        y2 = float.Parse(dataDecoupe[i+2]);
                    }
                }
            }
        }
        catch(Exception e)
        {
            Debug.Log(e);
        }
    }
}
